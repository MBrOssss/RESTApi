using AutoMapper;
using RESTApi.Models;
using RESTApi.Repository;

namespace RESTApi.Services
{
    public class DoctorService : GenericService<Doctor, int, Doctor, int>, IDoctorService
    {
        public DoctorService(IGenericRepository<Doctor, int> repository,
            IGenericRepository<Doctor, int> viewRepository,
            IMapper mapper) : base(repository, viewRepository, mapper)
        {
        }
    }
}
