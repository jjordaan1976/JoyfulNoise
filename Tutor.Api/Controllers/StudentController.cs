using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
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
        public Task<ResponseBase<Student?>> GetStudent([FromQuery] int id)
            => Execute(() => _studentRepository.GetStudentAsync(id), _logger, "Error getting student");

        [HttpGet("GetByAccountHolder")]
        public Task<ResponseBase<IEnumerable<Student>>> GetByAccountHolder([FromQuery] int accountHolderId)
            => Execute(() => _studentRepository.GetByAccountHolderAsync(accountHolderId), _logger, "Error getting students by account holder");

        [HttpPost("AddStudent")]
        public Task<ResponseBase<int?>> AddStudent([FromBody] Student req)
            => Execute(() => _studentRepository.AddStudentAsync(req), _logger, "Error adding student");

        [HttpPut("UpdateStudent")]
        public Task<ResponseBase<bool>> UpdateStudent([FromBody] Student req)
            => Execute(() => _studentRepository.UpdateStudentAsync(req), _logger, "Error updating student");
    }
}
