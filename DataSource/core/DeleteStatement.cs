using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class DeleteStatement : NonQueryStatement
    {

        public expr cond;

        public DeleteStatement(Database db, Table tbl)
        {
            this.db = db;
            this.tbl = tbl;
        }

        public DeleteStatement where(expr cond)
        {
            this.cond = cond;
            return this;
        }

        public override int execute()
        {
            return db.executeDelete(this);
        }

        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM `{0}`", tbl.Name);
            if (cond != null)
            {
                sb.AppendFormat(" WHERE ({0})", cond.Render(this) );
            }
            return sb.ToString();
        }
    }
}
