using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dataSource.mySql
{

    class MySqlInteger : IntColumn
    {


        public MySqlInteger(string name)
            : base(name)
        {
            this.type = "BIGINT";
        }
    }

    class MySqlReal : DoubleColumn
    {


        public MySqlReal(string name)
            : base(name)
        {
            this.type = "REAL"; //or DOUBLE
        }
    }

    class MySqlString : StringColumn
    {

        public MySqlString(string name, int max)
            : base(name, max)
        {
            this.type = string.Format("VARCHAR({0})", max);
        }
    }

    class MySqlText : TextColumn
    {

        public MySqlText(string name)
            : base(name)
        {
            this.type = "LONGTEXT";
        }
    }

    class MySqlBool : BoolColumn
    {

        public MySqlBool(string name)
            : base(name)
        {
            this.type = "BIT";
        }
    }

    class MySqlImage : BinaryColumn
    {

        public MySqlImage(string name)
            : base(name)
        {
            this.type = "LONGBLOB";
        }


    }

    class MySqlDate : DateColumn
    {
        public MySqlDate(string name)
            : base(name)
        {
            this.type = "DATE";
        }


    }

    class MySqlDateTime : DateTimeColumn
    {
        public MySqlDateTime(string name)
            : base(name)
        {
            this.type = "DATETIME";
        }


    }

}
