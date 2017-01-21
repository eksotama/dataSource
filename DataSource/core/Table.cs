using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{
    public class Table : DbTable
    {
        
        public List<Column> fields=new List<Column>();
        public List<ForeinKey> FKs=new List<ForeinKey>();

        public IntColumn id
        {
            get
            {
                return (IntColumn)fields[0];
            }
        }

        public int newId()
        {
            object tmp= db.select().fields(expr.Max(this.id)).from(this)
                .executeScalar();
            return tmp == null ? 1 : (Convert.ToInt32(tmp) + 1);
        }

        public Table(string name, Database database, params Column[] fields)
        {
            this.Name = name;
            this.db = database;
            db.addIdColumn(this);
            this.fields.AddRange(fields);
            
            foreach (Column f in fields)
            {
                f.setTable(this);
            }
        }

        public override string renderInSelect()
        {
            return Name;
        }

        /// <summary>
        /// adds a new fk to the table and adds the field to fields list
        /// </summary>
        public IntColumn addFKto(Table table)
        {
            IntColumn tmp= db.intColumn( string.Format("{0}_id", table.Name));

            this.fields.Add(tmp);

            this.FKs.Add(
                new ForeinKey( tmp, table)
                );

            return tmp;
        }

        /// <summary>
        /// adds a new fk to the table and adds the field to fields list, specifying it's name in the db
        /// </summary>
        public IntColumn addFKto(Table table, string name)
        {
            IntColumn tmp = db.intColumn(name);

            this.fields.Add(tmp);

            this.FKs.Add(
                new ForeinKey(tmp, table)
                );

            return tmp;
        }

        public override string createCommand()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS `{0}` (", this.Name);
            //columns 
            foreach (Column col in fields)
            {
                sb.AppendFormat("{0} ,", col.columnDefinition());
            }
            //forein keys
            foreach (ForeinKey fk in this.FKs)
            {
                sb.AppendFormat("FOREIGN KEY (`{0}`) REFERENCES `{1}` ON DELETE CASCADE,", fk.coln.Name, fk.tbl.Name);
            }
            //primary key
            sb.AppendFormat("PRIMARY KEY (`{0}`)", this.fields[0].Name);
            sb.Append(" )");
            //
            return sb.ToString();
        }


    }

    public struct ForeinKey
    {
        public Column coln;
        public Table tbl;

        public ForeinKey(Column coln, Table tbl)
        {
            this.coln = coln;
            this.tbl = tbl;
        }
    }

}
