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
        protected string conectionString;
        /// <summary>
        /// implemented by the db, pass columns except primary and forein keys
        /// </summary>
        public abstract Table newTable(string name, params Column[] fields);

        public abstract DbContext newContext();

        #region excute commands

        public abstract void addIdColumn(Table t);

        #endregion

        #region db-objects

        
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
            //db context
            var dbc = this.newContext();
            //fields
            meta_name = stringColumn("name", 150);
            meta_value = (IntColumn)intColumn("value").allowNull();
            meta_text = (StringColumn)stringColumn("text", 150).allowNull();

            //table
            tbl_meta= this.newTable("ds_meta", meta_name, meta_value, meta_text);

            //create if not exists
            dbc.create(tbl_meta);
            //look version
            int ver = currentVersion(dbc);
            if (ver == -1)
            {
                // insert version row
                dbc.insertInto(tbl_meta).Values(meta_name.value("schema version"), meta_value.value(0))
                    .execute();
                //
                ver = 0;
            }
            //
            for (int i = ver; i < schemaUpdates.Count; i++)
            {
                schemaUpdates[i].apply(this, dbc);
            }
            setSchemaVersion(schemaUpdates.Count, dbc);
            //commit
            dbc.SubmitChanges();
        }

        private int currentVersion(DbContext dbc)
        {
            object tmp= dbc.select().from(tbl_meta).fields(meta_value).where(meta_name.equal("schema version"))
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

        internal void setSchemaVersion(int version, DbContext dbc)
        {
            dbc.update(tbl_meta).Set(meta_value.value(version)).where(meta_name.equal("schema version"))
                .execute();
        }
    }
}
