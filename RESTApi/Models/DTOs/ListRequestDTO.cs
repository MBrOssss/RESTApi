using Newtonsoft.Json;
using RESTApi.Extensions;

namespace RESTApi.Models.DTOs
{
    public class ListRequestDTO
    {
        public int? Page { get; set; }
        public int? PerPage { get; set; }
        public string? Sort { get; set; }
        public string? Filter { get; set; }

        public KeyValuePair<string, string>? SortObject
        {
            get
            {

                if (string.IsNullOrEmpty(Sort))
                {
                    return null;
                }

                var obj = JsonConvert.DeserializeObject<string[]>(Sort);

                if (obj.Length == 2)
                {
                    return new KeyValuePair<string, string>(obj[0].CapitalizeFirstLetter(), obj[1]);
                }

                return null;
            }
        }

        public Dictionary<string, string>? FilterObject
        {
            get
            {
                if (string.IsNullOrEmpty(Filter))
                {
                    return null;
                }

                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);

                return obj;
            }
        }

        public void AddFilterObjectItem(string name, string value)
        {
            if (string.IsNullOrEmpty(Filter))
            {
                Filter = "{}";
            }

            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            obj.Add(name, value);
            Filter = JsonConvert.SerializeObject(obj);
        }

        public void SetFilterObjectItem(string name, string value)
        {
            Filter = string.Empty;

            //var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(Filter);
            var obj = new Dictionary<string, string>();
            obj.Add(name, value);
            Filter = JsonConvert.SerializeObject(obj);
        }
    }
}
