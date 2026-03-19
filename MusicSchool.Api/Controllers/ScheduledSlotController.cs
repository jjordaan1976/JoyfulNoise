using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("ScheduledSlot")]
    public class ScheduledSlotController : BaseController
    {
        private readonly IScheduledSlotRepository _scheduledSlotRepository;
        private readonly ILogger<ScheduledSlotController> _logger;

        public ScheduledSlotController(ILogger<ScheduledSlotController> logger, IScheduledSlotRepository scheduledSlotRepository)
        {
            _scheduledSlotRepository = scheduledSlotRepository;
            _logger = logger;
        }

        [HttpGet("GetSlot")]
        public async Task<ResponseBase<ScheduledSlot>> GetSlot([FromQuery] int id)
        {
            ResponseBase<ScheduledSlot> response = new ResponseBase<ScheduledSlot>() { ReturnCode = -1 };
            var result = await _scheduledSlotRepository.GetSlotAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetActiveByStudent")]
        public async Task<ResponseBase<IEnumerable<ScheduledSlot>>> GetActiveByStudent([FromQuery] int studentId)
        {
            ResponseBase<IEnumerable<ScheduledSlot>> response = new ResponseBase<IEnumerable<ScheduledSlot>>() { ReturnCode = -1 };
            var result = await _scheduledSlotRepository.GetActiveByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetActiveByTeacher")]
        public async Task<ResponseBase<IEnumerable<ScheduledSlot>>> GetActiveByTeacher([FromQuery] int teacherId)
        {
            ResponseBase<IEnumerable<ScheduledSlot>> response = new ResponseBase<IEnumerable<ScheduledSlot>>() { ReturnCode = -1 };
            var result = await _scheduledSlotRepository.GetActiveByTeacherAsync(teacherId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddSlot")]
        public async Task<ResponseBase<int?>> AddSlot([FromBody] ScheduledSlot req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _scheduledSlotRepository.AddSlotAsync(req);

            if (result is null)
            {
                response.ReturnCode = -1;
                response.ReturnMessage = "Cannot add slot: the student has no active bundle with remaining credits.";
                return response;
            }

            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("CloseSlot")]
        public async Task<ResponseBase<bool>> CloseSlot([FromQuery] int slotId, [FromQuery] DateOnly effectiveTo)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _scheduledSlotRepository.CloseSlotAsync(slotId, effectiveTo);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}
