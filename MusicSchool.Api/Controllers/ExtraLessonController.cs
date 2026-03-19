using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("ExtraLesson")]
    public class ExtraLessonController : BaseController
    {
        private readonly IExtraLessonRepository _extraLessonRepository;
        private readonly ILogger<ExtraLessonController> _logger;

        public ExtraLessonController(ILogger<ExtraLessonController> logger, IExtraLessonRepository extraLessonRepository)
        {
            _extraLessonRepository = extraLessonRepository;
            _logger = logger;
        }

        [HttpGet("GetExtraLesson")]
        public async Task<ResponseBase<ExtraLessonDetail>> GetExtraLesson([FromQuery] int extraLessonId)
        {
            ResponseBase<ExtraLessonDetail> response = new ResponseBase<ExtraLessonDetail>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetExtraLessonAsync(extraLessonId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacherAndDate")]
        public async Task<ResponseBase<IEnumerable<ExtraLessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateOnly scheduledDate)
        {
            ResponseBase<IEnumerable<ExtraLessonDetail>> response = new ResponseBase<IEnumerable<ExtraLessonDetail>>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByStudent")]
        public async Task<ResponseBase<IEnumerable<ExtraLesson>>> GetByStudent([FromQuery] int studentId)
        {
            ResponseBase<IEnumerable<ExtraLesson>> response = new ResponseBase<IEnumerable<ExtraLesson>>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddExtraLesson")]
        public async Task<ResponseBase<int?>> AddExtraLesson([FromBody] ExtraLesson req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.AddExtraLessonAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateExtraLessonStatus")]
        public async Task<ResponseBase<bool>> UpdateExtraLessonStatus([FromQuery] int extraLessonId, [FromQuery] string status)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.UpdateExtraLessonStatusAsync(extraLessonId, status);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
