using System.Reflection;

namespace RESTApi.Extensions
{
    public static class TEntityExtension
    {
        private static readonly List<string> DeleteColumnNames = ["Deleted", "IsDeleted", "Removed", "IsRemoved"];
        private static readonly List<string> IdColumnNames = ["Id"];

        private static PropertyInfo GetColumn<T>(T dbObject, List<string> possibleNames)
        {
            PropertyInfo columnProperty = null;
            foreach (string colmn in possibleNames)
            {
                columnProperty = dbObject.GetType().GetProperty(colmn);
                if (columnProperty != null)
                    return columnProperty;

            }
            return columnProperty;
        }

        public static PropertyInfo GetDeletedColumn<T>(this T dbObject)
        {
            return GetColumn(dbObject, DeleteColumnNames);
        }

        public static PropertyInfo GetIdColumn<T>(this T dbObject)
        {
            return GetColumn(dbObject, IdColumnNames);
        }

        public static object? GetIdColumnValue<T>(this T dbObject)
        {
            object? idValue = dbObject.GetIdColumn().GetValue(dbObject);
            return idValue;
        }

        public static PropertyInfo GetColumnByName<T>(this T dbObject, string columnName)
        {
            PropertyInfo id = dbObject.GetType().GetProperty(columnName);
            return id;
        }

        public static object GetColumnValue<T>(this T dbObject, string propName)
        {
            return dbObject.GetType().GetProperty(propName)?.GetValue(dbObject, null);
        }

        public static void SetColumnValue<T>(this T dbObject, string columnName, object value)
        {
            dbObject.GetType().GetProperty(columnName).SetValue(dbObject, value);
        }
    }
}