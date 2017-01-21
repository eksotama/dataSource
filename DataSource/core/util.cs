using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace dataSource
{
    public static class util
    {

        public static void removeLastChar(this StringBuilder sb)
        {
            sb.Remove(sb.Length - 1, 1);
        }

		public static string date(DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd");
		}

		public static string time(DateTime dt)
		{
			return dt.ToString("HH:mm:ss");
		}

		public static string dateTime(DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static int inT(object o)
		{
			return Convert.ToInt32(o);
		}

		//extensions for datarow
		public static int getInt(this DataRow dr, IntColumn col)
		{
			return Convert.ToInt32( dr[col.Name].ToString());
		}

        public static double getDouble(this DataRow dr, DoubleColumn col)
        {
            return Convert.ToDouble(dr[col.Name].ToString());
        }

        public static bool isNull(this DataRow dr, Column col)
        {
            return dr[col.Name] is DBNull;
        }

		public static string getString(this DataRow dr, Column col)
		{
			return dr[col.Name].ToString();
		}

		public static string getString(this DataRow dr, StringColumn col)
		{
			return dr[col.Name].ToString();
		}

		public static string getString(this DataRow dr, TextColumn col)
		{
			return dr[col.Name].ToString();
		}

		public static DateTime getDate(this DataRow dr, DateColumn col)
		{
			return dr.Field<DateTime>(col.Name);
		}

        public static DateTime getDate(this DataRow dr, DateTimeColumn col)
        {
            return dr.Field<DateTime>(col.Name);
        }

        public static bool getBool(this DataRow dr, BoolColumn col)
        {
            return Convert.ToBoolean(dr[col.Name].ToString());
        }

    }
}
