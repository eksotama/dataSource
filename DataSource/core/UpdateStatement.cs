using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class UpdateStatement : NonQueryStatement
    {
        
        Assignment[] assigns;
        expr cond;

        public UpdateStatement(Database db, Table tbl )
        {
            this.db = db;
            this.tbl = tbl;
        }

        public UpdateStatement Set(params Assignment[] assignments)
        {
            this.assigns = assignments;
            return this;
        }

        public UpdateStatement where(expr cond)
        {
            this.cond = cond;
            return this;
        }

        //=====

        public override string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE `{0}` SET ", this.tbl.Name);
            foreach (Assignment asn in assigns)
            {
                sb.AppendFormat("{0} ,", asn.render(this));
            }
            sb.removeLastChar();
            //
            if (cond != null)
            {
                sb.AppendFormat(" WHERE ({0}) ", cond.Render(this));
            }
            return sb.ToString();
        }

        public override int execute()
        {
            return db.executeUpdate(this);
        }



        
    }
}
