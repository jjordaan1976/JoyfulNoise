using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
{
    [Route("LessonType")]
    public class LessonTypeController : BaseController
    {
        private readonly ILessonTypeRepository _lessonTypeRepository;
        private readonly ILogger<LessonTypeController> _logger;

        public LessonTypeController(ILogger<LessonTypeController> logger, ILessonTypeRepository lessonTypeRepository)
        {
            _lessonTypeRepository = lessonTypeRepository;
            _logger = logger;
        }

        [HttpGet("GetLessonType")]
        public Task<ResponseBase<LessonType?>> GetLessonType([FromQuery] int id)
            => Execute(() => _lessonTypeRepository.GetLessonTypeAsync(id), _logger, "Error getting lesson type");

        [HttpGet("GetAllActive")]
        public Task<ResponseBase<IEnumerable<LessonType>>> GetAllActive()
            => Execute(() => _lessonTypeRepository.GetAllActiveAsync(), _logger, "Error getting active lesson types");

        [HttpPost("AddLessonType")]
        public Task<ResponseBase<int?>> AddLessonType([FromBody] LessonType req)
            => Execute(() => _lessonTypeRepository.AddLessonTypeAsync(req), _logger, "Error adding lesson type");

        [HttpPut("UpdateLessonType")]
        public Task<ResponseBase<bool>> UpdateLessonType([FromBody] LessonType req)
            => Execute(() => _lessonTypeRepository.UpdateLessonTypeAsync(req), _logger, "Error updating lesson type");
    }
}
