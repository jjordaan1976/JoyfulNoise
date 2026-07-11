using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
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
        public Task<ResponseBase<Teacher?>> GetTeacher([FromQuery] int id)
            => Execute(() => _teacherRepository.GetTeacherAsync(id), _logger, "Error getting teacher");

        [HttpGet("GetAllActive")]
        public Task<ResponseBase<IEnumerable<Teacher>>> GetAllActive()
            => Execute(() => _teacherRepository.GetAllActiveAsync(), _logger, "Error getting active teachers");

        [HttpPost("AddTeacher")]
        public Task<ResponseBase<int?>> AddTeacher([FromBody] Teacher req)
            => Execute(() => _teacherRepository.AddTeacherAsync(req), _logger, "Error adding teacher");

        [HttpPut("UpdateTeacher")]
        public Task<ResponseBase<bool>> UpdateTeacher([FromBody] Teacher req)
            => Execute(() => _teacherRepository.UpdateTeacherAsync(req), _logger, "Error updating teacher");
    }
}
