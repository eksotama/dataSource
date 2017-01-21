using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class Index : DbObject
    {
        Table tbl;
        Column[] cols;
        bool _unique;
        string name;

        public Index(string name, Table tbl,params Column[] cols)
        {
            this.name = name;
            this.tbl = tbl;
            this.cols = cols;
        }

        public Index unique()
        {
            this._unique = true;
            return this;
        }

        
        public string createCommand()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE {0} INDEX ", _unique? "UNIQUE" : "");
            sb.AppendFormat(" `{0}` ON `{1}` ( ", this.name, this.tbl.Name );
            foreach (Column col in this.cols)
            {
                sb.AppendFormat("`{0}` ,", col.Name);
            }
            sb.removeLastChar();
            sb.Append(")");
            //
            return sb.ToString();
        }

    }
}
