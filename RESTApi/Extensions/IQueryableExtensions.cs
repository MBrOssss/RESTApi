using System.Linq.Expressions;
using System.Reflection;

namespace RESTApi.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Generyczne ustawianie kolejności wyszukiwania
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="ordering">Kolumna, po której ma zostać wybrana kolejność</param>
        /// <param name="values">Kolejność wyszukiwania</param>
        /// <returns>Wyrażenie ustawiające kolejność</returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperty(ordering);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            string methodName = values.Length > 0 && (bool)values[0] == false ? "OrderByDescending" : "OrderBy";

            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości tekstowej
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterString<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            MemberExpression member = Expression.Property(item, searchColName);
            ConstantExpression constant = Expression.Constant(searchValue);
            MethodInfo toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { });
            Expression columnToLowerExp = Expression.Call(member, toLowerMethod);
            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

            var body = Expression.Call(columnToLowerExp, containsMethod, constant);

            return source.Where(Expression.Lambda<Func<T, bool>>(body, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty (dokładnej)
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateEqual<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            DateTime currentDate = DateTime.Parse(searchValue.Substring(0, 10));
            DateTime nextDate = currentDate.AddDays(1);
            var ex1 = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(currentDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            var ex2 = Expression.LessThan(propertyExpression, Expression.Constant(nextDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            var body = Expression.AndAlso(ex1, ex2);
            return source.Where(Expression.Lambda<Func<T, bool>>(body, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości w składni in
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterIn<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            BinaryExpression? expression;
            BinaryExpression? expression2;

            List<string> where_in = searchValue.Split(",").ToList();
            if (where_in.Count > 0)
            {
                if (propertyExpression.Type == typeof(Guid))
                    expression = Expression.Equal(propertyExpression, Expression.Constant(Guid.Parse(where_in[0])));
                if (propertyExpression.Type == typeof(int))
                    expression = Expression.Equal(propertyExpression, Expression.Constant(int.Parse(where_in[0])));
                else
                    expression = Expression.Equal(propertyExpression, Expression.Constant(where_in[0]));
                foreach (string one_item in where_in)
                {
                    if (propertyExpression.Type == typeof(Guid))
                        expression2 = Expression.Equal(propertyExpression, Expression.Constant(Guid.Parse(one_item)));
                    if (propertyExpression.Type == typeof(int))
                        expression2 = Expression.Equal(propertyExpression, Expression.Constant(int.Parse(one_item)));
                    else
                        expression2 = Expression.Equal(propertyExpression, Expression.Constant(one_item));
                    expression = Expression.Or(expression, expression2);
                }
                return source.Where(Expression.Lambda<Func<T, bool>>(expression, item));
            }
            else
                return source;
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości w skłądni in
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterNotIn<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            BinaryExpression? expression;
            BinaryExpression? expression2;

            List<string> where_in = searchValue.Split(",").ToList();
            if (where_in.Count > 0)
            {
                if (propertyExpression.Type == typeof(Guid))
                    expression = Expression.NotEqual(propertyExpression, Expression.Constant(Guid.Parse(where_in[0])));
                if (propertyExpression.Type == typeof(int))
                    expression = Expression.NotEqual(propertyExpression, Expression.Constant(int.Parse(where_in[0])));
                else
                    expression = Expression.NotEqual(propertyExpression, Expression.Constant(where_in[0]));
                foreach (string one_item in where_in)
                {
                    if (propertyExpression.Type == typeof(Guid))
                        expression2 = Expression.NotEqual(propertyExpression, Expression.Constant(Guid.Parse(one_item)));
                    if (propertyExpression.Type == typeof(int))
                        expression2 = Expression.NotEqual(propertyExpression, Expression.Constant(int.Parse(one_item)));
                    else
                        expression2 = Expression.NotEqual(propertyExpression, Expression.Constant(one_item));
                    expression = Expression.And(expression, expression2);
                }
                return source.Where(Expression.Lambda<Func<T, bool>>(expression, item));
            }
            else
                return source;
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości równych
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterEqualValue<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            BinaryExpression? body;

            var propertyExpression = Expression.Property(item, searchColName);
            var type = propertyExpression.Type;
            if (propertyExpression.Type == typeof(bool) || propertyExpression.Type == typeof(bool?))
            {
                body = Expression.Equal(propertyExpression, Expression.Convert(Expression.Constant(Convert.ToBoolean(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(int?) || propertyExpression.Type == typeof(int))
            {
                body = Expression.Equal(propertyExpression, Expression.Convert(Expression.Constant(string.IsNullOrEmpty(searchValue) ? null : int.Parse(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(float?) || propertyExpression.Type == typeof(float))
            {
                body = Expression.Equal(propertyExpression, Expression.Convert(Expression.Constant(string.IsNullOrEmpty(searchValue) ? null : float.Parse(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(Guid?) || propertyExpression.Type == typeof(Guid))
            {
                body = Expression.Equal(propertyExpression, Expression.Convert(Expression.Constant(Guid.Parse(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(string))
            {
                body = Expression.Equal(propertyExpression, Expression.Convert(Expression.Constant(searchValue), propertyExpression.Type));
            }
            else
            {
                throw new InvalidFilterCriteriaException();
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(body, item));
        }


        /// <summary>
        /// Generyczne wyszukiwanie wartości równych
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterNotEqualValue<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            BinaryExpression? body;

            var propertyExpression = Expression.Property(item, searchColName);
            if (propertyExpression.Type == typeof(bool) || propertyExpression.Type == typeof(bool?))
            {
                body = Expression.NotEqual(propertyExpression, Expression.Convert(Expression.Constant(Convert.ToBoolean(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(int?) || propertyExpression.Type == typeof(int))
            {
                body = Expression.NotEqual(propertyExpression, Expression.Convert(Expression.Constant(string.IsNullOrEmpty(searchValue) ? null : int.Parse(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(float?) || propertyExpression.Type == typeof(float))
            {
                body = Expression.NotEqual(propertyExpression, Expression.Convert(Expression.Constant(string.IsNullOrEmpty(searchValue) ? null : float.Parse(searchValue)), propertyExpression.Type));
            }
            else if (propertyExpression.Type == typeof(Guid?) || propertyExpression.Type == typeof(Guid))
            {
                body = Expression.NotEqual(propertyExpression, Expression.Convert(Expression.Constant(Guid.Parse(searchValue)), propertyExpression.Type));
            }
            else
            {
                throw new InvalidFilterCriteriaException();
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(body, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości która są lub nie są null
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Czy szukany jest null / not-null</param>
        /// <param name="isNull">Odwrócenie warunku tak/nie</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterNull<T>(this IQueryable<T> source, string searchColName, string searchValue, bool isNull = true)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            BinaryExpression body;
            if (searchValue.ToLower() != isNull.ToString().ToLower())
                body = Expression.NotEqual(propertyExpression, Expression.Constant(null));
            else body = Expression.Equal(propertyExpression, Expression.Constant(null));
            return source.Where(Expression.Lambda<Func<T, bool>>(body, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty od
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateFrom<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            DateTime currentDate = DateTime.Parse(searchValue.Substring(0, 10));
            DateTime nextDate = currentDate.AddDays(1);
            var ex1 = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(currentDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            return source.Where(Expression.Lambda<Func<T, bool>>(ex1, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty i czasu od
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateTimeFrom<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            DateTime currentDate = DateTime.Parse(searchValue);
            var ex1 = Expression.GreaterThanOrEqual(propertyExpression, Expression.Constant(currentDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            return source.Where(Expression.Lambda<Func<T, bool>>(ex1, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty i czasu do
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateTimeTo<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            DateTime currentDate = DateTime.Parse(searchValue);
            var ex1 = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(currentDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            return source.Where(Expression.Lambda<Func<T, bool>>(ex1, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty do
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchColName">Kolumna, w której ma zostać wyszukana wartość</param>
        /// <param name="searchValue">Poszukiwana wartość</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateTo<T>(this IQueryable<T> source, string searchColName, string searchValue)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpression = Expression.Property(item, searchColName);
            DateTime currentDate = DateTime.Parse(searchValue.Substring(0, 10));
            var ex1 = Expression.LessThanOrEqual(propertyExpression, Expression.Constant(currentDate, propertyExpression.Type == typeof(DateTime?) ? typeof(DateTime?) : typeof(DateTime)));
            return source.Where(Expression.Lambda<Func<T, bool>>(ex1, item));
        }

        /// <summary>
        /// Generyczne wyszukiwanie wartości daty w przedziale
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="searchFromColName">Kolumna, w której ma zostać wyszukana wartość od</param>
        /// <param name="searchToColName">Kolumna, w której ma zostać wyszukana wartość do</param>
        /// <param name="isValid">Prawidłowe</param>
        /// <returns>Wyrażenie filtrujące</returns>
        public static IQueryable<T> FilterDateValid<T>(this IQueryable<T> source, string searchFromColName, string searchToColName, bool isValid)
        {
            var item = Expression.Parameter(typeof(T), "o");
            var propertyExpressionFrom = Expression.Property(item, searchFromColName);
            var propertyExpressionTo = Expression.Property(item, searchToColName);
            DateTime currentDate = DateTime.Now;
            var expr1 = Expression.GreaterThanOrEqual(!isValid ? propertyExpressionFrom : propertyExpressionTo, Expression.Constant(currentDate));
            var expr2 = Expression.LessThanOrEqual(!isValid ? propertyExpressionTo : propertyExpressionFrom, Expression.Constant(currentDate));
            var exprComb = isValid ? Expression.And(expr1, expr2) : Expression.Or(expr1, expr2);
            return source.Where(Expression.Lambda<Func<T, bool>>(exprComb, item));
        }
    }
}
