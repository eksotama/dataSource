using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{
    public abstract class AbsSelect : AbsTable
    {

        //from AbsTable
        public override abstract string renderInSelect();

        //AbsSelect
        public abstract DataTable execute();

        #region produce compound selects
        public CompoundSelect union(AbsSelect select)
        {
            return new CompoundSelect(select_comp_op.union, this, select);
        }

        public CompoundSelect unionAll(AbsSelect select)
        {
            return new CompoundSelect(select_comp_op.unionAll, this, select);
        }

        public CompoundSelect except(AbsSelect select)
        {
            return new CompoundSelect(select_comp_op.except, this, select);
        }

        public CompoundSelect intersect(AbsSelect select)
        {
            return new CompoundSelect(select_comp_op.intersect, this, select);
        }

        #endregion

    }


}
