using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
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
        public Task<ResponseBase<ExtraLessonDetail?>> GetExtraLesson([FromQuery] int extraLessonId)
            => Execute(() => _extraLessonRepository.GetExtraLessonAsync(extraLessonId), _logger, "Error getting extra lesson");

        [HttpGet("GetByTeacherAndDate")]
        public Task<ResponseBase<IEnumerable<ExtraLessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateTime scheduledDate)
            => Execute(() => _extraLessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate), _logger, "Error getting extra lessons by teacher and date");

        [HttpGet("GetByStudent")]
        public Task<ResponseBase<IEnumerable<ExtraLesson>>> GetByStudent([FromQuery] int studentId)
            => Execute(() => _extraLessonRepository.GetByStudentAsync(studentId), _logger, "Error getting extra lessons by student");

        [HttpPost("AddExtraLesson")]
        public Task<ResponseBase<int?>> AddExtraLesson([FromBody] ExtraLesson req)
            => Execute(() => _extraLessonRepository.AddExtraLessonAsync(req), _logger, "Error adding extra lesson");

        [HttpPut("UpdateExtraLessonStatus")]
        public Task<ResponseBase<bool>> UpdateExtraLessonStatus(
            [FromQuery] int extraLessonId,
            [FromQuery] string status,
            [FromQuery] string? note = null)
            => Execute(() => _extraLessonRepository.UpdateExtraLessonStatusAsync(extraLessonId, status, note), _logger, "Error updating extra lesson status");
    }
}
