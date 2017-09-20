using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace dataSource
{
    public class SchemaUpdate
    {
        List<DbObject> objects = new List<DbObject>();

        List<NonQueryStatement> statements = new List<NonQueryStatement>();


		//constructor
		public SchemaUpdate() { }

		public SchemaUpdate(params DbObject[] dbo)
		{
			add(dbo);
		}

        /// <summary>
        /// executes to the database without commiting
        /// </summary>
        internal void apply(Database db, DbContext dbc)
        {
            foreach(DbObject dbo in objects)
            {
                dbc.create(dbo);
            }
            //
            foreach (NonQueryStatement st in statements)
            {
                st.execute();
            }
        }

        public SchemaUpdate add(params DbObject[] dbo)
        {
			foreach (DbObject item in dbo)
			{
				objects.Add(item);
			}
            
            return this;
        }

        public SchemaUpdate add(params NonQueryStatement[] st)
        {
			foreach (NonQueryStatement item in st)
			{
				statements.Add(item);
			}
            
            return this;
        }

    }
}
