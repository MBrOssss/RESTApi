using AutoMapper;
using RESTApi.Models;
using RESTApi.Models.DTOs;

namespace RESTApi.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Doctor
            CreateMap<Doctor, DoctorDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
            CreateMap<DoctorDTO, Doctor>();
            CreateMap<Doctor, DoctorAddEditDTO>();
            CreateMap<DoctorAddEditDTO, Doctor>();
            #endregion
        }
    }
}
