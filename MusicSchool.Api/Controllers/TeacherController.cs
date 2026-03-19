using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Teacher")]
    public class TeacherController : BaseController
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ILogger<TeacherController> logger, ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        [HttpGet("GetTeacher")]
        public async Task<ResponseBase<Teacher>> GetTeacher([FromQuery] int id)
        {
            ResponseBase<Teacher> response = new ResponseBase<Teacher>() { ReturnCode = -1 };
            var result = await _teacherRepository.GetTeacherAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetAllActive")]
        public async Task<ResponseBase<IEnumerable<Teacher>>> GetAllActive()
        {
            ResponseBase<IEnumerable<Teacher>> response = new ResponseBase<IEnumerable<Teacher>>() { ReturnCode = -1 };
            var result = await _teacherRepository.GetAllActiveAsync();
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddTeacher")]
        public async Task<ResponseBase<int?>> AddTeacher([FromBody] Teacher req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _teacherRepository.AddTeacherAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateTeacher")]
        public async Task<ResponseBase<bool>> UpdateTeacher([FromBody] Teacher req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _teacherRepository.UpdateTeacherAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
