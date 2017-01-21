using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource
{
    /// <summary>
    /// objects that may be populated to the db 
    /// such as Table, view, field, trigger, ....
    /// </summary>
    public interface DbObject
    {
        
        /// <summary>
        /// the text of the sql command that will create the object in the database
        /// </summary>
        string createCommand();

    }
}
