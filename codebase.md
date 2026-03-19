# Flattened Codebase

Generated: 03/20/2026 00:36:20


## File: MusicSchool.Api\Controllers\AccountHolderController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Api
{
    [Route("AccountHolder")]
    public class AccountHolderController : BaseController
    {
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly ILogger<AccountHolderController> _logger;

        public AccountHolderController(ILogger<AccountHolderController> logger, IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
            _logger = logger;
        }

        [HttpGet("GetAccountHolder")]
        public async Task<ResponseBase<AccountHolder>> GetAccountHolder([FromQuery] int id)
        {
            ResponseBase<AccountHolder> response = new ResponseBase<AccountHolder>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.GetAccountHolderAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacher")]
        public async Task<ResponseBase<IEnumerable<AccountHolder>>> GetByTeacher([FromQuery] int teacherId)
        {
            ResponseBase<IEnumerable<AccountHolder>> response = new ResponseBase<IEnumerable<AccountHolder>>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.GetByTeacherAsync(teacherId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddAccountHolder")]
        public async Task<ResponseBase<int?>> AddAccountHolder([FromBody] AccountHolder req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.AddAccountHolderAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateAccountHolder")]
        public async Task<ResponseBase<bool>> UpdateAccountHolder([FromBody] AccountHolder req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _accountHolderRepository.UpdateAccountHolderAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Controllers\BaseController.cs

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MusicSchool.Api
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
    }
}

```

## File: MusicSchool.Api\Controllers\ExtraLessonController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
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
        public async Task<ResponseBase<ExtraLessonDetail>> GetExtraLesson([FromQuery] int extraLessonId)
        {
            ResponseBase<ExtraLessonDetail> response = new ResponseBase<ExtraLessonDetail>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetExtraLessonAsync(extraLessonId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacherAndDate")]
        public async Task<ResponseBase<IEnumerable<ExtraLessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateTime scheduledDate)
        {
            ResponseBase<IEnumerable<ExtraLessonDetail>> response = new ResponseBase<IEnumerable<ExtraLessonDetail>>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByStudent")]
        public async Task<ResponseBase<IEnumerable<ExtraLesson>>> GetByStudent([FromQuery] int studentId)
        {
            ResponseBase<IEnumerable<ExtraLesson>> response = new ResponseBase<IEnumerable<ExtraLesson>>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.GetByStudentAsync(studentId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddExtraLesson")]
        public async Task<ResponseBase<int?>> AddExtraLesson([FromBody] ExtraLesson req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.AddExtraLessonAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateExtraLessonStatus")]
        public async Task<ResponseBase<bool>> UpdateExtraLessonStatus([FromQuery] int extraLessonId, [FromQuery] string status)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.UpdateExtraLessonStatusAsync(extraLessonId, status);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Controllers\InvoiceController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Invoice")]
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(ILogger<InvoiceController> logger, IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
        }

        [HttpGet("GetInvoice")]
        public async Task<ResponseBase<Invoice>> GetInvoice([FromQuery] int id)
        {
            ResponseBase<Invoice> response = new ResponseBase<Invoice>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetInvoiceAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByBundle")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetByBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetByBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetOutstandingByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Invoice>>> GetOutstandingByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Invoice>> response = new ResponseBase<IEnumerable<Invoice>>() { ReturnCode = -1 };
            var result = await _invoiceRepository.GetOutstandingByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateInvoiceStatus")]
        public async Task<ResponseBase<bool>> UpdateInvoiceStatus([FromQuery] int invoiceId, [FromQuery] string status, [FromQuery] DateOnly? paidDate)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _invoiceRepository.UpdateInvoiceStatusAsync(invoiceId, status, paidDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Controllers\LessonBundleController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;

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
        public async Task<ResponseBase<IEnumerable<LessonBundleWithQuarterDetail>>> GetBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<LessonBundleWithQuarterDetail>> response = new ResponseBase<IEnumerable<LessonBundleWithQuarterDetail>>() { ReturnCode = -1 };
            var result = await _lessonBundleRepository.GetBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByStudent")]
        public async Task<ResponseBase<IEnumerable<LessonBundleDetail>>> GetByStudent([FromQuery] int studentId)
        {
            ResponseBase<IEnumerable<LessonBundleDetail>> response = new ResponseBase<IEnumerable<LessonBundleDetail>>() { ReturnCode = -1 };
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

```

## File: MusicSchool.Api\Controllers\LessonController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Lesson")]
    public class LessonController : BaseController
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ILogger<LessonController> _logger;

        public LessonController(ILogger<LessonController> logger, ILessonRepository lessonRepository)
        {
            _lessonRepository = lessonRepository;
            _logger = logger;
        }

        [HttpGet("GetLesson")]
        public async Task<ResponseBase<LessonDetail>> GetLesson([FromQuery] int lessonId)
        {
            ResponseBase<LessonDetail> response = new ResponseBase<LessonDetail>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetLessonAsync(lessonId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByBundle")]
        public async Task<ResponseBase<IEnumerable<Lesson>>> GetByBundle([FromQuery] int bundleId)
        {
            ResponseBase<IEnumerable<Lesson>> response = new ResponseBase<IEnumerable<Lesson>>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetByBundleAsync(bundleId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByTeacherAndDate")]
        public async Task<ResponseBase<IEnumerable<LessonDetail>>> GetByTeacherAndDate([FromQuery] int teacherId, [FromQuery] DateTime scheduledDate)
        {
            ResponseBase<IEnumerable<LessonDetail>> response = new ResponseBase<IEnumerable<LessonDetail>>() { ReturnCode = -1 };
            var result = await _lessonRepository.GetByTeacherAndDateAsync(teacherId, scheduledDate);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddLesson")]
        public async Task<ResponseBase<int?>> AddLesson([FromBody] Lesson req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _lessonRepository.AddLessonAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateLessonStatus")]
        public async Task<ResponseBase<bool>> UpdateLessonStatus([FromQuery] int lessonId, [FromQuery] string status)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonRepository.UpdateLessonStatusAsync(
                lessonId, status,
                creditForfeited: status == LessonStatus.Forfeited,
                cancelledBy: status == LessonStatus.CancelledTeacher ? CancelledBy.Teacher
                           : status == LessonStatus.CancelledStudent || status == LessonStatus.Forfeited ? CancelledBy.Student
                           : null,
                cancellationReason: null,
                completedAt: status == LessonStatus.Completed ? DateTime.UtcNow : null);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Controllers\LessonTypeController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

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

```

## File: MusicSchool.Api\Controllers\ScheduledSlotController.cs

```csharp
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

```

## File: MusicSchool.Api\Controllers\StudentController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Student")]
    public class StudentController : BaseController
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger, IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }

        [HttpGet("GetStudent")]
        public async Task<ResponseBase<Student>> GetStudent([FromQuery] int id)
        {
            ResponseBase<Student> response = new ResponseBase<Student>() { ReturnCode = -1 };
            var result = await _studentRepository.GetStudentAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Student>>> GetByAccountHolder([FromQuery] int accountHolderId)
        {
            ResponseBase<IEnumerable<Student>> response = new ResponseBase<IEnumerable<Student>>() { ReturnCode = -1 };
            var result = await _studentRepository.GetByAccountHolderAsync(accountHolderId);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddStudent")]
        public async Task<ResponseBase<int?>> AddStudent([FromBody] Student req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _studentRepository.AddStudentAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateStudent")]
        public async Task<ResponseBase<bool>> UpdateStudent([FromBody] Student req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _studentRepository.UpdateStudentAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Controllers\TeacherController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Api;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Controllers
{
    [Route("Teacher")]
    public class TeacherController : BaseController
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ILogger<TeacherController> logger, ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        [HttpGet("GetTeacher")]
        public async Task<ResponseBase<Teacher>> GetTeacher([FromQuery] int id)
        {
            ResponseBase<Teacher> response = new ResponseBase<Teacher>() { ReturnCode = -1 };
            var result = await _teacherRepository.GetTeacherAsync(id);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpGet("GetAllActive")]
        public async Task<ResponseBase<IEnumerable<Teacher>>> GetAllActive()
        {
            ResponseBase<IEnumerable<Teacher>> response = new ResponseBase<IEnumerable<Teacher>>() { ReturnCode = -1 };
            var result = await _teacherRepository.GetAllActiveAsync();
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPost("AddTeacher")]
        public async Task<ResponseBase<int?>> AddTeacher([FromBody] Teacher req)
        {
            ResponseBase<int?> response = new ResponseBase<int?>() { ReturnCode = -1 };
            var result = await _teacherRepository.AddTeacherAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("UpdateTeacher")]
        public async Task<ResponseBase<bool>> UpdateTeacher([FromBody] Teacher req)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _teacherRepository.UpdateTeacherAsync(req);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }
    }
}

```

## File: MusicSchool.Api\Properties\launchSettings.json

```json
{
  "profiles": {
    "MusicSchool.Api": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:64100;http://localhost:64101"
    }
  }
}
```

## File: MusicSchool.Api\appsettings.Development.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Information",
        "System": "Information"
      }
    }
  }
}

```

## File: MusicSchool.Api\appsettings.json

```json
{
  "ConnectionStrings": {
    "MusicSchool": "Server=.;Database=JoyfullMusicSchool;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/musicschool-.log",
          "rollingInterval": "Day",
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ]
  },
  "AllowedHosts": "*"
}

```

## File: MusicSchool.Api\MusicSchool.Api.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>MusicSchool.Api</RootNamespace>
    <AssemblyName>MusicSchool.Api</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.72" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.5" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.2" />
    <PackageReference Include="Scalar.AspNetCore" Version="2.5.4" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Interfaces\MusicSchool.Interfaces.csproj" />
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
    <ProjectReference Include="..\MusicSchool.Repositories\MusicSchool.Repositories.csproj" />
  </ItemGroup>

</Project>

```

## File: MusicSchool.Api\Program.cs

```csharp
using MusicSchool.Api;
using Serilog;

namespace MusicSchool.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("Starting MusicSchool.Api");

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                

                builder.Host.UseSerilog((context, services, configuration) =>
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext());

                var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(name: MyAllowSpecificOrigins,
                        policy =>
                        {
                            policy.WithOrigins("https://localhost:64314") // Blazor WASM origin
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                });

                var startup = new Startup(builder.Configuration);
                startup.ConfigureServices(builder.Services);

                var app = builder.Build();
                startup.Configure(app, app.Environment);
                app.UseCors(MyAllowSpecificOrigins);
                app.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}

```

## File: MusicSchool.Api\Startup.cs

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using Scalar.AspNetCore;
using Serilog;
using System.Data;

namespace MusicSchool.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register Dapper type handler so TIME columns map to TimeOnly
            SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());

            // Database connection
            services.AddScoped<IDbConnection>(_ =>
                new SqlConnection(Configuration.GetConnectionString("MusicSchool")));

            // Repositories
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IAccountHolderRepository, AccountHolderRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ILessonTypeRepository, LessonTypeRepository>();
            services.AddScoped<ILessonBundleRepository, LessonBundleRepository>();
            services.AddScoped<IScheduledSlotRepository, ScheduledSlotRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IExtraLessonRepository, ExtraLessonRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IAccountHolderService, AccountHolderService>();
            services.AddScoped<IBundleQuarterService, BundleQuarterService>();
            services.AddScoped<IExtraLessonService, ExtraLessonService>();
            services.AddScoped<IExtraLessonAggregateService, ExtraLessonAggregateService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ILessonAggregateService, LessonAggregateService>();
            services.AddScoped<ILessonBundleService, LessonBundleService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ILessonTypeService, LessonTypeService>();
            services.AddScoped<IScheduledSlotService, ScheduledSlotService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ILessonBundleAggregateService, LessonBundleAggregateService>();

            services.AddControllers();

            services.AddOpenApi();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            });

            if (env.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "MusicSchool API";
                    options.Theme = ScalarTheme.DeepSpace;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}

```

## File: MusicSchool.Api\TimeOnlyTypeHandler.cs

```csharp
using Dapper;
using System.Data;

namespace MusicSchool.Api
{
    /// <summary>
    /// Tells Dapper how to read SQL Server TIME columns (returned as TimeSpan by ADO.NET)
    /// into TimeOnly, and how to write TimeOnly back as a TIME parameter.
    /// Register once at startup: SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler());
    /// </summary>
    public class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.ToTimeSpan();
        }

        public override TimeOnly Parse(object value)
            => TimeOnly.FromTimeSpan((TimeSpan)value);
    }
}

```

## File: MusicSchool.Interfaces\IAccountHolderRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IAccountHolderRepository
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int?> AddAccountHolderAsync(AccountHolder accountHolder);
        Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder);
    }
}
```

## File: MusicSchool.Interfaces\IAccountHolderService.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IAccountHolderService
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int> InsertAsync(AccountHolder accountHolder);
        Task<bool> UpdateAsync(AccountHolder accountHolder);
    }
}
```

## File: MusicSchool.Interfaces\IBundleQuarterService.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IBundleQuarterService
    {
        Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId);
        Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx);
        Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed);

        /// <summary>
        /// Atomically increments or decrements LessonsUsed for the quarter that
        /// owns <paramref name="lessonId"/> by <paramref name="delta"/> (+1 or -1).
        /// Uses a single UPDATE … SET LessonsUsed = LessonsUsed + @Delta to avoid
        /// read-then-write races. Clamps to zero so LessonsUsed never goes negative.
        /// </summary>
        Task<bool> AdjustLessonsUsedAsync(int lessonId, int delta);
    }
}

```

## File: MusicSchool.Interfaces\IExtraLessonAggregateService.cs

```csharp
namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonAggregateService
    {
        Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
    }
}

```

## File: MusicSchool.Interfaces\IExtraLessonRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonRepository
    {
        Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId);
        Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);
        Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson);
        Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status);
    }
}

```

## File: MusicSchool.Interfaces\IExtraLessonService.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonService
    {
        Task<ExtraLesson?> GetExtraLessonAsync(int id);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(ExtraLesson extraLesson);
        Task<bool> UpdateStatusAsync(int extraLessonId, string status);
    }
}

```

## File: MusicSchool.Interfaces\IInvoiceRepository.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId);
        Task<bool> AddInvoiceInstalmentsAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection);
        Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate);
    }
}

```

## File: MusicSchool.Interfaces\IInvoiceService.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId);
        Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection);
        Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate);
    }
}

```

## File: MusicSchool.Interfaces\ILessonAggregateService.cs

```csharp
namespace MusicSchool.Data.Interfaces
{
    public interface ILessonAggregateService
    {
        Task<LessonDetail?> GetLessonByIdAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
    }
}

```

## File: MusicSchool.Interfaces\ILessonBundleAggregateService.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleAggregateService
    {
        Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleByIdAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetBundleByStudentIdAsync(int bundleId);
    }
}

```

## File: MusicSchool.Interfaces\ILessonBundleRepository.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleRepository
    {
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId);
        Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<bool> UpdateBundleAsync(LessonBundle bundle);
    }
}

```

## File: MusicSchool.Interfaces\ILessonBundleService.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleService
    {
        Task<LessonBundle?> GetBundleAsync(int id);
        Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx);
        Task<bool> UpdateAsync(LessonBundle bundle);
    }
}

```

## File: MusicSchool.Interfaces\ILessonRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonRepository
    {
        Task<LessonDetail?> GetLessonAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<int?> AddLessonAsync(Lesson lesson);
        Task<bool> UpdateLessonStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt);
    }
}

```

## File: MusicSchool.Interfaces\ILessonService.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonService
    {
        Task<Lesson?> GetLessonAsync(int id);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Lesson>> GetByStatusAsync(string status);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(Lesson lesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(Lesson lesson, IDbTransaction tx);

        Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt);
    }
}

```

## File: MusicSchool.Interfaces\ILessonTypeRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonTypeRepository
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int?> AddLessonTypeAsync(LessonType lessonType);
        Task<bool> UpdateLessonTypeAsync(LessonType lessonType);
    }
}

```

## File: MusicSchool.Interfaces\ILessonTypeService.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonTypeService
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int> InsertAsync(LessonType lessonType);
        Task<bool> UpdateAsync(LessonType lessonType);
    }
}

```

## File: MusicSchool.Interfaces\IScheduledSlotRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotRepository
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);

        /// <summary>
        /// Inserts the slot and generates all future Lesson rows for the student's
        /// active bundle. Returns null if the student has no active bundle with
        /// remaining credits, or if the insert fails.
        /// </summary>
        Task<int?> AddSlotAsync(ScheduledSlot slot);

        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);
    }
}

```

## File: MusicSchool.Interfaces\IScheduledSlotService.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotService
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ScheduledSlot slot);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ScheduledSlot slot, IDbTransaction tx);

        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);

        /// <summary>
        /// Opens a connection (if not already open), begins a transaction,
        /// invokes <paramref name="work"/>, and commits. Rolls back on exception.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<IDbTransaction, IDbConnection, Task> work);
    }
}

```

## File: MusicSchool.Interfaces\IStudentRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetStudentAsync(int id);
        Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId);
        Task<int?> AddStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
    }
}

```

## File: MusicSchool.Interfaces\IStudentService.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IStudentService
    {
        Task<Student?> GetStudentAsync(int id);
        Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId);
        Task<int> InsertAsync(Student student);
        Task<bool> UpdateAsync(Student student);
    }
}

```

## File: MusicSchool.Interfaces\ITeacherRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherRepository
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int?> AddTeacherAsync(Teacher teacher);
        Task<bool> UpdateTeacherAsync(Teacher teacher);
    }
}

```

## File: MusicSchool.Interfaces\ITeacherService.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherService
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int> InsertAsync(Teacher teacher);
        Task<bool> UpdateAsync(Teacher teacher);
    }
}

```

## File: MusicSchool.Interfaces\MusicSchool.Interfaces.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

</Project>

```

## File: MusicSchool.Models\TransferModels\AddBundleRequest.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Models.TransferModels
{
    public class AddBundleRequest
    {
        public LessonBundle Bundle { get; set; }
        public IEnumerable<BundleQuarter> Quarters { get; set; }
    }
}
```

## File: MusicSchool.Models\TransferModels\RequestBase.cs

```csharp
namespace MusicSchool.Models.TransferModels
{
    public class RequestBase<T>
    {
        public T Data { get; set; }
    }
}

```

## File: MusicSchool.Models\TransferModels\ResponseBase.cs

```csharp
namespace MusicSchool.Models.TransferModels
{
    public class ResponseBase<T>
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;        
        public T Data { get; set; }
    }
}

```

## File: MusicSchool.Models\AccountHolder.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Contracted by a teacher. The billing party for one or more students.
    /// </summary>
    public class AccountHolder
    {
        public int      AccountHolderID { get; set; }
        public int      TeacherID       { get; set; }
        public string   FirstName       { get; set; } = string.Empty;
        public string   LastName        { get; set; } = string.Empty;
        public string   Email           { get; set; } = string.Empty;
        public string?  Phone           { get; set; }
        public string?  BillingAddress  { get; set; }
        public bool     IsActive        { get; set; } = true;
        public DateTime CreatedAt       { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}

```

## File: MusicSchool.Models\BundleQuarter.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// One of the four equal portions of a LessonBundle.
    /// LessonsUsed is incremented by the application each time a lesson
    /// is marked Completed or Forfeited.
    /// </summary>
    public class BundleQuarter
    {
        public int      QuarterID        { get; set; }
        public int      BundleID         { get; set; }

        /// <summary>
        /// Quarter sequence within the bundle: 1–4.
        /// </summary>
        public byte     QuarterNumber    { get; set; }

        public int      LessonsAllocated { get; set; }
        public int      LessonsUsed      { get; set; } = 0;
        public DateTime QuarterStartDate { get; set; }
        public DateTime QuarterEndDate   { get; set; }
    }
}

```

## File: MusicSchool.Models\ExtraLesson.cs

```csharp
namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="ExtraLesson.Status"/>.
    /// </summary>
    public static class ExtraLessonStatus
    {
        public const string Scheduled = "Scheduled";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Forfeited = "Forfeited";
    }

    /// <summary>
    /// Ad-hoc lesson purchased after a bundle is exhausted.
    /// PriceCharged holds the teacher-adjusted rate (base price or override).
    /// </summary>
    public class ExtraLesson
    {
        public int      ExtraLessonID { get; set; }
        public int      StudentID     { get; set; }
        public int      TeacherID     { get; set; }
        public int      LessonTypeID  { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime ScheduledTime { get; set; }
        public decimal  PriceCharged  { get; set; }

        /// <summary>
        /// See <see cref="ExtraLessonStatus"/> for valid values.
        /// </summary>
        public string   Status        { get; set; } = ExtraLessonStatus.Scheduled;

        public string?  Notes         { get; set; }
        public DateTime CreatedAt     { get; set; }
    }
}

```

## File: MusicSchool.Models\ExtraLessonDetail.cs

```csharp
public class ExtraLessonDetail
{
    // ExtraLesson fields
    public int ExtraLessonID { get; set; }
    public int StudentID { get; set; }
    public int TeacherID { get; set; }
    public int LessonTypeID { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public decimal PriceCharged { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Student fields
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentLastName { get; set; } = string.Empty;
    public string StudentFullName { get { return $"{StudentFirstName} {StudentLastName}"; } }

    // Teacher fields
    public string TeacherName { get; set; } = string.Empty;

    // LessonType fields
    public int DurationMinutes { get; set; }
    public decimal BasePricePerLesson { get; set; }
}
```

## File: MusicSchool.Models\Invoice.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Invoice.Status"/>.
    /// </summary>
    public static class InvoiceStatus
    {
        public const string Pending = "Pending";
        public const string Paid    = "Paid";
        public const string Overdue = "Overdue";
        public const string Void    = "Void";
    }

    /// <summary>
    /// One monthly instalment for a <see cref="LessonBundle"/>.
    /// A bundle produces 12 Invoice rows (InstallmentNumber 1–12).
    /// Amount = (TotalLessons * PricePerLesson) / 12,
    /// calculated and written by the application layer.
    /// </summary>
    public class Invoice
    {
        public int       InvoiceID         { get; set; }
        public int       BundleID          { get; set; }
        public int       AccountHolderID   { get; set; }

        /// <summary>
        /// Monthly instalment sequence number: 1–12.
        /// </summary>
        public byte      InstallmentNumber { get; set; }

        public decimal   Amount            { get; set; }
        public DateTime  DueDate           { get; set; }
        public DateTime? PaidDate          { get; set; }

        /// <summary>
        /// See <see cref="InvoiceStatus"/> for valid values.
        /// </summary>
        public string    Status            { get; set; } = InvoiceStatus.Pending;

        public string?   Notes             { get; set; }
        public DateTime  CreatedAt         { get; set; }
    }
}

```

## File: MusicSchool.Models\Lesson.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Lesson.Status"/>.
    /// </summary>
    public static class LessonStatus
    {
        /// <summary>Upcoming; not yet attended.</summary>
        public const string Scheduled        = "Scheduled";

        /// <summary>Attended; credit consumed.</summary>
        public const string Completed        = "Completed";

        /// <summary>
        /// Teacher cancelled. Credit is NOT forfeited;
        /// teacher is responsible for rescheduling.
        /// </summary>
        public const string CancelledTeacher = "CancelledTeacher";

        /// <summary>
        /// Lesson was moved. OriginalLessonID references the cancelled lesson.
        /// </summary>
        public const string Rescheduled      = "Rescheduled";

        /// <summary>Student cancelled; teacher decides the outcome.</summary>
        public const string CancelledStudent = "CancelledStudent";

        /// <summary>
        /// Student cancelled and teacher chose not to reschedule.
        /// Credit is forfeited.
        /// </summary>
        public const string Forfeited        = "Forfeited";
    }

    /// <summary>
    /// Valid values for <see cref="Lesson.CancelledBy"/>.
    /// </summary>
    public static class CancelledBy
    {
        public const string Teacher = "Teacher";
        public const string Student = "Student";
    }

    /// <summary>
    /// One instance of a lesson, generated from a <see cref="ScheduledSlot"/>.
    /// Draws a credit from a <see cref="BundleQuarter"/>.
    /// </summary>
    public class Lesson
    {
        public int       LessonID           { get; set; }
        public int       SlotID             { get; set; }
        public int       BundleID           { get; set; }
        public int       QuarterID          { get; set; }
        public DateTime  ScheduledDate      { get; set; }
        public TimeOnly  ScheduledTime      { get; set; }

        /// <summary>
        /// See <see cref="LessonStatus"/> for valid values.
        /// </summary>
        public string    Status             { get; set; } = LessonStatus.Scheduled;

        public bool      CreditForfeited    { get; set; } = false;

        /// <summary>
        /// See <see cref="CancelledBy"/> for valid values. Null when not cancelled.
        /// </summary>
        public string?   CancelledBy        { get; set; }

        public string?   CancellationReason { get; set; }

        /// <summary>
        /// Populated on rescheduled lessons. References the original lesson that was cancelled.
        /// </summary>
        public int?      OriginalLessonID   { get; set; }

        public DateTime? CompletedAt        { get; set; }
        public DateTime  CreatedAt          { get; set; }
    }
}

```

## File: MusicSchool.Models\LessonBundle.cs

```csharp
namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Annual lesson bundle purchased for a student.
    /// PricePerLesson holds the teacher-adjusted (possibly discounted) rate
    /// agreed at the time of purchase.
    /// </summary>
    public class LessonBundle
    {
        public int      BundleID       { get; set; }
        public int      StudentID      { get; set; }
        public int      TeacherID      { get; set; }
        public int      LessonTypeID   { get; set; }
        public int      TotalLessons   { get; set; }
        public decimal  PricePerLesson { get; set; }
        public DateTime StartDate      { get; set; }
        public DateTime EndDate        { get; set; }

        /// <summary>
        /// Computed by the database as TotalLessons / 4. Read-only.
        /// </summary>
        public int      QuarterSize    { get; set; }

        public bool     IsActive       { get; set; } = true;
        public string?  Notes          { get; set; }
        public DateTime CreatedAt      { get; set; }
    }
}

```

## File: MusicSchool.Models\LessonBundleWithQuarterDetail.cs

```csharp
namespace MusicSchool.Models
{
    public class LessonBundleWithQuarterDetail
    {
        // LessonBundle fields
        public int BundleID { get; set; }
        public int StudentID { get; set; }
        public int TeacherID { get; set; }
        public int LessonTypeID { get; set; }
        public int TotalLessons { get; set; }
        public decimal PricePerLesson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int QuarterSize { get; set; }
        public string? BundleNotes { get; set; }

        // Student fields
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;

        public string StudentFullName { get { return $"{StudentFirstName} {StudentLastName}"; } }

        // LessonType fields
        public int DurationMinutes { get; set; }
        public decimal BasePricePerLesson { get; set; }

        // BundleQuarter fields
        public int QuarterID { get; set; }
        public byte QuarterNumber { get; set; }
        public int LessonsAllocated { get; set; }
        public int LessonsUsed { get; set; }
        public DateTime QuarterStartDate { get; set; }
        public DateTime QuarterEndDate { get; set; }

        public int LessonsRemaining { get { return LessonsAllocated - LessonsUsed; } }
    }
    public class LessonBundleDetail
    {
        // LessonBundle fields
        public int BundleID { get; set; }
        public int StudentID { get; set; }
        public int TeacherID { get; set; }
        public int LessonTypeID { get; set; }
        public int TotalLessons { get; set; }
        public decimal PricePerLesson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int QuarterSize { get; set; }
        public string? BundleNotes { get; set; }

        // Student fields
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;

        public string StudentFullName { get { return $"{StudentFirstName} {StudentLastName}"; } }

        // LessonType fields
        public int DurationMinutes { get; set; }
        public decimal BasePricePerLesson { get; set; }
    }
}

```

## File: MusicSchool.Models\LessonDetails.cs

```csharp
public class LessonDetail
{
    // Lesson fields
    public int LessonID { get; set; }
    public int SlotID { get; set; }
    public int BundleID { get; set; }
    public int QuarterID { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool CreditForfeited { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }
    public int? OriginalLessonID { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Student fields
    public int StudentID { get; set; }
    public string StudentFirstName { get; set; } = string.Empty;
    public string StudentLastName { get; set; } = string.Empty;

    public string StudentFullName { get { return $"{StudentFirstName} {StudentLastName}"; } }

    // Teacher fields
    public int TeacherID { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    // LessonType fields
    public int LessonTypeID { get; set; }
    public int DurationMinutes { get; set; }
    public decimal BasePricePerLesson { get; set; }
}
```

## File: MusicSchool.Models\LessonType.cs

```csharp
namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Defines a lesson duration (30 / 45 / 60 minutes) and its base price.
    /// The application layer applies any teacher discount on top of BasePricePerLesson.
    /// </summary>
    public class LessonType
    {
        public int     LessonTypeID       { get; set; }
        public int     DurationMinutes    { get; set; }   // 30, 45, or 60
        public decimal BasePricePerLesson { get; set; }
        public bool    IsActive           { get; set; } = true;

        public string DisplayName { get { return $"{DurationMinutes} min"; } }
    }
}

```

## File: MusicSchool.Models\MusicSchool.Models.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

```

## File: MusicSchool.Models\ScheduledSlot.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// The recurring weekly pattern for a student/teacher pair.
    /// When a slot changes, close it by setting EffectiveTo and open a new one
    /// to preserve the history of past lessons.
    /// </summary>
    public class ScheduledSlot
    {
        public int       SlotID        { get; set; }
        public int       StudentID     { get; set; }
        public int       TeacherID     { get; set; }
        public int       LessonTypeID  { get; set; }

        /// <summary>
        /// ISO 8601 day of week: 1 = Monday … 7 = Sunday.
        /// </summary>
        public byte      DayOfWeek     { get; set; }
        public string DayName { get { return GetDayName(); } }

        public TimeOnly  SlotTime      { get; set; }
        public DateTime  EffectiveFrom { get; set; }

        /// <summary>
        /// Null indicates the slot is still active.
        /// </summary>
        public DateTime? EffectiveTo   { get; set; }

        public bool      IsActive      { get; set; } = true;

        public string GetDayName() 
        {
            return DayOfWeek switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                7 => "Sunday",
                _ => "Unknown"
            };
        }
    }
}

```

## File: MusicSchool.Models\Student.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Enrolled by an account holder.
    /// IsAccountHolder is true when the individual fills both roles.
    /// </summary>
    public class Student
    {
        public int       StudentID       { get; set; }
        public int       AccountHolderID { get; set; }
        public string    FirstName       { get; set; } = string.Empty;
        public string    LastName        { get; set; } = string.Empty;
        public DateTime? DateOfBirth     { get; set; }
        public bool      IsAccountHolder { get; set; } = false;
        public bool      IsActive        { get; set; } = true;
        public DateTime  CreatedAt       { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}

```

## File: MusicSchool.Models\Teacher.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    public class Teacher
    {
        public int      TeacherID { get; set; }
        public string   Name      { get; set; } = string.Empty;
        public string   Email     { get; set; } = string.Empty;
        public string?  Phone     { get; set; }
        public bool     IsActive  { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}

```

## File: MusicSchool.Repositories\AccountHolderRepository.cs

```csharp

using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly IAccountHolderService _accountHolderService;
        private readonly ILogger<AccountHolderRepository> _logger;

        public AccountHolderRepository(IAccountHolderService accountHolderService, ILogger<AccountHolderRepository> logger)
        {
            _accountHolderService = accountHolderService;
            _logger = logger;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            return await _accountHolderService.GetAccountHolderAsync(id);
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            return await _accountHolderService.GetByTeacherAsync(teacherId);
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.InsertAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert AccountHolder {FirstName} {LastName}",
                    accountHolder.FirstName, accountHolder.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                return await _accountHolderService.UpdateAsync(accountHolder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update AccountHolderID {AccountHolderID}",
                    accountHolder.AccountHolderID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\AccountHolderService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderService : IAccountHolderService
    {
        private readonly IDbConnection _connection;

        public AccountHolderService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE AccountHolderID = @AccountHolderID;";

            return await _connection.QuerySingleOrDefaultAsync<AccountHolder>(sql, new { AccountHolderID = id });
        }

        public async Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            const string sql = @"
                SELECT AccountHolderID,
                       TeacherID,
                       FirstName,
                       LastName,
                       Email,
                       Phone,
                       BillingAddress,
                       IsActive,
                       CreatedAt
                FROM AccountHolder
                WHERE TeacherID = @TeacherID
                  AND IsActive  = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<AccountHolder>(sql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                INSERT INTO AccountHolder
                    (TeacherID, FirstName, LastName, Email, Phone, BillingAddress, IsActive)
                VALUES
                    (@TeacherID, @FirstName, @LastName, @Email, @Phone, @BillingAddress, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, accountHolder);
        }

        public async Task<bool> UpdateAsync(AccountHolder accountHolder)
        {
            const string sql = @"
                UPDATE AccountHolder
                SET TeacherID      = @TeacherID,
                    FirstName      = @FirstName,
                    LastName       = @LastName,
                    Email          = @Email,
                    Phone          = @Phone,
                    BillingAddress = @BillingAddress,
                    IsActive       = @IsActive
                WHERE AccountHolderID = @AccountHolderID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, accountHolder);
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\BundleQuarterService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class BundleQuarterService : IBundleQuarterService
    {
        private readonly IDbConnection _connection;

        public BundleQuarterService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT QuarterID,
                       BundleID,
                       QuarterNumber,
                       LessonsAllocated,
                       LessonsUsed,
                       QuarterStartDate,
                       QuarterEndDate
                FROM BundleQuarter
                WHERE BundleID = @BundleID
                ORDER BY QuarterNumber;";

            return await _connection.QueryAsync<BundleQuarter>(sql, new { BundleID = bundleId });
        }

        public async Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO BundleQuarter
                    (BundleID, QuarterNumber, LessonsAllocated, LessonsUsed,
                     QuarterStartDate, QuarterEndDate)
                VALUES
                    (@BundleID, @QuarterNumber, @LessonsAllocated, @LessonsUsed,
                     @QuarterStartDate, @QuarterEndDate);";

            await _connection.ExecuteAsync(
                new CommandDefinition(sql, quarters, tx));
        }

        public async Task<bool> UpdateLessonsUsedAsync(int quarterId, int lessonsUsed)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = @LessonsUsed
                WHERE QuarterID = @QuarterID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { QuarterID = quarterId, LessonsUsed = lessonsUsed });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Atomically adjusts LessonsUsed for the quarter that owns the given lesson.
        /// Pass +1 when a lesson is completed or forfeited, -1 when that is reversed.
        /// Clamps to zero so LessonsUsed never goes negative.
        /// </summary>
        public async Task<bool> AdjustLessonsUsedAsync(int lessonId, int delta)
        {
            const string sql = @"
                UPDATE BundleQuarter
                SET LessonsUsed = CASE
                                      WHEN LessonsUsed + @Delta < 0 THEN 0
                                      ELSE LessonsUsed + @Delta
                                  END
                WHERE QuarterID = (
                    SELECT QuarterID FROM Lesson WHERE LessonID = @LessonID
                );";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { LessonID = lessonId, Delta = delta });
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\ExtraLessonAggregateService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonAggregateService : IExtraLessonAggregateService
    {
        private readonly IDbConnection _connection;

        public static readonly string SELECT_EXTRA_LESSON_DETAIL_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.ExtraLessonID = @ExtraLessonID;";

        public static readonly string SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT el.ExtraLessonID,
                   el.StudentID,
                   el.TeacherID,
                   el.LessonTypeID,
                   el.ScheduledDate,
                   el.ScheduledTime,
                   el.PriceCharged,
                   el.Status,
                   el.Notes,
                   s.FirstName       AS StudentFirstName,
                   s.LastName        AS StudentLastName,
                   t.Name            AS TeacherName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM ExtraLesson  el
            JOIN Student       s ON s.StudentID     = el.StudentID
            JOIN Teacher       t ON t.TeacherID     = el.TeacherID
            JOIN LessonType   lt ON lt.LessonTypeID = el.LessonTypeID
            WHERE el.TeacherID    = @TeacherID
              AND el.ScheduledDate = @ScheduledDate
            ORDER BY el.ScheduledTime;";

        public ExtraLessonAggregateService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSON_DETAIL_QRY,
                new { ExtraLessonID = extraLessonId });
        }

        public async Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<ExtraLessonDetail>(
                SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }
    }
}

```

## File: MusicSchool.Repositories\ExtraLessonRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonRepository : IExtraLessonRepository
    {
        private readonly IExtraLessonAggregateService _aggregateService;
        private readonly IExtraLessonService _extraLessonService;
        private readonly ILogger<ExtraLessonRepository> _logger;

        public ExtraLessonRepository(
            IExtraLessonAggregateService aggregateService,
            IExtraLessonService extraLessonService,
            ILogger<ExtraLessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _extraLessonService = extraLessonService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a single extra lesson with full context (student, teacher, lesson type).
        /// </summary>
        public async Task<ExtraLessonDetail?> GetExtraLessonAsync(int extraLessonId)
        {
            return await _aggregateService.GetExtraLessonByIdAsync(extraLessonId);
        }

        /// <summary>
        /// Returns all extra lessons for a teacher on a given date, with full context.
        /// </summary>
        public async Task<IEnumerable<ExtraLessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _aggregateService.GetExtraLessonsByTeacherAndDateAsync(teacherId, scheduledDate);
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            return await _extraLessonService.GetByStudentAsync(studentId);
        }

        public async Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson)
        {
            try
            {
                return await _extraLessonService.InsertAsync(extraLesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ExtraLesson for StudentID {StudentID} on {ScheduledDate}",
                    extraLesson.StudentID, extraLesson.ScheduledDate);
                return null;
            }
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status)
        {
            try
            {
                return await _extraLessonService.UpdateStatusAsync(extraLessonId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to update status for ExtraLessonID {ExtraLessonID}", extraLessonId);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\ExtraLessonService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonService : IExtraLessonService
    {
        private readonly IDbConnection _connection;

        public ExtraLessonService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ExtraLesson?> GetExtraLessonAsync(int id)
        {
            const string sql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE ExtraLessonID = @ExtraLessonID;";

            return await _connection.QuerySingleOrDefaultAsync<ExtraLesson>(sql, new { ExtraLessonID = id });
        }

        public async Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT ExtraLessonID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       ScheduledDate,
                       ScheduledTime,
                       PriceCharged,
                       Status,
                       Notes,
                       CreatedAt
                FROM ExtraLesson
                WHERE StudentID = @StudentID
                ORDER BY ScheduledDate DESC, ScheduledTime DESC;";

            return await _connection.QueryAsync<ExtraLesson>(sql, new { StudentID = studentId });
        }

        public async Task<int> InsertAsync(ExtraLesson extraLesson)
        {
            const string sql = @"
                INSERT INTO ExtraLesson
                    (StudentID, TeacherID, LessonTypeID, ScheduledDate,
                     ScheduledTime, PriceCharged, Status, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @ScheduledDate,
                     @ScheduledTime, @PriceCharged, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, extraLesson);
        }

        public async Task<bool> UpdateStatusAsync(int extraLessonId, string status)
        {
            const string sql = @"
                UPDATE ExtraLesson
                SET Status = @Status
                WHERE ExtraLessonID = @ExtraLessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { ExtraLessonID = extraLessonId, Status = status });
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\InvoiceRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IInvoiceService invoiceService, ILogger<InvoiceRepository> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            return await _invoiceService.GetInvoiceAsync(id);
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            return await _invoiceService.GetByBundleAsync(bundleId);
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            return await _invoiceService.GetOutstandingByAccountHolderAsync(accountHolderId);
        }

        /// <summary>
        /// Saves all 12 instalment rows for a bundle atomically.
        /// The application layer is responsible for calculating the Amount
        /// and setting the DueDate for each instalment before calling this method.
        /// </summary>
        public async Task<bool> AddInvoiceInstalmentsAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            try
            {
                await _invoiceService.InsertBatchAsync(invoices, tx, connection);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert invoice instalments for BundleID {BundleID}",
                    invoices.FirstOrDefault()?.BundleID);
                return false;
            }
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                return await _invoiceService.UpdateStatusAsync(invoiceId, status, paidDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for InvoiceID {InvoiceID}", invoiceId);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\InvoiceService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IDbConnection _connection;

        public InvoiceService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE InvoiceID = @InvoiceID;";

            return await _connection.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceID = id });
        }

        public async Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE BundleID = @BundleID
                ORDER BY InstallmentNumber;";

            return await _connection.QueryAsync<Invoice>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       AccountHolderID,
                       InstallmentNumber,
                       Amount,
                       DueDate,
                       PaidDate,
                       Status,
                       Notes,
                       CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                  AND Status IN ('Pending', 'Overdue')
                ORDER BY DueDate;";

            return await _connection.QueryAsync<Invoice>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO Invoice
                    (BundleID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, invoices, tx));
        }

        public async Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            const string sql = @"
                UPDATE Invoice
                SET Status   = @Status,
                    PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";
            DateTime? paidDateTime = paidDate.HasValue
                ? paidDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;
            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { InvoiceID = invoiceId, Status = status, PaidDate = paidDateTime });
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\LessonAggregateService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonAggregateService : ILessonAggregateService
    {
        private readonly IDbConnection _connection;

        public static readonly string SELECT_LESSON_DETAIL_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE l.LessonID = @LessonID;";

        public static readonly string SELECT_LESSONS_BY_TEACHER_DATE_QRY = @"
            SELECT l.LessonID,
                   l.SlotID,
                   l.BundleID,
                   l.QuarterID,
                   l.ScheduledDate,
                   l.ScheduledTime,
                   l.Status,
                   l.CreditForfeited,
                   l.CancelledBy,
                   l.CancellationReason,
                   l.OriginalLessonID,
                   l.CompletedAt,
                   s.StudentID,
                   s.FirstName      AS StudentFirstName,
                   s.LastName       AS StudentLastName,
                   t.TeacherID,
                   t.Name           AS TeacherName,
                   lt.LessonTypeID,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM Lesson         l
            JOIN ScheduledSlot  ss ON ss.SlotID       = l.SlotID
            JOIN Student         s ON s.StudentID     = ss.StudentID
            JOIN Teacher         t ON t.TeacherID     = ss.TeacherID
            JOIN LessonType     lt ON lt.LessonTypeID = ss.LessonTypeID
            WHERE t.TeacherID      = @TeacherID
              AND l.ScheduledDate  = @ScheduledDate
            ORDER BY l.ScheduledTime;";

        public LessonAggregateService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonDetail?> GetLessonByIdAsync(int lessonId)
        {
            return await _connection.QuerySingleOrDefaultAsync<LessonDetail>(
                SELECT_LESSON_DETAIL_QRY,
                new { LessonID = lessonId });
        }

        public async Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
        {
            return await _connection.QueryAsync<LessonDetail>(
                SELECT_LESSONS_BY_TEACHER_DATE_QRY,
                new { TeacherID = teacherId, ScheduledDate = scheduledDate });
        }
    }
}

```

## File: MusicSchool.Repositories\LessonBundleAggregateService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;
namespace MusicSchool.Data.Implementations
{
    public class LessonBundleAggregateService : ILessonBundleAggregateService
    {
        private readonly IDbConnection _connection;
        private readonly ILessonBundleService _lessonBundleService;
        private readonly IBundleQuarterService _bundleQuarterService;
        private readonly IInvoiceService _invoiceService;

        public static readonly string SELECT_BUNDLE_WITH_QUARTERS_QRY = @"
            SELECT lb.BundleID,
                   lb.StudentID,
                   lb.TeacherID,
                   lb.LessonTypeID,
                   lb.TotalLessons,
                   lb.PricePerLesson,
                   lb.StartDate,
                   lb.EndDate,
                   lb.QuarterSize,
                   lb.Notes        AS BundleNotes,
                   s.FirstName     AS StudentFirstName,
                   s.LastName      AS StudentLastName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson,
                   bq.QuarterID,
                   bq.QuarterNumber,
                   bq.LessonsAllocated,
                   bq.LessonsUsed,
                   bq.QuarterStartDate,
                   bq.QuarterEndDate
            FROM LessonBundle lb
            JOIN Student       s  ON s.StudentID     = lb.StudentID
            JOIN LessonType    lt ON lt.LessonTypeID = lb.LessonTypeID
            JOIN BundleQuarter bq ON bq.BundleID     = lb.BundleID
            WHERE lb.BundleID = @BundleID
            ORDER BY bq.QuarterNumber;";

        public static readonly string SELECT_BUNDLE_QRY_BY_STUDENT = @"
            SELECT lb.BundleID,
                   lb.StudentID,
                   lb.TeacherID,
                   lb.LessonTypeID,
                   lb.TotalLessons,
                   lb.PricePerLesson,
                   lb.StartDate,
                   lb.EndDate,
                   lb.QuarterSize,
                   lb.Notes        AS BundleNotes,
                   s.FirstName     AS StudentFirstName,
                   s.LastName      AS StudentLastName,
                   lt.DurationMinutes,
                   lt.BasePricePerLesson
            FROM LessonBundle lb
            JOIN Student       s  ON s.StudentID     = lb.StudentID
            JOIN LessonType    lt ON lt.LessonTypeID = lb.LessonTypeID
            WHERE s.StudentID = @StudentID
            ORDER BY lb.BundleID;";

        public LessonBundleAggregateService(
            IDbConnection connection,
            ILessonBundleService lessonBundleService,
            IBundleQuarterService bundleQuarterService,
            IInvoiceService invoiceService)
        {
            _connection = connection;
            _lessonBundleService = lessonBundleService;
            _bundleQuarterService = bundleQuarterService;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Saves a new LessonBundle, its 4 BundleQuarter rows, and 12 monthly Invoice
        /// instalments — all in a single transaction. Returns the new BundleID.
        ///
        /// Invoice due dates are the 1st of each month starting from the month the
        /// bundle starts, running for 12 consecutive months.
        /// Amount per instalment = (TotalLessons × PricePerLesson) / 12, rounded to 2dp.
        /// </summary>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve AccountHolderID inside the transaction so we don't need
                //    a separate round-trip before opening the connection.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { bundle.StudentID }, transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {bundle.StudentID} not found when creating bundle.");

                // 2. Insert bundle
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                // 3. Insert quarters
                foreach (var quarter in quarters)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarters, transaction);

                // 4. Generate 12 monthly invoice instalments
                var instalmentAmount = Math.Round(bundle.TotalLessons * bundle.PricePerLesson / 12, 2);
                var invoices = BuildInstalments(bundleId, accountHolderId, instalmentAmount, bundle.StartDate);

                await _invoiceService.InsertBatchAsync(invoices, transaction, _connection);

                transaction.Commit();
                return bundleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Returns a flat LessonBundleWithQuarterDetail row per quarter (4 rows total)
        /// for the given bundle, joining Student and LessonType for context.
        /// </summary>
        public async Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleByIdAsync(int bundleId)
        {
            return await _connection.QueryAsync<LessonBundleWithQuarterDetail>(
                SELECT_BUNDLE_WITH_QUARTERS_QRY,
                new { BundleID = bundleId });
        }

        public async Task<IEnumerable<LessonBundleDetail>> GetBundleByStudentIdAsync(int studentId)
        {
            return await _connection.QueryAsync<LessonBundleDetail>(
                SELECT_BUNDLE_QRY_BY_STUDENT,
                new { StudentID = studentId });
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Builds 12 Invoice rows for a bundle. DueDate is the 1st of each month,
        /// starting from the 1st of the month in which <paramref name="bundleStartDate"/> falls.
        /// </summary>
        private static IEnumerable<Invoice> BuildInstalments(
            int bundleId,
            int accountHolderId,
            decimal instalmentAmount,
            DateTime bundleStartDate)
        {
            // Anchor to the 1st of the start month regardless of the day the bundle starts.
            var firstDue = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);

            for (byte i = 1; i <= 12; i++)
            {
                yield return new Invoice
                {
                    BundleID          = bundleId,
                    AccountHolderID   = accountHolderId,
                    InstallmentNumber = i,
                    Amount            = instalmentAmount,
                    DueDate           = firstDue.AddMonths(i - 1),
                    Status            = InvoiceStatus.Pending,
                };
            }
        }
    }
}

```

## File: MusicSchool.Repositories\LessonBundleRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleRepository : ILessonBundleRepository
    {
        private readonly ILessonBundleAggregateService _aggregateService;
        private readonly ILessonBundleService _lessonBundleService;
        private readonly ILogger<LessonBundleRepository> _logger;

        public LessonBundleRepository(
            ILessonBundleAggregateService aggregateService,
            ILessonBundleService lessonBundleService,
            ILogger<LessonBundleRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonBundleService = lessonBundleService;
            _logger = logger;
        }

        /// <summary>
        /// Returns the bundle with all four quarters as flat detail rows.
        /// </summary>
        public async Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            return await _aggregateService.GetBundleByIdAsync(bundleId);
        }

        public async Task<IEnumerable<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            return await _aggregateService.GetBundleByStudentIdAsync(studentId);
        }

        /// <summary>
        /// Saves the bundle and its 4 quarters atomically.
        /// The application layer is responsible for building the quarter list
        /// before calling this method.
        /// </summary>
        public async Task<int?> AddBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            try
            {
                return await _aggregateService.SaveNewBundleAsync(bundle, quarters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save LessonBundle for StudentID {StudentID}",
                    bundle.StudentID);
                return null;
            }
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            try
            {
                return await _lessonBundleService.UpdateAsync(bundle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update BundleID {BundleID}", bundle.BundleID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\LessonBundleService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleService : ILessonBundleService
    {
        private readonly IDbConnection _connection;

        public LessonBundleService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonBundle?> GetBundleAsync(int id)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE BundleID = @BundleID;";

            return await _connection.QuerySingleOrDefaultAsync<LessonBundle>(sql, new { BundleID = id });
        }

        public async Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT BundleID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       TotalLessons,
                       PricePerLesson,
                       StartDate,
                       EndDate,
                       QuarterSize,
                       IsActive,
                       Notes,
                       CreatedAt
                FROM LessonBundle
                WHERE StudentID = @StudentID
ORDER BY StudentID";

            return await _connection.QueryAsync<LessonBundle>(sql, new { StudentID = studentId });
        }

        public async Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO LessonBundle
                    (StudentID, TeacherID, LessonTypeID, 
                     TotalLessons, PricePerLesson, StartDate, EndDate, IsActive, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, 
                     @TotalLessons, @PricePerLesson, @StartDate, @EndDate, @IsActive, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, bundle, tx));
        }

        public async Task<bool> UpdateAsync(LessonBundle bundle)
        {
            const string sql = @"
                UPDATE LessonBundle
                SET StudentID      = @StudentID,
                    TeacherID      = @TeacherID,
                    LessonTypeID   = @LessonTypeID,
                    AcademicYear   = @AcademicYear,
                    TotalLessons   = @TotalLessons,
                    PricePerLesson = @PricePerLesson,
                    StartDate      = @StartDate,
                    EndDate        = @EndDate,
                    IsActive       = @IsActive,
                    Notes          = @Notes
                WHERE BundleID = @BundleID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, bundle);
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\LessonRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ILessonAggregateService _aggregateService;
        private readonly ILessonService _lessonService;
        private readonly IBundleQuarterService _bundleQuarterService;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(
            ILessonAggregateService aggregateService,
            ILessonService lessonService,
            IBundleQuarterService bundleQuarterService,
            ILogger<LessonRepository> logger)
        {
            _aggregateService = aggregateService;
            _lessonService = lessonService;
            _bundleQuarterService = bundleQuarterService;
            _logger = logger;
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
            => await _aggregateService.GetLessonByIdAsync(lessonId);

        public async Task<IEnumerable<LessonDetail>> GetByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate)
            => await _aggregateService.GetLessonsByTeacherAndDateAsync(teacherId, scheduledDate);

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
            => await _lessonService.GetByBundleAsync(bundleId);

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            try
            {
                return await _lessonService.InsertAsync(lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert Lesson for BundleID {BundleID} on {ScheduledDate}",
                    lesson.BundleID, lesson.ScheduledDate);
                return null;
            }
        }

        /// <summary>
        /// Updates the lesson status and keeps BundleQuarter.LessonsUsed in sync:
        ///   Completed / Forfeited → +1 (credit consumed)
        ///   CancelledTeacher / CancelledStudent → -1 only if the previous status
        ///   had already consumed a credit (i.e. was Completed or Forfeited).
        /// The delta approach is atomic — no separate read is needed.
        /// </summary>
        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status,
            bool creditForfeited, string? cancelledBy, string? cancellationReason,
            DateTime? completedAt)
        {
            try
            {
                // 1. Read current status so we know whether to adjust the quarter.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                // 2. Update the lesson row.
                var updated = await _lessonService.UpdateStatusAsync(
                    lessonId, status, creditForfeited,
                    cancelledBy, cancellationReason, completedAt);

                if (!updated) return false;

                // 3. Adjust BundleQuarter.LessonsUsed.
                //    Credit is consumed when status moves TO Completed or Forfeited.
                //    Credit is released when status moves FROM Completed or Forfeited
                //    to anything that doesn't consume a credit.
                bool previousConsumed = lesson.Status == LessonStatus.Completed
                                     || lesson.Status == LessonStatus.Forfeited;
                bool newConsumed = status == LessonStatus.Completed
                                || status == LessonStatus.Forfeited;

                int delta = (newConsumed ? 1 : 0) - (previousConsumed ? 1 : 0);

                if (delta != 0)
                    await _bundleQuarterService.AdjustLessonsUsedAsync(lessonId, delta);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update status for LessonID {LessonID}", lessonId);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\LessonService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly IDbConnection _connection;

        public LessonService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Lesson?> GetLessonAsync(int id)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       CreatedAt
                FROM Lesson
                WHERE LessonID = @LessonID;";

            return await _connection.QuerySingleOrDefaultAsync<Lesson>(sql, new { LessonID = id });
        }

        public async Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       CreatedAt
                FROM Lesson
                WHERE BundleID = @BundleID
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { BundleID = bundleId });
        }

        public async Task<IEnumerable<Lesson>> GetByStatusAsync(string status)
        {
            const string sql = @"
                SELECT LessonID,
                       SlotID,
                       BundleID,
                       QuarterID,
                       ScheduledDate,
                       ScheduledTime,
                       Status,
                       CreditForfeited,
                       CancelledBy,
                       CancellationReason,
                       OriginalLessonID,
                       CompletedAt,
                       CreatedAt
                FROM Lesson
                WHERE Status = @Status
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { Status = status });
        }

        public async Task<int> InsertAsync(Lesson lesson)
            => await InsertAsync(lesson);

        public async Task<int> InsertAsync(Lesson lesson, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Lesson
                    (SlotID, BundleID, QuarterID, ScheduledDate, ScheduledTime,
                     Status, CreditForfeited, CancelledBy, CancellationReason,
                     OriginalLessonID, CompletedAt)
                VALUES
                    (@SlotID, @BundleID, @QuarterID, @ScheduledDate, @ScheduledTime,
                     @Status, @CreditForfeited, @CancelledBy, @CancellationReason,
                     @OriginalLessonID, @CompletedAt);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, lesson, tx));
        }

        public async Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt)
        {
            const string sql = @"
                UPDATE Lesson
                SET Status             = @Status,
                    CreditForfeited    = @CreditForfeited,
                    CancelledBy        = @CancelledBy,
                    CancellationReason = @CancellationReason,
                    CompletedAt        = @CompletedAt
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID           = lessonId,
                Status             = status,
                CreditForfeited    = creditForfeited,
                CancelledBy        = cancelledBy,
                CancellationReason = cancellationReason,
                CompletedAt        = completedAt
            });
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\LessonTypeRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly ILessonTypeService _lessonTypeService;
        private readonly ILogger<LessonTypeRepository> _logger;

        public LessonTypeRepository(ILessonTypeService lessonTypeService, ILogger<LessonTypeRepository> logger)
        {
            _lessonTypeService = lessonTypeService;
            _logger = logger;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            return await _lessonTypeService.GetLessonTypeAsync(id);
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            return await _lessonTypeService.GetAllActiveAsync();
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.InsertAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert LessonType {DurationMinutes}min",
                    lessonType.DurationMinutes);
                return null;
            }
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                return await _lessonTypeService.UpdateAsync(lessonType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update LessonTypeID {LessonTypeID}",
                    lessonType.LessonTypeID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\LessonTypeService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeService : ILessonTypeService
    {
        private readonly IDbConnection _connection;

        public LessonTypeService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE LessonTypeID = @LessonTypeID;";

            return await _connection.QuerySingleOrDefaultAsync<LessonType>(sql, new { LessonTypeID = id });
        }

        public async Task<IEnumerable<LessonType>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT LessonTypeID,
                       DurationMinutes,
                       BasePricePerLesson,
                       IsActive
                FROM LessonType
                WHERE IsActive = 1
                ORDER BY DurationMinutes;";

            return await _connection.QueryAsync<LessonType>(sql);
        }

        public async Task<int> InsertAsync(LessonType lessonType)
        {
            const string sql = @"
                INSERT INTO LessonType (DurationMinutes, BasePricePerLesson, IsActive)
                VALUES (@DurationMinutes, @BasePricePerLesson, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, lessonType);
        }

        public async Task<bool> UpdateAsync(LessonType lessonType)
        {
            const string sql = @"
                UPDATE LessonType
                SET DurationMinutes    = @DurationMinutes,
                    BasePricePerLesson = @BasePricePerLesson,
                    IsActive           = @IsActive
                WHERE LessonTypeID = @LessonTypeID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, lessonType);
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\MusicSchool.Repositories.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.72" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.5" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Interfaces\MusicSchool.Interfaces.csproj" />
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

</Project>

```

## File: MusicSchool.Repositories\ScheduledSlotRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotRepository : IScheduledSlotRepository
    {
        private readonly IScheduledSlotService _slotService;
        private readonly ILessonBundleService _bundleService;
        private readonly IBundleQuarterService _quarterService;
        private readonly ILessonService _lessonService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(
            IScheduledSlotService slotService,
            ILessonBundleService bundleService,
            IBundleQuarterService quarterService,
            ILessonService lessonService,
            ILogger<ScheduledSlotRepository> logger)
        {
            _slotService = slotService;
            _bundleService = bundleService;
            _quarterService = quarterService;
            _lessonService = lessonService;
            _logger = logger;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
            => await _slotService.GetSlotAsync(id);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
            => await _slotService.GetActiveByStudentAsync(studentId);

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
            => await _slotService.GetActiveByTeacherAsync(teacherId);

        /// <summary>
        /// Validates that the student has an active bundle with remaining credits,
        /// inserts the slot, then generates all future Lesson rows up to the bundle's
        /// EndDate — one per weekly occurrence matching the slot's DayOfWeek.
        /// Everything runs in a single transaction; nothing is committed if any step fails.
        /// Returns null if the student has no usable bundle, or on any error.
        /// </summary>
        public async Task<int?> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                // 1. Find the student's active bundle that still has remaining credits.
                //    "Active" means IsActive = true, not yet expired, and at least one
                //    quarter still has lessons remaining.
                var bundles = await _bundleService.GetByStudentAsync(slot.StudentID);

                LessonBundle? bundle = null;

                foreach (var b in bundles.Where(b => b.IsActive && b.EndDate >= DateTime.Today))
                {
                    var quartersl = (await _quarterService.GetByBundleAsync(b.BundleID)).ToList();
                    if (quartersl.Any(q => q.LessonsUsed < q.LessonsAllocated))
                    {
                        bundle = b;
                        break;
                    }
                }

                if (bundle is null)
                {
                    _logger.LogWarning(
                        "AddSlotAsync rejected: StudentID {StudentID} has no active bundle with remaining credits.",
                        slot.StudentID);
                    return null;
                }

                var quarters = (await _quarterService.GetByBundleAsync(bundle.BundleID)).ToList();

                // 2. Insert the slot and generate lessons in one transaction.
                await _slotService.ExecuteInTransactionAsync(async (tx, conn) =>
                {
                    var slotId = await _slotService.InsertAsync(slot, tx);
                    slot.SlotID = slotId;

                    // Generate one Lesson per weekly occurrence from the slot's EffectiveFrom
                    // through the bundle's EndDate. EffectiveFrom is authoritative — do not
                    // clamp to today, as slots may be created retroactively or in advance.
                    var lessonDates = GetOccurrences(slot.EffectiveFrom.Date, bundle.EndDate, slot.DayOfWeek);

                    foreach (var date in lessonDates)
                    {
                        var quarter = quarters.FirstOrDefault(q =>
                            date >= q.QuarterStartDate && date <= q.QuarterEndDate);

                        if (quarter is null) continue;

                        await _lessonService.InsertAsync(new Lesson
                        {
                            SlotID          = slotId,
                            BundleID        = bundle.BundleID,
                            QuarterID       = quarter.QuarterID,
                            ScheduledDate   = date,
                            ScheduledTime   = slot.SlotTime,
                            Status          = LessonStatus.Scheduled,
                            CreditForfeited = false,
                        }, tx);
                    }
                });

                return slot.SlotID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ScheduledSlot for StudentID {StudentID}", slot.StudentID);
                return null;
            }
        }

        /// <summary>
        /// Closes a slot by setting EffectiveTo and IsActive = false.
        /// Call AddSlotAsync afterwards to open the replacement slot.
        /// </summary>
        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            try
            {
                return await _slotService.CloseSlotAsync(slotId, effectiveTo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to close SlotID {SlotID}", slotId);
                return false;
            }
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        /// <summary>
        /// Enumerates every calendar date between <paramref name="from"/> and
        /// <paramref name="to"/> (inclusive) that falls on <paramref name="isoDayOfWeek"/>
        /// (1 = Monday … 7 = Sunday, matching the ScheduledSlot.DayOfWeek convention).
        /// </summary>
        private static IEnumerable<DateTime> GetOccurrences(
            DateTime from, DateTime to, byte isoDayOfWeek)
        {
            // .NET DayOfWeek: Sunday=0, Monday=1 … Saturday=6
            // ISO DayOfWeek:  Monday=1, Tuesday=2 … Sunday=7
            var targetDotNet = isoDayOfWeek == 7
                ? DayOfWeek.Sunday
                : (DayOfWeek)isoDayOfWeek;

            var date = from;
            // Advance to the first matching weekday
            while (date.DayOfWeek != targetDotNet)
                date = date.AddDays(1);

            while (date <= to)
            {
                yield return date;
                date = date.AddDays(7);
            }
        }
    }
}

```

## File: MusicSchool.Repositories\ScheduledSlotService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotService : IScheduledSlotService
    {
        private readonly IDbConnection _connection;

        public ScheduledSlotService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE SlotID = @SlotID;";

            return await _connection.QuerySingleOrDefaultAsync<ScheduledSlot>(sql, new { SlotID = id });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE StudentID  = @StudentID
                  AND IsActive   = 1
                  AND EffectiveTo IS NULL
                ORDER BY DayOfWeek, SlotTime;";

            return await _connection.QueryAsync<ScheduledSlot>(sql, new { StudentID = studentId });
        }

        public async Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            const string sql = @"
                SELECT SlotID,
                       StudentID,
                       TeacherID,
                       LessonTypeID,
                       DayOfWeek,
                       SlotTime,
                       EffectiveFrom,
                       EffectiveTo,
                       IsActive
                FROM ScheduledSlot
                WHERE TeacherID  = @TeacherID
                  AND IsActive   = 1
                  AND EffectiveTo IS NULL
                ORDER BY DayOfWeek, SlotTime;";

            return await _connection.QueryAsync<ScheduledSlot>(sql, new { TeacherID = teacherId });
        }

        public async Task<int> InsertAsync(ScheduledSlot slot)
            => await InsertAsync(slot);

        public async Task<int> InsertAsync(ScheduledSlot slot, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO ScheduledSlot
                    (StudentID, TeacherID, LessonTypeID, DayOfWeek,
                     SlotTime, EffectiveFrom, EffectiveTo, IsActive)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @DayOfWeek,
                     @SlotTime, @EffectiveFrom, @EffectiveTo, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, slot, tx));
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            const string sql = @"
                UPDATE ScheduledSlot
                SET EffectiveTo = @EffectiveTo,
                    IsActive    = 0
                WHERE SlotID = @SlotID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { SlotID = slotId, EffectiveTo = effectiveTo });
            return rowsAffected > 0;
        }

        /// <summary>
        /// Opens the connection if needed, begins a transaction, runs <paramref name="work"/>,
        /// and commits. Rolls back on any exception.
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<IDbTransaction, IDbConnection, Task> work)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var tx = _connection.BeginTransaction();
            try
            {
                await work(tx, _connection);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\StudentRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(IStudentService studentService, ILogger<StudentRepository> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            return await _studentService.GetStudentAsync(id);
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            return await _studentService.GetByAccountHolderAsync(accountHolderId);
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            try
            {
                return await _studentService.InsertAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Student {FirstName} {LastName}",
                    student.FirstName, student.LastName);
                return null;
            }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                return await _studentService.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update StudentID {StudentID}", student.StudentID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\StudentService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IDbConnection _connection;

        public StudentService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE StudentID = @StudentID;";

            return await _connection.QuerySingleOrDefaultAsync<Student>(sql, new { StudentID = id });
        }

        public async Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT StudentID,
                       AccountHolderID,
                       FirstName,
                       LastName,
                       DateOfBirth,
                       IsAccountHolder,
                       IsActive,
                       CreatedAt
                FROM Student
                WHERE AccountHolderID = @AccountHolderID
                  AND IsActive        = 1
                ORDER BY LastName, FirstName;";

            return await _connection.QueryAsync<Student>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Student student)
        {
            const string sql = @"
                INSERT INTO Student
                    (AccountHolderID, FirstName, LastName, DateOfBirth, IsAccountHolder, IsActive)
                VALUES
                    (@AccountHolderID, @FirstName, @LastName, @DateOfBirth, @IsAccountHolder, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, student);
        }

        public async Task<bool> UpdateAsync(Student student)
        {
            const string sql = @"
                UPDATE Student
                SET AccountHolderID = @AccountHolderID,
                    FirstName       = @FirstName,
                    LastName        = @LastName,
                    DateOfBirth     = @DateOfBirth,
                    IsAccountHolder = @IsAccountHolder,
                    IsActive        = @IsActive
                WHERE StudentID = @StudentID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, student);
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Repositories\TeacherRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ITeacherService teacherService, ILogger<TeacherRepository> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            return await _teacherService.GetTeacherAsync(id);
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            return await _teacherService.GetAllActiveAsync();
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            try
            {
                return await _teacherService.InsertAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert Teacher {Name}", teacher.Name);
                return null;
            }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                return await _teacherService.UpdateAsync(teacher);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update TeacherID {TeacherID}", teacher.TeacherID);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\TeacherService.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly IDbConnection _connection;

        public TeacherService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE TeacherID = @TeacherID;";

            return await _connection.QuerySingleOrDefaultAsync<Teacher>(sql, new { TeacherID = id });
        }

        public async Task<IEnumerable<Teacher>> GetAllActiveAsync()
        {
            const string sql = @"
                SELECT TeacherID,
                       Name,
                       Email,
                       Phone,
                       IsActive,
                       CreatedAt
                FROM Teacher
                WHERE IsActive = 1
                ORDER BY Name;";

            return await _connection.QueryAsync<Teacher>(sql);
        }

        public async Task<int> InsertAsync(Teacher teacher)
        {
            const string sql = @"
                INSERT INTO Teacher (Name, Email, Phone, IsActive)
                VALUES (@Name, @Email, @Phone, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, teacher);
        }

        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            const string sql = @"
                UPDATE Teacher
                SET Name     = @Name,
                    Email    = @Email,
                    Phone    = @Phone,
                    IsActive = @IsActive
                WHERE TeacherID = @TeacherID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, teacher);
            return rowsAffected > 0;
        }
    }
}

```

## File: MusicSchool.Web\Pages\AccountHolderDetail.razor

```razor
@page "/account-holders/{AccountHolderID:int}"
@using MusicSchool.Data.Models
@using MusicSchool.Web.Shared
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
@inject InvoiceService InvoiceSvc
@inject ISnackbar Snackbar
@inject NavigationManager Nav

<PageTitle>Account Holder — Music School</PageTitle>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}
else if (_accountHolder is null)
{
    <MudText>Account holder not found.</MudText>
}
else
{
    <div class="page-header d-flex justify-space-between align-center">
        <div>
            <MudBreadcrumbs Items="_breadcrumbs" />
            <MudText Typo="Typo.h5">@_accountHolder.FullName</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">@_accountHolder.Email</MudText>
        </div>
        <MudStack Row="true" Spacing="2">
            <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Edit"
                       OnClick="OpenEditDialog">Edit</MudButton>
            <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.PersonAdd"
                       OnClick="OpenAddStudentDialog">Add Student</MudButton>
        </MudStack>
    </div>

    <MudGrid>
        <MudItem xs="12" md="4">
            <MudPaper Class="pa-4" Elevation="1">
                <MudText Typo="Typo.h6" Class="mb-3">Account Details</MudText>
                <MudDivider Class="mb-3" />
                <MudText Typo="Typo.body2" Color="Color.Secondary">Phone</MudText>
                <MudText Class="mb-2">@(_accountHolder.Phone ?? "—")</MudText>
                <MudText Typo="Typo.body2" Color="Color.Secondary">Billing Address</MudText>
                <MudText Class="mb-2">@(_accountHolder.BillingAddress ?? "—")</MudText>
                <MudText Typo="Typo.body2" Color="Color.Secondary">Status</MudText>
                <MudChip T="string" Size="Size.Small" Color="@(_accountHolder.IsActive ? Color.Success : Color.Default)">
                    @(_accountHolder.IsActive ? "Active" : "Inactive")
                </MudChip>
            </MudPaper>

            <MudPaper Class="pa-4 mt-4" Elevation="1">
                <MudText Typo="Typo.h6" Class="mb-3">Outstanding Invoices</MudText>
                @if (_outstandingInvoices.Any())
                {
                    @foreach (var inv in _outstandingInvoices)
                    {
                        <MudPaper Class="pa-2 mb-2" Elevation="0" Style="background:#F0F2F5;">
                            <MudStack Row="true" Justify="Justify.SpaceBetween">
                                <MudText Typo="Typo.body2">Instalment #@inv.InstallmentNumber — R@(inv.Amount.ToString("N2"))</MudText>
                                <StatusChip Status="@inv.Status" />
                            </MudStack>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Due: @inv.DueDate.ToString("dd MMM yyyy")</MudText>
                        </MudPaper>
                    }
                }
                else
                {
                    <MudText Color="Color.Secondary" Typo="Typo.body2">No outstanding invoices.</MudText>
                }
            </MudPaper>
        </MudItem>

        <MudItem xs="12" md="8">
            <MudPaper Elevation="1">
                <MudTable Items="_students" Hover="true" Dense="false" Loading="_studentsLoading">
                    <ToolBarContent>
                        <MudText Typo="Typo.h6">Students (@_students.Count)</MudText>
                    </ToolBarContent>
                    <HeaderContent>
                        <MudTh>Name</MudTh>
                        <MudTh>Date of Birth</MudTh>
                        <MudTh>Is Account Holder</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>
                            <MudButton Variant="Variant.Text" Color="Color.Primary"
                                       OnClick="@(() => Nav.NavigateTo($"/students/{context.StudentID}"))">
                                @context.FullName
                            </MudButton>
                        </MudTd>
                        <MudTd>@(context.DateOfBirth?.ToString("dd MMM yyyy") ?? "—")</MudTd>
                        <MudTd>
                            @if (context.IsAccountHolder)
                            {
                                <MudIcon Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Success" Size="Size.Small" />
                            }
                        </MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small" Color="@(context.IsActive ? Color.Success : Color.Default)">
                                @(context.IsActive ? "Active" : "Inactive")
                            </MudChip>
                        </MudTd>
                        <MudTd>
                            <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" Color="Color.Primary"
                                           OnClick="@(() => OpenEditStudentDialog(context))" />
                            <MudIconButton Icon="@Icons.Material.Filled.OpenInNew" Size="Size.Small" Color="Color.Secondary"
                                           OnClick="@(() => Nav.NavigateTo($"/students/{context.StudentID}"))" />
                        </MudTd>
                    </RowTemplate>
                    <NoRecordsContent>
                        <MudText>No students enrolled.</MudText>
                    </NoRecordsContent>
                </MudTable>
            </MudPaper>
        </MudItem>
    </MudGrid>
}

<!-- Edit Account Holder Dialog -->
<MudDialog @ref="_editDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Edit Account Holder</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_editForm">
            <MudTextField @bind-Value="_editAH.FirstName" Label="First Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.LastName" Label="Last Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.Email" Label="Email" Required="true"
                          InputType="InputType.Email" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.Phone" Label="Phone" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.BillingAddress" Label="Billing Address" Lines="3" Class="mb-3" />
            <MudSwitch @bind-Value="_editAH.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _editDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveEditAH">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Add Student Dialog -->
<MudDialog @ref="_addStudentDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Student</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_addStudentForm">
            <MudTextField @bind-Value="_newStudent.FirstName" Label="First Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_newStudent.LastName" Label="Last Name" Required="true" Class="mb-3" />
            <MudDatePicker @bind-Date="_dobDate" Label="Date of Birth" Class="mb-3" />
            <MudSwitch @bind-Value="_newStudent.IsAccountHolder" Label="Same as Account Holder" Color="Color.Primary" Class="mb-2" />
            <MudSwitch @bind-Value="_newStudent.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addStudentDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveNewStudent">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Edit Student Dialog -->
<MudDialog @ref="_editStudentDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Edit Student</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_editStudentForm">
            <MudTextField @bind-Value="_editStudent.FirstName" Label="First Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_editStudent.LastName" Label="Last Name" Required="true" Class="mb-3" />
            <MudDatePicker @bind-Date="_editDobDate" Label="Date of Birth" Class="mb-3" />
            <MudSwitch @bind-Value="_editStudent.IsAccountHolder" Label="Same as Account Holder" Color="Color.Primary" Class="mb-2" />
            <MudSwitch @bind-Value="_editStudent.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _editStudentDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveEditStudent">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [Parameter] public int AccountHolderID { get; set; }

    private bool _loading = true;
    private bool _studentsLoading;
    private AccountHolder? _accountHolder;
    private List<Student> _students = [];
    private List<Invoice> _outstandingInvoices = [];

    private MudDialog? _editDialog, _addStudentDialog, _editStudentDialog;
    private MudForm? _editForm, _addStudentForm, _editStudentForm;
    private AccountHolder _editAH = new();
    private Student _newStudent = new();
    private Student _editStudent = new();
    private DateTime? _dobDate;
    private DateTime? _editDobDate;

    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };
    private List<BreadcrumbItem> _breadcrumbs = [];

    protected override async Task OnInitializedAsync()
    {
        _breadcrumbs =
        [
            new BreadcrumbItem("Account Holders", href: "/account-holders"),
            new BreadcrumbItem("Detail", href: null, disabled: true)
        ];

        _loading = true;
        _accountHolder = await AccountHolderSvc.GetAccountHolderAsync(AccountHolderID);

        if (_accountHolder is not null)
        {
            _studentsLoading = true;
            _students = await StudentSvc.GetByAccountHolderAsync(AccountHolderID);
            _studentsLoading = false;
            _outstandingInvoices = await InvoiceSvc.GetOutstandingByAccountHolderAsync(AccountHolderID);
        }

        _loading = false;
    }

    private async Task OpenEditDialog()
    {
        if (_accountHolder is null) return;
        _editAH = new AccountHolder
        {
            AccountHolderID = _accountHolder.AccountHolderID,
            TeacherID = _accountHolder.TeacherID,
            FirstName = _accountHolder.FirstName, LastName = _accountHolder.LastName,
            Email = _accountHolder.Email, Phone = _accountHolder.Phone,
            BillingAddress = _accountHolder.BillingAddress, IsActive = _accountHolder.IsActive
        };
        await _editDialog!.ShowAsync();
    }

    private async Task SaveEditAH()
    {
        await _editForm!.Validate();
        if (!_editForm.IsValid) return;
        var result = await AccountHolderSvc.UpdateAccountHolderAsync(_editAH);
        if (result)
        {
            Snackbar.Add("Account holder updated.", Severity.Success);
            _accountHolder = _editAH;
            await _editDialog!.CloseAsync(DialogResult.Ok(true));
        }
        else Snackbar.Add("Failed to update.", Severity.Error);
    }

    private async Task OpenAddStudentDialog()
    {
        _newStudent = new Student { IsActive = true, AccountHolderID = AccountHolderID };
        _dobDate = null;
        await _addStudentDialog!.ShowAsync();
    }

    private async Task OpenEditStudentDialog(Student s)
    {
        _editStudent = new Student
        {
            StudentID = s.StudentID, AccountHolderID = s.AccountHolderID,
            FirstName = s.FirstName, LastName = s.LastName,
            IsAccountHolder = s.IsAccountHolder, IsActive = s.IsActive
        };
        _editDobDate = s.DateOfBirth.HasValue
            ? new DateTime(s.DateOfBirth.Value.Year, s.DateOfBirth.Value.Month, s.DateOfBirth.Value.Day)
            : null;
        await _editStudentDialog!.ShowAsync();
    }

    private async Task SaveNewStudent()
    {
        await _addStudentForm!.Validate();
        if (!_addStudentForm.IsValid) return;
        if (_dobDate.HasValue)
            _newStudent.DateOfBirth = _dobDate.Value;
        var result = await StudentSvc.AddStudentAsync(_newStudent);
        if (result.HasValue)
        {
            Snackbar.Add("Student added.", Severity.Success);
            await _addStudentDialog!.CloseAsync(DialogResult.Ok(true));
            _students = await StudentSvc.GetByAccountHolderAsync(AccountHolderID);
        }
        else Snackbar.Add("Failed to add student.", Severity.Error);
    }

    private async Task SaveEditStudent()
    {
        await _editStudentForm!.Validate();
        if (!_editStudentForm.IsValid) return;
        if (_editDobDate.HasValue)
            _editStudent.DateOfBirth = _editDobDate.Value;
        var result = await StudentSvc.UpdateStudentAsync(_editStudent);
        if (result)
        {
            Snackbar.Add("Student updated.", Severity.Success);
            await _editStudentDialog!.CloseAsync(DialogResult.Ok(true));
            _students = await StudentSvc.GetByAccountHolderAsync(AccountHolderID);
        }
        else Snackbar.Add("Failed to update student.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\AccountHolders.razor

```razor
@page "/account-holders"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
@inject ISnackbar Snackbar
@inject NavigationManager Nav

<PageTitle>Account Holders — Music School</PageTitle>

<div class="page-header d-flex justify-space-between align-center">
    <div>
        <MudText Typo="Typo.h5">Account Holders</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">Manage account holders and their enrolled students</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add"
               OnClick="OpenAddDialog" Disabled="_teachers.Count == 0">Add Account Holder</MudButton>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid>
        <MudItem xs="12" sm="6" md="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Filter by Teacher"
                       @bind-Value:after="OnTeacherChanged" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudTextField @bind-Value="_searchString" Label="Search" Immediate="true"
                          Adornment="Adornment.Start" AdornmentIcon="@Icons.Material.Filled.Search" />
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

<MudPaper Elevation="1">
    <MudTable Items="FilteredAccountHolders" Hover="true" Dense="false" Loading="_loading"
              MultiSelection="false" @bind-SelectedItem="_selected">
        <HeaderContent>
            <MudTh>Name</MudTh>
            <MudTh>Email</MudTh>
            <MudTh>Phone</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd>
                <MudButton Variant="Variant.Text" Color="Color.Primary"
                           OnClick="@(() => Nav.NavigateTo($"/account-holders/{context.AccountHolderID}"))">
                    @context.FullName
                </MudButton>
            </MudTd>
            <MudTd>@context.Email</MudTd>
            <MudTd>@(context.Phone ?? "—")</MudTd>
            <MudTd>
                <MudChip T="string" Size="Size.Small" Color="@(context.IsActive ? Color.Success : Color.Default)">
                    @(context.IsActive ? "Active" : "Inactive")
                </MudChip>
            </MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" Color="Color.Primary"
                               OnClick="@(() => OpenEditDialog(context))" />
                <MudIconButton Icon="@Icons.Material.Filled.OpenInNew" Size="Size.Small" Color="Color.Secondary"
                               OnClick="@(() => Nav.NavigateTo($"/account-holders/{context.AccountHolderID}"))" />
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>No account holders found. Select a teacher to load account holders.</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>

<!-- Add Dialog -->
<MudDialog @ref="_addDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Account Holder</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_addForm">
            <MudSelect @bind-Value="_newAH.TeacherID" Label="Teacher" Required="true"
                       RequiredError="Teacher is required" Class="mb-3">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
            <MudTextField @bind-Value="_newAH.FirstName" Label="First Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_newAH.LastName" Label="Last Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_newAH.Email" Label="Email" Required="true"
                          InputType="InputType.Email" Class="mb-3" />
            <MudTextField @bind-Value="_newAH.Phone" Label="Phone" Class="mb-3" />
            <MudTextField @bind-Value="_newAH.BillingAddress" Label="Billing Address" Lines="3" Class="mb-3" />
            <MudSwitch @bind-Value="_newAH.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveNew">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Edit Dialog -->
<MudDialog @ref="_editDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Edit Account Holder</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_editForm">
            <MudSelect @bind-Value="_editAH.TeacherID" Label="Teacher" Required="true" Class="mb-3">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
            <MudTextField @bind-Value="_editAH.FirstName" Label="First Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.LastName" Label="Last Name" Required="true" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.Email" Label="Email" Required="true"
                          InputType="InputType.Email" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.Phone" Label="Phone" Class="mb-3" />
            <MudTextField @bind-Value="_editAH.BillingAddress" Label="Billing Address" Lines="3" Class="mb-3" />
            <MudSwitch @bind-Value="_editAH.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _editDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveEdit">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private AccountHolder? _selected;
    private int _selectedTeacherId;
    private string _searchString = string.Empty;

    private MudDialog? _addDialog, _editDialog;
    private MudForm? _addForm, _editForm;
    private AccountHolder _newAH = new();
    private AccountHolder _editAH = new();
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    private IEnumerable<AccountHolder> FilteredAccountHolders =>
        _accountHolders.Where(a =>
            string.IsNullOrWhiteSpace(_searchString) ||
            a.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
            a.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
    }

    private async Task OnTeacherChanged()
    {
        if (_selectedTeacherId > 0)
        {
            _loading = true;
            _accountHolders = await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId);
            _loading = false;
        }
        else
        {
            _accountHolders = [];
        }
    }

    private async Task OpenAddDialog()
    {
        _newAH = new AccountHolder { IsActive = true, TeacherID = _selectedTeacherId };
        await _addDialog!.ShowAsync();
    }

    private async Task OpenEditDialog(AccountHolder ah)
    {
        _editAH = new AccountHolder
        {
            AccountHolderID = ah.AccountHolderID, TeacherID = ah.TeacherID,
            FirstName = ah.FirstName, LastName = ah.LastName,
            Email = ah.Email, Phone = ah.Phone,
            BillingAddress = ah.BillingAddress, IsActive = ah.IsActive,
            CreatedAt = ah.CreatedAt
        };
        await _editDialog!.ShowAsync();
    }

    private async Task SaveNew()
    {
        await _addForm!.Validate();
        if (!_addForm.IsValid) return;
        var result = await AccountHolderSvc.AddAccountHolderAsync(_newAH);
        if (result.HasValue)
        {
            Snackbar.Add("Account holder added.", Severity.Success);
            await _addDialog!.CloseAsync(DialogResult.Ok(true));
            await OnTeacherChanged();
        }
        else Snackbar.Add("Failed to add account holder.", Severity.Error);
    }

    private async Task SaveEdit()
    {
        await _editForm!.Validate();
        if (!_editForm.IsValid) return;
        var result = await AccountHolderSvc.UpdateAccountHolderAsync(_editAH);
        if (result)
        {
            Snackbar.Add("Account holder updated.", Severity.Success);
            await _editDialog!.CloseAsync(DialogResult.Ok(true));
            await OnTeacherChanged();
        }
        else Snackbar.Add("Failed to update account holder.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\ExtraLessons.razor

```razor
@page "/extra-lessons"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
@inject ExtraLessonService ExtraLessonSvc
@inject LessonTypeService LessonTypeSvc
@inject ISnackbar Snackbar

<PageTitle>Extra Lessons — Music School</PageTitle>

<div class="page-header d-flex justify-space-between align-center">
    <div>
        <MudText Typo="Typo.h5">Extra Lessons</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">Ad-hoc lessons purchased outside of a bundle</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add"
               OnClick="OpenAddDialog" Disabled="_selectedStudentId == 0">Add Extra Lesson</MudButton>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Teacher"
                       @bind-Value:after="OnTeacherChanged" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedAccountHolderId" Label="Account Holder"
                       @bind-Value:after="OnAccountHolderChanged"
                       Disabled="_accountHolders.Count == 0" Clearable="true">
                @foreach (var ah in _accountHolders)
                {
                    <MudSelectItem Value="ah.AccountHolderID">@ah.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedStudentId" Label="Student"
                       @bind-Value:after="OnStudentChanged"
                       Disabled="_students.Count == 0" Clearable="true">
                @foreach (var s in _students)
                {
                    <MudSelectItem Value="s.StudentID">@s.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

<MudPaper Elevation="1">
    <MudTable Items="_extraLessons" Hover="true" Dense="false" Loading="_loading">
        <HeaderContent>
            <MudTh>Date</MudTh>
            <MudTh>Time</MudTh>
            <MudTh>Duration</MudTh>
            <MudTh>Price Charged</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Notes</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd>@context.ScheduledDate.ToString("dd MMM yyyy")</MudTd>
            <MudTd>@context.ScheduledTime.ToString("HH:mm")</MudTd>
            <MudTd>@(_lessonTypes.FirstOrDefault(lt => lt.LessonTypeID == context.LessonTypeID)?.DurationMinutes ?? 0) min</MudTd>
            <MudTd>R @context.PriceCharged.ToString("N2")</MudTd>
            <MudTd><StatusChip Status="@context.Status" /></MudTd>
            <MudTd>@(context.Notes ?? "—")</MudTd>
            <MudTd>
                @if (context.Status == ExtraLessonStatus.Scheduled)
                {
                    <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small" Color="Color.Success"
                                   Title="Complete" OnClick="@(() => UpdateStatus(context, ExtraLessonStatus.Completed))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Cancel" Size="Size.Small" Color="Color.Warning"
                                   Title="Cancel" OnClick="@(() => UpdateStatus(context, ExtraLessonStatus.Cancelled))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Block" Size="Size.Small" Color="Color.Error"
                                   Title="Forfeit" OnClick="@(() => UpdateStatus(context, ExtraLessonStatus.Forfeited))" />
                }
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>@(_selectedStudentId == 0 ? "Select a student to view extra lessons." : "No extra lessons found.")</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>

<!-- Add Extra Lesson Dialog -->
<MudDialog @ref="_addDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Extra Lesson</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_addForm">
            <MudSelect @bind-Value="_newExtra.LessonTypeID" Label="Lesson Type" Required="true" Class="mb-3"
                       @bind-Value:after="SetBasePrice">
                @foreach (var lt in _lessonTypes)
                {
                    <MudSelectItem Value="lt.LessonTypeID">@lt.DisplayName</MudSelectItem>
                }
            </MudSelect>
            <MudDatePicker @bind-Date="_extraDate" Label="Date" Required="true" Class="mb-3" />
            <MudTimePicker @bind-Time="_extraTime" Label="Time" Required="true" AmPm="false" Class="mb-3" />
            <MudNumericField @bind-Value="_newExtra.PriceCharged" Label="Price Charged"
                             Required="true" Min="0m" Format="N2"
                             Adornment="Adornment.Start" AdornmentText="R" Class="mb-3"
                             HelperText="Base price auto-filled; teacher may override" />
            <MudTextField @bind-Value="_newExtra.Notes" Label="Notes" Lines="2" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveExtra">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private List<Student> _students = [];
    private List<ExtraLesson> _extraLessons = [];
    private List<LessonType> _lessonTypes = [];
    private int _selectedTeacherId, _selectedAccountHolderId, _selectedStudentId;

    private MudDialog? _addDialog;
    private MudForm? _addForm;
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    private ExtraLesson _newExtra = new();
    private DateTime? _extraDate;
    private TimeSpan? _extraTime;

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();
    }

    private async Task OnTeacherChanged()
    {
        _accountHolders = _selectedTeacherId > 0
            ? await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId) : [];
        _students = [];
        _extraLessons = [];
        _selectedAccountHolderId = 0;
        _selectedStudentId = 0;
    }

    private async Task OnAccountHolderChanged()
    {
        _students = _selectedAccountHolderId > 0
            ? await StudentSvc.GetByAccountHolderAsync(_selectedAccountHolderId) : [];
        _extraLessons = [];
        _selectedStudentId = 0;
    }

    private async Task OnStudentChanged()
    {
        if (_selectedStudentId > 0)
        {
            _loading = true;
            _extraLessons = await ExtraLessonSvc.GetByStudentAsync(_selectedStudentId);
            _loading = false;
        }
        else _extraLessons = [];
    }

    private async Task UpdateStatus(ExtraLesson extra, string status)
    {
        var result = await ExtraLessonSvc.UpdateExtraLessonStatusAsync(extra.ExtraLessonID, status);
        if (result) { Snackbar.Add("Updated.", Severity.Success); await OnStudentChanged(); }
        else Snackbar.Add("Failed.", Severity.Error);
    }

    private async Task OpenAddDialog()
    {
        _newExtra = new ExtraLesson
        {
            StudentID = _selectedStudentId,
            TeacherID = _selectedTeacherId,
            Status = ExtraLessonStatus.Scheduled
        };
        _extraDate = DateTime.Today;
        _extraTime = TimeSpan.FromHours(9);
        await _addDialog!.ShowAsync();
    }

    private void SetBasePrice()
    {
        var lt = _lessonTypes.FirstOrDefault(l => l.LessonTypeID == _newExtra.LessonTypeID);
        if (lt is not null) _newExtra.PriceCharged = lt.BasePricePerLesson;
    }

    private async Task SaveExtra()
    {
        await _addForm!.Validate();
        if (!_addForm.IsValid) return;
        if (_extraDate is null || _extraTime is null) { Snackbar.Add("Date and time required.", Severity.Warning); return; }
        _newExtra.ScheduledDate = _extraDate.Value.Date;
        _newExtra.ScheduledTime = _extraDate.Value.Date.AddHours(_extraTime.Value.Hours);

        var result = await ExtraLessonSvc.AddExtraLessonAsync(_newExtra);
        if (result.HasValue)
        {
            Snackbar.Add("Extra lesson added.", Severity.Success);
            await _addDialog!.CloseAsync(DialogResult.Ok(true));
            await OnStudentChanged();
        }
        else Snackbar.Add("Failed to add extra lesson.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\Index.razor

```razor
@page "/"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject LessonTypeService LessonTypeSvc

<PageTitle>Dashboard — Music School</PageTitle>

<div class="page-header">
    <MudText Typo="Typo.h5">Dashboard</MudText>
    <MudText Typo="Typo.body2" Color="Color.Secondary">Welcome to the Music School Management Portal</MudText>
</div>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="mb-4" />
}

<MudGrid>
    <MudItem xs="12" sm="6" md="3">
        <MudPaper Class="pa-4 stats-card" Elevation="1">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.Person" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_teacherCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Active Teachers</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudPaper Class="pa-4 stats-card" Elevation="1">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.People" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_accountHolderCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Account Holders</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudPaper Class="pa-4 stats-card" Elevation="1">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.Timer" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_lessonTypeCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Lesson Types</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>
    <MudItem xs="12" sm="6" md="3">
        <MudPaper Class="pa-4 stats-card" Elevation="1">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.Today" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@DateTime.Today.ToString("dd MMM")</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Today</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>
</MudGrid>

<MudGrid Class="mt-4">
    <MudItem xs="12" md="6">
        <MudPaper Class="pa-4" Elevation="1">
            <MudText Typo="Typo.h6" Class="mb-3">Quick Navigation</MudText>
            <MudStack Spacing="2">
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.People"
                           Href="/account-holders" FullWidth="true">Manage Account Holders</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Inventory"
                           Href="/lesson-bundles" FullWidth="true">View Lesson Bundles</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.CalendarMonth"
                           Href="/schedule" FullWidth="true">Today's Schedule</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Receipt"
                           Href="/invoices" FullWidth="true">Manage Invoices</MudButton>
            </MudStack>
        </MudPaper>
    </MudItem>
    <MudItem xs="12" md="6">
        <MudPaper Class="pa-4" Elevation="1">
            <MudText Typo="Typo.h6" Class="mb-3">Active Lesson Types</MudText>
            @if (_lessonTypes.Any())
            {
                <MudTable Items="_lessonTypes" Dense="true" Hover="true" Elevation="0">
                    <HeaderContent>
                        <MudTh>Duration</MudTh>
                        <MudTh>Base Price / Lesson</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.DurationMinutes minutes</MudTd>
                        <MudTd>R @context.BasePricePerLesson.ToString("N2")</MudTd>
                    </RowTemplate>
                </MudTable>
            }
            else if (!_loading)
            {
                <MudText Color="Color.Secondary">No lesson types configured.</MudText>
            }
        </MudPaper>
    </MudItem>
</MudGrid>

@code {
    private bool _loading = true;
    private int _teacherCount;
    private int _accountHolderCount;
    private int _lessonTypeCount;
    private List<LessonType> _lessonTypes = [];

    protected override async Task OnInitializedAsync()
    {
        var teachers = await TeacherSvc.GetAllActiveAsync();
        _teacherCount = teachers.Count;

        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();
        _lessonTypeCount = _lessonTypes.Count;

        // Account holder count: sum across all teachers
        int ahCount = 0;
        foreach (var t in teachers)
        {
            var ahs = await AccountHolderSvc.GetByTeacherAsync(t.TeacherID);
            ahCount += ahs.Count(a => a.IsActive);
        }
        _accountHolderCount = ahCount;

        _loading = false;
    }
}

```

## File: MusicSchool.Web\Pages\Invoices.razor

```razor
@page "/invoices"

@using MusicSchool.Data.Models
@using MusicSchool.Web.Shared

@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject InvoiceService InvoiceSvc
@inject ISnackbar Snackbar

<PageTitle>Invoices — Music School</PageTitle>

<div class="page-header">
    <MudText Typo="Typo.h5">Invoices</MudText>
    <MudText Typo="Typo.body2" Color="Color.Secondary">Manage monthly instalments for lesson bundles</MudText>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Teacher"
                       @bind-Value:after="OnTeacherChanged" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedAccountHolderId" Label="Account Holder"
                       @bind-Value:after="LoadInvoices"
                       Disabled="_accountHolders.Count == 0" Clearable="true">
                @foreach (var ah in _accountHolders)
                {
                    <MudSelectItem Value="ah.AccountHolderID">@ah.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_statusFilter" Label="Status Filter"
                       @bind-Value:after="ApplyFilter" Clearable="true">
                <MudSelectItem Value="@("Pending")">Pending</MudSelectItem>
                <MudSelectItem Value="@("Paid")">Paid</MudSelectItem>
                <MudSelectItem Value="@("Overdue")">Overdue</MudSelectItem>
                <MudSelectItem Value="@("Void")">Void</MudSelectItem>
            </MudSelect>
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

@if (_filteredInvoices.Any())
{
    <!-- Summary cards -->
    <MudGrid Class="mb-4">
        <MudItem xs="6" sm="3">
            <MudPaper Class="pa-3 stats-card" Elevation="1">
                <MudText Typo="Typo.body2" Color="Color.Secondary">Total</MudText>
                <MudText Typo="Typo.h6">R @_filteredInvoices.Sum(i => i.Amount).ToString("N2")</MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="6" sm="3">
            <MudPaper Class="pa-3 stats-card" Elevation="1">
                <MudText Typo="Typo.body2" Color="Color.Secondary">Paid</MudText>
                <MudText Typo="Typo.h6" Style="color:#2E7D32;">
                    R @_filteredInvoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Amount).ToString("N2")
                </MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="6" sm="3">
            <MudPaper Class="pa-3 stats-card" Elevation="1">
                <MudText Typo="Typo.body2" Color="Color.Secondary">Outstanding</MudText>
                <MudText Typo="Typo.h6" Style="color:#F57F17;">
                    R @_filteredInvoices.Where(i => i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.Overdue).Sum(i => i.Amount).ToString("N2")
                </MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="6" sm="3">
            <MudPaper Class="pa-3 stats-card" Elevation="1">
                <MudText Typo="Typo.body2" Color="Color.Secondary">Overdue</MudText>
                <MudText Typo="Typo.h6" Style="color:#C62828;">
                    R @_filteredInvoices.Where(i => i.Status == InvoiceStatus.Overdue).Sum(i => i.Amount).ToString("N2")
                </MudText>
            </MudPaper>
        </MudItem>
    </MudGrid>
}

<MudPaper Elevation="1">
    <MudTable Items="_filteredInvoices" Hover="true" Dense="false" Loading="_loading"
              GroupBy="_groupByBundle">
        <HeaderContent>
            <MudTh>Instalment #</MudTh>
            <MudTh>Amount</MudTh>
            <MudTh>Due Date</MudTh>
            <MudTh>Paid Date</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <GroupHeaderTemplate>
            <MudTh colspan="6" Style="padding:8px 16px; background:#F0F2F5;">
                <MudText Typo="Typo.subtitle2">Bundle #@context.Key</MudText>
            </MudTh>
        </GroupHeaderTemplate>
        <RowTemplate>
            <MudTd>Instalment @context.InstallmentNumber of 12</MudTd>
            <MudTd>R @context.Amount.ToString("N2")</MudTd>
            <MudTd>@context.DueDate.ToString("dd MMM yyyy")</MudTd>
            <MudTd>@(context.PaidDate?.ToString("dd MMM yyyy") ?? "—")</MudTd>
            <MudTd><StatusChip Status="@context.Status" /></MudTd>
            <MudTd>
                @if (context.Status == InvoiceStatus.Pending || context.Status == InvoiceStatus.Overdue)
                {
                    <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                   Color="Color.Success" Title="Mark Paid"
                                   OnClick="@(() => OpenMarkPaidDialog(context))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Warning" Size="Size.Small"
                                   Color="Color.Warning" Title="Mark Overdue"
                                   OnClick="@(() => QuickUpdateStatus(context, InvoiceStatus.Overdue))" />
                    <MudIconButton Icon="@Icons.Material.Filled.Block" Size="Size.Small"
                                   Color="Color.Error" Title="Void"
                                   OnClick="@(() => QuickUpdateStatus(context, InvoiceStatus.Void))" />
                }
                else if (context.Status == InvoiceStatus.Paid)
                {
                    <MudIconButton Icon="@Icons.Material.Filled.Undo" Size="Size.Small"
                                   Color="Color.Default" Title="Revert to Pending"
                                   OnClick="@(() => QuickUpdateStatus(context, InvoiceStatus.Pending))" />
                }
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>@(_selectedAccountHolderId == 0 ? "Select an account holder to view invoices." : "No invoices found.")</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>

<!-- Mark Paid Dialog -->
<MudDialog @ref="_markPaidDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Mark Invoice as Paid</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-3">
            Instalment #@_invoiceToPay?.InstallmentNumber — R@_invoiceToPay?.Amount.ToString("N2")
        </MudText>
        <MudDatePicker @bind-Date="_paidDate" Label="Payment Date" Required="true" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _markPaidDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Success" OnClick="ConfirmMarkPaid">Mark Paid</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private List<Invoice> _allInvoices = [];
    private List<Invoice> _filteredInvoices = [];
    private int _selectedTeacherId, _selectedAccountHolderId;
    private string? _statusFilter;

    private MudDialog? _markPaidDialog;
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };
    private Invoice? _invoiceToPay;
    private DateTime? _paidDate;

    private TableGroupDefinition<Invoice> _groupByBundle = new()
    {
        GroupName = "Bundle",
        Indentation = false,
        Expandable = true,
        IsInitiallyExpanded = true,
        Selector = i => i.BundleID
    };

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
    }

    private async Task OnTeacherChanged()
    {
        _accountHolders = _selectedTeacherId > 0
            ? await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId) : [];
        _allInvoices = [];
        _filteredInvoices = [];
        _selectedAccountHolderId = 0;
    }

    private async Task LoadInvoices()
    {
        if (_selectedAccountHolderId > 0)
        {
            _loading = true;
            _allInvoices = await InvoiceSvc.GetByAccountHolderAsync(_selectedAccountHolderId);
            ApplyFilter();
            _loading = false;
        }
        else
        {
            _allInvoices = [];
            _filteredInvoices = [];
        }
    }

    private void ApplyFilter()
    {
        _filteredInvoices = string.IsNullOrEmpty(_statusFilter)
            ? [.. _allInvoices]
            : [.. _allInvoices.Where(i => i.Status == _statusFilter)];
    }

    private async Task QuickUpdateStatus(Invoice invoice, string status)
    {
        var result = await InvoiceSvc.UpdateInvoiceStatusAsync(invoice.InvoiceID, status, null);
        if (result) { Snackbar.Add("Invoice updated.", Severity.Success); await LoadInvoices(); }
        else Snackbar.Add("Failed.", Severity.Error);
    }

    private async Task OpenMarkPaidDialog(Invoice invoice)
    {
        _invoiceToPay = invoice;
        _paidDate = DateTime.Today;
        await _markPaidDialog!.ShowAsync();
    }

    private async Task ConfirmMarkPaid()
    {
        if (_invoiceToPay is null || _paidDate is null) return;
        var paidDate = DateOnly.FromDateTime(_paidDate.Value);
        var result = await InvoiceSvc.UpdateInvoiceStatusAsync(_invoiceToPay.InvoiceID, InvoiceStatus.Paid, paidDate);
        if (result)
        {
            Snackbar.Add("Marked as paid.", Severity.Success);
            await _markPaidDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadInvoices();
        }
        else Snackbar.Add("Failed.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\LessonBundleDetails.razor

```razor
@page "/lesson-bundles/{BundleID:int}"

@using MusicSchool.Data.Models

@inject LessonBundleService BundleSvc
@inject LessonService LessonSvc
@inject InvoiceService InvoiceSvc
@inject ISnackbar Snackbar
@inject NavigationManager Nav

<PageTitle>Bundle Detail — Music School</PageTitle>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}
else if (_bundleDetails.Count == 0)
{
    <MudText>Bundle not found.</MudText>
}
else
{
    var first = _bundleDetails.First();

    <div class="page-header d-flex justify-space-between align-center">
        <div>
            <MudBreadcrumbs Items="_breadcrumbs" />
            <MudText Typo="Typo.h5">
                @first.StudentFullName
            </MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">
                @first.DurationMinutes min · @first.TotalLessons lessons ·
                R@(first.PricePerLesson.ToString("N2")) lesson ·
                R@((first.TotalLessons * first.PricePerLesson).ToString("N2")) total
            </MudText>
        </div>
    </div>

    <!-- Quarter summary cards -->
    <MudGrid Class="mb-4">
        @foreach (var q in _bundleDetails.OrderBy(b => b.QuarterNumber))
        {
            var pct = q.LessonsAllocated > 0 ? (double)q.LessonsUsed / q.LessonsAllocated * 100 : 0;
            <MudItem xs="12" sm="6" md="3">
                <MudPaper Class="pa-3 quarter-card" Elevation="1">
                    <MudText Typo="Typo.subtitle1" Style="font-weight:600;">Quarter @q.QuarterNumber</MudText>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">
                        @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("dd MMM yyyy")
                    </MudText>
                    <MudProgressLinear Color="Color.Primary" Value="pct" Class="my-2" Rounded="true" />
                    <MudStack Row="true" Justify="Justify.SpaceBetween">
                        <MudText Typo="Typo.body2">@q.LessonsUsed used</MudText>
                        <MudText Typo="Typo.body2">@q.LessonsRemaining left</MudText>
                    </MudStack>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">of @q.LessonsAllocated allocated</MudText>
                </MudPaper>
            </MudItem>
        }
    </MudGrid>

    <MudTabs Elevation="1" Rounded="false" ApplyEffectsToContainer="true" PanelClass="pa-4">

        <!-- LESSONS TAB -->
        <MudTabPanel Text="Lessons" Icon="@Icons.Material.Filled.MusicNote">
            @if (_lessons.Count == 0)
            {
                <MudText Color="Color.Secondary">No lessons recorded for this bundle.</MudText>
            }
            else
            {
                <MudTable Items="_lessons" Hover="true" Dense="false" Elevation="0"
                          GroupBy="_groupByQuarter" GroupHeaderStyle="background:#F0F2F5;">
                    <HeaderContent>
                        <MudTh>Date</MudTh>
                        <MudTh>Time</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Credit Forfeited</MudTh>
                        <MudTh>Cancelled By</MudTh>
                        <MudTh>Notes</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <GroupHeaderTemplate>
                        <MudTh colspan="7" Style="padding:8px 16px;">
                            <MudText Typo="Typo.subtitle2">Quarter @context.Key</MudText>
                        </MudTh>
                    </GroupHeaderTemplate>
                    <RowTemplate>
                        <MudTd>@context.ScheduledDate.ToString("dd MMM yyyy")</MudTd>
                        <MudTd>@context.ScheduledTime.ToString("HH:mm")</MudTd>
                        <MudTd><StatusChip Status="@context.Status" /></MudTd>
                        <MudTd>
                            @if (context.CreditForfeited)
                            {
                                <MudIcon Icon="@Icons.Material.Filled.Warning" Color="Color.Warning" Size="Size.Small" />
                            }
                        </MudTd>
                        <MudTd>@(context.CancelledBy ?? "—")</MudTd>
                        <MudTd>@(context.CancellationReason ?? "—")</MudTd>
                        <MudTd>
                            @if (context.Status == LessonStatus.Scheduled)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Mark Completed"
                                               OnClick="@(() => UpdateLessonStatus(context, LessonStatus.Completed))" />
                                <MudIconButton Icon="@Icons.Material.Filled.PersonOff" Size="Size.Small"
                                               Color="Color.Warning" Title="Student Cancelled"
                                               OnClick="@(() => OpenCancelDialog(context))" />
                                <MudIconButton Icon="@Icons.Material.Filled.EventBusy" Size="Size.Small"
                                               Color="Color.Error" Title="Teacher Cancelled"
                                               OnClick="@(() => UpdateLessonStatus(context, LessonStatus.CancelledTeacher))" />
                            }
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            }
        </MudTabPanel>

        <!-- INVOICES TAB -->
        <MudTabPanel Text="Invoices" Icon="@Icons.Material.Filled.Receipt">
            @if (_invoices.Count == 0)
            {
                <MudText Color="Color.Secondary">No invoices found for this bundle.</MudText>
            }
            else
            {
                <MudTable Items="_invoices" Hover="true" Dense="false" Elevation="0">
                    <HeaderContent>
                        <MudTh>Instalment #</MudTh>
                        <MudTh>Amount</MudTh>
                        <MudTh>Due Date</MudTh>
                        <MudTh>Paid Date</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.InstallmentNumber</MudTd>
                        <MudTd>R @context.Amount.ToString("N2")</MudTd>
                        <MudTd>@context.DueDate.ToString("dd MMM yyyy")</MudTd>
                        <MudTd>@(context.PaidDate?.ToString("dd MMM yyyy") ?? "—")</MudTd>
                        <MudTd><StatusChip Status="@context.Status" /></MudTd>
                        <MudTd>
                            @if (context.Status != InvoiceStatus.Paid && context.Status != InvoiceStatus.Void)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Mark Paid"
                                               OnClick="@(() => OpenMarkPaidDialog(context))" />
                                <MudIconButton Icon="@Icons.Material.Filled.Block" Size="Size.Small"
                                               Color="Color.Error" Title="Void"
                                               OnClick="@(() => VoidInvoice(context))" />
                            }
                        </MudTd>
                    </RowTemplate>
                    <FooterContent>
                        <MudTd colspan="2">
                            <MudText Style="font-weight:600;">
                                Paid: R@_invoices.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Amount).ToString("N2")
                            </MudText>
                        </MudTd>
                        <MudTd colspan="4">
                            <MudText Color="Color.Warning" Style="font-weight:600;">
                                Outstanding: R@_invoices.Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Void).Sum(i => i.Amount).ToString("N2")
                            </MudText>
                        </MudTd>
                    </FooterContent>
                </MudTable>
            }
        </MudTabPanel>

    </MudTabs>
}

<!-- Cancel Lesson Dialog -->
<MudDialog @ref="_cancelDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Student Cancellation</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-3">
            Lesson on <strong>@_lessonToCancel?.ScheduledDate.ToString("dd MMM yyyy")</strong> cancelled by student.
        </MudText>
        <MudText Typo="Typo.body2" Class="mb-2">Does the teacher forfeit this lesson credit?</MudText>
        <MudTextField @bind-Value="_cancelReason" Label="Reason (optional)" Lines="2" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _cancelDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Color="Color.Warning" OnClick="@(() => ConfirmCancel(false))">Keep Credit (Reschedule)</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Error" OnClick="@(() => ConfirmCancel(true))">Forfeit Credit</MudButton>
    </DialogActions>
</MudDialog>

<!-- Mark Paid Dialog -->
<MudDialog @ref="_markPaidDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Mark Invoice as Paid</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-3">
            Mark instalment #@_invoiceToPay?.InstallmentNumber (R@_invoiceToPay?.Amount.ToString("N2")) as paid?
        </MudText>
        <MudDatePicker @bind-Date="_paidDate" Label="Payment Date" Required="true" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _markPaidDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Success" OnClick="ConfirmMarkPaid">Mark Paid</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [Parameter] public int BundleID { get; set; }

    private bool _loading = true;
    private List<LessonBundleWithQuarterDetail> _bundleDetails = [];

    // FIX: Changed from List<LessonDetail> to List<Lesson>.
    // LessonService.GetByBundleAsync now calls Lesson/GetByBundle which returns Lesson objects,
    // not LessonDetail. The table renders only the fields available on Lesson.
    private List<Lesson> _lessons = [];
    private List<Invoice> _invoices = [];

    private MudDialog? _cancelDialog, _markPaidDialog;
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    private Lesson? _lessonToCancel;
    private string _cancelReason = string.Empty;

    private Invoice? _invoiceToPay;
    private DateTime? _paidDate;

    private List<BreadcrumbItem> _breadcrumbs = [];

    // FIX: Group by QuarterID, which is available on the Lesson model.
    private TableGroupDefinition<Lesson> _groupByQuarter = new()
    {
        GroupName = "Quarter",
        Indentation = false,
        Expandable = true,
        IsInitiallyExpanded = true,
        Selector = l => l.QuarterID
    };

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _bundleDetails = await BundleSvc.GetBundleAsync(BundleID);

        if (_bundleDetails.Count > 0)
        {
            var first = _bundleDetails.First();
            _breadcrumbs =
            [
                new BreadcrumbItem("Bundles", href: "/lesson-bundles"),
                new BreadcrumbItem($"{first.StudentFullName}", href: null, disabled: true)
            ];

            // FIX: Call GetByBundleAsync(BundleID) directly — no student lookup needed,
            // no client-side filtering needed, and the endpoint actually exists on the API.
            _lessons = await LessonSvc.GetByBundleAsync(BundleID);
            _invoices = await InvoiceSvc.GetByBundleAsync(BundleID);
        }

        _loading = false;
    }

    private async Task UpdateLessonStatus(Lesson lesson, string status)
    {
        var result = await LessonSvc.UpdateLessonStatusAsync(lesson.LessonID, status);
        if (result)
        {
            Snackbar.Add("Lesson updated.", Severity.Success);
            _lessons = await LessonSvc.GetByBundleAsync(BundleID);
            _bundleDetails = await BundleSvc.GetBundleAsync(BundleID);
        }
        else Snackbar.Add("Failed to update lesson.", Severity.Error);
    }

    private async Task OpenCancelDialog(Lesson lesson)
    {
        _lessonToCancel = lesson;
        _cancelReason = string.Empty;
        await _cancelDialog!.ShowAsync();
    }

    private async Task ConfirmCancel(bool forfeit)
    {
        if (_lessonToCancel is null) return;
        var status = forfeit ? LessonStatus.Forfeited : LessonStatus.CancelledStudent;
        var result = await LessonSvc.UpdateLessonStatusAsync(_lessonToCancel.LessonID, status);
        if (result)
        {
            Snackbar.Add("Lesson cancelled.", Severity.Success);
            await _cancelDialog!.CloseAsync(DialogResult.Ok(true));
            _lessons = await LessonSvc.GetByBundleAsync(BundleID);
            _bundleDetails = await BundleSvc.GetBundleAsync(BundleID);
        }
        else Snackbar.Add("Failed.", Severity.Error);
    }

    private async Task OpenMarkPaidDialog(Invoice invoice)
    {
        _invoiceToPay = invoice;
        _paidDate = DateTime.Today;
        await _markPaidDialog!.ShowAsync();
    }

    private async Task ConfirmMarkPaid()
    {
        if (_invoiceToPay is null || _paidDate is null) return;
        var paidDate = DateOnly.FromDateTime(_paidDate.Value);
        var result = await InvoiceSvc.UpdateInvoiceStatusAsync(_invoiceToPay.InvoiceID, InvoiceStatus.Paid, paidDate);
        if (result)
        {
            Snackbar.Add("Invoice marked as paid.", Severity.Success);
            await _markPaidDialog!.CloseAsync(DialogResult.Ok(true));
            _invoices = await InvoiceSvc.GetByBundleAsync(BundleID);
        }
        else Snackbar.Add("Failed to update invoice.", Severity.Error);
    }

    private async Task VoidInvoice(Invoice invoice)
    {
        var result = await InvoiceSvc.UpdateInvoiceStatusAsync(invoice.InvoiceID, InvoiceStatus.Void, null);
        if (result)
        {
            Snackbar.Add("Invoice voided.", Severity.Success);
            _invoices = await InvoiceSvc.GetByBundleAsync(BundleID);
        }
        else Snackbar.Add("Failed to void invoice.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\LessonBundles.razor

```razor
@page "/lesson-bundles"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
@inject LessonBundleService BundleSvc
@inject NavigationManager Nav

<PageTitle>Lesson Bundles — Music School</PageTitle>

<div class="page-header">
    <MudText Typo="Typo.h5">Lesson Bundles</MudText>
    <MudText Typo="Typo.body2" Color="Color.Secondary">View and manage annual lesson bundles</MudText>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Teacher"
                       @bind-Value:after="OnTeacherChanged" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedAccountHolderId" Label="Account Holder"
                       @bind-Value:after="OnAccountHolderChanged"
                       Disabled="_accountHolders.Count == 0" Clearable="true">
                @foreach (var ah in _accountHolders)
                {
                    <MudSelectItem Value="ah.AccountHolderID">@ah.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="4">
            <MudSelect @bind-Value="_selectedStudentId" Label="Student"
                       @bind-Value:after="OnStudentChanged"
                       Disabled="_students.Count == 0" Clearable="true">
                @foreach (var s in _students)
                {
                    <MudSelectItem Value="s.StudentID">@s.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

@if (_bundles.Any())
{
    @foreach (var b in _bundles)
    {
        <MudPaper Class="pa-4 mb-3" Elevation="1">
            <MudStack Row="true" Justify="Justify.SpaceBetween" AlignItems="AlignItems.Start">
                <div>
                    <MudText Style="font-weight:600;">
                        @b.DurationMinutes min lessons
                    </MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">
                        @b.StudentFirstName @b.StudentLastName ·
                        @b.TotalLessons lessons · R@(b.PricePerLesson.ToString("N2")) lesson ·
                        Total: R@((b.TotalLessons * b.PricePerLesson).ToString("N2"))
                    </MudText>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">
                        @b.StartDate.ToString("dd MMM yyyy") – @b.EndDate.ToString("dd MMM yyyy")
                    </MudText>
                </div>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" Size="Size.Small"
                           OnClick="@(() => Nav.NavigateTo($"/lesson-bundles/{b.BundleID}"))">
                    View Detail
                </MudButton>
            </MudStack>
        </MudPaper>
    }
}
else if (!_loading)
{
    <MudText Color="Color.Secondary">Select a student to view their bundles.</MudText>
}

@code {
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private List<Student> _students = [];
    private List<LessonBundleDetail> _bundles = [];
    private int _selectedTeacherId, _selectedAccountHolderId, _selectedStudentId;

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
    }

    private async Task OnTeacherChanged()
    {
        _accountHolders = _selectedTeacherId > 0
            ? await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId) : [];
        _students = [];
        _bundles = [];
        _selectedAccountHolderId = 0;
        _selectedStudentId = 0;
    }

    private async Task OnAccountHolderChanged()
    {
        _students = _selectedAccountHolderId > 0
            ? await StudentSvc.GetByAccountHolderAsync(_selectedAccountHolderId) : [];
        _bundles = [];
        _selectedStudentId = 0;
    }

    private async Task OnStudentChanged()
    {
        if (_selectedStudentId > 0)
        {
            _loading = true;
            _bundles = await BundleSvc.GetByStudentAsync(_selectedStudentId);
            _loading = false;
        }
        else _bundles = [];
    }
}

```

## File: MusicSchool.Web\Pages\LessonTypes.razor

```razor
@page "/lesson-types"
@using MusicSchool.Data.Models
@inject LessonTypeService LessonTypeSvc
@inject ISnackbar Snackbar

<PageTitle>Lesson Types — Music School</PageTitle>

<div class="page-header d-flex justify-space-between align-center">
    <div>
        <MudText Typo="Typo.h5">Lesson Types</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">Configure lesson durations and base prices</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add"
               OnClick="OpenAddDialog">Add Lesson Type</MudButton>
</div>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

<MudPaper Elevation="1">
    <MudTable Items="_lessonTypes" Hover="true" Dense="false" Loading="_loading">
        <HeaderContent>
            <MudTh>Duration (minutes)</MudTh>
            <MudTh>Base Price / Lesson</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd>
                <MudText Style="font-weight:600;">@context.DurationMinutes min</MudText>
            </MudTd>
            <MudTd>R @context.BasePricePerLesson.ToString("N2")</MudTd>
            <MudTd>
                <MudChip T="string" Size="Size.Small" Color="@(context.IsActive ? Color.Success : Color.Default)">
                    @(context.IsActive ? "Active" : "Inactive")
                </MudChip>
            </MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" Color="Color.Primary"
                               OnClick="@(() => OpenEditDialog(context))" />
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>No lesson types found.</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>

<!-- Add Dialog -->
<MudDialog @ref="_addDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Lesson Type</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_addForm">
            <MudSelect @bind-Value="_newType.DurationMinutes" Label="Duration (minutes)" Required="true"
                       RequiredError="Duration is required" Class="mb-3">
                <MudSelectItem Value="30">30 minutes</MudSelectItem>
                <MudSelectItem Value="45">45 minutes</MudSelectItem>
                <MudSelectItem Value="60">60 minutes</MudSelectItem>
            </MudSelect>
            <MudNumericField @bind-Value="_newType.BasePricePerLesson" Label="Base Price per Lesson"
                             Required="true" Min="0m" Format="N2" Adornment="Adornment.Start"
                             AdornmentText="R" Class="mb-3" />
            <MudSwitch @bind-Value="_newType.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveNew">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Edit Dialog -->
<MudDialog @ref="_editDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Edit Lesson Type</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_editForm">
            <MudSelect @bind-Value="_editType.DurationMinutes" Label="Duration (minutes)" Required="true"
                       RequiredError="Duration is required" Class="mb-3">
                <MudSelectItem Value="30">30 minutes</MudSelectItem>
                <MudSelectItem Value="45">45 minutes</MudSelectItem>
                <MudSelectItem Value="60">60 minutes</MudSelectItem>
            </MudSelect>
            <MudNumericField @bind-Value="_editType.BasePricePerLesson" Label="Base Price per Lesson"
                             Required="true" Min="0m" Format="N2" Adornment="Adornment.Start"
                             AdornmentText="R" Class="mb-3" />
            <MudSwitch @bind-Value="_editType.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _editDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveEdit">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading = true;
    private List<LessonType> _lessonTypes = [];
    private MudDialog? _addDialog, _editDialog;
    private MudForm? _addForm, _editForm;
    private LessonType _newType = new();
    private LessonType _editType = new();
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    protected override async Task OnInitializedAsync() => await Load();

    private async Task Load()
    {
        _loading = true;
        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();
        _loading = false;
    }

    private async Task OpenAddDialog()
    {
        _newType = new LessonType { IsActive = true, DurationMinutes = 30 };
        await _addDialog!.ShowAsync();
    }

    private async Task OpenEditDialog(LessonType lt)
    {
        _editType = new LessonType
        {
            LessonTypeID = lt.LessonTypeID,
            DurationMinutes = lt.DurationMinutes,
            BasePricePerLesson = lt.BasePricePerLesson,
            IsActive = lt.IsActive
        };
        await _editDialog!.ShowAsync();
    }

    private async Task SaveNew()
    {
        await _addForm!.Validate();
        if (!_addForm.IsValid) return;
        var result = await LessonTypeSvc.AddLessonTypeAsync(_newType);
        if (result.HasValue)
        {
            Snackbar.Add("Lesson type added.", Severity.Success);
            await _addDialog!.CloseAsync(DialogResult.Ok(true));
            await Load();
        }
        else Snackbar.Add("Failed to add lesson type.", Severity.Error);
    }

    private async Task SaveEdit()
    {
        await _editForm!.Validate();
        if (!_editForm.IsValid) return;
        var result = await LessonTypeSvc.UpdateLessonTypeAsync(_editType);
        if (result)
        {
            Snackbar.Add("Lesson type updated.", Severity.Success);
            await _editDialog!.CloseAsync(DialogResult.Ok(true));
            await Load();
        }
        else Snackbar.Add("Failed to update lesson type.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\Schedule.razor

```razor
@page "/schedule"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject LessonService LessonSvc
@inject ExtraLessonService ExtraLessonSvc
@inject LessonTypeService LessonTypeSvc
@inject ISnackbar Snackbar

<PageTitle>Schedule — Music School</PageTitle>

<div class="page-header d-flex justify-space-between align-center">
    <div>
        <MudText Typo="Typo.h5">Lesson Schedule</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">View and manage daily lessons</MudText>
    </div>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid AlignItems="AlignItems.Center">
        <MudItem xs="12" sm="5" md="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Teacher"
                       @bind-Value:after="LoadSchedule" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="5" md="4">
            <MudDatePicker @bind-Date="_selectedDate" Label="Date"
                           @bind-Date:after="LoadSchedule" />
        </MudItem>
        <MudItem xs="12" sm="2" md="4">
            <MudStack Row="true" Spacing="1" AlignItems="AlignItems.Center">
                <MudIconButton Icon="@Icons.Material.Filled.ChevronLeft" OnClick="PreviousDay" />
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" Size="Size.Small"
                           OnClick="GoToToday">Today</MudButton>
                <MudIconButton Icon="@Icons.Material.Filled.ChevronRight" OnClick="NextDay" />
            </MudStack>
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

@if (_selectedTeacherId > 0 && _selectedDate.HasValue)
{
    <MudGrid>
        <MudItem xs="12" md="6">
            <MudPaper Elevation="1">
                <MudTable Items="_lessons" Hover="true" Dense="false" Loading="_loading" Elevation="0">
                    <ToolBarContent>
                        <MudText Typo="Typo.h6">Bundle Lessons (@_lessons.Count)</MudText>
                    </ToolBarContent>
                    <HeaderContent>
                        <MudTh>Time</MudTh>
                        <MudTh>Student</MudTh>
                        <MudTh>Duration</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.ScheduledTime.ToString("HH:mm")</MudTd>
                        <MudTd>@context.StudentFullName</MudTd>
                        <MudTd>@context.DurationMinutes min</MudTd>
                        <MudTd><StatusChip Status="@context.Status" /></MudTd>
                        <MudTd>
                            @if (context.Status == LessonStatus.Scheduled)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Mark Completed"
                                               OnClick="@(() => QuickUpdateLesson(context, LessonStatus.Completed))" />
                                <MudIconButton Icon="@Icons.Material.Filled.Cancel" Size="Size.Small"
                                               Color="Color.Warning" Title="Teacher Cancelled"
                                               OnClick="@(() => QuickUpdateLesson(context, LessonStatus.CancelledTeacher))" />
                                <MudIconButton Icon="@Icons.Material.Filled.PersonOff" Size="Size.Small"
                                               Color="Color.Error" Title="Student Cancelled / Forfeit"
                                               OnClick="@(() => OpenForfeitDialog(context))" />
                            }
                        </MudTd>
                    </RowTemplate>
                    <NoRecordsContent>
                        <MudText Class="pa-3">No bundle lessons for this day.</MudText>
                    </NoRecordsContent>
                </MudTable>
            </MudPaper>
        </MudItem>

        <MudItem xs="12" md="6">
            <MudPaper Elevation="1">
                <MudTable Items="_extraLessons" Hover="true" Dense="false" Loading="_loading" Elevation="0">
                    <ToolBarContent>
                        <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween" Style="width:100%">
                            <MudText Typo="Typo.h6">Extra Lessons (@_extraLessons.Count)</MudText>
                            <MudButton Variant="Variant.Outlined" Color="Color.Primary" Size="Size.Small"
                                       StartIcon="@Icons.Material.Filled.Add"
                                       OnClick="OpenAddExtraLessonDialog">Add</MudButton>
                        </MudStack>
                    </ToolBarContent>
                    <HeaderContent>
                        <MudTh>Time</MudTh>
                        <MudTh>Student</MudTh>
                        <MudTh>Price</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.ScheduledTime.ToString("HH:mm")</MudTd>
                        <MudTd>@context.StudentFullName</MudTd>
                        <MudTd>R @context.PriceCharged.ToString("N2")</MudTd>
                        <MudTd><StatusChip Status="@context.Status" /></MudTd>
                        <MudTd>
                            @if (context.Status == ExtraLessonStatus.Scheduled)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Complete"
                                               OnClick="@(() => QuickUpdateExtra(context, ExtraLessonStatus.Completed))" />
                                <MudIconButton Icon="@Icons.Material.Filled.Cancel" Size="Size.Small"
                                               Color="Color.Error" Title="Cancel"
                                               OnClick="@(() => QuickUpdateExtra(context, ExtraLessonStatus.Cancelled))" />
                            }
                        </MudTd>
                    </RowTemplate>
                    <NoRecordsContent>
                        <MudText Class="pa-3">No extra lessons for this day.</MudText>
                    </NoRecordsContent>
                </MudTable>
            </MudPaper>
        </MudItem>
    </MudGrid>
}
else
{
    <MudAlert Severity="Severity.Info" Variant="Variant.Outlined">
        Select a teacher and date to view the schedule.
    </MudAlert>
}

<!-- Forfeit Dialog -->
<MudDialog @ref="_forfeitDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Student Cancellation</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-2">
            <strong>@_lessonToAction?.StudentFullName</strong>
            — @_lessonToAction?.ScheduledDate.ToString("dd MMM yyyy")
        </MudText>
        <MudDivider Class="mb-3" />
        <MudText Typo="Typo.body2" Class="mb-3">
            Should the teacher forfeit this lesson credit, or keep it for rescheduling?
        </MudText>
        <MudTextField @bind-Value="_actionReason" Label="Reason (optional)" Lines="2" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _forfeitDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Color="Color.Warning" Variant="Variant.Outlined"
                   OnClick="@(() => ConfirmForfeit(false))">Keep Credit</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Error"
                   OnClick="@(() => ConfirmForfeit(true))">Forfeit Credit</MudButton>
    </DialogActions>
</MudDialog>

<!-- Add Extra Lesson Dialog -->
<MudDialog @ref="_addExtraDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Extra Lesson</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_extraForm">
            <MudNumericField @bind-Value="_newExtra.StudentID" Label="Student ID" Required="true"
                             Min="1" Class="mb-3" HelperText="Enter the student's ID number" />
            <MudSelect @bind-Value="_newExtra.LessonTypeID" Label="Lesson Type" Required="true"
                       @bind-Value:after="SetExtraBasePrice" Class="mb-3">
                @foreach (var lt in _lessonTypes)
                {
                    <MudSelectItem Value="lt.LessonTypeID">@lt.DisplayName</MudSelectItem>
                }
            </MudSelect>
            <MudTimePicker @bind-Time="_extraTime" Label="Time" Required="true"
                           AmPm="false" Class="mb-3" />
            <MudNumericField @bind-Value="_newExtra.PriceCharged" Label="Price Charged"
                             Required="true" Min="0m" Format="N2"
                             Adornment="Adornment.Start" AdornmentText="R" Class="mb-3"
                             HelperText="Auto-filled from base price; teacher may override" />
            <MudTextField @bind-Value="_newExtra.Notes" Label="Notes" Lines="2" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addExtraDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveExtraLesson">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<LessonDetail> _lessons = [];
    private List<ExtraLessonDetail> _extraLessons = [];
    private List<LessonType> _lessonTypes = [];
    private int _selectedTeacherId;
    private DateTime? _selectedDate = DateTime.Today;

    private MudDialog? _forfeitDialog, _addExtraDialog;
    private MudForm? _extraForm;
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    private LessonDetail? _lessonToAction;
    private string _actionReason = string.Empty;

    private ExtraLesson _newExtra = new();
    private TimeSpan? _extraTime;

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();
    }

    private async Task LoadSchedule()
    {
        if (_selectedTeacherId <= 0 || _selectedDate is null) return;
        _loading = true;
        var date = _selectedDate.Value.Date;
        _lessons = await LessonSvc.GetByTeacherAndDateAsync(_selectedTeacherId, date);
        _extraLessons = await ExtraLessonSvc.GetByTeacherAndDateAsync(_selectedTeacherId, date);
        _loading = false;
    }

    private void GoToToday() { _selectedDate = DateTime.Today; _ = LoadSchedule(); }
    private void PreviousDay() { _selectedDate = (_selectedDate ?? DateTime.Today).AddDays(-1); _ = LoadSchedule(); }
    private void NextDay() { _selectedDate = (_selectedDate ?? DateTime.Today).AddDays(1); _ = LoadSchedule(); }

    private async Task QuickUpdateLesson(LessonDetail lesson, string status)
    {
        var result = await LessonSvc.UpdateLessonStatusAsync(lesson.LessonID, status);
        if (result) { Snackbar.Add("Lesson updated.", Severity.Success); await LoadSchedule(); }
        else Snackbar.Add("Failed to update lesson.", Severity.Error);
    }

    private async Task QuickUpdateExtra(ExtraLessonDetail extra, string status)
    {
        var result = await ExtraLessonSvc.UpdateExtraLessonStatusAsync(extra.ExtraLessonID, status);
        if (result) { Snackbar.Add("Extra lesson updated.", Severity.Success); await LoadSchedule(); }
        else Snackbar.Add("Failed to update extra lesson.", Severity.Error);
    }

    private async Task OpenForfeitDialog(LessonDetail lesson)
    {
        _lessonToAction = lesson;
        _actionReason = string.Empty;
        await _forfeitDialog!.ShowAsync();
    }

    private async Task ConfirmForfeit(bool forfeit)
    {
        if (_lessonToAction is null) return;
        var status = forfeit ? LessonStatus.Forfeited : LessonStatus.CancelledStudent;
        var result = await LessonSvc.UpdateLessonStatusAsync(_lessonToAction.LessonID, status);
        if (result)
        {
            Snackbar.Add(forfeit ? "Credit forfeited." : "Credit kept — please reschedule.", Severity.Success);
            await _forfeitDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadSchedule();
        }
        else Snackbar.Add("Failed.", Severity.Error);
    }

    private async Task OpenAddExtraLessonDialog()
    {
        _newExtra = new ExtraLesson
        {
            TeacherID = _selectedTeacherId,
            ScheduledDate = _selectedDate ?? DateTime.Today,
            Status = ExtraLessonStatus.Scheduled
        };
        _extraTime = new TimeSpan(9, 0, 0);
        await _addExtraDialog!.ShowAsync();
    }

    private void SetExtraBasePrice()
    {
        var lt = _lessonTypes.FirstOrDefault(l => l.LessonTypeID == _newExtra.LessonTypeID);
        if (lt is not null) _newExtra.PriceCharged = lt.BasePricePerLesson;
    }

    private async Task SaveExtraLesson()
    {
        await _extraForm!.Validate();
        if (!_extraForm.IsValid) return;
        if (_extraTime is null) { Snackbar.Add("Time is required.", Severity.Warning); return; }
        _newExtra.ScheduledTime = _newExtra.ScheduledDate.AddHours(_extraTime.Value.Hours);

        var result = await ExtraLessonSvc.AddExtraLessonAsync(_newExtra);
        if (result.HasValue)
        {
            Snackbar.Add("Extra lesson added.", Severity.Success);
            await _addExtraDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadSchedule();
        }
        else Snackbar.Add("Failed to add extra lesson.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\StudentDetail.razor

```razor
@page "/students/{StudentID:int}"
@using MusicSchool.Data.Models
@using MusicSchool.Models.TransferModels
@inject StudentService StudentSvc
@inject AccountHolderService AccountHolderSvc
@inject LessonBundleService BundleSvc
@inject ScheduledSlotService SlotSvc
@inject LessonTypeService LessonTypeSvc
@inject TeacherService TeacherSvc
@inject ISnackbar Snackbar
@inject NavigationManager Nav

<PageTitle>Student — Music School</PageTitle>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}
else if (_student is null)
{
    <MudText>Student not found.</MudText>
}
else
{
    <div class="page-header d-flex justify-space-between align-center">
        <div>
            <MudBreadcrumbs Items="_breadcrumbs" />
            <MudText Typo="Typo.h5">@_student.FullName</MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">
                @(_student.IsAccountHolder ? "Account Holder & Student" : "Student")
            </MudText>
        </div>
        <MudStack Row="true" Spacing="2">
            <MudButton Variant="Variant.Outlined" Color="Color.Primary"
                       StartIcon="@Icons.Material.Filled.Inventory"
                       OnClick="OpenAddBundleDialog">New Bundle</MudButton>
            <MudTooltip Text="@(_bundles.Count == 0 ? "Add a lesson bundle before assigning a slot." : "")"
                        Disabled="_bundles.Count > 0">
                <MudButton Variant="Variant.Outlined" Color="Color.Secondary"
                           StartIcon="@Icons.Material.Filled.Schedule"
                           OnClick="OpenAddSlotDialog"
                           Disabled="_bundles.Count == 0">Add Slot</MudButton>
            </MudTooltip>
        </MudStack>
    </div>

    <MudTabs Elevation="1" Rounded="false" ApplyEffectsToContainer="true" PanelClass="pa-4">

        <!-- BUNDLES TAB -->
        <MudTabPanel Text="Lesson Bundles" Icon="@Icons.Material.Filled.Inventory">
            @if (_bundles.Count == 0)
            {
                <MudText Color="Color.Secondary">No lesson bundles purchased yet.</MudText>
            }
            else
            {
                @* FIX: Uncommented this block. _bundles is now List<LessonBundleWithQuarterDetail>,
                   so GroupBy(BundleID) correctly yields one group per bundle with 4 quarter rows each. *@
                @foreach (var bundleGroup in _bundles.GroupBy(b => b.BundleID))
                {
                    var first = bundleGroup.First();
                    var quarters = bundleGroup.ToList();
                    <MudExpansionPanel Class="mb-3">
                        <TitleContent>
                            <MudStack Row="true" AlignItems="AlignItems.Center" Justify="Justify.SpaceBetween" Style="width:100%">
                                <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                                    <MudIcon Icon="@Icons.Material.Filled.Inventory" Color="Color.Primary" />
                                    <div>
                                        <MudText Typo="Typo.caption" Color="Color.Secondary">
                                            @first.TotalLessons lessons · R@(first.PricePerLesson.ToString("N2"))/lesson
                                            · @(first.StartDate.ToString("dd MMM yyyy")) – @(first.EndDate.ToString("dd MMM yyyy"))
                                        </MudText>
                                    </div>
                                </MudStack>
                                <MudButton Variant="Variant.Text" Color="Color.Primary" Size="Size.Small"
                                           OnClick="@(() => Nav.NavigateTo($"/lesson-bundles/{first.BundleID}"))">
                                    Full Detail
                                </MudButton>
                            </MudStack>
                        </TitleContent>
                        <ChildContent>
                            <MudGrid>
                                @foreach (var q in quarters)
                                {
                                    var pct = q.LessonsAllocated > 0 ? (double)q.LessonsUsed / q.LessonsAllocated * 100 : 0;
                                    <MudItem xs="12" sm="6" md="3">
                                        <MudPaper Class="pa-3 quarter-card" Elevation="0" Style="background:#F0F2F5;">
                                            <MudText Typo="Typo.subtitle2">Quarter @q.QuarterNumber</MudText>
                                            <MudText Typo="Typo.caption" Color="Color.Secondary">
                                                @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("dd MMM yyyy")
                                            </MudText>
                                            <MudProgressLinear Color="Color.Primary" Value="pct" Class="my-2" />
                                            <MudText Typo="Typo.body2">@q.LessonsUsed / @q.LessonsAllocated used</MudText>
                                            <MudText Typo="Typo.caption" Color="Color.Secondary">@q.LessonsRemaining remaining</MudText>
                                        </MudPaper>
                                    </MudItem>
                                }
                            </MudGrid>
                        </ChildContent>
                    </MudExpansionPanel>
                }
            }
        </MudTabPanel>

        <!-- SCHEDULED SLOTS TAB -->
        <MudTabPanel Text="Scheduled Slots" Icon="@Icons.Material.Filled.Schedule">
            @if (_slots.Count == 0)
            {
                <MudText Color="Color.Secondary">No scheduled slots.</MudText>
            }
            else
            {
                <MudTable Items="_slots" Hover="true" Dense="false" Elevation="0">
                    <HeaderContent>
                        <MudTh>Day</MudTh>
                        <MudTh>Time</MudTh>
                        <MudTh>Duration</MudTh>
                        <MudTh>Effective From</MudTh>
                        <MudTh>Effective To</MudTh>
                        <MudTh>Status</MudTh>
                        <MudTh>Actions</MudTh>
                    </HeaderContent>
                    <RowTemplate>
                        <MudTd>@context.DayName</MudTd>
                        <MudTd>@context.SlotTime.ToString("HH:mm")</MudTd>
                        <MudTd>@(_lessonTypes.FirstOrDefault(lt => lt.LessonTypeID == context.LessonTypeID)?.DurationMinutes ?? 0) min</MudTd>
                        <MudTd>@context.EffectiveFrom.ToString("dd MMM yyyy")</MudTd>
                        <MudTd>@(context.EffectiveTo?.ToString("dd MMM yyyy") ?? "Active")</MudTd>
                        <MudTd>
                            <MudChip T="string" Size="Size.Small" Color="@(context.IsActive? Color.Success: Color.Default)">
                                @(context.IsActive ? "Active" : "Closed")
                            </MudChip>
                        </MudTd>
                        <MudTd>
                            @if (context.IsActive)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.EventBusy" Size="Size.Small"
                                               Color="Color.Warning" Title="Close Slot"
                                               OnClick="@(() => OpenCloseSlotDialog(context))" />
                            }
                        </MudTd>
                    </RowTemplate>
                </MudTable>
            }
        </MudTabPanel>

    </MudTabs>
}

<!-- Add Bundle Dialog -->
<MudDialog @ref="_addBundleDialog" Options="_wideDialogOptions">
    <TitleContent><MudText Typo="Typo.h6">New Lesson Bundle</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_bundleForm">
            <MudGrid>
                <MudItem xs="12" sm="6">
                    <MudSelect @bind-Value="_newBundle.LessonTypeID" Label="Lesson Type" Required="true"
                               RequiredError="Required" Class="mb-3">
                        @foreach (var lt in _lessonTypes)
                        {
                            <MudSelectItem Value="lt.LessonTypeID">@lt.DisplayName</MudSelectItem>
                        }
                    </MudSelect>
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudNumericField @bind-Value="_newBundle.TotalLessons" Label="Total Lessons"
                                     Required="true" Min="4" Step="4" Class="mb-3"
                                     @bind-Value:after="RecalcBundle"
                                     HelperText="Must be divisible by 4" />
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudNumericField @bind-Value="_newBundle.PricePerLesson" Label="Price per Lesson"
                                     Required="true" Min="0m" Format="N2"
                                     Adornment="Adornment.Start" AdornmentText="R" Class="mb-3"
                                     @bind-Value:after="RecalcBundle" />
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudDatePicker @bind-Date="_bundleStartDate" Label="Start Date" Required="true"
                                   Class="mb-3" @bind-Date:after="RecalcBundle" />
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudDatePicker @bind-Date="_bundleEndDate" Label="End Date" Required="true" Class="mb-3" />
                </MudItem>
                <MudItem xs="12">
                    <MudTextField @bind-Value="_newBundle.Notes" Label="Notes" Lines="2" Class="mb-3" />
                </MudItem>
            </MudGrid>

            @if (_newBundle.TotalLessons > 0 && _newBundle.TotalLessons % 4 == 0)
            {
                <MudText Typo="Typo.subtitle2" Class="mb-2">Quarter Summary</MudText>
                <MudGrid>
                    @for (int i = 0; i < _quarters.Count; i++)
                    {
                        var q = _quarters[i];
                        <MudItem xs="12" sm="6" md="3">
                            <MudPaper Class="pa-2" Elevation="0" Style="background:#F0F2F5;border-top:3px solid #F3D395;">
                                <MudText Typo="Typo.subtitle2">Q@(q.QuarterNumber)</MudText>
                                <MudText Typo="Typo.caption">@q.LessonsAllocated lessons</MudText>
                                <MudText Typo="Typo.caption" Color="Color.Secondary">
                                    @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("dd MMM yyyy")
                                </MudText>
                            </MudPaper>
                        </MudItem>
                    }
                </MudGrid>
                <MudText Typo="Typo.body2" Class="mt-3">
                    Total: R@((_newBundle.TotalLessons * _newBundle.PricePerLesson).ToString("N2"))
                    · Monthly: R@((_newBundle.TotalLessons * _newBundle.PricePerLesson / 12).ToString("N2"))
                </MudText>
            }
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addBundleDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveBundle">Save Bundle</MudButton>
    </DialogActions>
</MudDialog>

<!-- Add Slot Dialog -->
<MudDialog @ref="_addSlotDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Add Scheduled Slot</MudText></TitleContent>
    <DialogContent>
        <MudForm @ref="_slotForm">
            <MudSelect @bind-Value="_newSlot.LessonTypeID" Label="Lesson Type" Required="true" Class="mb-3">
                @foreach (var lt in _lessonTypes)
                {
                    <MudSelectItem Value="lt.LessonTypeID">@lt.DisplayName</MudSelectItem>
                }
            </MudSelect>
            <MudSelect @bind-Value="_newSlot.DayOfWeek" Label="Day of Week" Required="true" Class="mb-3">
                <MudSelectItem Value="(byte)1">Monday</MudSelectItem>
                <MudSelectItem Value="(byte)2">Tuesday</MudSelectItem>
                <MudSelectItem Value="(byte)3">Wednesday</MudSelectItem>
                <MudSelectItem Value="(byte)4">Thursday</MudSelectItem>
                <MudSelectItem Value="(byte)5">Friday</MudSelectItem>
                <MudSelectItem Value="(byte)6">Saturday</MudSelectItem>
                <MudSelectItem Value="(byte)7">Sunday</MudSelectItem>
            </MudSelect>
            <MudTimePicker @bind-Time="_slotTime" Label="Slot Time" Required="true" Class="mb-3" AmPm="false" />
            <MudDatePicker @bind-Date="_slotEffectiveFrom" Label="Effective From" Required="true" Class="mb-3" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addSlotDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveSlot">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Close Slot Dialog -->
<MudDialog @ref="_closeSlotDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Close Scheduled Slot</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-3">
            Close the <strong>@_slotToClose?.DayName @_slotToClose?.SlotTime.ToString("HH:mm")</strong> slot?
        </MudText>
        <MudDatePicker @bind-Date="_closeSlotDate" Label="Effective To (last date)" Required="true" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _closeSlotDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Warning" OnClick="ConfirmCloseSlot">Close Slot</MudButton>
    </DialogActions>
</MudDialog>

@code {
    [Parameter] public int StudentID { get; set; }

    private bool _loading = true;
    private Student? _student;
    private AccountHolder? _accountHolder;

    // FIX: Changed from List<LessonBundleDetail> to List<LessonBundleWithQuarterDetail>
    // so that quarter fields (QuarterNumber, LessonsAllocated, etc.) are available for rendering.
    private List<LessonBundleWithQuarterDetail> _bundles = [];

    private List<ScheduledSlot> _slots = [];
    private List<LessonType> _lessonTypes = [];

    private MudDialog? _addBundleDialog, _addSlotDialog, _closeSlotDialog;
    private MudForm? _bundleForm, _slotForm;

    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };
    private DialogOptions _wideDialogOptions = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    private LessonBundle _newBundle = new();
    private List<BundleQuarter> _quarters = [];
    private DateTime? _bundleStartDate;
    private DateTime? _bundleEndDate;

    private ScheduledSlot _newSlot = new();
    private TimeSpan? _slotTime;
    private DateTime? _slotEffectiveFrom;

    private ScheduledSlot? _slotToClose;
    private DateTime? _closeSlotDate;

    private List<BreadcrumbItem> _breadcrumbs = [];

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        _student = await StudentSvc.GetStudentAsync(StudentID);
        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();
        _accountHolder = await AccountHolderSvc.GetAccountHolderAsync(_student.AccountHolderID);

        if (_student is not null)
        {
            _breadcrumbs =
            [
                new BreadcrumbItem("Account Holders", href: "/account-holders"),
                new BreadcrumbItem("Account Holder", href: $"/account-holders/{_student.AccountHolderID}"),
                new BreadcrumbItem(_student.FullName, href: null, disabled: true)
            ];

            // FIX: Correctly load bundles with quarter data.
            // GetByStudentAsync returns one LessonBundleDetail per bundle (no quarters).
            // GetBundleAsync returns 4 LessonBundleWithQuarterDetail rows per bundle.
            // We iterate the bundle list and AddRange the quarter rows into _bundles.
            var bundleList = await BundleSvc.GetByStudentAsync(StudentID);
            _bundles = [];
            foreach (var b in bundleList)
            {
                var details = await BundleSvc.GetBundleAsync(b.BundleID);
                _bundles.AddRange(details);
            }

            _slots = await SlotSvc.GetActiveByStudentAsync(StudentID);
        }

        _loading = false;
    }

    private void RecalcBundle()
    {
        _quarters = [];
        if (_newBundle.TotalLessons <= 0 || _newBundle.TotalLessons % 4 != 0 || _bundleStartDate is null)
            return;

        var startDate = _bundleStartDate.Value;
        var quarterSize = _newBundle.TotalLessons / 4;
        var totalDays = _bundleEndDate.HasValue
            ? _bundleEndDate.Value.DayOfYear - startDate.DayOfYear
            : 365;
        var daysPerQ = totalDays / 4;

        for (byte i = 1; i <= 4; i++)
        {
            var qStart = startDate.AddDays((i - 1) * daysPerQ);
            var qEnd = i < 4 ? qStart.AddDays(daysPerQ - 1) : (
                _bundleEndDate.HasValue ? _bundleEndDate.Value : qStart.AddDays(daysPerQ - 1));
            _quarters.Add(new BundleQuarter
            {
                QuarterNumber = i,
                LessonsAllocated = quarterSize,
                QuarterStartDate = qStart,
                QuarterEndDate = qEnd
            });
        }
    }

    private async Task OpenAddBundleDialog()
    {
        _newBundle = new LessonBundle
        {
            StudentID = StudentID,
            TotalLessons = 32,
            IsActive = true
        };
        _bundleStartDate = new DateTime(DateTime.Today.Year, 1, 1);
        _bundleEndDate = new DateTime(DateTime.Today.Year, 12, 31);
        _quarters = [];
        RecalcBundle();
        await _addBundleDialog!.ShowAsync();
    }

    private async Task SaveBundle()
    {
        await _bundleForm!.Validate();
        if (!_bundleForm.IsValid) return;
        if (_newBundle.TotalLessons % 4 != 0)
        {
            Snackbar.Add("Total lessons must be divisible by 4.", Severity.Warning);
            return;
        }
        if (_bundleStartDate is null || _bundleEndDate is null)
        {
            Snackbar.Add("Start and end dates are required.", Severity.Warning);
            return;
        }

        // Get teacherID from active slot or first bundle
        var existingSlots = await SlotSvc.GetActiveByStudentAsync(StudentID);

        _newBundle.StartDate = _bundleStartDate.Value.Date;
        _newBundle.EndDate = _bundleEndDate.Value.Date;
        _newBundle.TeacherID = _accountHolder.TeacherID;

        var req = new AddBundleRequest { Bundle = _newBundle, Quarters = _quarters };
        var result = await BundleSvc.AddBundleAsync(req);
        if (result.HasValue)
        {
            Snackbar.Add("Bundle created.", Severity.Success);
            await _addBundleDialog!.CloseAsync(DialogResult.Ok(true));

            // FIX: Same corrected loading pattern as OnInitializedAsync —
            // accumulate LessonBundleWithQuarterDetail rows across all bundles.
            var bundleList = await BundleSvc.GetByStudentAsync(StudentID);
            _bundles = [];
            foreach (var b in bundleList)
            {
                var details = await BundleSvc.GetBundleAsync(b.BundleID);
                _bundles.AddRange(details);
            }
        }
        else Snackbar.Add("Failed to create bundle.", Severity.Error);
    }

    private async Task OpenAddSlotDialog()
    {
        _newSlot = new ScheduledSlot { StudentID = StudentID, IsActive = true, DayOfWeek = 1 };
        _slotTime = new TimeSpan(9, 0, 0);
        _slotEffectiveFrom = DateTime.Today;

        var existingSlots = await SlotSvc.GetActiveByStudentAsync(StudentID);
        _newSlot.TeacherID = existingSlots.FirstOrDefault()?.TeacherID ?? _accountHolder!.TeacherID;
        await _addSlotDialog!.ShowAsync();
    }

    private async Task SaveSlot()
    {
        await _slotForm!.Validate();
        if (!_slotForm.IsValid) return;
        if (_slotTime is null || _slotEffectiveFrom is null)
        {
            Snackbar.Add("Time and effective date are required.", Severity.Warning);
            return;
        }
        _newSlot.SlotTime = TimeOnly.FromTimeSpan(_slotTime.Value);
        _newSlot.EffectiveFrom = _slotEffectiveFrom.Value.Date;

        var (slotId, error) = await SlotSvc.AddSlotAsync(_newSlot);
        if (slotId.HasValue)
        {
            Snackbar.Add("Slot added and lessons generated.", Severity.Success);
            await _addSlotDialog!.CloseAsync(DialogResult.Ok(true));
            _slots = await SlotSvc.GetActiveByStudentAsync(StudentID);
        }
        else Snackbar.Add(error ?? "Failed to add slot.", Severity.Error);
    }

    private async Task OpenCloseSlotDialog(ScheduledSlot slot)
    {
        _slotToClose = slot;
        _closeSlotDate = DateTime.Today;
        await _closeSlotDialog!.ShowAsync();
    }

    private async Task ConfirmCloseSlot()
    {
        if (_slotToClose is null || _closeSlotDate is null) return;
        var effectiveTo = DateOnly.FromDateTime(_closeSlotDate.Value);
        var result = await SlotSvc.CloseSlotAsync(_slotToClose.SlotID, effectiveTo);
        if (result)
        {
            Snackbar.Add("Slot closed.", Severity.Success);
            await _closeSlotDialog!.CloseAsync(DialogResult.Ok(true));
            _slots = await SlotSvc.GetActiveByStudentAsync(StudentID);
        }
        else Snackbar.Add("Failed to close slot.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\Students.razor

```razor
@page "/students"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
@inject NavigationManager Nav

<PageTitle>Students — Music School</PageTitle>

<div class="page-header">
    <MudText Typo="Typo.h5">Students</MudText>
    <MudText Typo="Typo.body2" Color="Color.Secondary">Browse all enrolled students</MudText>
</div>

<MudPaper Class="pa-3 mb-4" Elevation="1">
    <MudGrid>
        <MudItem xs="12" sm="6" md="4">
            <MudSelect @bind-Value="_selectedTeacherId" Label="Teacher"
                       @bind-Value:after="OnTeacherChanged" Clearable="true">
                @foreach (var t in _teachers)
                {
                    <MudSelectItem Value="t.TeacherID">@t.Name</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudSelect @bind-Value="_selectedAccountHolderId" Label="Account Holder"
                       @bind-Value:after="OnAccountHolderChanged" Disabled="_accountHolders.Count == 0" Clearable="true">
                @foreach (var ah in _accountHolders)
                {
                    <MudSelectItem Value="ah.AccountHolderID">@ah.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudTextField @bind-Value="_searchString" Label="Search" Immediate="true"
                          Adornment="Adornment.Start" AdornmentIcon="@Icons.Material.Filled.Search" />
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

<MudPaper Elevation="1">

    <MudTable Items="FilteredStudents" Hover="true" Dense="false" Loading="_loading">
        <HeaderContent>
            <MudTh>Name</MudTh>
            <MudTh>Date of Birth</MudTh>
            <MudTh>Account Holder</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd>@context.FullName</MudTd>
            <MudTd>@(context.DateOfBirth?.ToString("dd MMM yyyy") ?? "—")</MudTd>
            <MudTd>
                @if (context.IsAccountHolder)
                {
                    <MudIcon Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Primary" Size="Size.Small" />
                    <MudText Typo="Typo.caption"> Self</MudText>
                }
            </MudTd>
            <MudTd>
                <MudChip T="string" Size="Size.Small" Color="@(context.IsActive ? Color.Success : Color.Default)">
                    @(context.IsActive ? "Active" : "Inactive")
                </MudChip>
            </MudTd>
            <MudTd>
                <MudButton Variant="Variant.Text" Color="Color.Primary" Size="Size.Small"
                           OnClick="@(() => Nav.NavigateTo($"/students/{context.StudentID}"))">
                    View
                </MudButton>
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>Select a teacher and account holder to view students.</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>


@code {
    
    private bool _loading;
    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private List<Student> _students = [];
    private int _selectedTeacherId;
    private int _selectedAccountHolderId;
    private string _searchString = string.Empty;    

    private IEnumerable<Student> FilteredStudents =>
        _students.Where(s => string.IsNullOrWhiteSpace(_searchString) ||
            s.FullName.Contains(_searchString, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
    }

    private async Task OnTeacherChanged()
    {
        _accountHolders = _selectedTeacherId > 0
            ? await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId)
            : [];
        _students = [];
        _selectedAccountHolderId = 0;
    }

    private async Task OnAccountHolderChanged()
    {
        if (_selectedAccountHolderId > 0)
        {
            _loading = true;
            _students = await StudentSvc.GetByAccountHolderAsync(_selectedAccountHolderId);
            _loading = false;
        }
        else _students = [];
    }
}

```

## File: MusicSchool.Web\Pages\Teachers.razor

```razor
@page "/teachers"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject ISnackbar Snackbar

<PageTitle>Teachers — Music School</PageTitle>

<div class="page-header d-flex justify-space-between align-center">
    <div>
        <MudText Typo="Typo.h5">Teachers</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">Manage school teachers</MudText>
    </div>
    <MudButton Variant="Variant.Filled" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Add"
               OnClick="OpenAddDialog">Add Teacher</MudButton>
</div>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" />
}

<MudPaper Elevation="1">
    <MudTable Items="_teachers" Hover="true" Dense="false" Loading="_loading"
              Filter="FilterFunc" @bind-SelectedItem="_selectedTeacher">
        <ToolBarContent>
            <MudTextField @bind-Value="_searchString" Placeholder="Search teachers..."
                          Adornment="Adornment.Start" AdornmentIcon="@Icons.Material.Filled.Search"
                          IconSize="Size.Medium" Class="mt-0" Immediate="true" />
        </ToolBarContent>
        <HeaderContent>
            <MudTh>Name</MudTh>
            <MudTh>Email</MudTh>
            <MudTh>Phone</MudTh>
            <MudTh>Status</MudTh>
            <MudTh>Actions</MudTh>
        </HeaderContent>
        <RowTemplate>
            <MudTd>@context.Name</MudTd>
            <MudTd>@context.Email</MudTd>
            <MudTd>@(context.Phone ?? "—")</MudTd>
            <MudTd>
                <MudChip T="string" Size="Size.Small" Color="@(context.IsActive ? Color.Success : Color.Default)">
                    @(context.IsActive ? "Active" : "Inactive")
                </MudChip>
            </MudTd>
            <MudTd>
                <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" Color="Color.Primary"
                               OnClick="@(() => OpenEditDialog(context))" />
            </MudTd>
        </RowTemplate>
        <NoRecordsContent>
            <MudText>No teachers found.</MudText>
        </NoRecordsContent>
    </MudTable>
</MudPaper>

<!-- Add Dialog -->
<MudDialog @ref="_addDialog" Options="_dialogOptions">
    <TitleContent>
        <MudText Typo="Typo.h6">Add Teacher</MudText>
    </TitleContent>
    <DialogContent>
        <MudForm @ref="_addForm">
            <MudTextField @bind-Value="_newTeacher.Name" Label="Full Name" Required="true"
                          RequiredError="Name is required" Class="mb-3" />
            <MudTextField @bind-Value="_newTeacher.Email" Label="Email" Required="true"
                          RequiredError="Email is required" InputType="InputType.Email" Class="mb-3" />
            <MudTextField @bind-Value="_newTeacher.Phone" Label="Phone" Class="mb-3" />
            <MudSwitch @bind-Value="_newTeacher.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _addDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveNewTeacher">Save</MudButton>
    </DialogActions>
</MudDialog>

<!-- Edit Dialog -->
<MudDialog @ref="_editDialog" Options="_dialogOptions">
    <TitleContent>
        <MudText Typo="Typo.h6">Edit Teacher</MudText>
    </TitleContent>
    <DialogContent>
        <MudForm @ref="_editForm">
            <MudTextField @bind-Value="_editTeacher.Name" Label="Full Name" Required="true"
                          RequiredError="Name is required" Class="mb-3" />
            <MudTextField @bind-Value="_editTeacher.Email" Label="Email" Required="true"
                          RequiredError="Email is required" InputType="InputType.Email" Class="mb-3" />
            <MudTextField @bind-Value="_editTeacher.Phone" Label="Phone" Class="mb-3" />
            <MudSwitch @bind-Value="_editTeacher.IsActive" Label="Active" Color="Color.Primary" />
        </MudForm>
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _editDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="SaveEditTeacher">Save</MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading = true;
    private List<Teacher> _teachers = [];
    private Teacher? _selectedTeacher;
    private string _searchString = string.Empty;

    private MudDialog? _addDialog;
    private MudDialog? _editDialog;
    private MudForm? _addForm;
    private MudForm? _editForm;

    private Teacher _newTeacher = new();
    private Teacher _editTeacher = new();

    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    protected override async Task OnInitializedAsync()
    {
        await LoadTeachers();
    }

    private async Task LoadTeachers()
    {
        _loading = true;
        _teachers = await TeacherSvc.GetAllActiveAsync();
        _loading = false;
    }

    private bool FilterFunc(Teacher t) =>
        string.IsNullOrWhiteSpace(_searchString) ||
        t.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
        t.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase);

    private async Task OpenAddDialog()
    {
        _newTeacher = new Teacher { IsActive = true };
        await _addDialog!.ShowAsync();
    }

    private async Task OpenEditDialog(Teacher teacher)
    {
        _editTeacher = new Teacher
        {
            TeacherID = teacher.TeacherID,
            Name = teacher.Name,
            Email = teacher.Email,
            Phone = teacher.Phone,
            IsActive = teacher.IsActive,
            CreatedAt = teacher.CreatedAt
        };
        await _editDialog!.ShowAsync();
    }

    private async Task SaveNewTeacher()
    {
        await _addForm!.Validate();
        if (!_addForm.IsValid) return;

        var result = await TeacherSvc.AddTeacherAsync(_newTeacher);
        if (result.HasValue)
        {
            Snackbar.Add("Teacher added successfully.", Severity.Success);
            await _addDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadTeachers();
        }
        else
        {
            Snackbar.Add("Failed to add teacher.", Severity.Error);
        }
    }

    private async Task SaveEditTeacher()
    {
        await _editForm!.Validate();
        if (!_editForm.IsValid) return;

        var result = await TeacherSvc.UpdateTeacherAsync(_editTeacher);
        if (result)
        {
            Snackbar.Add("Teacher updated successfully.", Severity.Success);
            await _editDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadTeachers();
        }
        else
        {
            Snackbar.Add("Failed to update teacher.", Severity.Error);
        }
    }
}

```

## File: MusicSchool.Web\Properties\launchSettings.json

```json
{
  "profiles": {
    "MusicSchool.Web": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:64314;http://localhost:64315"
    }
  }
}
```

## File: MusicSchool.Web\Services\Services.cs

```csharp
using System.Net.Http.Json;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Services
{
    public class TeacherService(HttpClient http)
    {
        public async Task<List<Teacher>> GetAllActiveAsync()
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Teacher>>>("Teacher/GetAllActive");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<Teacher?> GetTeacherAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<Teacher>>($"Teacher/GetTeacher?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddTeacherAsync(Teacher teacher)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Teacher/AddTeacher", teacher);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateTeacherAsync(Teacher teacher)
        {
            try
            {
                var r = await http.PutAsJsonAsync("Teacher/UpdateTeacher", teacher);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class AccountHolderService(HttpClient http)
    {
        public async Task<List<AccountHolder>> GetByTeacherAsync(int teacherId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<AccountHolder>>>($"AccountHolder/GetByTeacher?teacherId={teacherId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<AccountHolder>>($"AccountHolder/GetAccountHolder?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                var r = await http.PostAsJsonAsync("AccountHolder/AddAccountHolder", accountHolder);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateAccountHolderAsync(AccountHolder accountHolder)
        {
            try
            {
                var r = await http.PutAsJsonAsync("AccountHolder/UpdateAccountHolder", accountHolder);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class StudentService(HttpClient http)
    {
        public async Task<List<Student>> GetByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Student>>>($"Student/GetByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<Student>>($"Student/GetStudent?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddStudentAsync(Student student)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Student/AddStudent", student);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            try
            {
                var r = await http.PutAsJsonAsync("Student/UpdateStudent", student);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonTypeService(HttpClient http)
    {
        public async Task<List<LessonType>> GetAllActiveAsync()
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonType>>>("LessonType/GetAllActive");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<LessonType?> GetLessonTypeAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<LessonType>>($"LessonType/GetLessonType?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                var r = await http.PostAsJsonAsync("LessonType/AddLessonType", lessonType);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateLessonTypeAsync(LessonType lessonType)
        {
            try
            {
                var r = await http.PutAsJsonAsync("LessonType/UpdateLessonType", lessonType);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonBundleService(HttpClient http)
    {
        public async Task<List<LessonBundleWithQuarterDetail>> GetBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleWithQuarterDetail>>>($"LessonBundle/GetBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<LessonBundleDetail>> GetByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonBundleDetail>>>($"LessonBundle/GetByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<int?> AddBundleAsync(AddBundleRequest request)
        {
            try
            {
                var r = await http.PostAsJsonAsync("LessonBundle/AddBundle", request);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateBundleAsync(LessonBundle bundle)
        {
            try
            {
                var r = await http.PutAsJsonAsync("LessonBundle/UpdateBundle", bundle);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class ScheduledSlotService(HttpClient http)
    {
        public async Task<List<ScheduledSlot>> GetActiveByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ScheduledSlot>>>($"ScheduledSlot/GetActiveByTeacher?teacherId={teacherId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<ScheduledSlot?> GetSlotAsync(int id)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<ScheduledSlot>>($"ScheduledSlot/GetSlot?id={id}");
                return r?.Data;
            }
            catch { return null; }
        }

        /// <summary>
        /// Returns (newSlotId, null) on success, or (null, errorMessage) when the API
        /// rejects the request (e.g. no active bundle with remaining credits).
        /// </summary>
        public async Task<(int? Id, string? Error)> AddSlotAsync(ScheduledSlot slot)
        {
            try
            {
                var r = await http.PostAsJsonAsync("ScheduledSlot/AddSlot", slot);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                if (result is null) return (null, "Unexpected error communicating with the server.");
                if (result.ReturnCode != 0) return (null, result.ReturnMessage);
                return (result.Data, null);
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

        public async Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo)
        {
            try
            {
                var r = await http.PutAsync($"ScheduledSlot/CloseSlot?slotId={slotId}&effectiveTo={effectiveTo:yyyy-MM-dd}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class LessonService(HttpClient http)
    {
        public async Task<List<LessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime date)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<LessonDetail>>>($"Lesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Lesson>> GetByBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Lesson>>>($"Lesson/GetByBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<LessonDetail?> GetLessonAsync(int lessonId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<LessonDetail>>($"Lesson/GetLesson?lessonId={lessonId}");
                return r?.Data;
            }
            catch { return null; }
        }

        public async Task<int?> AddLessonAsync(Lesson lesson)
        {
            try
            {
                var r = await http.PostAsJsonAsync("Lesson/AddLesson", lesson);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status)
        {
            try
            {
                var r = await http.PutAsync($"Lesson/UpdateLessonStatus?lessonId={lessonId}&status={status}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class ExtraLessonService(HttpClient http)
    {
        public async Task<List<ExtraLessonDetail>> GetByTeacherAndDateAsync(int teacherId, DateTime date)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLessonDetail>>>($"ExtraLesson/GetByTeacherAndDate?teacherId={teacherId}&scheduledDate={date:yyyy-MM-dd}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<ExtraLesson>> GetByStudentAsync(int studentId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<ExtraLesson>>>($"ExtraLesson/GetByStudent?studentId={studentId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<int?> AddExtraLessonAsync(ExtraLesson lesson)
        {
            try
            {
                var r = await http.PostAsJsonAsync("ExtraLesson/AddExtraLesson", lesson);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<int?>>();
                return result?.Data;
            }
            catch { return null; }
        }

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status)
        {
            try
            {
                var r = await http.PutAsync($"ExtraLesson/UpdateExtraLessonStatus?extraLessonId={extraLessonId}&status={status}", null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }

    public class InvoiceService(HttpClient http)
    {
        public async Task<List<Invoice>> GetByBundleAsync(int bundleId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByBundle?bundleId={bundleId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Invoice>> GetByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<List<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId)
        {
            try
            {
                var r = await http.GetFromJsonAsync<ResponseBase<List<Invoice>>>($"Invoice/GetOutstandingByAccountHolder?accountHolderId={accountHolderId}");
                return r?.Data ?? [];
            }
            catch { return []; }
        }

        public async Task<bool> UpdateInvoiceStatusAsync(int invoiceId, string status, DateOnly? paidDate)
        {
            try
            {
                var url = $"Invoice/UpdateInvoiceStatus?invoiceId={invoiceId}&status={status}";
                if (paidDate.HasValue) url += $"&paidDate={paidDate.Value:yyyy-MM-dd}";
                var r = await http.PutAsync(url, null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }
    }
}
```

## File: MusicSchool.Web\Shared\MainLayout.razor

```razor
@inherits LayoutComponentBase

@namespace MusicSchool.Web.Shared

<MudLayout>
    <MudAppBar Elevation="1" Color="Color.Primary">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start"
                       OnClick="@ToggleDrawer" />
        <MudIcon Icon="@Icons.Material.Filled.MusicNote" Class="mr-2" />
        <MudText Typo="Typo.h6" Style="font-weight:600;">Music School</MudText>
        <MudSpacer />
        <MudText Typo="Typo.body2">Management Portal</MudText>
    </MudAppBar>

    <MudDrawer @bind-Open="_drawerOpen" Elevation="2" Variant="DrawerVariant.Responsive"
               ClipMode="DrawerClipMode.Always">
        <MudDrawerHeader>
            <MudText Typo="Typo.subtitle1" Style="font-weight:600; color:#3A3A3A;">Navigation</MudText>
        </MudDrawerHeader>
        <MudNavMenu>
            <MudNavLink Href="/" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Dashboard">
                Dashboard
            </MudNavLink>
            <MudNavGroup Title="Administration" Icon="@Icons.Material.Filled.Settings" Expanded="true">
                <MudNavLink Href="/teachers" Icon="@Icons.Material.Filled.Person">Teachers</MudNavLink>
                <MudNavLink Href="/lesson-types" Icon="@Icons.Material.Filled.Timer">Lesson Types</MudNavLink>
            </MudNavGroup>
            <MudNavGroup Title="Accounts" Icon="@Icons.Material.Filled.AccountCircle" Expanded="true">
                <MudNavLink Href="/account-holders" Icon="@Icons.Material.Filled.People">Account Holders</MudNavLink>
                <MudNavLink Href="/students" Icon="@Icons.Material.Filled.School">Students</MudNavLink>
            </MudNavGroup>
            <MudNavGroup Title="Lessons" Icon="@Icons.Material.Filled.LibraryMusic" Expanded="true">
                <MudNavLink Href="/lesson-bundles" Icon="@Icons.Material.Filled.Inventory">Bundles</MudNavLink>
                <MudNavLink Href="/schedule" Icon="@Icons.Material.Filled.CalendarMonth">Schedule</MudNavLink>
                <MudNavLink Href="/extra-lessons" Icon="@Icons.Material.Filled.AddCircle">Extra Lessons</MudNavLink>
            </MudNavGroup>
            <MudNavGroup Title="Finance" Icon="@Icons.Material.Filled.Payment" Expanded="true">
                <MudNavLink Href="/invoices" Icon="@Icons.Material.Filled.Receipt">Invoices</MudNavLink>
            </MudNavGroup>
        </MudNavMenu>
    </MudDrawer>

    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.ExtraLarge" Class="pa-4 pa-md-6">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

@code {
    private bool _drawerOpen = true;

    private void ToggleDrawer() => _drawerOpen = !_drawerOpen;
}

```

## File: MusicSchool.Web\Shared\StatusChip.razor

```razor
@* Shared/StatusChip.razor *@
@namespace MusicSchool.Web.Shared

<MudChip T="string" Size="Size.Small" Class="@($"mud-chip-filled {GetCssClass()}")" Style="font-size:0.7rem;">
    @Status
</MudChip>

@code {
    [Parameter] public string Status { get; set; } = string.Empty;

    private string GetCssClass() => Status?.ToLower() switch
    {
        "scheduled"        => "status-scheduled",
        "completed"        => "status-completed",
        "forfeited"        => "status-forfeited",
        "cancelledteacher" => "status-cancelled",
        "cancelledstudent" => "status-cancelled",
        "cancelled"        => "status-cancelled",
        "rescheduled"      => "status-rescheduled",
        "pending"          => "status-pending",
        "paid"             => "status-paid",
        "overdue"          => "status-overdue",
        "void"             => "status-void",
        _                  => "status-scheduled"
    };
}

```

## File: MusicSchool.Web\wwwroot\appsettings.Development.json

```json
{
  "ApiBaseUrl": "https://localhost:64100/"
}

```

## File: MusicSchool.Web\wwwroot\appsettings.json

```json
{
  "ApiBaseUrl": "https://localhost:64100/"
}

```

## File: MusicSchool.Web\App.razor

```razor
@using MudBlazor
@namespace MusicSchool.Web

<MudThemeProvider Theme="_theme" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<Router AppAssembly="typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(Shared.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="typeof(Shared.MainLayout)">
            <MudText Typo="Typo.h5" Class="pa-4">Sorry, there's nothing at this address.</MudText>
        </LayoutView>
    </NotFound>
</Router>

@code {
    private MudTheme _theme = new MudTheme
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#F3D395",
            PrimaryContrastText = "#3A3A3A",
            Secondary = "#78797A",
            SecondaryContrastText = "#FFFFFF",
            Background = "#F0F2F5",
            Surface = "#FFFFFF",
            AppbarBackground = "#F3D395",
            AppbarText = "#3A3A3A",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#3A3A3A",
            TextPrimary = "#3A3A3A",
            TextSecondary = "#78797A",
            ActionDefault = "#78797A",
            Divider = "#DFE1E3",
            TableLines = "#DFE1E3",
            LinesDefault = "#DFE1E3",
        }
    };
}

```

## File: MusicSchool.Web\MusicSchool.Web.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <ServiceWorkerAssetsManifest>service-worker-assets.js</ServiceWorkerAssetsManifest>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.0" PrivateAssets="all" />
    <PackageReference Include="MudBlazor" Version="7.15.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

</Project>

```

## File: MusicSchool.Web\Program.cs

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicSchool.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<MusicSchool.Web.App>("#app");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/")
});

builder.Services.AddMudServices();

builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<AccountHolderService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<LessonTypeService>();
builder.Services.AddScoped<LessonBundleService>();
builder.Services.AddScoped<ScheduledSlotService>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<ExtraLessonService>();
builder.Services.AddScoped<InvoiceService>();

// Catch any unhandled exception and write it to the browser console instead of
// crashing the JS debug adapter with exit code 0xffffffff.
AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

await builder.Build().RunAsync();
```

## File: MusicSchool.Web\_Imports.razor

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using MusicSchool
@using MusicSchool.Models
@using MusicSchool.Services
@using MusicSchool.Web.Shared
```
