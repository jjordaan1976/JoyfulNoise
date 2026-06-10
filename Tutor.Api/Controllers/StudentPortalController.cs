using Microsoft.AspNetCore.Mvc;
using Tutor.Data.Interfaces;
using Tutor.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("StudentPortal")]
    [ApiController]
    public class StudentPortalController : ControllerBase
    {

        private readonly IStudentRepository _studentRepository;
        private readonly ILessonBundleRepository _bundleRepository;
        private readonly IScheduledSlotRepository _slotRepository;
        private readonly ILessonRepository _lessonRepository;

        public StudentPortalController(
            IStudentRepository studentRepository,
            ILessonBundleRepository bundleRepository,
            IScheduledSlotRepository slotRepository,
            ILessonRepository lessonRepository)
        {
            _studentRepository = studentRepository;
            _bundleRepository = bundleRepository;
            _slotRepository = slotRepository;
            _lessonRepository = lessonRepository;
        }

        [HttpGet("GetStudent")]
        public async Task<ResponseBase<Tutor.Data.Models.Student>> GetStudent([FromQuery] int studentId)
        {
            var response = new ResponseBase<Tutor.Data.Models.Student> { ReturnCode = -1 };
            var result = await _studentRepository.GetStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetBundles")]
        public async Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetBundles([FromQuery] int studentId)
        {
            var response = new ResponseBase<IEnumerable<LessonBundleDetail>> { ReturnCode = -1 };
            var result = await _bundleRepository.GetByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetSlots")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.ScheduledSlot>>> GetSlots([FromQuery] int studentId)
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.ScheduledSlot>> { ReturnCode = -1 };
            var result = await _slotRepository.GetActiveByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetLessonsByBundle")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.Lesson>>> GetLessonsByBundle([FromQuery] int studentId, [FromQuery] int bundleId)
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.Lesson>> { ReturnCode = -1 };

            // Verify the bundle belongs to this student before returning lessons
            var bundles = await _bundleRepository.GetByStudentAsync(studentId);
            if (!bundles.Any(b => b.BundleID == bundleId))
            {
                response.ReturnMessage = "Bundle not found for this student.";
                return response;
            }

            var result = await _lessonRepository.GetByBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
