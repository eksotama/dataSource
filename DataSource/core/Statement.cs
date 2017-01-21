using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    /// <summary>
    /// Any statement must have parameterized slots
    /// </summary>
    public interface Statement
    {

        void addParam(string name, object value);

    }


    public struct NameValuePair
    {
        object Val;
        string Name;

        public NameValuePair(string name, object val)
        {
            this.Name = name;
            this.Val = val;
        }

        public object value()
        {
            return this.Val;
        }

        public string name()
        {
            return string.Format("@{0}", Name);
        }

    }

}
