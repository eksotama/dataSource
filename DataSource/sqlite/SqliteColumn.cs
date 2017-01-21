using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace dataSource.sqlite
{
    class SqliteInteger : IntColumn
    {
        

        public SqliteInteger(string name)
            : base(name)
        {
            this.type = "INTEGER";
        }
    }

    class SqliteReal : DoubleColumn
    {
        

        public SqliteReal(string name)
            : base(name)
        {
            this.type = "REAL";
        }
    }

    class SqliteString : StringColumn
    {

        public SqliteString(string name, int max)
            : base(name, max)
        {
            this.type = "TEXT";
        }
    }

    class SqliteText : TextColumn
    {

        public SqliteText(string name)
            : base(name)
        {
            this.type = "TEXT";
        }
    }

    class SqliteBool : BoolColumn
    {

        public SqliteBool(string name)
            : base(name)
        {
            this.type = "INTEGER";
        }
    }

    class SqliteImage : BinaryColumn
    {

        public SqliteImage(string name)
            : base(name)
        {
            this.type = "BLOB";
        }


    }

    class SqliteDate : DateColumn
    {
        public SqliteDate(string name)
            : base(name)
        {
            this.type = "TEXT";
        }

       
    }

    class SqliteDateTime : DateTimeColumn
    {
        public SqliteDateTime(string name)
            : base(name)
        {
            this.type = "TEXT";
        }


    }

}
