using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class View : DbTable
    {

        AbsSelect select;

        public AbsSelect selectStmnt
        {
            get
            {
                return selectStmnt;
            }
        }

        public override List<FieldInfo> fieldsInfo
        {
            get
            {
                return select.fieldsInfo;
            }
        }

        public View(Database db, string name, AbsSelect select)
        {
            this.db = db;
            this.Name = name;
            this.select = select;
        }

        public override string renderInSelect()
        {
            return Name;
        }

        public override string createCommand()
        {
            return string.Format(
                "CREATE VIEW `{0}` AS  {1}", 
                this.Name, 
                this.select.renderInSelect()
                );
        }
    }
}
