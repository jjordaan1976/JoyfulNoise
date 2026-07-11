using Microsoft.AspNetCore.Mvc;
using Tutor.Api;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.Models.TransferModels;

namespace Tutor.Controllers
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
        public Task<ResponseBase<ScheduledSlot?>> GetSlot([FromQuery] int id)
            => Execute(() => _scheduledSlotRepository.GetSlotAsync(id), _logger, "Error getting slot");

        [HttpGet("GetActiveByStudent")]
        public Task<ResponseBase<IEnumerable<ScheduledSlot>>> GetActiveByStudent([FromQuery] int studentId)
            => Execute(() => _scheduledSlotRepository.GetActiveByStudentAsync(studentId), _logger, "Error getting active slots by student");

        [HttpGet("GetActiveByTeacher")]
        public Task<ResponseBase<IEnumerable<ScheduledSlot>>> GetActiveByTeacher([FromQuery] int teacherId)
            => Execute(() => _scheduledSlotRepository.GetActiveByTeacherAsync(teacherId), _logger, "Error getting active slots by teacher");

        [HttpPost("AddSlot")]
        public Task<ResponseBase<int?>> AddSlot([FromBody] ScheduledSlot req)
            => Execute(() => _scheduledSlotRepository.AddSlotAsync(req), _logger, "Error adding slot");

        [HttpPut("CloseSlot")]
        public Task<ResponseBase<bool>> CloseSlot([FromQuery] int slotId, [FromQuery] DateOnly effectiveTo)
            => Execute(() => _scheduledSlotRepository.CloseSlotAsync(slotId, effectiveTo), _logger, "Error closing slot");
    }
}
