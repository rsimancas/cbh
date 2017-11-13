namespace Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using Utilidades;

    public static class EnumExtension
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {

            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Convert  list to Data Table
        /// </summary>
        /// <typeparam name="T">Target Class</typeparam>
        /// <param name="varlist">list you want to convert it to Data Table</param>
        /// <param name="fn">Delegate Function to Create Row</param>
        /// <returns>Data Table That Represent List data</returns>
        public static DataTable ToADOTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn)
        {
            DataTable toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            T TopRec = varlist.ElementAtOrDefault(0);

            if (TopRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            PropertyInfo[] oProps = ((Type)TopRec.GetType()).GetProperties();

            foreach (PropertyInfo pi in oProps)
            {
                Type pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (T rec in varlist)
            {
                DataRow dr = toReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    object o = pi.GetValue(rec, null);
                    if (o == null)
                        dr[pi.Name] = DBNull.Value;
                    else
                        dr[pi.Name] = o;
                }
                toReturn.Rows.Add(dr);
            }

            return toReturn;
        }

        /// <summary>
        /// Convert  list to Data Table
        /// </summary>
        /// <typeparam name="T">Target Class</typeparam>
        /// <param name="varlist">list you want to convert it to Data Table</param>
        /// <returns>Data Table That Represent List data</returns>
        public static DataTable ToADOTable<T>(this IEnumerable<T> varlist)
        {
            DataTable toReturn = new DataTable();

            // Could add a check to verify that there is an element 0
            T TopRec = varlist.ElementAtOrDefault(0);

            if (TopRec == null)
                return toReturn;

            // Use reflection to get property names, to create table
            // column names

            PropertyInfo[] oProps = ((Type)TopRec.GetType()).GetProperties();

            foreach (PropertyInfo pi in oProps)
            {
                Type pt = pi.PropertyType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    pt = Nullable.GetUnderlyingType(pt);
                toReturn.Columns.Add(pi.Name, pt);
            }

            foreach (T rec in varlist)
            {
                DataRow dr = toReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    object o = pi.GetValue(rec, null);

                    if (o == null)
                        dr[pi.Name] = DBNull.Value;
                    else
                        dr[pi.Name] = o;
                }
                toReturn.Rows.Add(dr);
            }

            return toReturn;
        }

        /// <summary>
        /// Converts datatable to list<T> dynamically
        /// </summary>
        /// <typeparam name="T">Class name</typeparam>
        /// <param name="dataTable">data table to convert</param>
        /// <returns>List<T></returns>
        /// 
        /// <summary>
        /// Converts datatable to list<T> dynamically
        /// </summary>
        /// <typeparam name="T">Class name</typeparam>
        /// <param name="dataTable">data table to convert</param>
        /// <returns>List<T></returns>
        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    if (propertyInfos == null || Convert.IsDBNull(dataRow[dtField.Name]))
                    {
                        continue;
                    }
 
                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {

                        if (propertyInfos.PropertyType == typeof(DateTime))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDateTime(dataRow[dtField.Name]).ToUniversalTime(), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToInt32(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(bool))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToBoolean(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<decimal>))
                        {
                            propertyInfos.SetValue
                            (classObj, (decimal)(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int?))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToInt32((dataRow[dtField.Name])), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDateTime(dataRow[dtField.Name]).ToUniversalTime(), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<bool>))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToBoolean(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {
                            if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                            {
                                propertyInfos.SetValue
                                (classObj, Convert.ToDateTime(dataRow[dtField.Name]).ToUniversalTime().ToString(), null);
                            }
                            else
                            {
                                propertyInfos.SetValue
                                (classObj, Convert.ToString(dataRow[dtField.Name]), null);
                            }
                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        public static List<T> OlderToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    PropertyInfo propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    if (propertyInfos == null || Convert.IsDBNull(dataRow[dtField.Name]))
                    {
                        continue;
                    }

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field != null)
                    {
                        if (propertyInfos.PropertyType == typeof(DateTime))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToInt32(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(bool))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToBoolean(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(long))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(decimal))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDecimal(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<decimal>))
                        {
                            propertyInfos.SetValue
                            (classObj, (decimal)(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(int?))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToInt32((dataRow[dtField.Name])), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<DateTime>))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToDateTime(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(Nullable<bool>))
                        {
                            propertyInfos.SetValue
                            (classObj, Convert.ToBoolean(dataRow[dtField.Name]), null);
                        }
                        else if (propertyInfos.PropertyType == typeof(String))
                        {
                            if (dataRow[dtField.Name].GetType() == typeof(DateTime))
                            {
                                propertyInfos.SetValue
                                (classObj, Convert.ToDateTime(dataRow[dtField.Name]).ToString(), null);
                            }
                            else
                            {
                                propertyInfos.SetValue
                                (classObj, Convert.ToString(dataRow[dtField.Name]), null);
                            }
                        }
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        /// <summary>
        /// Convert Data Table To List of Type T
        /// </summary>
        /// <typeparam name="T">Target Class to convert data table to List of T </typeparam>
        /// <param name="datatable">Data Table you want to convert it</param>
        /// <returns>List of Target Class</returns>
        public static List<T> OldToList<T>(this DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch { return Temp; }
        }

        public static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties; Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                } return obj;
            }
            catch { return obj; }
        }

        public delegate object[] CreateRowDelegate<T>(T t);

        //public static void x_setListValues(object clase, string identityName, ref string sql)
        //{
        //    try
        //    {
        //        string propname = "";
        //        string colNames = "";
        //        string colValues = "";
        //        string colValuesDebug = "";

        //        PropertyInfo[] Properties; Properties = clase.GetType().GetProperties();
        //        foreach (PropertyInfo objProperty in Properties)
        //        {

        //            propname = objProperty.Name;
        //            if (propname != identityName && propname.Substring(0, 2) != "x_")
        //            {
        //                if (objProperty.GetValue(clase) != null)
        //                {
        //                    string value = "";
        //                    switch (objProperty.GetValue(clase).GetType().ToString())
        //                    {
        //                        case "System.String":
        //                            value = "'" + objProperty.GetValue(clase) + "'";
        //                            break;
        //                        case "System.DateTime":
        //                            DateTime date = (DateTime)objProperty.GetValue(clase);
        //                            //string dateformatted = String.Format("{0:s}", date.ToUniversalTime()); ;
        //                            string dateformatted = String.Format("{0:s}", date.ToLocalTime()); ;
        //                            value = "'" + dateformatted + "'";
        //                            break;
        //                        case "System.Boolean":
        //                            value = ((Boolean)objProperty.GetValue(clase)) ? "CAST(1 as BIT)" : "CAST(0 as BIT)";
        //                            break;
        //                        default:
        //                            value = objProperty.GetValue(clase).ToString();
        //                            break;
        //                    };

        //                    colValues += "," + value;
        //                    colNames += "," + propname;

        //                    colValuesDebug += String.Format("{0} = {1}\n", propname, value);
        //                }
        //            }
        //        };

        //        LogManager.Write(colValuesDebug);

        //        sql = String.Format(sql, colNames.Substring(1), colValues.Substring(1));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManager.Write(ex.Source + " : " + ex.Message);
        //    }

        //    //return "";
        //}

        public static void setListValues(object clase, string identityName, ref string sql)
        {
            try
            {
                string propname = "";
                string colNames = "";
                string colValues = "";

                PropertyInfo[] Properties; Properties = clase.GetType().GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {

                    propname = objProperty.Name;
                    if (propname != identityName && propname.Substring(0, 2) != "x_" && !objProperty.GetGetMethod().IsVirtual)
                    {
                        if (objProperty.GetValue(clase) != null)
                        {
                            string value = "";
                            switch (objProperty.GetValue(clase).GetType().ToString())
                            {
                                case "System.String":
                                    value = "'" + objProperty.GetValue(clase) + "'";
                                    break;
                                case "System.DateTime":
                                    DateTime date = (DateTime)objProperty.GetValue(clase);
                                    //string dateformatted = String.Format("{0:s}", date.ToUniversalTime()); ;
                                    string dateformatted = String.Format("{0:s}", date.ToLocalTime()); ;
                                    value = "'" + dateformatted + "'";
                                    break;
                                case "System.Decimal":
                                    value = Convert.ToString(objProperty.GetValue(clase));
                                    value = value.Replace(',', '.');
                                    break;
                                case "System.Boolean":
                                    value = ((Boolean)objProperty.GetValue(clase)) ? "CAST(1 as BIT)" : "CAST(0 as BIT)";
                                    break;
                                default:
                                    value = objProperty.GetValue(clase).ToString();
                                    break;
                            };

                            colValues += "," + value;
                            colNames += "," + propname;
                        }
                    }
                };

                sql = String.Format(sql, colNames.Substring(1), colValues.Substring(1));

            }
            catch (Exception ex)
            {
                LogManager.Write(ex.Source + " : " + ex.Message);
            }

            //return "";
        }

        public static void setUpdateValues(object clase, string identitycol, ref string sql)
        {
            try
            {
                string propname = "";
                string setColumnValues = "";

                // Variables para obtener el tipo de datos en la base de datos
                string dataType = "";
                string tableName = GetTableName(sql);

                PropertyInfo[] Properties; Properties = clase.GetType().GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {

                    propname = objProperty.Name;

                    if (propname != identitycol && propname.Substring(0, 2) != "x_" && !objProperty.GetGetMethod().IsVirtual)
                    {
                        // Obtenemos el tipo de datos
                        if (CheckIfComputedColumn(tableName, propname)) continue;

                        if (objProperty.GetValue(clase) != null)
                        {
                            string value = "";
                            switch (objProperty.GetValue(clase).GetType().ToString())
                            {
                                case "System.String":
                                    value = (string)objProperty.GetValue(clase);
                                    value = value.Replace("'", "''");
                                    value = "'" + value + "'";
                                    
                                    break;
                                case "System.Decimal":
                                    value = Convert.ToString(objProperty.GetValue(clase));
                                    value = value.Replace(',', '.');
                                    break;
                                case "System.DateTime":
                                    DateTime date = (DateTime)objProperty.GetValue(clase);
                                    //string dateformatted = String.Format("{0:s}", date.ToUniversalTime());
                                    string dateformatted = String.Format("{0:s}", date.ToLocalTime());
                                    value = "'" + dateformatted + "'";
                                    break;
                                case "System.Boolean":
                                    value = ((Boolean)objProperty.GetValue(clase)) ? "CAST(1 as BIT)" : "CAST(0 as BIT)";
                                    break;
                                default:
                                    value = objProperty.GetValue(clase).ToString();
                                    break;
                            };

                            setColumnValues += "," + propname + " = " + value;
                        }
                        else
                        {
                            setColumnValues += "," + propname + " = NULL";
                        };
                    };
                };

                sql = String.Format(sql, setColumnValues.Substring(1));
            }
            catch (Exception ex)
            {
                LogManager.Write(ex.Source + " : " + ex.Message);
            }
        }

        public static string generateLikeWhere(string query, string field)
        {
            string where = "";

            while (query.IndexOf("  ") != -1)
                query = query.Replace("  ", " ");

            string[] split = query.Split(' ');

            foreach (string item in split)
            {

                if (!string.IsNullOrEmpty(item)) where += " and " + field + " like '%" + item + "%'";
            }

            if (string.IsNullOrEmpty(where))
            {
                where = "(1=1)";
            }
            else
            {
                where = "(" + where.Substring(5) + ")";
            }

            return where;
        }

        // Check if column is Computed Column
        // Check if column is Computed Column
        private static bool CheckIfComputedColumn(string tableName, string columnName)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write(ex.Source + " : " + ex.Message);
                return false;
            };

            string sqlCmd = " DECLARE @DataType bit; " +
                            " if exists(select 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}')    " +
                            " begin    " +
                            "   set @DataType=(SELECT COLUMNPROPERTY(OBJECT_ID(TABLE_SCHEMA+'.'+TABLE_NAME),COLUMN_NAME,'IsComputed') FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}')    " +
                            " select   @DataType " +
                            " End";

            sqlCmd = String.Format(sqlCmd, tableName, columnName);

            SqlCommand cmd = new SqlCommand(sqlCmd, oConn);

            bool isComputed = false;

            try
            {
                isComputed = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LogManager.Write(ex.Source + " : " + ex.Message);
                return false;
            }
            finally
            {
                ConnManager.CloseConn(oConn);
            }

            return isComputed;
        }

        private static string GetTableName(string sqlCmd)
        {
            string tableName = "";

            int a = sqlCmd.ToUpper().IndexOf("SET") - 1;

            tableName = sqlCmd.Substring(7, a - 7);

            return tableName;
        }
    }
}