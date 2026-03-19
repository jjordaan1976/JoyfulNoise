using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Api.TransferModels;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Controllers
{
    [Route("Student")]
    public class StudentController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger, IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        [HttpGet("GetStudent")]
        public async Task<ResponseBase<Student>> GetStudent([FromQuery] int id)
        {
            ResponseBase<Student> response = new ResponseBase<Student>() { ReturnCode = -1 };
            var result = await _studentRepository.GetStudentAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Student>>> GetByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Student>> response = new ResponseBase<IEnumerable<Student>>() { ReturnCode = -1 };
            var result = await _studentRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddStudent")]
        public async Task<ResponseBase<int?>> AddStudent([FromBody] Student req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _studentRepository.AddStudentAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateStudent")]
        public async Task<ResponseBase<bool>> UpdateStudent([FromBody] Student req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _studentRepository.UpdateStudentAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
