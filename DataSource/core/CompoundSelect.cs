using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{

    public enum select_comp_op { union, unionAll, intersect, except }

    public class CompoundSelect : AbsSelect
    {
        AbsSelect select1, select2;
        select_comp_op op;

        static string[] ops = new string[] { "UNION", "UNION ALL" , "INTERSECT" , "EXCEPT" };

        public CompoundSelect(select_comp_op op, AbsSelect select1, AbsSelect select2)
        {
            this.select1 = select1;
            this.select2 = select2;
            this.op = op;
        }

        public override string renderInSelect()
        {
            return string.Format(
                " ({0}) {1} ({2}) ", 
                select1.renderInSelect(), 
                ops[(int)op], 
                select2.renderInSelect()
                );
        }

        
        public override DataTable execute()
        {
            throw new NotImplementedException();
        }
    }
}
