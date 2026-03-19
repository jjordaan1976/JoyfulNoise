using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Api.TransferModels;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Controllers
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
        public async Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<LessonBundleDetail>> response = new ResponseBase<IEnumerable<LessonBundleDetail>>() { ReturnCode = -1 };
            var result = await _lessonBundleRepository.GetBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByStudent")]
        public async Task<ResponseBase<IEnumerable<LessonBundle>>> GetByStudent([FromQuery] int studentId)
        {
            ResponseBase<IEnumerable<LessonBundle>> response = new ResponseBase<IEnumerable<LessonBundle>>() { ReturnCode = -1 };
            var result = await _lessonBundleRepository.GetByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddBundle")]
        public async Task<ResponseBase<int?>> AddBundle([FromBody] AddBundleRequest req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _lessonBundleRepository.AddBundleAsync(req.Bundle, req.Quarters);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateBundle")]
        public async Task<ResponseBase<bool>> UpdateBundle([FromBody] LessonBundle req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonBundleRepository.UpdateBundleAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
