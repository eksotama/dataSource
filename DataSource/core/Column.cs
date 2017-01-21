using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace dataSource
{
    

    public abstract class Column : expr, DbObject
    {
        
        public Table table;
        public string Name;
        public string displayName;

        public Column(string name, string display_name)
        {
            this.Name = name;
            this.displayName = display_name;

        }

        public Column(string name)
        {
            this.Name = name;
        }

        public Column setDisplayName(string display_name)
        {
            this.displayName = display_name;
            return this;
        }

        /// <summary>
        /// must be expr to be assigned to Value
        /// </summary>
        public ValueExpr defaultValue;

        bool Nullable = false;
        bool unique = false;

        public Column allowNull()
        {
            this.Nullable = true;
            return this;
        }

        public Column Unique()
        {
            this.unique = true;
            return this;
        }

        /// <summary>
        /// parametrized query will be used
        /// </summary>
        public Assignment value(object value)
        {
            return new Assignment(this, 
                new ValueExpr(this.Name, value)
                );
            
        }

        public Assignment value(expr exp)
        {
            return new Assignment(this, exp);
        }
        //
        protected string type;

        public virtual string Type()
        {
            return type;
        }


        internal void setTable(Table table)
        {
            this.table = table;
        }

        public string columnDefinition()
        {
            return string.Format("`{0}` {1} {2} {3} {4}", Name, Type(), Nullable? "" : "NOT NULL", unique? "UNIQUE" : "", defaultValue==null? "" : "DEFAULT "+ defaultValue.Render(null));
        }

        public Column setDefaultValue(object dv)
        {
            this.defaultValue = new ValueExpr(dv);
            return this;
        }

        #region functions

        public static byte[] imageToBinary(Image img)
        {
            MemoryStream ms = new MemoryStream();

            img.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }

        public static byte[] FileToBinary(string filePath)
        {
            FileStream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read);
            BinaryReader reader = new BinaryReader(stream);

            byte[] photo = reader.ReadBytes((int)stream.Length);

            reader.Close();
            stream.Close();

            return photo;
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null) return null;
            //
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;

        } 
        
        #endregion



        public string createCommand()
        {
            return string.Format(
                "ALTER TABLE `{0}` ADD COLUMN {1}", 
                table.Name, 
                columnDefinition()
                );
        }


        public override string Render(Statement st)
        {
            return this.Name;
        }
    }

    


    #region column-children

    public abstract class IntColumn : Column
    {
        public bool autoInc = false;
        public IntColumn(string name) : base(name) { }

        public Assignment value(int v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }

        public IntColumn autoIncrement()
        {
            autoInc = true;
            return this;
        }

        public override string Type()
        {
            if (autoInc)
            {
                return base.Type() + " AUTO_INCREMENT";
            }
            else
            {
                return base.Type();
            }
        }

    }

    public abstract class DoubleColumn : Column
    {
        public DoubleColumn(string name) : base(name) { }

        public Assignment value(double v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }
    }

    public abstract class TextColumn : Column
    {
        public TextColumn(string name) : base(name) { }

        public Assignment value(string v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }

    }

    public abstract class StringColumn : Column
    {
        private int maxLength;

        public StringColumn(string name, int max)
            : base(name)
        {
            this.maxLength = max;
        }

        public Assignment value(string v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }

    }

    public abstract class BoolColumn : Column
    {
        public BoolColumn(string name) : base(name) { }

        public Assignment value(bool v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }
    }

    public abstract class DateColumn : Column
    {
        public DateColumn(string name) : base(name) { }

        public Assignment value(DateTime v)
        {
            return new Assignment(this, new ValueExpr(this.Name, util.date(v) ));
        }
    }

    public abstract class DateTimeColumn : Column
    {
        public DateTimeColumn(string name) : base(name) { }

        public Assignment value(DateTime v)
        {
            return new Assignment(this, new ValueExpr(this.Name, util.dateTime(v) ));
        }
    }

    //public abstract class TimeColumn : Column
    //{
    //    public TimeColumn(string name) : base(name) { }
    //
    //    public Column value(DateTime v)
    //    {
    //        this.Value = new ValueExpr(this.Name, v);
    //        return this;
    //    }
    //}

    public abstract class BinaryColumn : Column
    {
        public BinaryColumn(string name) : base(name) { }

        public Assignment value(byte[] v)
        {
            return new Assignment(this, new ValueExpr(this.Name, v));
        }

        public Assignment value(Image img)
        {
            return new Assignment(this, new ValueExpr(this.Name, imageToBinary(img)));
        }

        /// <summary>
        /// take its value from a file, and saves it as binary
        /// </summary>
        public Assignment valueFile(string path)
        {
            return value(FileToBinary(path));
        }

    }

    #endregion
    
}
