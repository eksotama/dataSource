using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource.mySql
{
    class MySqlTable : Table
    {
        

        public MySqlTable(string name, Database db, Column[] fields)
            : base(name, db, fields)
        {
            
        }
       

        #region create

        public override string createCommand()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS `{0}` (", this.Name);
            //columns 
            bool frst = true;
            foreach (Column col in fields)
            {
                if (frst)
                {
                    sb.AppendFormat("{0} ,", col.columnDefinition() + " PRIMARY KEY");
                    frst = false;
                }
                else
                {
                    sb.AppendFormat("{0} ,", col.columnDefinition());
                }
            }
            //forein keys
            foreach (ForeinKey fk in this.FKs)
            {
                sb.AppendFormat("FOREIGN KEY (`{0}`) REFERENCES `{1}`(`{2}`) ON DELETE CASCADE,", fk.coln.Name, fk.tbl.Name, fk.tbl.id.Name);
            }
            //primary key
            sb.removeLastChar();
            //sb.AppendFormat("PRIMARY KEY ([{0}])", this.fields[0].Name);
            sb.Append(" )");
            //
            return sb.ToString();
        }

        #endregion


    }
}
