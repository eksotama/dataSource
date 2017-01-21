using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{

    /// <summary>
    /// use addSchemaUpdate() and updateSchemaIfNeeded()
    /// </summary>
    public abstract class Database
    {
        public IDbConnection connection;
        public IDbTransaction trns;


        public void InitializeTransaction()
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

        /// <summary>
        /// implemented by the db, pass columns except primary and forein keys
        /// </summary>
        public abstract Table newTable(string name, params Column[] fields);

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

        public abstract void addIdColumn(Table t);

        /// <summary>
        /// returns a typed command with connection set
        /// </summary>
        protected abstract IDbCommand getCommand();

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
            IDataReader dr= cmd.ExecuteReader();
            dt.Load(dr);
            if(dt.Rows.Count == 0)
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
            object tmp= cmd.ExecuteScalar();
            //
            return tmp is DBNull ? null : tmp;
        }

        #endregion

        #region db-objects

        public int create(DbObject obj)
        {
            InitializeTransaction();
            IDbCommand cmd = this.getCommand();
            cmd.CommandText = obj.createCommand();
            return cmd.ExecuteNonQuery();
        }

        public virtual Index newIndex(string name, Table tbl,params Column[] cols)
        {
            return new Index( name,  tbl,  cols);

        }

        public virtual Trigger newTrigger(string name, Table tbl)
        {
            return new Trigger(name, tbl);
        }

        public virtual View newView(string name, AbsSelect select)
        {
            return new View(this, name, select);
        }

        #endregion

        #region db-columns

        public abstract IntColumn intColumn(string name);

        public abstract DoubleColumn doubleColumn(string name);

        public abstract TextColumn textColumn(string name);

        public abstract StringColumn stringColumn(string name, int maxLength);

        public abstract DateColumn dateColumn(string name);

        public abstract DateTimeColumn dateTimeColumn(string name);

        public abstract BinaryColumn binaryColumn(string name);

        public abstract BoolColumn boolColumn(string name);

        #endregion

        #region META

        StringColumn meta_name;
        IntColumn meta_value;
        StringColumn meta_text;
        Table tbl_meta;

        public void updateSchemaIfNeeded()
        {
            //fields
            meta_name = stringColumn("name", 150);
            meta_value = (IntColumn)intColumn("value").allowNull();
            meta_text = (StringColumn)stringColumn("text", 150).allowNull();

            //table
            tbl_meta= this.newTable("ds_meta", meta_name, meta_value, meta_text);

            //create if not exists
            this.create(tbl_meta);
            //look version
            int ver = currentVersion();
            if (ver == -1)
            {
                // insert version row
                insertInto(tbl_meta).Values(meta_name.value("schema version"), meta_value.value(0))
                    .execute();
                //
                ver = 0;
            }
            //
            for (int i = ver; i < schemaUpdates.Count; i++)
            {
                schemaUpdates[i].apply(this);
            }
            setSchemaVersion(schemaUpdates.Count);
            //commit
            this.SubmitChanges();
        }

        private int currentVersion()
        {
            object tmp= select().from(tbl_meta).fields(meta_value).where(meta_name.equal("schema version"))
                .executeScalar();
            //
            return tmp == null ? -1 : Convert.ToInt32(tmp);
        }

        #endregion


        #region schema updates
        private List<SchemaUpdate> schemaUpdates = new List<SchemaUpdate>();

        public void addSchemaUpdate(SchemaUpdate su)
        {
            schemaUpdates.Add(su);
        }

        public void addSchemaUpdate(params DbObject[] dbObjs)
        {
            schemaUpdates.Add(new SchemaUpdate(dbObjs));
        }
        #endregion

        internal void setSchemaVersion(int version)
        {
            update(tbl_meta).Set(meta_value.value(version)).where(meta_name.equal("schema version"))
                .execute();
        }
    }
}
