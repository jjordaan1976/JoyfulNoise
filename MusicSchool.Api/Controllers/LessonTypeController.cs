using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Api.TransferModels;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Controllers
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
        public async Task<ResponseBase<LessonType>> GetLessonType([FromQuery] int id)
        {
            ResponseBase<LessonType> response = new ResponseBase<LessonType>() { ReturnCode = -1 };
            var result = await _lessonTypeRepository.GetLessonTypeAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetAllActive")]
        public async Task<ResponseBase<IEnumerable<LessonType>>> GetAllActive()
        {
            ResponseBase<IEnumerable<LessonType>> response = new ResponseBase<IEnumerable<LessonType>>() { ReturnCode = -1 };
            var result = await _lessonTypeRepository.GetAllActiveAsync();
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddLessonType")]
        public async Task<ResponseBase<int?>> AddLessonType([FromBody] LessonType req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _lessonTypeRepository.AddLessonTypeAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateLessonType")]
        public async Task<ResponseBase<bool>> UpdateLessonType([FromBody] LessonType req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonTypeRepository.UpdateLessonTypeAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
