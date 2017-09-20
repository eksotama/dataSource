using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    public abstract class NonQueryStatement : Statement
    {
        //must be initialized
        public DbContext dbc;
        public Table tbl;

        public abstract int execute();

        public abstract string Render();

        
        #region Statement interface

        public List<NameValuePair> parameters = new List<NameValuePair>();

        public void addParam(string name, object value)
        {
            parameters.Add( new NameValuePair(name, value) );
        }

        #endregion

    }
}
