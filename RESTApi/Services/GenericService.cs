using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RESTApi.Extensions;
using RESTApi.Models.DTOs;
using RESTApi.Repository;
using System.Reflection;

namespace RESTApi.Services
{
    public class GenericService<TEntity, TKey, TViewEntity, TViewKey> : IGenericService<TEntity, TKey, TViewEntity, TViewKey>
        where TEntity : class
        where TViewEntity : class
    {
        private readonly IGenericRepository<TEntity, TKey> _repository;
        private readonly IGenericRepository<TViewEntity, TViewKey> _viewRepository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<TEntity, TKey> repository,
            IGenericRepository<TViewEntity, TViewKey> viewRepository,
            IMapper mapper
            )
        {
            _repository = repository;
            _viewRepository = viewRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDTO<TDTO>> Add<TDTO>(TDTO model)
        {
            var newEnt = await _repository.AddAsync(_mapper.Map<TEntity>(model));

            return new ResponseDTO<TDTO>
            {
                Data = _mapper.Map<TDTO>(newEnt)
            };
        }

        public async Task<ResponseDTO<TDTO>> Edit<TDTO>(TDTO model, TKey id)
        {
            var dbObject = _repository.GetById(id);
            dbObject = _mapper.Map(model, dbObject);

            var ent = await _repository.UpdateAsync(dbObject);

            return new ResponseDTO<TDTO>
            {
                Data = _mapper.Map<TDTO>(ent)
            };
        }

        public async Task<ResponseDTO<TDTO>> DeleteSoft<TDTO>(TKey id)
        {
            var dbObject = _repository.GetById(id);
            PropertyInfo deleted = dbObject.GetDeletedColumn();
            if (deleted == null)
                throw new Exception("Brak w obiekcie kolumny Deleted");

            deleted.SetValue(dbObject, true);
            var ent = await _repository.UpdateAsync(dbObject);
            return new ResponseDTO<TDTO>
            {
                Data = _mapper.Map<TDTO>(ent)
            };
        }

        public async Task<ResponseDTO<TDTO>> GetById<TDTO>(TKey id)
        {
            var obj = await _repository.GetByIdAsync(id);
            var data = _mapper.Map<TDTO>(obj);
            return new ResponseDTO<TDTO>
            {
                Data = data
            };
        }

        public async Task<ListResponseDTO<IList<TDTO>>> GetList<TDTO>(ListRequestDTO model)
        {
            var list = await _viewRepository.Search(model, out int filteredCount, out int totalCount).ToListAsync();
            var listDto = new List<TDTO>();

            foreach (var item in list)
            {
                listDto.Add(_mapper.Map<TDTO>(item));
            }
            return new ListResponseDTO<IList<TDTO>>()
            {
                Data = listDto,
                TotalCount = filteredCount
            };
        }

        public async Task<ResponseDTO<TDTO>> Delete<TDTO>(TKey id)
        {
            var obj = await _repository.GetByIdAsync(id);

            try
            {
                await _repository.RemoveAsync(obj);

                return new ResponseDTO<TDTO>
                {
                    Data = _mapper.Map<TDTO>(obj)
                };
            }
            catch
            {
                return new ResponseDTO<TDTO>
                {
                    Message = "Błąd! Ten element jest używany i nie można go usunąć.",
                    Succeeded = false
                };
            }
        }
    }
}
