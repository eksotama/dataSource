using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public enum JoinType { inner, left, right, full, cross }

    public class JoinTable : AbsTable
    {
        AbsTable Tbl1, tbl2;
        expr on;
        JoinType join;
        static string[] joinNames = new string[] { "INNER", "LEFT OUTER", "RIGHT OUTER", "FULL OUTER", "CROSS" };

        public override string renderInSelect()
        {
            return string.Format("({0}) {1} JOIN ({2}) ON ({3})", Tbl1.renderInSelect(), joinNames[(int)join], tbl2.renderInSelect(), on.Render(null));
        }

        public JoinTable(AbsTable t1, JoinType j, AbsTable t2, expr on)
        {
            this.Tbl1 = t1;
            this.tbl2 = t2;
            this.join = j;
            this.on = on;
        }


        public override List<FieldInfo> fieldsInfo
        {
            get 
            {
                var tmp = new List<FieldInfo>();
                tmp.AddRange(Tbl1.fieldsInfo);
                tmp.AddRange(tbl2.fieldsInfo);
                return tmp;
            }
        }
    }
}
