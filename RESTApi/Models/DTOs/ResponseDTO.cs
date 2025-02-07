using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RESTApi.Models.DTOs
{
    public class ResponseDTO<T>
    {
        public bool Succeeded { get; set; } = true;
        public T? Data { get; set; }
        public string? Message { get; set; } = "OK";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), Formatting = Formatting.Indented });
        }
    }
}
