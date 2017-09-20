using dataSource.entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace dataSource
{
    public abstract class DbContext: IDisposable
    {

        public Database db;
        public IDbConnection connection;
        public IDbTransaction trns;

        public DbContext(Database db, IDbConnection con)
        {
            this.db = db;
            this.connection = con;
        }

        #region main

        internal void InitializeTransaction()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                trns = connection.BeginTransaction();
            }
        }

        public void RollBack()
        {
            if (connection.State == ConnectionState.Open)
            {
                trns.Rollback();
                connection.Close();
            }
        }

        public void SubmitChanges()
        {
            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    trns.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public int create(DbObject obj)
        {
            
            this.InitializeTransaction();
            IDbCommand cmd = getCommand();
            cmd.CommandText = obj.createCommand();
            return cmd.ExecuteNonQuery();
        }

        #endregion

        #region ===== select update insert delete =====

        public SelectStatement select()
        {
            return new SelectStatement(this);
        }

        public SelectStatement selectDistinct()
        {
            return new SelectStatement(this, true);
        }

        public SelectStatement selectListItems(Table tbl, expr nameField)
        {
            return select().from(tbl).fields(tbl.id.As("id"), nameField.As("name"));
        }

        public UpdateStatement update(Table tbl)
        {
            return new UpdateStatement(this, tbl);
        }

        public InsertStatement insertInto(Table tbl)
        {
            return new InsertStatement(this, tbl);
        }

        public DeleteStatement deleteFrom(Table tbl)
        {
            return new DeleteStatement(this, tbl);
        }

        #endregion


        #region excute commands

        /// <summary>
        /// must be used before closing the connection
        /// </summary>
        public abstract int lastId();

        /// <summary>
        /// returns a typed command with connection set
        /// </summary>
        internal abstract IDbCommand getCommand();


        protected abstract void addParam(IDbCommand cmd, string name, object value);

        internal int executeUpdate(UpdateStatement updateStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = updateStatement.Render();
            foreach (NameValuePair nv in updateStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }

            return cmd.ExecuteNonQuery();
        }

        internal int executeDelete(DeleteStatement deleteStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = deleteStatement.Render();
            foreach (NameValuePair nv in deleteStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }
            //
            return cmd.ExecuteNonQuery();
        }


        internal int executeInsert(InsertStatement insertStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = insertStatement.Render();
            foreach (NameValuePair nv in insertStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// returns null if no data found
        /// </summary>
        internal DataRow executeSelectRow(SelectStatement selectStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = selectStatement.render();
            foreach (NameValuePair nv in selectStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }
            //
            DataTable dt = new DataTable();
            IDataReader dr = cmd.ExecuteReader();
            dt.Load(dr);
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            return dt.Rows[0];
        }

        /// <summary>
        /// returns empty datatable if no data
        /// </summary>
        internal DataTable executeSelect(SelectStatement selectStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = selectStatement.render();
            foreach (NameValuePair nv in selectStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }
            //
            DataTable dt = new DataTable();
            IDataReader dr = cmd.ExecuteReader();
            dt.Load(dr);
            //if (dt.Rows.Count == 0)
            //{
            //	return null;
            //}
            return dt;
        }


        /// <summary>
        /// returns null instead of empty DBNull
        /// </summary>
        internal object executeScalar(SelectStatement selectStatement)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = selectStatement.render();
            foreach (NameValuePair nv in selectStatement.parameters)
            {
                addParam(cmd, nv.name(), nv.value());
            }
            //
            DataTable dt = new DataTable();
            object tmp = cmd.ExecuteScalar();
            //
            return tmp is DBNull ? null : tmp;
        }

        #endregion

        #region entity

        public T selectFirstWhere<T>(expr cond) where T : Entity, new()
        {
            var ent = new T();
            var r = this.select().from(ent.Tbl).where(cond).executeRow();
            if (r == null)
            {
                return null;
            }
            else
            {
                ent.attach(r);
                return ent;
            }
        }

        public T selectById<T>(int id) where T : TableEntity, new()
        {
            var ent = new T();
            var tbl = ent.Tbl as Table;
            var r = this.select().from(tbl).where(tbl.id.equal(id)).executeRow();
            if (r == null)
            {
                return null;
            }
            else
            {
                ent.attach(r);
                return ent;
            }
        }

        public List<T> selectWhere<T>(expr cond) where T : Entity, new()
        {
            var tbl = new T().Tbl;
            var list = new List<T>();
            var dt = this.select().from(tbl).where(cond).execute();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new T();
                item.attach(dr);
                list.Add(item);
            }

            return list;
        }

        public List<T> selectPage<T>(int index, int size) where T : Entity, new()
        {
            var tbl = new T().Tbl;
            var list = new List<T>();
            var dt = this.select().from(tbl).page(index, size).execute();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new T();
                item.attach(dr);
                list.Add(item);
            }

            return list;
        }

        public List<T> select<T>(expr cond, int pageIndex, int pageSize) where T : Entity, new()
        {
            var tbl = new T().Tbl;
            var list = new List<T>();
            var dt = this.select().from(tbl).where(cond).page(pageIndex, pageSize).execute();

            foreach (DataRow dr in dt.Rows)
            {
                var item = new T();
                item.attach(dr);
                list.Add(item);
            }

            return list;
        }

        #endregion

        public void Dispose()
        {
            this.connection.Dispose();
            this.trns.Dispose();
        }
    }
}
