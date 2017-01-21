using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public class Trigger : DbObject
    {

        static string[] events = new string[] {"INSERT", "UPDATE", "DELETE"};
        //
        string name;
        bool before = false; //befor or after
        Table tbl;
        dbEvent _event;
        NonQueryStatement[] triggerSteps;
        expr when;

        public Trigger(string name, Table tbl)
        {
            this.name = name;
            this.tbl = tbl;
        }

        public Trigger Before(dbEvent ev)
        {
            this._event = ev;
            this.before = true;
            return this;
        }

        public Trigger After(dbEvent ev)
        {
            this._event = ev;
            this.before = false;
            return this;
        }

        public Trigger Actions(params NonQueryStatement[] actions)
        {
            this.triggerSteps = actions;
            return this;
        }

        

        public virtual string createCommand()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TRIGGER `{0}` {1} {2} ON `{3}`", this.name, before? "BEFORE" : "AFTER", events[(int)_event], tbl.Name);
            sb.Append(" FOR EACH ROW ");
            sb.AppendFormat(" WHEN ({0}) BEGIN", when.Render(null));
            foreach (NonQueryStatement st in triggerSteps)
            {
                sb.AppendFormat("{0} ;", st.Render());
            }
            sb.removeLastChar();
            sb.Append(" END");
            //
            return sb.ToString();
        }



    }

    public enum dbEvent { Insert, Update, Delete }

}
