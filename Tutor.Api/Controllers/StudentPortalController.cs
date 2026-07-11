using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("StudentPortal")]
    public class StudentPortalController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILessonBundleRepository _bundleRepository;
        private readonly IScheduledSlotRepository _slotRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<StudentPortalController> _logger;

        public StudentPortalController(
            IStudentRepository studentRepository,
            ILessonBundleRepository bundleRepository,
            IScheduledSlotRepository slotRepository,
            ILessonRepository lessonRepository,
            ICurrentUserService currentUser,
            ILogger<StudentPortalController> logger)
        {
            _studentRepository = studentRepository;
            _bundleRepository = bundleRepository;
            _slotRepository = slotRepository;
            _lessonRepository = lessonRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet("GetStudent")]
        public Task<ResponseBase<Tutor.Data.Models.Student?>> GetStudent()
            => Execute(() => _studentRepository.GetStudentAsync(_currentUser.RequireStudentId()), _logger, "Error getting student");

        [HttpGet("GetBundles")]
        public Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetBundles()
            => Execute(() => _bundleRepository.GetByStudentAsync(_currentUser.RequireStudentId()), _logger, "Error getting bundles");

        [HttpGet("GetSlots")]
        public Task<ResponseBase<IEnumerable<Tutor.Data.Models.ScheduledSlot>>> GetSlots()
            => Execute(() => _slotRepository.GetActiveByStudentAsync(_currentUser.RequireStudentId()), _logger, "Error getting slots");

        [HttpGet("GetLessonsByBundle")]
        public Task<ResponseBase<IEnumerable<Tutor.Data.Models.Lesson>>> GetLessonsByBundle([FromQuery] int bundleId)
            => Execute(() => _lessonRepository.GetByBundleForStudentAsync(bundleId, _currentUser.RequireStudentId()), _logger, "Error getting lessons");
    }
}
