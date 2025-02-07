namespace RESTApi.Models.DTOs
{
    public class ListResponseDTO<T> : ResponseDTO<T>
    {
        public int TotalCount { get; set; }
    }
}
