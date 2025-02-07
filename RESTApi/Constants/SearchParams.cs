namespace RESTApi.Constants
{
    public class SearchParams
    {
        public const string SORT_ASC = "ASC";
        public const string SORT_DESC = "DESC";

        public const bool SORTING_ASC = true;
        public const bool SORTING_DESC = false;

        /// <summary>
        /// Wyszukiwanie po tekście
        /// </summary>
        public const string SEARCH_STRING = "string_";
        /// <summary>
        /// Wyszukiwanie po dacie
        /// </summary>
        public const string SEARCH_DATE = "date_";
        /// <summary>
        /// Wyszukiwanie po dacie do
        /// </summary>
        public const string SEARCH_DATE_TO = "date_to_";
        /// <summary>
        /// Wyszukiwanie po dacie od
        /// </summary>
        public const string SEARCH_DATE_FROM = "date_from_";
        /// <summary>
        /// Wyszukiwanie po dacie i czasie od
        /// </summary>
        public const string SEARCH_DATE_TIME_FROM = "datetime_from_";
        /// <summary>
        /// Wyszukiwanie po dacie i czasie do
        /// </summary>
        public const string SEARCH_DATE_TIME_TO = "datetime_to_";
        /// <summary>
        /// Wyszukiwanie wartości różnych od null
        /// </summary>
        public const string SEARCH_NOT_NULL = "not_null_";
        /// <summary>
        /// Wyszukiwanie dokładnej wartości
        /// </summary>
        public const string SEARCH_EQUAL = "equal_";
        /// <summary>
        /// Wyszukiwanie dokładnej wartości
        /// </summary>
        public const string SEARCH_NOT_EQUAL = "notequal_";
        /// <summary>
        /// Wyszukiwanie po dacie pomiędzy wartościami
        /// </summary>
        public const string SEARCH_DATE_BETWEEN = "date_between_";
        /// <summary>
        /// Wyszukiwanie w tablicy wartości where in (1,2,..)
        /// </summary>
        public const string SEARCH_IN = "in_";
        /// <summary>
        /// Recordy inne niż tablicy, where not in (1,2,..)
        /// </summary>
        public const string SEARCH_NOT_IN = "notin_";
    }
}
