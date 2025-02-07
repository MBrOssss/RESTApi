namespace RESTApi.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            if (s.Length == 1)
                return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }

        /// <summary>
        /// Sprawdzenie poprawności danych wyszukiwania
        /// </summary>
        /// <param name="value">Wartość parametru wyszukiwania</param>
        /// <returns>Czy dane poprawne</returns>
        public static bool ValidateSearchParam(this string value)
        {
            if (value == Guid.Empty.ToString() ||
                value == int.MinValue.ToString())
                return false;
            return true;
        }
    }
}
