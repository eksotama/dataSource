using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class InsertStatement : NonQueryStatement
    {

        public Assignment[] assigns;
        

        public InsertStatement(Database db, Table tbl)
        {
            this.db = db;
            this.tbl = tbl;
        }

        public InsertStatement Values(params Assignment[] assigns)
        {
            this.assigns = assigns;
            return this;
        }



        public override int execute()
        {
            return db.executeInsert(this);
        }

        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO `{0}` ( ", tbl.Name);
            //fields
            foreach (Assignment asn in assigns)
            {
                sb.AppendFormat("`{0}` ,", asn.cln.Name);
            }
            sb.removeLastChar();
            sb.Append(") VALUES (");
            //values
            foreach (Assignment asn in assigns)
            {
                sb.AppendFormat("{0} ,",asn.val.Render(this) );
            }
            sb.removeLastChar();
            sb.Append(")");
            return sb.ToString();
        }


    }

    public struct Assignment
    {
        public Column cln;
        public expr val;

        public Assignment(Column cln, expr value)
        {
            this.cln = cln;
            this.val = value;
        }

        public string render(Statement st)
        {
            return string.Format("`{0}` = ({1})", cln.Name, val.Render(st));
        }

    }

}
