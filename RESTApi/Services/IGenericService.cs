using RESTApi.Models.DTOs;

namespace RESTApi.Services
{
    public interface IGenericService<TEntity, TKey, TViewEntity, TViewKey>
        where TEntity : class
        where TViewEntity : class
    {
        Task<ListResponseDTO<IList<TDTO>>> GetList<TDTO>(ListRequestDTO request);
        Task<ResponseDTO<TDTO>> GetById<TDTO>(TKey id);
        Task<ResponseDTO<TDTO>> Edit<TDTO>(TDTO model, TKey id);
        Task<ResponseDTO<TDTO>> Add<TDTO>(TDTO model);
        Task<ResponseDTO<TDTO>> Delete<TDTO>(TKey id);
        Task<ResponseDTO<TDTO>> DeleteSoft<TDTO>(TKey id);
    }
}
