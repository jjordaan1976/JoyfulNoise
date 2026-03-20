using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Lesson")]
    public class LessonController : BaseController
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<LessonController> _logger;

        public LessonController(ILogger<LessonController> logger, ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
            _logger = logger;
        }

        [HttpGet("GetLesson")]
        public async Task<ResponseBase<LessonDetail>> GetLesson([FromQuery] int lessonId)
        {
            ResponseBase<LessonDetail> response = new ResponseBase<LessonDetail>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetLessonAsync(lessonId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByBundle")]
        public async Task<ResponseBase<IEnumerable<Lesson>>> GetByBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<Lesson>> response = new ResponseBase<IEnumerable<Lesson>>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetByBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacherAndDate")]
        public async Task<ResponseBase<IEnumerable<LessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateTime scheduledDate)
        {
            ResponseBase<IEnumerable<LessonDetail>> response = new ResponseBase<IEnumerable<LessonDetail>>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddLesson")]
        public async Task<ResponseBase<int?>> AddLesson([FromBody] Lesson req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _lessonRepository.AddLessonAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateLessonStatus")]
        public async Task<ResponseBase<bool>> UpdateLessonStatus(
            [FromQuery] int lessonId,
            [FromQuery] string status,
            [FromQuery] string? note = null)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonRepository.UpdateLessonStatusAsync(
                lessonId, status,
                creditForfeited: status == LessonStatus.Forfeited,
                cancelledBy: status == LessonStatus.CancelledTeacher ? CancelledBy.Teacher
                           : status == LessonStatus.CancelledStudent || status == LessonStatus.Forfeited ? CancelledBy.Student
                           : null,
                cancellationReason: null,
                completedAt: status == LessonStatus.Completed ? DateTime.UtcNow : null,
                note: note);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
