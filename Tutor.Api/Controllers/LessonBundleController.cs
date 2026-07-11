using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
{
    [Route("LessonBundle")]
    public class LessonBundleController : BaseController
    {
        private readonly ILessonBundleRepository _lessonBundleRepository;
        private readonly ILogger<LessonBundleController> _logger;

        public LessonBundleController(ILogger<LessonBundleController> logger, ILessonBundleRepository lessonBundleRepository)
        {
            _lessonBundleRepository = lessonBundleRepository;
            _logger = logger;
        }

        [HttpGet("GetBundle")]
        public Task<ResponseBase<IEnumerable<LessonBundleWithQuarterDetail>>> GetBundle([FromQuery] int bundleId)
            => Execute(() => _lessonBundleRepository.GetBundleAsync(bundleId), _logger, "Error getting bundle");

        [HttpGet("GetByStudent")]
        public Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetByStudent([FromQuery] int studentId)
            => Execute(() => _lessonBundleRepository.GetByStudentAsync(studentId), _logger, "Error getting bundles by student");

        [HttpPost("AddBundle")]
        public Task<ResponseBase<int?>> AddBundle([FromBody] AddBundleRequest req)
            => Execute(() => _lessonBundleRepository.AddBundleAsync(req.Bundle, req.Mode), _logger, "Error adding bundle");

        [HttpPut("UpdateBundle")]
        public Task<ResponseBase<bool>> UpdateBundle([FromBody] LessonBundle req)
            => Execute(() => _lessonBundleRepository.UpdateBundleAsync(req), _logger, "Error updating bundle");
    }
}
