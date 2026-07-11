using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
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
        public Task<ResponseBase<LessonDetail?>> GetLesson([FromQuery] int lessonId)
            => Execute(() => _lessonRepository.GetLessonAsync(lessonId), _logger, "Error getting lesson");

        [HttpGet("GetByBundle")]
        public Task<ResponseBase<IEnumerable<Lesson>>> GetByBundle([FromQuery] int bundleId)
            => Execute(() => _lessonRepository.GetByBundleAsync(bundleId), _logger, "Error getting lessons by bundle");

        [HttpGet("GetByTeacherAndDate")]
        public Task<ResponseBase<IEnumerable<LessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateTime scheduledDate)
            => Execute(() => _lessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate), _logger, "Error getting lessons by teacher and date");

        [HttpPost("AddLesson")]
        public Task<ResponseBase<int?>> AddLesson([FromBody] Lesson req)
            => Execute(() => _lessonRepository.AddLessonAsync(req), _logger, "Error adding lesson");

        [HttpPut("UpdateLessonStatus")]
        public Task<ResponseBase<bool>> UpdateLessonStatus(
            [FromQuery] int lessonId,
            [FromQuery] string status,
            [FromQuery] string? note = null)
            => Execute(() => _lessonRepository.UpdateLessonStatusAsync(
                lessonId, status,
                creditForfeited: status == LessonStatus.Forfeited,
                cancelledBy: status == LessonStatus.CancelledTeacher ? CancelledBy.Teacher
                           : status == LessonStatus.CancelledStudent || status == LessonStatus.Forfeited ? CancelledBy.Student
                           : null,
                cancellationReason: null,
                completedAt: status == LessonStatus.Completed ? DateTime.UtcNow : null,
                note: note), _logger, "Error updating lesson status");

        [HttpPut("RescheduleLesson")]
        public Task<ResponseBase<bool>> RescheduleLesson(
            [FromQuery] int lessonId,
            [FromQuery] DateTime newDate,
            [FromQuery] TimeOnly newTime)
            => Execute(() => _lessonRepository.RescheduleLessonAsync(lessonId, newDate, newTime), _logger, "Error rescheduling lesson");
    }
}
