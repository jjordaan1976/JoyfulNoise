using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tutor.Api.Auth;
using Tutor.Data.Interfaces;
using Tutor.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Api
{
    [Route("StudentPortal")]
    [Authorize(AuthenticationSchemes = MagicLinkAuthenticationHandler.SchemeName)]
    [ApiController]
    public class StudentPortalController : ControllerBase
    {
        private int StudentId =>
            int.Parse(User.FindFirst(MagicLinkAuthenticationHandler.ClaimEntityId)!.Value);

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
        public async Task<ResponseBase<Tutor.Data.Models.Student>> GetStudent()
        {
            var response = new ResponseBase<Tutor.Data.Models.Student> { ReturnCode = -1 };
            var result = await _studentRepository.GetStudentAsync(StudentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetBundles")]
        public async Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetBundles()
        {
            var response = new ResponseBase<IEnumerable<LessonBundleDetail>> { ReturnCode = -1 };
            var result = await _bundleRepository.GetByStudentAsync(StudentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetSlots")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.ScheduledSlot>>> GetSlots()
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.ScheduledSlot>> { ReturnCode = -1 };
            var result = await _slotRepository.GetActiveByStudentAsync(StudentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetLessonsByBundle")]
        public async Task<ResponseBase<IEnumerable<Tutor.Data.Models.Lesson>>> GetLessonsByBundle([FromQuery] int bundleId)
        {
            var response = new ResponseBase<IEnumerable<Tutor.Data.Models.Lesson>> { ReturnCode = -1 };

            // Verify the bundle belongs to this student before returning lessons
            var bundles = await _bundleRepository.GetByStudentAsync(StudentId);
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
