using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace dataSource
{
    //public enum Permission { Select, Insert, Update, Delete }

    /// <summary>
    /// an abstract table is any form may be used to select from as Table, View, JoinTable, SelectStatement
    /// </summary>
    public abstract class AbsTable
    {
        public abstract string renderInSelect();

        #region join types


        public JoinTable innerJoin(AbsTable t, expr on)
        {
            return new JoinTable(this, JoinType.inner, t, on);
        }

        public JoinTable outerJoin(AbsTable t, expr on)
        {
            return new JoinTable(this, JoinType.full, t, on);
        }

        public JoinTable crossJoin(AbsTable t, expr on)
        {
            return new JoinTable(this, JoinType.cross, t, on);
        }

        public JoinTable leftJoin(AbsTable t, expr on)
        {
            return new JoinTable(this, JoinType.left, t, on);
        }

        public JoinTable rightJoin(AbsTable t, expr on)
        {
            return new JoinTable(this, JoinType.right, t, on);
        }

        #endregion


    }


    /// <summary>
    /// a table or a view in the database
    /// </summary>
    public abstract class DbTable : AbsTable, DbObject
    {
        public string Name;
        public Database db;


        public override string renderInSelect()
        {
            return string.Format("`{0}`",this.Name);
        }

        
        public abstract string createCommand();



    }



}
