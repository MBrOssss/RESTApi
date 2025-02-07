using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTApi.Constants;
using RESTApi.Models.DTOs;
using RESTApi.Services;

namespace RESTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorsController(IDoctorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ListResponseDTO<IList<DoctorDTO>>> GetList([FromQuery] ListRequestDTO model)
        {
            return await _service.GetList<DoctorDTO>(model);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<ResponseDTO<DoctorDTO>> Get(int id)
        {
            return await _service.GetById<DoctorDTO>(id);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDTO<DoctorAddEditDTO>> Add(DoctorAddEditDTO model)
        {
            return await _service.Add(model);
        }

        [Route("{id:int}")]
        [HttpPut]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDTO<DoctorAddEditDTO>> Edit(int id, DoctorAddEditDTO model)
        {
            return await _service.Edit(model, id);
        }

        [Route("{id:int}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ResponseDTO<DoctorDTO>> Delete(int id)
        {
            return await _service.Delete<DoctorDTO>(id);
        }
    }
}
