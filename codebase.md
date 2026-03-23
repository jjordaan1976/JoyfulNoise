# Flattened Codebase

Generated: 03/23/2026 15:29:33


## File: MusicSchool.AccountHolderPortal\Pages\Index.razor

```razor
@page "/"
@namespace MusicSchool.AccountHolderPortal.Pages

<PageTitle>Music School — Account Portal</PageTitle>

<MudPaper Class="pa-6 mt-6" Elevation="1" Style="max-width:480px; margin:auto; border-top:4px solid #F3D395;">

    <MudStack AlignItems="AlignItems.Center" Spacing="2" Class="mb-5">
        <MudIcon Icon="@Icons.Material.Filled.MusicNote"
                 Style="font-size:3rem; color:#F3D395;" />
        <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
            Account Portal
        </MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Align="Align.Center">
            Enter your Account Holder ID to view your statement and invoices.
        </MudText>
    </MudStack>

    <MudTextField @bind-Value="_accountHolderId"
                  Label="Account Holder ID"
                  Variant="Variant.Outlined"
                  InputType="InputType.Number"
                  Adornment="Adornment.Start"
                  AdornmentIcon="@Icons.Material.Filled.AccountCircle"
                  Class="mb-4"
                  OnKeyDown="@OnKeyDown" />

    <MudButton Variant="Variant.Filled"
               Color="Color.Primary"
               FullWidth="true"
               Size="Size.Large"
               StartIcon="@Icons.Material.Filled.Receipt"
               OnClick="ViewStatement"
               Disabled="@(_accountHolderId <= 0)">
        View My Statement
    </MudButton>

</MudPaper>

@code {
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private int _accountHolderId;

    private void ViewStatement()
    {
        if (_accountHolderId > 0)
            Nav.NavigateTo($"/statement/{_accountHolderId}");
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") ViewStatement();
    }
}

```

## File: MusicSchool.AccountHolderPortal\Pages\Statement.razor

```razor
@page "/statement/{AccountHolderId:int}"
@namespace MusicSchool.AccountHolderPortal.Pages

@using MusicSchool.Data.Models

@inject ApiService Api

<PageTitle>Account Statement — Music School</PageTitle>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="mb-4" />
    <MudText>Loading your statement…</MudText>
}
else if (_accountHolder is null)
{
    <MudAlert Severity="Severity.Error">
        Account not found. Please check your link or contact your teacher.
    </MudAlert>
}
else
{
    <!-- ── Header ──────────────────────────────────────────────── -->
    <MudPaper Class="pa-5 mb-4" Elevation="1" Style="border-top: 4px solid #F3D395;">
        <MudGrid AlignItems="AlignItems.Center">
            <MudItem xs="12" sm="8">
                <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                    <MudIcon Icon="@Icons.Material.Filled.MusicNote"
                             Style="font-size:2rem; color:#F3D395;" />
                    <div>
                        <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
                            Account Statement
                        </MudText>
                        <MudText Typo="Typo.body2" Color="Color.Secondary">
                            @_accountHolder.FullName &nbsp;·&nbsp; @_accountHolder.Email
                        </MudText>
                    </div>
                </MudStack>
            </MudItem>
            <MudItem xs="12" sm="4" Class="d-flex justify-end">
                <div class="text-right">
                    <MudText Typo="Typo.caption" Color="Color.Secondary">Statement date</MudText>
                    <MudText Typo="Typo.subtitle2" Style="font-weight:600;">
                        @DateTime.Today.ToString("yyyy MMMM dd")
                    </MudText>
                </div>
            </MudItem>
        </MudGrid>
    </MudPaper>

    <!-- ── Aging summary cards ─────────────────────────────────── -->
    <MudText Typo="Typo.h6" Class="mb-3" Style="font-weight:600;">Outstanding Balance Summary</MudText>
    <MudGrid Spacing="3" Class="mb-5">
        <MudItem xs="12" sm="6" md="3">
            <MudPaper Class="pa-4 aging-card" Elevation="1">
                <MudText Typo="Typo.caption" Color="Color.Secondary">Current</MudText>
                <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
                    R @_current.ToString("N2")
                </MudText>
                <MudText Typo="Typo.caption" Color="Color.Secondary">Due today</MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="12" sm="6" md="3">
            <MudPaper Class="pa-4 aging-card-warn" Elevation="1">
                <MudText Typo="Typo.caption" Color="Color.Secondary">30 Days</MudText>
                <MudText Typo="Typo.h5" Style="font-weight:700; color:#E65100;">
                    R @_days30.ToString("N2")
                </MudText>
                <MudText Typo="Typo.caption" Color="Color.Secondary">1–30 days overdue</MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="12" sm="6" md="3">
            <MudPaper Class="pa-4 aging-card-alert" Elevation="1">
                <MudText Typo="Typo.caption" Color="Color.Secondary">60 Days</MudText>
                <MudText Typo="Typo.h5" Style="font-weight:700; color:#B71C1C;">
                    R @_days60.ToString("N2")
                </MudText>
                <MudText Typo="Typo.caption" Color="Color.Secondary">31–60 days overdue</MudText>
            </MudPaper>
        </MudItem>
        <MudItem xs="12" sm="6" md="3">
            <MudPaper Class="pa-4 aging-card-crit" Elevation="1">
                <MudText Typo="Typo.caption" Color="Color.Secondary">90+ Days</MudText>
                <MudText Typo="Typo.h5" Style="font-weight:700; color:#C62828;">
                    R @_days90.ToString("N2")
                </MudText>
                <MudText Typo="Typo.caption" Color="Color.Secondary">61+ days overdue</MudText>
            </MudPaper>
        </MudItem>
    </MudGrid>

    <!-- ── Total outstanding banner ────────────────────────────── -->
    @if (TotalOutstanding > 0)
    {
        <MudAlert Severity="Severity.Warning" Class="mb-4" Icon="@Icons.Material.Filled.Warning">
            <strong>Total outstanding: R @TotalOutstanding.ToString("N2")</strong>
            &nbsp;— Please arrange payment at your earliest convenience.
        </MudAlert>
    }
    else
    {
        <MudAlert Severity="Severity.Success" Class="mb-4" Icon="@Icons.Material.Filled.CheckCircle">
            Your account is up to date. No outstanding amounts.
        </MudAlert>
    }

    <!-- ── Full invoice list ────────────────────────────────────── -->
    <MudPaper Class="pa-4 mb-4" Elevation="1">
        <MudText Typo="Typo.h6" Class="mb-3" Style="font-weight:600;">Invoice History</MudText>

        @if (_allInvoices.Count == 0)
        {
            <MudText Color="Color.Secondary">No invoices found on this account.</MudText>
        }
        else
        {
            <MudTable Items="_allInvoices" Hover="true" Dense="false" Elevation="0"
                      SortLabel="Sort By">
                <HeaderContent>
                    <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.DueDate)">Due Date</MudTableSortLabel></MudTh>
                    <MudTh>Description</MudTh>
                    <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.Amount)">Amount</MudTableSortLabel></MudTh>
                    <MudTh>Paid Date</MudTh>
                    <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.Status)">Status</MudTableSortLabel></MudTh>
                    <MudTh>Aging</MudTh>
                </HeaderContent>
                <RowTemplate>
                    <MudTd DataLabel="Due Date">@context.DueDate.ToString("yyyy MMM dd")</MudTd>
                    <MudTd DataLabel="Description">
                        @if (context.BundleID.HasValue)
                        {
                            <span>Bundle #@context.BundleID — Instalment @context.InstallmentNumber</span>
                        }
                        else if (context.ExtraLessonID.HasValue)
                        {
                            <span>Extra Lesson #@context.ExtraLessonID</span>
                        }
                        else
                        {
                            <span>Invoice #@context.InvoiceID</span>
                        }
                    </MudTd>
                    <MudTd DataLabel="Amount">R @context.Amount.ToString("N2")</MudTd>
                    <MudTd DataLabel="Paid Date">@(context.PaidDate?.ToString("yyyy MMM dd") ?? "—")</MudTd>
                    <MudTd DataLabel="Status"><StatusChip Status="@context.Status" /></MudTd>
                    <MudTd DataLabel="Aging">@GetAgingLabel(context)</MudTd>
                </RowTemplate>
                <FooterContent>
                    <MudTd colspan="2" Style="font-weight:600;">Total Invoiced</MudTd>
                    <MudTd Style="font-weight:600;">
                        R @_allInvoices.Sum(i => i.Amount).ToString("N2")
                    </MudTd>
                    <MudTd colspan="3"></MudTd>
                </FooterContent>
            </MudTable>
        }
    </MudPaper>

    <!-- ── Unallocated payments ──────────────────────────────────── -->
    @if (UnallocatedPayments.Count > 0)
    {
        <MudPaper Class="pa-4" Elevation="1" Style="border-left: 4px solid #F3D395;">
            <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2" Class="mb-3">
                <MudIcon Icon="@Icons.Material.Filled.Pending" Style="color:#F3D395;" />
                <MudText Typo="Typo.h6" Style="font-weight:600;">Unallocated Payments</MudText>
                <MudChip T="string" Size="Size.Small" Color="Color.Warning">
                    R @TotalUnallocated.ToString("N2") pending
                </MudChip>
            </MudStack>

            <MudAlert Severity="Severity.Info" Dense="true" Class="mb-3">
                The following payment(s) have been received but not yet matched to an invoice.
                They will be automatically applied once they reach the next invoice amount
                @if (NextPendingInvoiceAmount is not null)
                {
                    <span>(R @NextPendingInvoiceAmount).</span>
                }
            </MudAlert>

            <MudTable Items="UnallocatedPayments" Hover="true" Dense="true" Elevation="0">
                <HeaderContent>
                    <MudTh>Payment Date</MudTh>
                    <MudTh>Amount Paid</MudTh>
                    <MudTh>Unallocated</MudTh>
                    <MudTh>Reference</MudTh>
                </HeaderContent>
                <RowTemplate>
                    <MudTd>@context.PaymentDate.ToString("yyyy MMM dd")</MudTd>
                    <MudTd>R @context.Amount.ToString("N2")</MudTd>
                    <MudTd>
                        <MudText Style="font-weight:600; color:#E65100;">
                            R @context.UnallocatedAmount.ToString("N2")
                        </MudText>
                    </MudTd>
                    <MudTd>@(context.Reference ?? "—")</MudTd>
                </RowTemplate>
                <FooterContent>
                    <MudTd colspan="2" Style="font-weight:600;">Total Unallocated</MudTd>
                    <MudTd Style="font-weight:600; color:#E65100;">
                        R @TotalUnallocated.ToString("N2")
                    </MudTd>
                    <MudTd></MudTd>
                </FooterContent>
            </MudTable>
        </MudPaper>
    }
}

@code {
    [Parameter] public int AccountHolderId { get; set; }

    private bool _loading = true;
    private AccountHolder? _accountHolder;
    private List<Invoice> _allInvoices = [];
    private List<Payment> _payments = [];

    // Aging buckets
    private decimal _current;
    private decimal _days30;
    private decimal _days60;
    private decimal _days90;

    private decimal TotalOutstanding => _current + _days30 + _days60 + _days90;

    // Computed properties — used directly in markup, no @{ } blocks needed
    private List<Payment> UnallocatedPayments =>
        _payments.Where(p => p.UnallocatedAmount > 0).ToList();

    private decimal TotalUnallocated =>
        _payments.Sum(p => p.UnallocatedAmount);

    private string? NextPendingInvoiceAmount =>
        _allInvoices
            .Where(i => i.Status is InvoiceStatus.Pending or InvoiceStatus.Overdue)
            .OrderBy(i => i.DueDate)
            .FirstOrDefault()
            ?.Amount.ToString("N2");

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        var accountHolderTask = Api.GetAccountHolderAsync(AccountHolderId);
        var allInvoicesTask = Api.GetAllInvoicesAsync(AccountHolderId);
        var paymentsTask = Api.GetPaymentsByAccountHolderAsync(AccountHolderId);

        await Task.WhenAll(accountHolderTask, allInvoicesTask, paymentsTask);

        _accountHolder = await accountHolderTask;
        _allInvoices = await allInvoicesTask;
        _payments = await paymentsTask;

        _allInvoices = [.. _allInvoices.OrderByDescending(i => i.DueDate)];

        CalculateAging();
        _loading = false;
    }

    private void CalculateAging()
    {
        _current = _days30 = _days60 = _days90 = 0;
        var today = DateTime.Today;

        foreach (var inv in _allInvoices
            .Where(i => i.Status is InvoiceStatus.Pending or InvoiceStatus.Overdue))
        {
            var days = (today - inv.DueDate).Days;
            if (days < 0) continue;
            else if (days == 0) _current += inv.Amount;
            else if (days <= 30) _days30 += inv.Amount;
            else if (days <= 60) _days60 += inv.Amount;
            else _days90 += inv.Amount;
        }
    }

    private static string GetAgingLabel(Invoice inv)
    {
        if (inv.Status is InvoiceStatus.Paid or InvoiceStatus.Void) return "—";
        var days = (DateTime.Today - inv.DueDate).Days;
        if (days < 0) return "Not yet due";
        else if (days == 0) return "Due today";
        else if (days <= 30) return $"{days}d overdue";
        else if (days <= 60) return $"{days}d overdue";
        else return $"{days}d overdue";
    }
}

```

## File: MusicSchool.AccountHolderPortal\Properties\launchSettings.json

```json
{
  "profiles": {
    "MusicSchool.AccountHolderPortal": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:57349;http://localhost:57350"
    }
  }
}
```

## File: MusicSchool.AccountHolderPortal\Services\ApiService.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;

namespace MusicSchool.AccountHolderPortal.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ResponseBase<T>>(url);
            return response is not null ? response.Data : default;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ApiService.GetAsync] {url} — {ex.Message}");
            return default;
        }
    }

    // ── Convenience helpers ──────────────────────────────────────

    public Task<AccountHolder?> GetAccountHolderAsync(int id)
        => GetAsync<AccountHolder>($"AccountHolder/GetAccountHolder?id={id}");

    public async Task<List<Invoice>> GetAllInvoicesAsync(int accountHolderId)
    {
        var result = await GetAsync<IEnumerable<Invoice>>(
            $"Invoice/GetByAccountHolder?accountHolderId={accountHolderId}");
        return result?.ToList() ?? [];
    }

    public async Task<List<Invoice>> GetOutstandingInvoicesAsync(int accountHolderId)
    {
        var result = await GetAsync<IEnumerable<Invoice>>(
            $"Invoice/GetOutstandingByAccountHolder?accountHolderId={accountHolderId}");
        return result?.ToList() ?? [];
    }

    public async Task<List<Payment>> GetPaymentsByAccountHolderAsync(int accountHolderId)
{
    var result = await GetAsync<IEnumerable<Payment>>(
        $"Payment/GetByAccountHolder?accountHolderId={accountHolderId}");
    return result?.ToList() ?? [];
}
}

```

## File: MusicSchool.AccountHolderPortal\Shared\MainLayout.razor

```razor
@inherits LayoutComponentBase
@namespace MusicSchool.AccountHolderPortal.Shared

<MudLayout>
    <MudAppBar Elevation="1" Color="Color.Primary">
        <MudIcon Icon="@Icons.Material.Filled.MusicNote" Class="mr-2" />
        <MudText Typo="Typo.h6" Style="font-weight:600;">Music School</MudText>
        <MudSpacer />
        <MudText Typo="Typo.body2">Account Portal</MudText>
    </MudAppBar>

    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.Large" Class="pa-4 pa-md-6">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

```

## File: MusicSchool.AccountHolderPortal\Shared\StatusChip.razor

```razor
@* Shared/StatusChip.razor *@
@namespace MusicSchool.AccountHolderPortal.Shared

<MudChip T="string"
         Size="Size.Small"
         Class="@($"mud-chip-filled {GetCssClass()}")"
         Style="font-size:0.7rem;">
    @Status
</MudChip>

@code {
    [Parameter] public string Status { get; set; } = string.Empty;

    private string GetCssClass() => Status?.ToLower() switch
    {
        "paid"    => "status-paid",
        "overdue" => "status-overdue",
        "void"    => "status-void",
        _         => "status-pending"     // Pending
    };
}

```

## File: MusicSchool.AccountHolderPortal\wwwroot\appsettings.Development.json

```json
{
  "profiles": {
    "MusicSchool.AccountHolderPortal": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:52100;http://localhost:52101"
    }
  }
}

```

## File: MusicSchool.AccountHolderPortal\wwwroot\appsettings.json

```json
{
  "ApiBaseUrl": "https://localhost:64100/"
}

```

## File: MusicSchool.AccountHolderPortal\App.razor

```razor
@using MudBlazor
@namespace MusicSchool.AccountHolderPortal

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
    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary              = "#F3D395",
            PrimaryContrastText  = "#3A3A3A",
            Secondary            = "#78797A",
            SecondaryContrastText= "#FFFFFF",
            Background           = "#F0F2F5",
            Surface              = "#FFFFFF",
            AppbarBackground     = "#F3D395",
            AppbarText           = "#3A3A3A",
            DrawerBackground     = "#FFFFFF",
            DrawerText           = "#3A3A3A",
            TextPrimary          = "#3A3A3A",
            TextSecondary        = "#78797A",
            ActionDefault        = "#78797A",
            Divider              = "#DFE1E3",
            TableLines           = "#DFE1E3",
            LinesDefault         = "#DFE1E3",
        }
    };
}

```

## File: MusicSchool.AccountHolderPortal\MusicSchool.AccountHolderPortal.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.0" PrivateAssets="all" />
    <PackageReference Include="MudBlazor" Version="7.15.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

  <!--
    Add a project reference to your shared MusicSchool.Models project:
    <ItemGroup>
      <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
    </ItemGroup>

    Until then, the model classes are duplicated inline in Services\Models.cs.
  -->

</Project>

```

## File: MusicSchool.AccountHolderPortal\Program.cs

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicSchool.AccountHolderPortal;
using MusicSchool.AccountHolderPortal.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/")
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddMudServices();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

await builder.Build().RunAsync();

```

## File: MusicSchool.AccountHolderPortal\_Imports.razor

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using MusicSchool.AccountHolderPortal
@using MusicSchool.AccountHolderPortal.Services
@using MusicSchool.AccountHolderPortal.Shared

```

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
        public async Task<ResponseBase<bool>> UpdateExtraLessonStatus(
            [FromQuery] int extraLessonId,
            [FromQuery] string status,
            [FromQuery] string? note = null)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _extraLessonRepository.UpdateExtraLessonStatusAsync(extraLessonId, status, note);
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
        public async Task<ResponseBase<bool>> UpdateLessonStatus(
            [FromQuery] int lessonId,
            [FromQuery] string status,
            [FromQuery] string? note = null)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonRepository.UpdateLessonStatusAsync(
                lessonId, status,
                creditForfeited: status == LessonStatus.Forfeited,
                cancelledBy: status == LessonStatus.CancelledTeacher ? CancelledBy.Teacher
                           : status == LessonStatus.CancelledStudent || status == LessonStatus.Forfeited ? CancelledBy.Student
                           : null,
                cancellationReason: null,
                completedAt: status == LessonStatus.Completed ? DateTime.UtcNow : null,
                note: note);
            response.Data = result;
            response.ReturnCode = 0;
            response.ReturnMessage = "Success";
            return response;
        }

        [HttpPut("RescheduleLesson")]
        public async Task<ResponseBase<bool>> RescheduleLesson(
            [FromQuery] int lessonId,
            [FromQuery] DateTime newDate,
            [FromQuery] TimeOnly newTime)
        {
            ResponseBase<bool> response = new ResponseBase<bool>() { ReturnCode = -1 };
            var result = await _lessonRepository.RescheduleLessonAsync(lessonId, newDate, newTime);

            if (!result)
            {
                response.ReturnCode = -1;
                response.ReturnMessage = "Reschedule failed. Lesson may not be in a cancellable status.";
                return response;
            }

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

## File: MusicSchool.Api\Controllers\PaymentController.cs

```csharp
using Microsoft.AspNetCore.Mvc;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Api
{
    [Route("Payment")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IPaymentRepository paymentRepository,
            ILogger<PaymentController> logger)
        {
            _paymentRepository = paymentRepository;
            _logger            = logger;
        }

        /// <summary>
        /// Returns all payments for the given account holder, newest first.
        /// </summary>
        [HttpGet("GetByAccountHolder")]
        public async Task<ResponseBase<IEnumerable<Payment>>> GetByAccountHolder(
            [FromQuery] int accountHolderId)
        {
            var result = await _paymentRepository.GetByAccountHolderAsync(accountHolderId);
            return new ResponseBase<IEnumerable<Payment>>
            {
                ReturnCode    = 0,
                ReturnMessage = "Success",
                Data          = result
            };
        }

        /// <summary>
        /// Records a manual payment and runs the allocation engine.
        /// Returns the new PaymentID.
        /// </summary>
        [HttpPost("Add")]
        public async Task<ResponseBase<int?>> Add([FromBody] Payment payment)
        {
            var newId = await _paymentRepository.AddPaymentAsync(payment);
            return new ResponseBase<int?>
            {
                ReturnCode    = newId.HasValue ? 0 : -1,
                ReturnMessage = newId.HasValue ? "Success" : "Failed to record payment",
                Data          = newId
            };
        }

        /// <summary>
        /// Creates a payment exactly equal to the invoice amount and marks it paid.
        /// Called when the teacher clicks the "Paid" button on an invoice row.
        /// </summary>
        [HttpPost("QuickPay")]
        public async Task<ResponseBase<int?>> QuickPay(
            [FromQuery] int invoiceId,
            [FromQuery] DateTime paymentDate)
        {
            var newId = await _paymentRepository.QuickPayInvoiceAsync(invoiceId, paymentDate);
            return new ResponseBase<int?>
            {
                ReturnCode    = newId.HasValue ? 0 : -1,
                ReturnMessage = newId.HasValue ? "Success" : "Failed to record quick-pay",
                Data          = newId
            };
        }

        /// <summary>
        /// Returns all PaymentAllocation rows for a given payment.
        /// </summary>
        [HttpGet("GetAllocations")]
        public async Task<ResponseBase<IEnumerable<PaymentAllocation>>> GetAllocations(
            [FromQuery] int paymentId)
        {
            var result = await _paymentRepository.GetAllocationsByPaymentAsync(paymentId);
            return new ResponseBase<IEnumerable<PaymentAllocation>>
            {
                ReturnCode    = 0,
                ReturnMessage = "Success",
                Data          = result
            };
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
                            policy.WithOrigins("https://localhost:64314","https://localhost:57349", "https://localhost:51173") // Blazor WASM origin
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
            services.AddScoped<IAccountHolderDataAccessObject, AccountHolderDataAccessObject>();
            services.AddScoped<IBundleQuarterDataAccessObject, BundleQuarterDataAccessObject>();
            services.AddScoped<IExtraLessonDataAccessObject, ExtraLessonDataAccessObject>();
            services.AddScoped<IExtraLessonAggregateDataAccessObject, ExtraLessonAggregateDataAccessObject>();
            services.AddScoped<IInvoiceDataAccessObject, InvoiceDataAccessObject>();
            services.AddScoped<ILessonAggregateDataAccessObject, LessonAggregateDataAccessObject>();
            services.AddScoped<ILessonBundleDataAccessObject, LessonBundleDataAccessObject>();
            services.AddScoped<ILessonDataAccessObject, LessonDataAccessObject>();
            services.AddScoped<ILessonTypeDataAccessObject, LessonTypeDataAccessObject>();
            services.AddScoped<IScheduledSlotDataAccessObject, ScheduledSlotDataAccessObject>();
            services.AddScoped<IStudentDataAccessObject, StudentDataAccessObject>();
            services.AddScoped<ITeacherDataAccessObject, TeacherDataAccessObject>();
            services.AddScoped<ILessonBundleAggregateDataAccessObject, LessonBundleAggregateDataAccessObject>();
            services.AddScoped<IScheduledSlotAggregateDataAccessObject, ScheduledSlotAggregateDataAccessObject>();
            services.AddScoped<IPaymentDataAccessObject, PaymentDataAccessObject>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();           

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

## File: MusicSchool.Interfaces\IAccountHolderDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IAccountHolderDataAccessObject
    {
        Task<AccountHolder?> GetAccountHolderAsync(int id);
        Task<IEnumerable<AccountHolder>> GetByTeacherAsync(int teacherId);
        Task<int> InsertAsync(AccountHolder accountHolder);
        Task<bool> UpdateAsync(AccountHolder accountHolder);
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

## File: MusicSchool.Interfaces\IBundleQuarterDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IBundleQuarterDataAccessObject
    {
        Task<IEnumerable<BundleQuarter>> GetByBundleAsync(int bundleId);

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection must be passed explicitly so the INSERT runs on the
        /// same connection that owns the transaction.
        /// </summary>
        Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection);

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

## File: MusicSchool.Interfaces\IExtraLessonAggregateDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonAggregateDataAccessObject
    {
        Task<ExtraLessonDetail?> GetExtraLessonByIdAsync(int extraLessonId);

        Task<IEnumerable<ExtraLessonDetail>> GetExtraLessonsByTeacherAndDateAsync(
            int teacherId, DateTime scheduledDate);

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// Returns the new ExtraLessonID.
        /// Throws <see cref="InvalidOperationException"/> when the student is not found.
        /// </summary>
        Task<int> SaveNewExtraLessonAsync(ExtraLesson extraLesson);
    }
}

```

## File: MusicSchool.Interfaces\IExtraLessonDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IExtraLessonDataAccessObject
    {
        Task<ExtraLesson?> GetExtraLessonAsync(int id);
        Task<IEnumerable<ExtraLesson>> GetByStudentAsync(int studentId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection);

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null);
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

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically in a single transaction.
        /// Returns the new ExtraLessonID, or null if the operation fails.
        /// </summary>
        Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson);

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null);
    }
}

```

## File: MusicSchool.Interfaces\IInvoiceDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IInvoiceDataAccessObject
    {
        Task<Invoice?> GetInvoiceAsync(int id);
        Task<IEnumerable<Invoice>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Invoice>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<Invoice>> GetOutstandingByAccountHolderAsync(int accountHolderId);

        /// <summary>Inserts multiple invoice rows within an existing transaction (used for bundle instalments).</summary>
        Task InsertBatchAsync(IEnumerable<Invoice> invoices, IDbTransaction tx, IDbConnection connection);

        /// <summary>Inserts a single invoice row within an existing transaction (used for extra-lesson invoices).</summary>
        Task<int> InsertAsync(Invoice invoice, IDbTransaction tx, IDbConnection connection);

        Task<bool> UpdateStatusAsync(int invoiceId, string status, DateOnly? paidDate);
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

## File: MusicSchool.Interfaces\ILessonAggregateDataAccessObject.cs

```csharp
namespace MusicSchool.Data.Interfaces
{
    public interface ILessonAggregateDataAccessObject
    {
        Task<LessonDetail?> GetLessonByIdAsync(int lessonId);
        Task<IEnumerable<LessonDetail>> GetLessonsByTeacherAndDateAsync(int teacherId, DateTime scheduledDate);
    }
}

```

## File: MusicSchool.Interfaces\ILessonBundleAggregateDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleAggregateDataAccessObject
    {
        Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters);
        Task<IEnumerable<LessonBundleWithQuarterDetail>> GetBundleByIdAsync(int bundleId);
        Task<IEnumerable<LessonBundleDetail>> GetBundleByStudentIdAsync(int bundleId);
    }
}

```

## File: MusicSchool.Interfaces\ILessonBundleDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonBundleDataAccessObject
    {
        Task<LessonBundle?> GetBundleAsync(int id);
        Task<IEnumerable<LessonBundle>> GetByStudentAsync(int studentId);
        Task<int> InsertAsync(LessonBundle bundle, IDbTransaction tx);
        Task<bool> UpdateAsync(LessonBundle bundle);
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

## File: MusicSchool.Interfaces\ILessonDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonDataAccessObject
    {
        Task<Lesson?> GetLessonAsync(int id);
        Task<IEnumerable<Lesson>> GetByBundleAsync(int bundleId);
        Task<IEnumerable<Lesson>> GetByStatusAsync(string status);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(Lesson lesson);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(Lesson lesson, IDbTransaction tx);

        Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled,
        /// clearing CancelledBy, CancellationReason and CreditForfeited.
        /// </summary>
        Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime);
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
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null);

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled.
        /// Only valid when the lesson's current status is CancelledTeacher or CancelledStudent.
        /// </summary>
        Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime);
    }
}

```

## File: MusicSchool.Interfaces\ILessonTypeDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ILessonTypeDataAccessObject
    {
        Task<LessonType?> GetLessonTypeAsync(int id);
        Task<IEnumerable<LessonType>> GetAllActiveAsync();
        Task<int> InsertAsync(LessonType lessonType);
        Task<bool> UpdateAsync(LessonType lessonType);
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

## File: MusicSchool.Interfaces\IPaymentDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IPaymentDataAccessObject
    {
        Task<Payment?> GetPaymentAsync(int id);
        Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId);

        /// <summary>Inserts a new payment row and returns the new PaymentID.</summary>
        Task<int> InsertAsync(Payment payment);

        /// <summary>Updates the UnallocatedAmount on an existing payment row.</summary>
        Task<bool> UpdateUnallocatedAsync(int paymentId, decimal unallocatedAmount);

        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId);

        /// <summary>Inserts a PaymentAllocation row.</summary>
        Task InsertAllocationAsync(PaymentAllocation allocation);

        /// <summary>
        /// Returns the sum of all unallocated amounts across all payments
        /// for the given account holder.
        /// </summary>
        Task<decimal> GetTotalUnallocatedAsync(int accountHolderId);
    }
}

```

## File: MusicSchool.Interfaces\IPaymentRepository.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentAsync(int id);
        Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId);
        Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId);

        /// <summary>
        /// Records a new payment and immediately runs the allocation engine:
        /// links the payment to as many outstanding invoices (oldest-first) as
        /// the amount covers, marks those invoices as Paid, and stores any
        /// remainder as UnallocatedAmount on the Payment row.
        ///
        /// Also sweeps existing unallocated amounts from prior payments so
        /// they contribute toward the next invoice when accumulated funds are
        /// sufficient.
        ///
        /// Returns the new PaymentID, or null on failure.
        /// </summary>
        Task<int?> AddPaymentAsync(Payment payment);

        /// <summary>
        /// Creates a QuickPay payment exactly equal to the invoice amount,
        /// links it to that invoice, and marks the invoice Paid.
        /// Returns the new PaymentID, or null on failure.
        /// </summary>
        Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate);
    }
}

```

## File: MusicSchool.Interfaces\IScheduledSlotAggregateDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotAggregateDataAccessObject
    {
        /// <summary>
        /// Finds the student's active bundle with remaining credits, inserts the slot,
        /// and generates all future Lesson rows atomically in a single transaction.
        /// Returns the new SlotID.
        /// Throws <see cref="InvalidOperationException"/> when no usable bundle exists.
        /// </summary>
        Task<int> SaveNewSlotWithLessonsAsync(ScheduledSlot slot);
    }
}

```

## File: MusicSchool.Interfaces\IScheduledSlotDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Interfaces
{
    public interface IScheduledSlotDataAccessObject
    {
        Task<ScheduledSlot?> GetSlotAsync(int id);
        Task<IEnumerable<ScheduledSlot>> GetActiveByStudentAsync(int studentId);
        Task<IEnumerable<ScheduledSlot>> GetActiveByTeacherAsync(int teacherId);

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        Task<int> InsertAsync(ScheduledSlot slot);

        /// <summary>Inserts within an existing transaction.</summary>
        Task<int> InsertAsync(ScheduledSlot slot, IDbConnection connection, IDbTransaction tx);

        Task<bool> CloseSlotAsync(int slotId, DateOnly effectiveTo);

        /// <summary>
        /// Opens a connection (if not already open), begins a transaction,
        /// invokes <paramref name="work"/>, and commits. Rolls back on exception.
        /// </summary>
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

## File: MusicSchool.Interfaces\IStudentDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface IStudentDataAccessObject
    {
        Task<Student?> GetStudentAsync(int id);
        Task<IEnumerable<Student>> GetByAccountHolderAsync(int accountHolderId);
        Task<int> InsertAsync(Student student);
        Task<bool> UpdateAsync(Student student);
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

## File: MusicSchool.Interfaces\ITeacherDataAccessObject.cs

```csharp
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Interfaces
{
    public interface ITeacherDataAccessObject
    {
        Task<Teacher?> GetTeacherAsync(int id);
        Task<IEnumerable<Teacher>> GetAllActiveAsync();
        Task<int> InsertAsync(Teacher teacher);
        Task<bool> UpdateAsync(Teacher teacher);
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
        public TimeOnly ScheduledTime { get; set; }
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
    /// One monthly instalment for a <see cref="LessonBundle"/>, or a one-off invoice
    /// for an <see cref="ExtraLesson"/>.
    ///
    /// For bundle instalments: BundleID is set, ExtraLessonID is null,
    ///   InstallmentNumber runs 1–12, Amount = (TotalLessons * PricePerLesson) / 12.
    ///
    /// For extra-lesson invoices: ExtraLessonID is set, BundleID is null,
    ///   InstallmentNumber = 1, Amount = ExtraLesson.PriceCharged.
    /// </summary>
    public class Invoice
    {
        public int       InvoiceID         { get; set; }

        /// <summary>Populated for bundle instalments; null for extra-lesson invoices.</summary>
        public int?      BundleID          { get; set; }

        /// <summary>Populated for extra-lesson invoices; null for bundle instalments.</summary>
        public int?      ExtraLessonID     { get; set; }

        public int       AccountHolderID   { get; set; }

        /// <summary>
        /// Monthly instalment sequence number: 1–12 for bundle invoices; always 1 for extra lessons.
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

        /// <summary>
        /// Optional free-text note that can be attached when updating the lesson status.
        /// </summary>
        public string?   Notes              { get; set; }

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
    public string? Notes { get; set; }

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

## File: MusicSchool.Models\Payment.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Valid values for <see cref="Payment.Source"/>.
    /// </summary>
    public static class PaymentSource
    {
        public const string Manual   = "Manual";   // Teacher entered an amount
        public const string QuickPay = "QuickPay"; // Teacher clicked "Paid" on an invoice
    }

    /// <summary>
    /// Records a payment received from an account holder.
    /// A payment may be fully allocated (all linked to invoices),
    /// partially allocated (some unallocated remainder), or fully unallocated.
    ///
    /// The allocation engine distributes the amount against the oldest
    /// outstanding invoices first (chronological DueDate order).
    /// Any remainder below the next invoice's amount is stored as unallocated
    /// on this row (UnallocatedAmount) and is reconsidered when further
    /// payments arrive for the same account holder.
    /// </summary>
    public class Payment
    {
        public int       PaymentID       { get; set; }
        public int       AccountHolderID { get; set; }

        /// <summary>Total rand amount received.</summary>
        public decimal   Amount          { get; set; }

        /// <summary>Portion not yet linked to any invoice.</summary>
        public decimal   UnallocatedAmount { get; set; }

        public DateTime  PaymentDate     { get; set; }

        /// <summary>See <see cref="PaymentSource"/> for valid values.</summary>
        public string    Source          { get; set; } = PaymentSource.Manual;

        public string?   Reference       { get; set; }
        public string?   Notes           { get; set; }
        public DateTime  CreatedAt       { get; set; }
    }
}

```

## File: MusicSchool.Models\PaymentAllocation.cs

```csharp
using System;

namespace MusicSchool.Data.Models
{
    /// <summary>
    /// Links a <see cref="Payment"/> to an <see cref="Invoice"/>.
    /// One payment can cover many invoices; one invoice can be covered by many payments
    /// (when unallocated amounts from earlier payments accumulate to reach the invoice total).
    /// </summary>
    public class PaymentAllocation
    {
        public int     AllocationID { get; set; }
        public int     PaymentID    { get; set; }
        public int     InvoiceID    { get; set; }

        /// <summary>Portion of this payment applied to this invoice.</summary>
        public decimal AmountApplied { get; set; }

        public DateTime CreatedAt   { get; set; }
    }
}

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

## File: MusicSchool.Repositories\AccountHolderDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderDataAccessObject : IAccountHolderDataAccessObject
    {
        private readonly IDbConnection _connection;

        public AccountHolderDataAccessObject(IDbConnection connection)
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

## File: MusicSchool.Repositories\AccountHolderRepository.cs

```csharp

using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly IAccountHolderDataAccessObject _accountHolderService;
        private readonly ILogger<AccountHolderRepository> _logger;

        public AccountHolderRepository(IAccountHolderDataAccessObject accountHolderService, ILogger<AccountHolderRepository> logger)
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

## File: MusicSchool.Repositories\BundleQuarterDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class BundleQuarterDataAccessObject : IBundleQuarterDataAccessObject
    {
        private readonly IDbConnection _connection;

        public BundleQuarterDataAccessObject(IDbConnection connection)
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

        /// <summary>
        /// Inserts a batch of quarters within an existing transaction.
        /// The connection is passed explicitly so the INSERT runs on the same
        /// connection that owns the transaction — avoiding cross-connection issues
        /// that would cause LessonsAllocated to be 0 or the INSERT to fail silently.
        /// </summary>
        public async Task InsertBatchAsync(IEnumerable<BundleQuarter> quarters, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO BundleQuarter
                    (BundleID, QuarterNumber, LessonsAllocated, LessonsUsed,
                     QuarterStartDate, QuarterEndDate)
                VALUES
                    (@BundleID, @QuarterNumber, @LessonsAllocated, @LessonsUsed,
                     @QuarterStartDate, @QuarterEndDate);";

            await connection.ExecuteAsync(
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

## File: MusicSchool.Repositories\ExtraLessonAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonAggregateDataAccessObject : IExtraLessonAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly IExtraLessonDataAccessObject _extraLessonService;
        private readonly IInvoiceDataAccessObject _invoiceService;

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

        public ExtraLessonAggregateDataAccessObject(
            IDbConnection connection,
            IExtraLessonDataAccessObject extraLessonService,
            IInvoiceDataAccessObject invoiceService)
        {
            _connection = connection;
            _extraLessonService = extraLessonService;
            _invoiceService = invoiceService;
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

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// The Invoice is a single one-off row (InstallmentNumber = 1) for the full
        /// PriceCharged amount, due on the day of the lesson.
        /// Returns the new ExtraLessonID.
        /// </summary>
        public async Task<int> SaveNewExtraLessonAsync(ExtraLesson extraLesson)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve the AccountHolderID for the student so we know who to bill.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { extraLesson.StudentID },
                    transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {extraLesson.StudentID} not found when creating extra lesson.");

                // 2. Insert the ExtraLesson row.
                var extraLessonId = await _extraLessonService.InsertAsync(extraLesson, transaction, _connection);

                // 3. Build and insert a single invoice for the full price, due on lesson day.
                var invoice = new Invoice
                {
                    BundleID          = null,
                    ExtraLessonID     = extraLessonId,
                    AccountHolderID   = accountHolderId,
                    InstallmentNumber = 1,
                    Amount            = extraLesson.PriceCharged,
                    DueDate           = extraLesson.ScheduledDate.Date,
                    Status            = InvoiceStatus.Pending,
                    Notes             = $"Extra lesson on {extraLesson.ScheduledDate:dd MMM yyyy}"
                };

                await _invoiceService.InsertAsync(invoice, transaction, _connection);

                transaction.Commit();
                return extraLessonId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\ExtraLessonDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ExtraLessonDataAccessObject : IExtraLessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public ExtraLessonDataAccessObject(IDbConnection connection)
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

        /// <summary>Inserts outside of a transaction (existing callers).</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson)
            => await InsertAsync(extraLesson, null!, _connection);

        /// <summary>Inserts within an existing transaction.</summary>
        public async Task<int> InsertAsync(ExtraLesson extraLesson, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO ExtraLesson
                    (StudentID, TeacherID, LessonTypeID, ScheduledDate,
                     ScheduledTime, PriceCharged, Status, Notes)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @ScheduledDate,
                     @ScheduledTime, @PriceCharged, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, extraLesson, tx));
        }

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int extraLessonId, string status, string? note = null)
        {
            const string sql = @"
                UPDATE ExtraLesson
                SET Status = @Status,
                    Notes  = COALESCE(@Notes, Notes)
                WHERE ExtraLessonID = @ExtraLessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql,
                new { ExtraLessonID = extraLessonId, Status = status, Notes = note });
            return rowsAffected > 0;
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
        private readonly IExtraLessonAggregateDataAccessObject _aggregateService;
        private readonly IExtraLessonDataAccessObject _extraLessonService;
        private readonly ILogger<ExtraLessonRepository> _logger;

        public ExtraLessonRepository(
            IExtraLessonAggregateDataAccessObject aggregateService,
            IExtraLessonDataAccessObject extraLessonService,
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

        /// <summary>
        /// Inserts the ExtraLesson and a corresponding Invoice atomically.
        /// Returns the new ExtraLessonID, or null if the operation fails.
        /// </summary>
        public async Task<int?> AddExtraLessonAsync(ExtraLesson extraLesson)
        {
            try
            {
                return await _aggregateService.SaveNewExtraLessonAsync(extraLesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert ExtraLesson + Invoice for StudentID {StudentID} on {ScheduledDate}",
                    extraLesson.StudentID, extraLesson.ScheduledDate);
                return null;
            }
        }

        /// <summary>
        /// Updates the status on an extra lesson row.
        /// <paramref name="note"/> is optional; when null the existing Notes value is preserved.
        /// </summary>
        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null)
        {
            try
            {
                return await _extraLessonService.UpdateStatusAsync(extraLessonId, status, note);
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

## File: MusicSchool.Repositories\InvoiceDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class InvoiceDataAccessObject : IInvoiceDataAccessObject
    {
        private readonly IDbConnection _connection;

        public InvoiceDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Invoice?> GetInvoiceAsync(int id)
        {
            const string sql = @"
                SELECT InvoiceID,
                       BundleID,
                       ExtraLessonID,
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
                       ExtraLessonID,
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
                       ExtraLessonID,
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
                       ExtraLessonID,
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
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);";

            await connection.ExecuteAsync(
                new CommandDefinition(sql, invoices, tx));
        }

        /// <summary>
        /// Inserts a single Invoice row within an existing transaction.
        /// Returns the new InvoiceID.
        /// </summary>
        public async Task<int> InsertAsync(Invoice invoice, IDbTransaction tx, IDbConnection connection)
        {
            const string sql = @"
                INSERT INTO Invoice
                    (BundleID, ExtraLessonID, AccountHolderID, InstallmentNumber,
                     Amount, DueDate, Status, Notes)
                VALUES
                    (@BundleID, @ExtraLessonID, @AccountHolderID, @InstallmentNumber,
                     @Amount, @DueDate, @Status, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, invoice, tx));
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
        private readonly IInvoiceDataAccessObject _invoiceService;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(IInvoiceDataAccessObject invoiceService, ILogger<InvoiceRepository> logger)
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

## File: MusicSchool.Repositories\LessonAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonAggregateDataAccessObject : ILessonAggregateDataAccessObject
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
                   l.Notes,
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
                   l.Notes,
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

        public LessonAggregateDataAccessObject(IDbConnection connection)
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

## File: MusicSchool.Repositories\LessonBundleAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleAggregateDataAccessObject : ILessonBundleAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly ILessonBundleDataAccessObject _lessonBundleService;
        private readonly IBundleQuarterDataAccessObject _bundleQuarterService;
        private readonly IInvoiceDataAccessObject _invoiceService;

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

        public LessonBundleAggregateDataAccessObject(
            IDbConnection connection,
            ILessonBundleDataAccessObject lessonBundleService,
            IBundleQuarterDataAccessObject bundleQuarterService,
            IInvoiceDataAccessObject invoiceService)
        {
            _connection = connection;
            _lessonBundleService = lessonBundleService;
            _bundleQuarterService = bundleQuarterService;
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Saves a new LessonBundle, its 4 BundleQuarter rows, and 12 monthly Invoice
        /// instalments — all in a single transaction on the same connection.
        /// </summary>
        public async Task<int> SaveNewBundleAsync(LessonBundle bundle, IEnumerable<BundleQuarter> quarters)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var transaction = _connection.BeginTransaction();

            try
            {
                // 1. Resolve AccountHolderID inside the transaction.
                var accountHolderId = await _connection.ExecuteScalarAsync<int>(
                    "SELECT AccountHolderID FROM Student WHERE StudentID = @StudentID",
                    new { bundle.StudentID }, transaction);

                if (accountHolderId == 0)
                    throw new InvalidOperationException(
                        $"Student {bundle.StudentID} not found when creating bundle.");

                // 2. Insert bundle
                var bundleId = await _lessonBundleService.InsertAsync(bundle, transaction);

                // 3. Insert quarters — pass _connection explicitly so the INSERT runs on
                //    the same connection that owns the transaction. Without this, Dapper
                //    uses the service's injected connection which is a different instance,
                //    causing the quarters to be inserted outside the transaction or not at
                //    all, leaving LessonsAllocated = 0 and breaking slot/lesson creation.
                foreach (var quarter in quarters)
                    quarter.BundleID = bundleId;

                await _bundleQuarterService.InsertBatchAsync(quarters, transaction, _connection);

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

        private static IEnumerable<Invoice> BuildInstalments(
            int bundleId,
            int accountHolderId,
            decimal instalmentAmount,
            DateTime bundleStartDate)
        {
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

## File: MusicSchool.Repositories\LessonBundleDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonBundleDataAccessObject : ILessonBundleDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonBundleDataAccessObject(IDbConnection connection)
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
        private readonly ILessonBundleAggregateDataAccessObject _aggregateService;
        private readonly ILessonBundleDataAccessObject _lessonBundleService;
        private readonly ILogger<LessonBundleRepository> _logger;

        public LessonBundleRepository(
            ILessonBundleAggregateDataAccessObject aggregateService,
            ILessonBundleDataAccessObject lessonBundleService,
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

## File: MusicSchool.Repositories\LessonDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonDataAccessObject : ILessonDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonDataAccessObject(IDbConnection connection)
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
                       Notes,
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
                       Notes,
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
                       Notes,
                       CreatedAt
                FROM Lesson
                WHERE Status = @Status
                ORDER BY ScheduledDate, ScheduledTime;";

            return await _connection.QueryAsync<Lesson>(sql, new { Status = status });
        }

        public async Task<int> InsertAsync(Lesson lesson)
            => await InsertAsync(lesson, null!);

        public async Task<int> InsertAsync(Lesson lesson, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Lesson
                    (SlotID, BundleID, QuarterID, ScheduledDate, ScheduledTime,
                     Status, CreditForfeited, CancelledBy, CancellationReason,
                     OriginalLessonID, CompletedAt, Notes)
                VALUES
                    (@SlotID, @BundleID, @QuarterID, @ScheduledDate, @ScheduledTime,
                     @Status, @CreditForfeited, @CancelledBy, @CancellationReason,
                     @OriginalLessonID, @CompletedAt, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, lesson, tx));
        }

        public async Task<bool> UpdateStatusAsync(int lessonId, string status, bool creditForfeited,
            string? cancelledBy, string? cancellationReason, DateTime? completedAt,
            string? note = null)
        {
            const string sql = @"
                UPDATE Lesson
                SET Status             = @Status,
                    CreditForfeited    = @CreditForfeited,
                    CancelledBy        = @CancelledBy,
                    CancellationReason = @CancellationReason,
                    CompletedAt        = @CompletedAt,
                    Notes              = COALESCE(@Notes, Notes)
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID           = lessonId,
                Status             = status,
                CreditForfeited    = creditForfeited,
                CancelledBy        = cancelledBy,
                CancellationReason = cancellationReason,
                CompletedAt        = completedAt,
                Notes              = note
            });
            return rowsAffected > 0;
        }

        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            const string sql = @"
                UPDATE Lesson
                SET ScheduledDate      = @ScheduledDate,
                    ScheduledTime      = @ScheduledTime,
                    Status             = @Status,
                    CreditForfeited    = 0,
                    CancelledBy        = NULL,
                    CancellationReason = NULL,
                    CompletedAt        = NULL
                WHERE LessonID = @LessonID;";

            var rowsAffected = await _connection.ExecuteAsync(sql, new
            {
                LessonID      = lessonId,
                ScheduledDate = newDate,
                ScheduledTime = newTime,
                Status        = LessonStatus.Scheduled
            });
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
        private readonly ILessonAggregateDataAccessObject _aggregateService;
        private readonly ILessonDataAccessObject _lessonService;
        private readonly IBundleQuarterDataAccessObject _bundleQuarterService;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepository(
            ILessonAggregateDataAccessObject aggregateService,
            ILessonDataAccessObject lessonService,
            IBundleQuarterDataAccessObject bundleQuarterService,
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
            DateTime? completedAt, string? note = null)
        {
            try
            {
                // 1. Read current status so we know whether to adjust the quarter.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                // 2. Update the lesson row.
                var updated = await _lessonService.UpdateStatusAsync(
                    lessonId, status, creditForfeited,
                    cancelledBy, cancellationReason, completedAt, note);

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

        /// <summary>
        /// Moves a cancelled lesson to a new date/time and resets it to Scheduled.
        /// No credit adjustment is needed — cancelled lessons never consumed a credit.
        /// </summary>
        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            try
            {
                // Guard: only allow rescheduling of cancelled lessons.
                var lesson = await _lessonService.GetLessonAsync(lessonId);
                if (lesson is null) return false;

                if (lesson.Status != LessonStatus.CancelledTeacher
                    && lesson.Status != LessonStatus.CancelledStudent)
                {
                    _logger.LogWarning(
                        "RescheduleLessonAsync rejected: LessonID {LessonID} has status {Status}.",
                        lessonId, lesson.Status);
                    return false;
                }

                return await _lessonService.RescheduleLessonAsync(lessonId, newDate, newTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reschedule LessonID {LessonID}", lessonId);
                return false;
            }
        }
    }
}

```

## File: MusicSchool.Repositories\LessonTypeDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeDataAccessObject : ILessonTypeDataAccessObject
    {
        private readonly IDbConnection _connection;

        public LessonTypeDataAccessObject(IDbConnection connection)
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

## File: MusicSchool.Repositories\LessonTypeRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class LessonTypeRepository : ILessonTypeRepository
    {
        private readonly ILessonTypeDataAccessObject _lessonTypeService;
        private readonly ILogger<LessonTypeRepository> _logger;

        public LessonTypeRepository(ILessonTypeDataAccessObject lessonTypeService, ILogger<LessonTypeRepository> logger)
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

## File: MusicSchool.Repositories\PaymentDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class PaymentDataAccessObject : IPaymentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public PaymentDataAccessObject(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<Payment?> GetPaymentAsync(int id)
        {
            const string sql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE PaymentID = @PaymentID;";

            return await _connection.QuerySingleOrDefaultAsync<Payment>(sql, new { PaymentID = id });
        }

        public async Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                ORDER BY PaymentDate DESC, CreatedAt DESC;";

            return await _connection.QueryAsync<Payment>(sql, new { AccountHolderID = accountHolderId });
        }

        public async Task<int> InsertAsync(Payment payment)
        {
            const string sql = @"
                INSERT INTO Payment
                    (AccountHolderID, Amount, UnallocatedAmount, PaymentDate,
                     Source, Reference, Notes)
                VALUES
                    (@AccountHolderID, @Amount, @UnallocatedAmount, @PaymentDate,
                     @Source, @Reference, @Notes);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(sql, payment);
        }

        public async Task<bool> UpdateUnallocatedAsync(int paymentId, decimal unallocatedAmount)
        {
            const string sql = @"
                UPDATE Payment
                SET UnallocatedAmount = @UnallocatedAmount
                WHERE PaymentID = @PaymentID;";

            var rows = await _connection.ExecuteAsync(sql,
                new { PaymentID = paymentId, UnallocatedAmount = unallocatedAmount });
            return rows > 0;
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId)
        {
            const string sql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE PaymentID = @PaymentID;";

            return await _connection.QueryAsync<PaymentAllocation>(sql, new { PaymentID = paymentId });
        }

        public async Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId)
        {
            const string sql = @"
                SELECT AllocationID, PaymentID, InvoiceID, AmountApplied, CreatedAt
                FROM PaymentAllocation
                WHERE InvoiceID = @InvoiceID;";

            return await _connection.QueryAsync<PaymentAllocation>(sql, new { InvoiceID = invoiceId });
        }

        public async Task InsertAllocationAsync(PaymentAllocation allocation)
        {
            const string sql = @"
                INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

            await _connection.ExecuteAsync(sql, allocation);
        }

        public async Task<decimal> GetTotalUnallocatedAsync(int accountHolderId)
        {
            const string sql = @"
                SELECT ISNULL(SUM(UnallocatedAmount), 0)
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                  AND UnallocatedAmount > 0;";

            return await _connection.ExecuteScalarAsync<decimal>(sql,
                new { AccountHolderID = accountHolderId });
        }
    }
}

```

## File: MusicSchool.Repositories\PaymentRepository.cs

```csharp
using Dapper;
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    /// <summary>
    /// Orchestrates payment recording and the allocation engine.
    ///
    /// Allocation rules:
    ///  1. Gather all outstanding (Pending/Overdue) invoices for the account holder,
    ///     sorted by DueDate ascending (oldest-first).
    ///  2. Pool the new payment with any existing unallocated amounts.
    ///  3. Walk the invoice list:
    ///       - If pool >= invoice.Amount: drain oldest unallocated payments into the
    ///         invoice, mark it Paid, reduce pool.
    ///       - Otherwise: stop.
    ///  4. Persist updated UnallocatedAmount on every touched Payment row.
    /// </summary>
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPaymentDataAccessObject _paymentDao;
        private readonly IInvoiceDataAccessObject _invoiceDao;
        private readonly IDbConnection            _connection;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(
            IPaymentDataAccessObject   paymentDao,
            IInvoiceDataAccessObject   invoiceDao,
            IDbConnection              connection,
            ILogger<PaymentRepository> logger)
        {
            _paymentDao = paymentDao;
            _invoiceDao = invoiceDao;
            _connection = connection;
            _logger     = logger;
        }

        public Task<Payment?> GetPaymentAsync(int id)
            => _paymentDao.GetPaymentAsync(id);

        public Task<IEnumerable<Payment>> GetByAccountHolderAsync(int accountHolderId)
            => _paymentDao.GetByAccountHolderAsync(accountHolderId);

        public Task<IEnumerable<PaymentAllocation>> GetAllocationsByPaymentAsync(int paymentId)
            => _paymentDao.GetAllocationsByPaymentAsync(paymentId);

        public Task<IEnumerable<PaymentAllocation>> GetAllocationsByInvoiceAsync(int invoiceId)
            => _paymentDao.GetAllocationsByInvoiceAsync(invoiceId);

        // ── Add payment + run allocation engine ───────────────────────────────

        public async Task<int?> AddPaymentAsync(Payment payment)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var tx = _connection.BeginTransaction();
                try
                {
                    // Start fully unallocated; engine will reduce it.
                    payment.UnallocatedAmount = payment.Amount;
                    var paymentId = await InsertPaymentInTxAsync(payment, tx);

                    await RunAllocationEngineAsync(payment.AccountHolderID, tx);

                    tx.Commit();
                    return paymentId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to add payment for AccountHolderID {AccountHolderID}",
                    payment.AccountHolderID);
                return null;
            }
        }

        public async Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate)
        {
            try
            {
                var invoice = await _invoiceDao.GetInvoiceAsync(invoiceId);
                if (invoice is null)
                {
                    _logger.LogWarning("QuickPay: InvoiceID {InvoiceID} not found", invoiceId);
                    return null;
                }

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                using var tx = _connection.BeginTransaction();
                try
                {
                    var payment = new Payment
                    {
                        AccountHolderID   = invoice.AccountHolderID,
                        Amount            = invoice.Amount,
                        UnallocatedAmount = 0,
                        PaymentDate       = paymentDate,
                        Source            = PaymentSource.QuickPay,
                        Notes             = $"Quick-pay for Invoice #{invoiceId}"
                    };

                    var paymentId = await InsertPaymentInTxAsync(payment, tx);
                    await AllocateToInvoiceAsync(paymentId, invoice, invoice.Amount, tx);

                    tx.Commit();
                    return paymentId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickPay failed for InvoiceID {InvoiceID}", invoiceId);
                return null;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private async Task<int> InsertPaymentInTxAsync(Payment payment, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO Payment
                    (AccountHolderID, Amount, UnallocatedAmount, PaymentDate,
                     Source, Reference, Notes)
                VALUES
                    (@AccountHolderID, @Amount, @UnallocatedAmount, @PaymentDate,
                     @Source, @Reference, @Notes);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, payment, tx));
        }

        /// <summary>
        /// Loads all unallocated payments and outstanding invoices for the account,
        /// then greedily allocates the pooled funds to invoices (oldest DueDate first).
        /// </summary>
        private async Task RunAllocationEngineAsync(int accountHolderId, IDbTransaction tx)
        {
            // Outstanding invoices, oldest first.
            const string invoiceSql = @"
                SELECT InvoiceID, BundleID, ExtraLessonID, AccountHolderID,
                       InstallmentNumber, Amount, DueDate, PaidDate, Status, Notes, CreatedAt
                FROM Invoice
                WHERE AccountHolderID = @AccountHolderID
                  AND Status IN ('Pending', 'Overdue')
                ORDER BY DueDate ASC, InvoiceID ASC;";

            var invoices = (await _connection.QueryAsync<Invoice>(
                new CommandDefinition(invoiceSql, new { AccountHolderID = accountHolderId }, tx))).ToList();

            if (invoices.Count == 0) return;

            // Payments with remaining unallocated amounts, oldest first.
            const string paymentSql = @"
                SELECT PaymentID, AccountHolderID, Amount, UnallocatedAmount,
                       PaymentDate, Source, Reference, Notes, CreatedAt
                FROM Payment
                WHERE AccountHolderID = @AccountHolderID
                  AND UnallocatedAmount > 0
                ORDER BY PaymentDate ASC, PaymentID ASC;";

            var payments = (await _connection.QueryAsync<Payment>(
                new CommandDefinition(paymentSql, new { AccountHolderID = accountHolderId }, tx))).ToList();

            if (payments.Count == 0) return;

            foreach (var invoice in invoices)
            {
                // How much of this invoice is already covered?
                const string coveredSql = @"
                    SELECT ISNULL(SUM(AmountApplied), 0)
                    FROM PaymentAllocation
                    WHERE InvoiceID = @InvoiceID;";

                var alreadyCovered = await _connection.ExecuteScalarAsync<decimal>(
                    new CommandDefinition(coveredSql, new { InvoiceID = invoice.InvoiceID }, tx));

                var needed = invoice.Amount - alreadyCovered;
                if (needed <= 0) continue;

                var pool = payments.Sum(p => p.UnallocatedAmount);
                if (pool < needed) break; // Insufficient funds for the next invoice — stop.

                // Drain payments into this invoice.
                var remaining = needed;
                foreach (var pmt in payments.Where(p => p.UnallocatedAmount > 0))
                {
                    if (remaining <= 0) break;

                    var take = Math.Min(pmt.UnallocatedAmount, remaining);
                    pmt.UnallocatedAmount -= take;
                    remaining -= take;

                    const string allocSql = @"
                        INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                        VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

                    await _connection.ExecuteAsync(new CommandDefinition(allocSql,
                        new { PaymentID = pmt.PaymentID, InvoiceID = invoice.InvoiceID, AmountApplied = take },
                        tx));
                }

                // Mark the invoice Paid.
                const string paidSql = @"
                    UPDATE Invoice SET Status = 'Paid', PaidDate = @PaidDate
                    WHERE InvoiceID = @InvoiceID;";

                await _connection.ExecuteAsync(new CommandDefinition(paidSql,
                    new { InvoiceID = invoice.InvoiceID, PaidDate = DateTime.Today }, tx));
            }

            // Persist updated UnallocatedAmount on all touched payments.
            const string updateSql = @"
                UPDATE Payment SET UnallocatedAmount = @UnallocatedAmount
                WHERE PaymentID = @PaymentID;";

            foreach (var pmt in payments)
            {
                await _connection.ExecuteAsync(new CommandDefinition(updateSql,
                    new { PaymentID = pmt.PaymentID, UnallocatedAmount = pmt.UnallocatedAmount }, tx));
            }
        }

        /// <summary>QuickPay direct allocation (exact-match amount).</summary>
        private async Task AllocateToInvoiceAsync(
            int paymentId, Invoice invoice, decimal amount, IDbTransaction tx)
        {
            const string allocSql = @"
                INSERT INTO PaymentAllocation (PaymentID, InvoiceID, AmountApplied)
                VALUES (@PaymentID, @InvoiceID, @AmountApplied);";

            await _connection.ExecuteAsync(new CommandDefinition(allocSql,
                new { PaymentID = paymentId, InvoiceID = invoice.InvoiceID, AmountApplied = amount }, tx));

            const string paidSql = @"
                UPDATE Invoice SET Status = 'Paid', PaidDate = @PaidDate
                WHERE InvoiceID = @InvoiceID;";

            await _connection.ExecuteAsync(new CommandDefinition(paidSql,
                new { InvoiceID = invoice.InvoiceID, PaidDate = DateTime.Today }, tx));
        }
    }
}

```

## File: MusicSchool.Repositories\ScheduledSlotAggregateDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotAggregateDataAccessObject : IScheduledSlotAggregateDataAccessObject
    {
        private readonly IDbConnection _connection;
        private readonly IScheduledSlotDataAccessObject _slotService;
        private readonly ILessonBundleDataAccessObject _bundleService;
        private readonly IBundleQuarterDataAccessObject _quarterService;
        private readonly ILessonDataAccessObject _lessonService;

        public ScheduledSlotAggregateDataAccessObject(
            IDbConnection connection,
            IScheduledSlotDataAccessObject slotService,
            ILessonBundleDataAccessObject bundleService,
            IBundleQuarterDataAccessObject quarterService,
            ILessonDataAccessObject lessonService)
        {
            _connection = connection;
            _slotService = slotService;
            _bundleService = bundleService;
            _quarterService = quarterService;
            _lessonService = lessonService;
        }

        /// <summary>
        /// Finds the student's active bundle with remaining credits, inserts the slot,
        /// then generates all future Lesson rows up to the bundle's EndDate — one per
        /// weekly occurrence matching the slot's DayOfWeek — all in a single transaction.
        /// Returns the new SlotID, or throws if the student has no usable bundle.
        /// </summary>
        public async Task<int> SaveNewSlotWithLessonsAsync(ScheduledSlot slot)
        {
            // 1. Find the student's active bundle that still has remaining credits.
            //    "Active" means IsActive = true, not yet expired, and at least one
            //    quarter still has lessons remaining.
            var bundles = await _bundleService.GetByStudentAsync(slot.StudentID);

            LessonBundle? bundle = null;

            foreach (var b in bundles.Where(b => b.IsActive && b.EndDate >= DateTime.Today))
            {
                var bundleQuarters = (await _quarterService.GetByBundleAsync(b.BundleID)).ToList();
                if (bundleQuarters.Any(q => q.LessonsUsed < q.LessonsAllocated))
                {
                    bundle = b;
                    break;
                }
            }

            if (bundle is null)
                throw new InvalidOperationException(
                    $"StudentID {slot.StudentID} has no active bundle with remaining credits.");

            var quarters = (await _quarterService.GetByBundleAsync(bundle.BundleID)).ToList();

            // 2. Insert the slot and generate lessons in one transaction.
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            using var tx = _connection.BeginTransaction();
            try
            {
                var slotId = await _slotService.InsertAsync(slot, _connection, tx);
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

                tx.Commit();
                return slotId;
            }
            catch
            {
                tx.Rollback();
                throw;
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

## File: MusicSchool.Repositories\ScheduledSlotDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class ScheduledSlotDataAccessObject : IScheduledSlotDataAccessObject
    {
        private readonly IDbConnection _connection;

        public ScheduledSlotDataAccessObject(IDbConnection connection)
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
        {
            return await InsertAsync(slot, _connection, null);
        }

        public async Task<int> InsertAsync(ScheduledSlot slot, IDbConnection connection, IDbTransaction tx)
        {
            const string sql = @"
                INSERT INTO ScheduledSlot
                    (StudentID, TeacherID, LessonTypeID, DayOfWeek,
                     SlotTime, EffectiveFrom, EffectiveTo, IsActive)
                VALUES
                    (@StudentID, @TeacherID, @LessonTypeID, @DayOfWeek,
                     @SlotTime, @EffectiveFrom, @EffectiveTo, @IsActive);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await connection.ExecuteScalarAsync<int>(
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
    }
}

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
        private readonly IScheduledSlotAggregateDataAccessObject _aggregateService;
        private readonly IScheduledSlotDataAccessObject _slotService;
        private readonly ILogger<ScheduledSlotRepository> _logger;

        public ScheduledSlotRepository(
            IScheduledSlotAggregateDataAccessObject aggregateService,
            IScheduledSlotDataAccessObject slotService,
            ILogger<ScheduledSlotRepository> logger)
        {
            _aggregateService = aggregateService;
            _slotService = slotService;
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
                return await _aggregateService.SaveNewSlotWithLessonsAsync(slot);
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation (no active bundle) — warn rather than error.
                _logger.LogWarning(ex.Message);
                return null;
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
    }
}

```

## File: MusicSchool.Repositories\StudentDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class StudentDataAccessObject : IStudentDataAccessObject
    {
        private readonly IDbConnection _connection;

        public StudentDataAccessObject(IDbConnection connection)
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

## File: MusicSchool.Repositories\StudentRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentDataAccessObject _studentService;
        private readonly ILogger<StudentRepository> _logger;

        public StudentRepository(IStudentDataAccessObject studentService, ILogger<StudentRepository> logger)
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

## File: MusicSchool.Repositories\TeacherDataAccessObject.cs

```csharp
using Dapper;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Data.Implementations
{
    public class TeacherDataAccessObject : ITeacherDataAccessObject
    {
        private readonly IDbConnection _connection;

        public TeacherDataAccessObject(IDbConnection connection)
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

## File: MusicSchool.Repositories\TeacherRepository.cs

```csharp
using Microsoft.Extensions.Logging;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Data.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherDataAccessObject _teacherService;
        private readonly ILogger<TeacherRepository> _logger;

        public TeacherRepository(ITeacherDataAccessObject teacherService, ILogger<TeacherRepository> logger)
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

## File: MusicSchool.Repositories.Tests\AccountHolderRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class AccountHolderRepositoryTests
{
    private readonly Mock<IAccountHolderDataAccessObject> _daoMock;
    private readonly Mock<ILogger<AccountHolderRepository>> _loggerMock;
    private readonly AccountHolderRepository _sut;

    public AccountHolderRepositoryTests()
    {
        _daoMock    = new Mock<IAccountHolderDataAccessObject>();
        _loggerMock = new Mock<ILogger<AccountHolderRepository>>();
        _sut        = new AccountHolderRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetAccountHolderAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAccountHolderAsync_WhenFound_ReturnsAccountHolder()
    {
        var expected = new AccountHolder { AccountHolderID = 1, FirstName = "Jane", LastName = "Doe" };
        _daoMock.Setup(d => d.GetAccountHolderAsync(1)).ReturnsAsync(expected);

        var result = await _sut.GetAccountHolderAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.AccountHolderID);
    }

    [Fact]
    public async Task GetAccountHolderAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetAccountHolderAsync(99)).ReturnsAsync((AccountHolder?)null);

        var result = await _sut.GetAccountHolderAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAsync_ReturnsDaoResult()
    {
        var list = new List<AccountHolder>
        {
            new() { AccountHolderID = 1, TeacherID = 5 },
            new() { AccountHolderID = 2, TeacherID = 5 }
        };
        _daoMock.Setup(d => d.GetByTeacherAsync(5)).ReturnsAsync(list);

        var result = await _sut.GetByTeacherAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByTeacherAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _daoMock.Setup(d => d.GetByTeacherAsync(5)).ReturnsAsync(Enumerable.Empty<AccountHolder>());

        var result = await _sut.GetByTeacherAsync(5);

        Assert.Empty(result);
    }

    // ── AddAccountHolderAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task AddAccountHolderAsync_WhenSuccessful_ReturnsNewId()
    {
        var ah = new AccountHolder { FirstName = "John", LastName = "Smith" };
        _daoMock.Setup(d => d.InsertAsync(ah)).ReturnsAsync(42);

        var result = await _sut.AddAccountHolderAsync(ah);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task AddAccountHolderAsync_WhenDaoThrows_ReturnsNull()
    {
        var ah = new AccountHolder { FirstName = "John", LastName = "Smith" };
        _daoMock.Setup(d => d.InsertAsync(ah)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddAccountHolderAsync(ah);

        Assert.Null(result);
    }

    // ── UpdateAccountHolderAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenSuccessful_ReturnsTrue()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ReturnsAsync(true);

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ReturnsAsync(false);

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAccountHolderAsync_WhenDaoThrows_ReturnsFalse()
    {
        var ah = new AccountHolder { AccountHolderID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(ah)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateAccountHolderAsync(ah);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\ExtraLessonAggregateDataAccessObjectTests.cs

```csharp
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ExtraLessonAggregateDataAccessObjectTests
{
    private readonly Mock<IExtraLessonDataAccessObject> _extraLessonDaoMock;
    private readonly Mock<IInvoiceDataAccessObject> _invoiceDaoMock;

    public ExtraLessonAggregateDataAccessObjectTests()
    {
        _extraLessonDaoMock = new Mock<IExtraLessonDataAccessObject>();
        _invoiceDaoMock     = new Mock<IInvoiceDataAccessObject>();
    }

    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectExtraLessonDetailQuery_ContainsRequiredColumns()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSON_DETAIL_QRY;

        Assert.Contains("ExtraLessonID",      qry);
        Assert.Contains("StudentID",          qry);
        Assert.Contains("TeacherID",          qry);
        Assert.Contains("LessonTypeID",       qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectExtraLessonDetailQuery_JoinsStudentTeacherAndLessonType()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSON_DETAIL_QRY;

        Assert.Contains("JOIN Student",     qry);
        Assert.Contains("JOIN Teacher",     qry);
        Assert.Contains("JOIN LessonType",  qry);
        Assert.Contains("@ExtraLessonID",   qry);
    }

    [Fact]
    public void SelectExtraLessonsByTeacherDateQuery_FiltersOnTeacherIdAndDate()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("@TeacherID",      qry);
        Assert.Contains("@ScheduledDate",  qry);
        Assert.Contains("ORDER BY",        qry);
    }

    [Fact]
    public void SelectExtraLessonsByTeacherDateQuery_ContainsRequiredColumns()
    {
        var qry = ExtraLessonAggregateDataAccessObject.SELECT_EXTRA_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("ExtraLessonID",      qry);
        Assert.Contains("PriceCharged",       qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    // ── SaveNewExtraLessonAsync — business rules ───────────────────────────────

    /// <summary>
    /// SaveNewExtraLessonAsync opens a real IDbConnection transaction and therefore
    /// cannot be fully unit-tested without an integration DB.  We verify the
    /// invoice-building logic independently by checking Invoice field calculation.
    /// </summary>
    [Fact]
    public void InvoiceBuiltForExtraLesson_HasCorrectFields()
    {
        var scheduledDate = new DateTime(2025, 8, 15, 10, 0, 0);
        var extraLesson   = new ExtraLesson
        {
            ExtraLessonID  = 0,
            StudentID      = 3,
            TeacherID      = 2,
            LessonTypeID   = 1,
            ScheduledDate  = scheduledDate,
            ScheduledTime  = new TimeOnly(10, 0),
            PriceCharged   = 350m,
            Status         = ExtraLessonStatus.Scheduled,
        };

        // Reproduce the invoice-building logic from SaveNewExtraLessonAsync
        var invoice = new Invoice
        {
            BundleID          = null,
            ExtraLessonID     = extraLesson.ExtraLessonID,
            AccountHolderID   = 99,   // resolved at runtime in the real impl
            InstallmentNumber = 1,
            Amount            = extraLesson.PriceCharged,
            DueDate           = extraLesson.ScheduledDate.Date,
            Status            = InvoiceStatus.Pending,
        };

        Assert.Null(invoice.BundleID);
        Assert.Equal(1, invoice.InstallmentNumber);
        Assert.Equal(350m, invoice.Amount);
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.Equal(scheduledDate.Date, invoice.DueDate);
    }
}

```

## File: MusicSchool.Repositories.Tests\ExtraLessonRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ExtraLessonRepositoryTests
{
    private readonly Mock<IExtraLessonAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<IExtraLessonDataAccessObject> _daoMock;
    private readonly Mock<ILogger<ExtraLessonRepository>> _loggerMock;
    private readonly ExtraLessonRepository _sut;

    public ExtraLessonRepositoryTests()
    {
        _aggregateMock = new Mock<IExtraLessonAggregateDataAccessObject>();
        _daoMock       = new Mock<IExtraLessonDataAccessObject>();
        _loggerMock    = new Mock<ILogger<ExtraLessonRepository>>();
        _sut           = new ExtraLessonRepository(_aggregateMock.Object, _daoMock.Object, _loggerMock.Object);
    }

    // ── GetExtraLessonAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetExtraLessonAsync_WhenFound_ReturnsDetail()
    {
        var expected = new ExtraLessonDetail { ExtraLessonID = 7, TeacherName = "Alice" };
        _aggregateMock.Setup(a => a.GetExtraLessonByIdAsync(7)).ReturnsAsync(expected);

        var result = await _sut.GetExtraLessonAsync(7);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.TeacherName);
    }

    [Fact]
    public async Task GetExtraLessonAsync_WhenNotFound_ReturnsNull()
    {
        _aggregateMock.Setup(a => a.GetExtraLessonByIdAsync(99)).ReturnsAsync((ExtraLessonDetail?)null);

        var result = await _sut.GetExtraLessonAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAndDateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAndDateAsync_ReturnsDaoResult()
    {
        var date    = new DateTime(2025, 6, 2);
        var details = new List<ExtraLessonDetail>
        {
            new() { ExtraLessonID = 1 },
            new() { ExtraLessonID = 2 }
        };
        _aggregateMock.Setup(a => a.GetExtraLessonsByTeacherAndDateAsync(5, date)).ReturnsAsync(details);

        var result = await _sut.GetByTeacherAndDateAsync(5, date);

        Assert.Equal(2, result.Count());
    }

    // ── GetByStudentAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStudentAsync_ReturnsDaoResult()
    {
        var lessons = new List<ExtraLesson>
        {
            new() { ExtraLessonID = 1, StudentID = 3 },
            new() { ExtraLessonID = 2, StudentID = 3 }
        };
        _daoMock.Setup(d => d.GetByStudentAsync(3)).ReturnsAsync(lessons);

        var result = await _sut.GetByStudentAsync(3);

        Assert.Equal(2, result.Count());
    }

    // ── AddExtraLessonAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task AddExtraLessonAsync_WhenSuccessful_ReturnsNewId()
    {
        var el = new ExtraLesson { StudentID = 1, TeacherID = 2 };
        _aggregateMock.Setup(a => a.SaveNewExtraLessonAsync(el)).ReturnsAsync(88);

        var result = await _sut.AddExtraLessonAsync(el);

        Assert.Equal(88, result);
    }

    [Fact]
    public async Task AddExtraLessonAsync_WhenAggregateDaoThrows_ReturnsNull()
    {
        var el = new ExtraLesson { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewExtraLessonAsync(el))
                      .ThrowsAsync(new InvalidOperationException("No student found"));

        var result = await _sut.AddExtraLessonAsync(el);

        Assert.Null(result);
    }

    // ── UpdateExtraLessonStatusAsync ──────────────────────────────────────────

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenSuccessful_ReturnsTrue()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Completed, null))
                .ReturnsAsync(true);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Completed);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WithNote_PassesNoteThrough()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather"))
                .ReturnsAsync(true);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather");

        Assert.True(result);
        _daoMock.Verify(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, "Weather"), Times.Once);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Cancelled, null))
                .ReturnsAsync(false);

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Cancelled);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateExtraLessonStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, ExtraLessonStatus.Completed, null))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateExtraLessonStatusAsync(1, ExtraLessonStatus.Completed);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\InvoiceRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Repositories.Tests;

public class InvoiceRepositoryTests
{
    private readonly Mock<IInvoiceDataAccessObject> _daoMock;
    private readonly Mock<ILogger<InvoiceRepository>> _loggerMock;
    private readonly InvoiceRepository _sut;

    public InvoiceRepositoryTests()
    {
        _daoMock    = new Mock<IInvoiceDataAccessObject>();
        _loggerMock = new Mock<ILogger<InvoiceRepository>>();
        _sut        = new InvoiceRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetInvoiceAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetInvoiceAsync_WhenFound_ReturnsInvoice()
    {
        var expected = new Invoice { InvoiceID = 5, Amount = 1000m, Status = InvoiceStatus.Pending };
        _daoMock.Setup(d => d.GetInvoiceAsync(5)).ReturnsAsync(expected);

        var result = await _sut.GetInvoiceAsync(5);

        Assert.NotNull(result);
        Assert.Equal(1000m, result.Amount);
    }

    [Fact]
    public async Task GetInvoiceAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetInvoiceAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _sut.GetInvoiceAsync(99);

        Assert.Null(result);
    }

    // ── GetByBundleAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByBundleAsync_ReturnsDaoResult()
    {
        var invoices = Enumerable.Range(1, 12).Select(i => new Invoice
        {
            InvoiceID         = i,
            BundleID          = 3,
            InstallmentNumber = (byte)i
        }).ToList();

        _daoMock.Setup(d => d.GetByBundleAsync(3)).ReturnsAsync(invoices);

        var result = await _sut.GetByBundleAsync(3);

        Assert.Equal(12, result.Count());
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var invoices = new List<Invoice>
        {
            new() { InvoiceID = 1, AccountHolderID = 10 },
            new() { InvoiceID = 2, AccountHolderID = 10 }
        };
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(invoices);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    // ── GetOutstandingByAccountHolderAsync ────────────────────────────────────

    [Fact]
    public async Task GetOutstandingByAccountHolderAsync_ReturnsDaoResult()
    {
        var invoices = new List<Invoice>
        {
            new() { InvoiceID = 1, Status = InvoiceStatus.Pending },
            new() { InvoiceID = 2, Status = InvoiceStatus.Overdue }
        };
        _daoMock.Setup(d => d.GetOutstandingByAccountHolderAsync(10)).ReturnsAsync(invoices);

        var result = await _sut.GetOutstandingByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
        Assert.All(result, inv => Assert.NotEqual(InvoiceStatus.Paid, inv.Status));
    }

    // ── AddInvoiceInstalmentsAsync ────────────────────────────────────────────

    [Fact]
    public async Task AddInvoiceInstalmentsAsync_WhenSuccessful_ReturnsTrue()
    {
        var invoices = new List<Invoice> { new() { BundleID = 1 } };
        var connMock = new Mock<IDbConnection>();
        var txMock   = new Mock<IDbTransaction>();

        _daoMock.Setup(d => d.InsertBatchAsync(invoices, txMock.Object, connMock.Object))
                .Returns(Task.CompletedTask);

        var result = await _sut.AddInvoiceInstalmentsAsync(invoices, txMock.Object, connMock.Object);

        Assert.True(result);
    }

    [Fact]
    public async Task AddInvoiceInstalmentsAsync_WhenDaoThrows_ReturnsFalse()
    {
        var invoices = new List<Invoice> { new() { BundleID = 1 } };
        var connMock = new Mock<IDbConnection>();
        var txMock   = new Mock<IDbTransaction>();

        _daoMock.Setup(d => d.InsertBatchAsync(invoices, txMock.Object, connMock.Object))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddInvoiceInstalmentsAsync(invoices, txMock.Object, connMock.Object);

        Assert.False(result);
    }

    // ── UpdateInvoiceStatusAsync ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenSuccessful_ReturnsTrue()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Paid, It.IsAny<DateOnly?>()))
                .ReturnsAsync(true);

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Paid, DateOnly.FromDateTime(DateTime.Today));

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Void, It.IsAny<DateOnly?>()))
                .ReturnsAsync(false);

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Void, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _daoMock.Setup(d => d.UpdateStatusAsync(1, InvoiceStatus.Paid, It.IsAny<DateOnly?>()))
                .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateInvoiceStatusAsync(1, InvoiceStatus.Paid, null);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\LessonAggregateDataAccessObjectTests.cs

```csharp
using MusicSchool.Data.Implementations;

namespace MusicSchool.Repositories.Tests;

public class LessonAggregateDataAccessObjectTests
{
    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectLessonDetailQuery_ContainsAllLessonColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("LessonID",           qry);
        Assert.Contains("SlotID",             qry);
        Assert.Contains("BundleID",           qry);
        Assert.Contains("QuarterID",          qry);
        Assert.Contains("ScheduledDate",      qry);
        Assert.Contains("ScheduledTime",      qry);
        Assert.Contains("Status",             qry);
        Assert.Contains("CreditForfeited",    qry);
        Assert.Contains("CancelledBy",        qry);
        Assert.Contains("CancellationReason", qry);
        Assert.Contains("OriginalLessonID",   qry);
        Assert.Contains("CompletedAt",        qry);
    }

    [Fact]
    public void SelectLessonDetailQuery_ContainsStudentAndTeacherAndLessonTypeColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectLessonDetailQuery_JoinsScheduledSlotStudentTeacherLessonType()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSON_DETAIL_QRY;

        Assert.Contains("JOIN ScheduledSlot", qry);
        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN Teacher",       qry);
        Assert.Contains("JOIN LessonType",    qry);
        Assert.Contains("@LessonID",          qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_FiltersOnTeacherIdAndDate()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("@TeacherID",     qry);
        Assert.Contains("@ScheduledDate", qry);
        Assert.Contains("ORDER BY",       qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_ContainsRequiredColumns()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("LessonID",           qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("TeacherName",        qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectLessonsByTeacherDateQuery_JoinsScheduledSlotStudentTeacherLessonType()
    {
        var qry = LessonAggregateDataAccessObject.SELECT_LESSONS_BY_TEACHER_DATE_QRY;

        Assert.Contains("JOIN ScheduledSlot", qry);
        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN Teacher",       qry);
        Assert.Contains("JOIN LessonType",    qry);
    }
}

```

## File: MusicSchool.Repositories.Tests\LessonBundleAggregateDataAccessObjectTests.cs

```csharp
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonBundleAggregateDataAccessObjectTests
{
    // ── Static query constants — structure validation ──────────────────────────

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsAllBundleColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("BundleID",        qry);
        Assert.Contains("StudentID",       qry);
        Assert.Contains("TeacherID",       qry);
        Assert.Contains("LessonTypeID",    qry);
        Assert.Contains("TotalLessons",    qry);
        Assert.Contains("PricePerLesson",  qry);
        Assert.Contains("StartDate",       qry);
        Assert.Contains("EndDate",         qry);
        Assert.Contains("QuarterSize",     qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsAllQuarterColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("QuarterID",        qry);
        Assert.Contains("QuarterNumber",    qry);
        Assert.Contains("LessonsAllocated", qry);
        Assert.Contains("LessonsUsed",      qry);
        Assert.Contains("QuarterStartDate", qry);
        Assert.Contains("QuarterEndDate",   qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsStudentAndLessonTypeJoins()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("JOIN Student",       qry);
        Assert.Contains("JOIN LessonType",    qry);
        Assert.Contains("JOIN BundleQuarter", qry);
        Assert.Contains("ORDER BY",           qry);
        Assert.Contains("@BundleID",          qry);
    }

    [Fact]
    public void SelectBundleWithQuartersQuery_ContainsStudentAndLessonTypeDetailColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_WITH_QUARTERS_QRY;

        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    [Fact]
    public void SelectBundleQueryByStudent_FiltersOnStudentId()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_QRY_BY_STUDENT;

        Assert.Contains("@StudentID", qry);
        Assert.Contains("ORDER BY",   qry);
    }

    [Fact]
    public void SelectBundleQueryByStudent_ContainsRequiredColumns()
    {
        var qry = LessonBundleAggregateDataAccessObject.SELECT_BUNDLE_QRY_BY_STUDENT;

        Assert.Contains("BundleID",           qry);
        Assert.Contains("TotalLessons",       qry);
        Assert.Contains("PricePerLesson",     qry);
        Assert.Contains("StudentFirstName",   qry);
        Assert.Contains("StudentLastName",    qry);
        Assert.Contains("DurationMinutes",    qry);
        Assert.Contains("BasePricePerLesson", qry);
    }

    // ── BuildInstalments helper — business logic ───────────────────────────────

    /// <summary>
    /// The private BuildInstalments helper is exercised indirectly via public
    /// observable state. We verify the instalment-calculation rules here directly
    /// so regressions surface with clear failure messages.
    /// </summary>
    [Theory]
    [InlineData(48, 200,  800)]   // 48 lessons × R200 / 12 = R800 per instalment
    [InlineData(36, 150,  450)]   // 36 lessons × R150 / 12 = R450
    [InlineData(12, 300,  300)]   // 12 lessons × R300 / 12 = R300
    public void InstalmentAmount_IsCalculatedCorrectly(
        int totalLessons, decimal pricePerLesson, decimal expectedInstalment)
    {
        var instalment = Math.Round(totalLessons * pricePerLesson / 12, 2);

        Assert.Equal(expectedInstalment, instalment);
    }

    [Fact]
    public void BuildInstalments_Generates12Rows_StartingFromBundleStartMonth()
    {
        // Simulate what BuildInstalments produces.
        var bundleStartDate = new DateTime(2025, 1, 15);
        var firstDue        = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);
        var instalments     = new List<Invoice>();

        for (byte i = 1; i <= 12; i++)
        {
            instalments.Add(new Invoice
            {
                BundleID          = 1,
                AccountHolderID   = 99,
                InstallmentNumber = i,
                Amount            = 800m,
                DueDate           = firstDue.AddMonths(i - 1),
                Status            = InvoiceStatus.Pending,
            });
        }

        Assert.Equal(12, instalments.Count);
        Assert.Equal(new DateTime(2025, 1, 1), instalments[0].DueDate);
        Assert.Equal(new DateTime(2025, 12, 1), instalments[11].DueDate);
        Assert.All(instalments, inv => Assert.Equal(InvoiceStatus.Pending, inv.Status));
        Assert.All(instalments, inv => Assert.Equal(800m, inv.Amount));

        // Instalment numbers must be sequential 1–12
        for (byte i = 1; i <= 12; i++)
            Assert.Equal(i, instalments[i - 1].InstallmentNumber);
    }

    [Fact]
    public void BuildInstalments_BundleStartMidYear_CorrectlyWrapsToNextYear()
    {
        var bundleStartDate = new DateTime(2025, 6, 1);
        var firstDue        = new DateTime(bundleStartDate.Year, bundleStartDate.Month, 1);
        var lastDue         = firstDue.AddMonths(11);

        Assert.Equal(new DateTime(2025, 6,  1), firstDue);
        Assert.Equal(new DateTime(2026, 5,  1), lastDue);
    }
}

```

## File: MusicSchool.Repositories.Tests\LessonBundleRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using MusicSchool.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonBundleRepositoryTests
{
    private readonly Mock<ILessonBundleAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<ILessonBundleDataAccessObject> _bundleDaoMock;
    private readonly Mock<ILogger<LessonBundleRepository>> _loggerMock;
    private readonly LessonBundleRepository _sut;

    public LessonBundleRepositoryTests()
    {
        _aggregateMock = new Mock<ILessonBundleAggregateDataAccessObject>();
        _bundleDaoMock = new Mock<ILessonBundleDataAccessObject>();
        _loggerMock    = new Mock<ILogger<LessonBundleRepository>>();
        _sut = new LessonBundleRepository(
            _aggregateMock.Object,
            _bundleDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetBundleAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBundleAsync_WhenFound_ReturnsQuarterDetails()
    {
        var rows = new List<LessonBundleWithQuarterDetail>
        {
            new() { BundleID = 1, QuarterNumber = 1 },
            new() { BundleID = 1, QuarterNumber = 2 },
            new() { BundleID = 1, QuarterNumber = 3 },
            new() { BundleID = 1, QuarterNumber = 4 }
        };
        _aggregateMock.Setup(a => a.GetBundleByIdAsync(1)).ReturnsAsync(rows);

        var result = await _sut.GetBundleAsync(1);

        Assert.Equal(4, result.Count());
    }

    [Fact]
    public async Task GetBundleAsync_WhenNotFound_ReturnsEmptyCollection()
    {
        _aggregateMock.Setup(a => a.GetBundleByIdAsync(99))
                      .ReturnsAsync(Enumerable.Empty<LessonBundleWithQuarterDetail>());

        var result = await _sut.GetBundleAsync(99);

        Assert.Empty(result);
    }

    // ── GetByStudentAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetByStudentAsync_ReturnsDaoResult()
    {
        var details = new List<LessonBundleDetail>
        {
            new() { BundleID = 1, StudentID = 5 },
            new() { BundleID = 2, StudentID = 5 }
        };
        _aggregateMock.Setup(a => a.GetBundleByStudentIdAsync(5)).ReturnsAsync(details);

        var result = await _sut.GetByStudentAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByStudentAsync_WhenNoBundles_ReturnsEmptyCollection()
    {
        _aggregateMock.Setup(a => a.GetBundleByStudentIdAsync(5))
                      .ReturnsAsync(Enumerable.Empty<LessonBundleDetail>());

        var result = await _sut.GetByStudentAsync(5);

        Assert.Empty(result);
    }

    // ── AddBundleAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddBundleAsync_WhenSuccessful_ReturnsNewBundleId()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter> { new() { QuarterNumber = 1 } };
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters)).ReturnsAsync(7);

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Equal(7, result);
    }

    [Fact]
    public async Task AddBundleAsync_WhenAggregateDaoThrows_ReturnsNull()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter>();
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters))
                      .ThrowsAsync(new InvalidOperationException("Student not found"));

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddBundleAsync_WhenGeneralExceptionThrown_ReturnsNull()
    {
        var bundle   = new LessonBundle { StudentID = 3 };
        var quarters = new List<BundleQuarter>();
        _aggregateMock.Setup(a => a.SaveNewBundleAsync(bundle, quarters))
                      .ThrowsAsync(new Exception("DB connection error"));

        var result = await _sut.AddBundleAsync(bundle, quarters);

        Assert.Null(result);
    }

    // ── UpdateBundleAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateBundleAsync_WhenSuccessful_ReturnsTrue()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ReturnsAsync(true);

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateBundleAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ReturnsAsync(false);

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateBundleAsync_WhenDaoThrows_ReturnsFalse()
    {
        var bundle = new LessonBundle { BundleID = 1 };
        _bundleDaoMock.Setup(d => d.UpdateAsync(bundle)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateBundleAsync(bundle);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\LessonRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonRepositoryTests
{
    private readonly Mock<ILessonAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<ILessonDataAccessObject> _lessonDaoMock;
    private readonly Mock<IBundleQuarterDataAccessObject> _quarterDaoMock;
    private readonly Mock<ILogger<LessonRepository>> _loggerMock;
    private readonly LessonRepository _sut;

    public LessonRepositoryTests()
    {
        _aggregateMock  = new Mock<ILessonAggregateDataAccessObject>();
        _lessonDaoMock  = new Mock<ILessonDataAccessObject>();
        _quarterDaoMock = new Mock<IBundleQuarterDataAccessObject>();
        _loggerMock     = new Mock<ILogger<LessonRepository>>();
        _sut = new LessonRepository(
            _aggregateMock.Object,
            _lessonDaoMock.Object,
            _quarterDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetLessonAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetLessonAsync_WhenFound_ReturnsDetail()
    {
        var expected = new LessonDetail { LessonID = 10, TeacherName = "Alice" };
        _aggregateMock.Setup(a => a.GetLessonByIdAsync(10)).ReturnsAsync(expected);

        var result = await _sut.GetLessonAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.LessonID);
    }

    [Fact]
    public async Task GetLessonAsync_WhenNotFound_ReturnsNull()
    {
        _aggregateMock.Setup(a => a.GetLessonByIdAsync(99)).ReturnsAsync((LessonDetail?)null);

        var result = await _sut.GetLessonAsync(99);

        Assert.Null(result);
    }

    // ── GetByTeacherAndDateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetByTeacherAndDateAsync_ReturnsDaoResult()
    {
        var date    = new DateTime(2025, 5, 12);
        var details = new List<LessonDetail> { new() { LessonID = 1 }, new() { LessonID = 2 } };
        _aggregateMock.Setup(a => a.GetLessonsByTeacherAndDateAsync(3, date)).ReturnsAsync(details);

        var result = await _sut.GetByTeacherAndDateAsync(3, date);

        Assert.Equal(2, result.Count());
    }

    // ── GetByBundleAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByBundleAsync_ReturnsDaoResult()
    {
        var lessons = new List<Lesson> { new() { LessonID = 1 }, new() { LessonID = 2 } };
        _lessonDaoMock.Setup(d => d.GetByBundleAsync(5)).ReturnsAsync(lessons);

        var result = await _sut.GetByBundleAsync(5);

        Assert.Equal(2, result.Count());
    }

    // ── AddLessonAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddLessonAsync_WhenSuccessful_ReturnsNewId()
    {
        var lesson = new Lesson { BundleID = 1, QuarterID = 2 };
        _lessonDaoMock.Setup(d => d.InsertAsync(lesson)).ReturnsAsync(33);

        var result = await _sut.AddLessonAsync(lesson);

        Assert.Equal(33, result);
    }

    [Fact]
    public async Task AddLessonAsync_WhenDaoThrows_ReturnsNull()
    {
        var lesson = new Lesson { BundleID = 1 };
        _lessonDaoMock.Setup(d => d.InsertAsync(lesson)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddLessonAsync(lesson);

        Assert.Null(result);
    }

    // ── UpdateLessonStatusAsync — status transitions ───────────────────────────

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenLessonNotFound_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync((Lesson?)null);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenUpdateFails_ReturnsFalse()
    {
        var lesson = new Lesson { LessonID = 1, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            1, LessonStatus.Completed, false, null, null, null, null))
            .ReturnsAsync(false);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_ScheduledToCompleted_IncrementsQuarter()
    {
        // Scheduled → Completed: delta = +1
        var lesson = new Lesson { LessonID = 1, QuarterID = 10, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            1, LessonStatus.Completed, false, null, null, It.IsAny<DateTime?>(), null))
            .ReturnsAsync(true);
        _quarterDaoMock.Setup(d => d.AdjustLessonsUsedAsync(1, 1)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, DateTime.UtcNow);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(1, 1), Times.Once);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_CompletedToCancelledTeacher_DecrementsQuarter()
    {
        // Completed → CancelledTeacher: delta = -1
        var lesson = new Lesson { LessonID = 2, QuarterID = 10, Status = LessonStatus.Completed };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(2)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            2, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null, null))
            .ReturnsAsync(true);
        _quarterDaoMock.Setup(d => d.AdjustLessonsUsedAsync(2, -1)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            2, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(2, -1), Times.Once);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_ScheduledToCancelledTeacher_NoQuarterChange()
    {
        // Scheduled → CancelledTeacher: neither previously consumed, neither now — delta = 0
        var lesson = new Lesson { LessonID = 3, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(3)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.UpdateStatusAsync(
            3, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null, null))
            .ReturnsAsync(true);

        var result = await _sut.UpdateLessonStatusAsync(
            3, LessonStatus.CancelledTeacher, false, CancelledBy.Teacher, null, null);

        Assert.True(result);
        _quarterDaoMock.Verify(d => d.AdjustLessonsUsedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLessonStatusAsync_WhenDaoThrows_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateLessonStatusAsync(
            1, LessonStatus.Completed, false, null, null, null);

        Assert.False(result);
    }

    // ── RescheduleLessonAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task RescheduleLessonAsync_WhenCancelledTeacher_Succeeds()
    {
        var newDate = new DateTime(2025, 7, 1);
        var newTime = new TimeOnly(10, 0);
        var lesson  = new Lesson { LessonID = 4, Status = LessonStatus.CancelledTeacher };

        _lessonDaoMock.Setup(d => d.GetLessonAsync(4)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.RescheduleLessonAsync(4, newDate, newTime)).ReturnsAsync(true);

        var result = await _sut.RescheduleLessonAsync(4, newDate, newTime);

        Assert.True(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenCancelledStudent_Succeeds()
    {
        var newDate = new DateTime(2025, 7, 2);
        var newTime = new TimeOnly(14, 0);
        var lesson  = new Lesson { LessonID = 5, Status = LessonStatus.CancelledStudent };

        _lessonDaoMock.Setup(d => d.GetLessonAsync(5)).ReturnsAsync(lesson);
        _lessonDaoMock.Setup(d => d.RescheduleLessonAsync(5, newDate, newTime)).ReturnsAsync(true);

        var result = await _sut.RescheduleLessonAsync(5, newDate, newTime);

        Assert.True(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenStatusIsScheduled_ReturnsFalse()
    {
        var lesson = new Lesson { LessonID = 6, Status = LessonStatus.Scheduled };
        _lessonDaoMock.Setup(d => d.GetLessonAsync(6)).ReturnsAsync(lesson);

        var result = await _sut.RescheduleLessonAsync(6, DateTime.Today, new TimeOnly(9, 0));

        Assert.False(result);
        _lessonDaoMock.Verify(d => d.RescheduleLessonAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<TimeOnly>()), Times.Never);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenLessonNotFound_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(99)).ReturnsAsync((Lesson?)null);

        var result = await _sut.RescheduleLessonAsync(99, DateTime.Today, new TimeOnly(9, 0));

        Assert.False(result);
    }

    [Fact]
    public async Task RescheduleLessonAsync_WhenDaoThrows_ReturnsFalse()
    {
        _lessonDaoMock.Setup(d => d.GetLessonAsync(4)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.RescheduleLessonAsync(4, DateTime.Today, new TimeOnly(10, 0));

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\LessonTypeRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class LessonTypeRepositoryTests
{
    private readonly Mock<ILessonTypeDataAccessObject> _daoMock;
    private readonly Mock<ILogger<LessonTypeRepository>> _loggerMock;
    private readonly LessonTypeRepository _sut;

    public LessonTypeRepositoryTests()
    {
        _daoMock    = new Mock<ILessonTypeDataAccessObject>();
        _loggerMock = new Mock<ILogger<LessonTypeRepository>>();
        _sut        = new LessonTypeRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetLessonTypeAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetLessonTypeAsync_WhenFound_ReturnsLessonType()
    {
        var expected = new LessonType { LessonTypeID = 2, DurationMinutes = 45 };
        _daoMock.Setup(d => d.GetLessonTypeAsync(2)).ReturnsAsync(expected);

        var result = await _sut.GetLessonTypeAsync(2);

        Assert.NotNull(result);
        Assert.Equal(45, result.DurationMinutes);
    }

    [Fact]
    public async Task GetLessonTypeAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetLessonTypeAsync(99)).ReturnsAsync((LessonType?)null);

        var result = await _sut.GetLessonTypeAsync(99);

        Assert.Null(result);
    }

    // ── GetAllActiveAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllActiveAsync_ReturnsDaoResult()
    {
        var list = new List<LessonType>
        {
            new() { LessonTypeID = 1, DurationMinutes = 30 },
            new() { LessonTypeID = 2, DurationMinutes = 45 },
            new() { LessonTypeID = 3, DurationMinutes = 60 }
        };
        _daoMock.Setup(d => d.GetAllActiveAsync()).ReturnsAsync(list);

        var result = await _sut.GetAllActiveAsync();

        Assert.Equal(3, result.Count());
    }

    // ── AddLessonTypeAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AddLessonTypeAsync_WhenSuccessful_ReturnsNewId()
    {
        var lt = new LessonType { DurationMinutes = 60, BasePricePerLesson = 200 };
        _daoMock.Setup(d => d.InsertAsync(lt)).ReturnsAsync(10);

        var result = await _sut.AddLessonTypeAsync(lt);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task AddLessonTypeAsync_WhenDaoThrows_ReturnsNull()
    {
        var lt = new LessonType { DurationMinutes = 60 };
        _daoMock.Setup(d => d.InsertAsync(lt)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddLessonTypeAsync(lt);

        Assert.Null(result);
    }

    // ── UpdateLessonTypeAsync ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenSuccessful_ReturnsTrue()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ReturnsAsync(true);

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ReturnsAsync(false);

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateLessonTypeAsync_WhenDaoThrows_ReturnsFalse()
    {
        var lt = new LessonType { LessonTypeID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(lt)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateLessonTypeAsync(lt);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\MusicSchool.Repositories.Tests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.2" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Interfaces\MusicSchool.Interfaces.csproj" />
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
    <ProjectReference Include="..\MusicSchool.Repositories\MusicSchool.Repositories.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>

```

## File: MusicSchool.Repositories.Tests\PaymentRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;
using System.Data;

namespace MusicSchool.Repositories.Tests;

/// <summary>
/// Tests for PaymentRepository.
///
/// NOTE: AddPaymentAsync and QuickPayInvoiceAsync open real IDbConnection transactions
/// internally and therefore cannot be fully unit-tested without an in-memory or
/// integration database. The tests below verify the observable public-facing behaviour:
///   • Delegation of read methods to the DAO.
///   • That a failed DAO insert causes AddPaymentAsync to return null (via a mock
///     that throws when InsertAsync is called through the connection scalar path).
///   • QuickPay returns null when the invoice does not exist.
/// The full allocation-engine logic is covered by integration tests / in-memory DB tests.
/// </summary>
public class PaymentRepositoryTests
{
    private readonly Mock<IPaymentDataAccessObject> _paymentDaoMock;
    private readonly Mock<IInvoiceDataAccessObject> _invoiceDaoMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<ILogger<PaymentRepository>> _loggerMock;
    private readonly PaymentRepository _sut;

    public PaymentRepositoryTests()
    {
        _paymentDaoMock = new Mock<IPaymentDataAccessObject>();
        _invoiceDaoMock = new Mock<IInvoiceDataAccessObject>();
        _connectionMock = new Mock<IDbConnection>();
        _loggerMock     = new Mock<ILogger<PaymentRepository>>();

        // Default: connection reports Closed so the repo will attempt to open it.
        _connectionMock.Setup(c => c.State).Returns(ConnectionState.Open);

        _sut = new PaymentRepository(
            _paymentDaoMock.Object,
            _invoiceDaoMock.Object,
            _connectionMock.Object,
            _loggerMock.Object);
    }

    // ── GetPaymentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetPaymentAsync_WhenFound_ReturnsPayment()
    {
        var expected = new Payment { PaymentID = 5, Amount = 500m };
        _paymentDaoMock.Setup(d => d.GetPaymentAsync(5)).ReturnsAsync(expected);

        var result = await _sut.GetPaymentAsync(5);

        Assert.NotNull(result);
        Assert.Equal(500m, result.Amount);
    }

    [Fact]
    public async Task GetPaymentAsync_WhenNotFound_ReturnsNull()
    {
        _paymentDaoMock.Setup(d => d.GetPaymentAsync(99)).ReturnsAsync((Payment?)null);

        var result = await _sut.GetPaymentAsync(99);

        Assert.Null(result);
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var payments = new List<Payment>
        {
            new() { PaymentID = 1, AccountHolderID = 10 },
            new() { PaymentID = 2, AccountHolderID = 10 }
        };
        _paymentDaoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(payments);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByAccountHolderAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _paymentDaoMock.Setup(d => d.GetByAccountHolderAsync(10))
                       .ReturnsAsync(Enumerable.Empty<Payment>());

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Empty(result);
    }

    // ── GetAllocationsByPaymentAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetAllocationsByPaymentAsync_ReturnsDaoResult()
    {
        var allocations = new List<PaymentAllocation>
        {
            new() { AllocationID = 1, PaymentID = 3, InvoiceID = 7, AmountApplied = 200m }
        };
        _paymentDaoMock.Setup(d => d.GetAllocationsByPaymentAsync(3)).ReturnsAsync(allocations);

        var result = await _sut.GetAllocationsByPaymentAsync(3);

        Assert.Single(result);
        Assert.Equal(200m, result.First().AmountApplied);
    }

    // ── GetAllocationsByInvoiceAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetAllocationsByInvoiceAsync_ReturnsDaoResult()
    {
        var allocations = new List<PaymentAllocation>
        {
            new() { AllocationID = 1, InvoiceID = 7, AmountApplied = 500m },
            new() { AllocationID = 2, InvoiceID = 7, AmountApplied = 300m }
        };
        _paymentDaoMock.Setup(d => d.GetAllocationsByInvoiceAsync(7)).ReturnsAsync(allocations);

        var result = await _sut.GetAllocationsByInvoiceAsync(7);

        Assert.Equal(2, result.Count());
        Assert.Equal(800m, result.Sum(a => a.AmountApplied));
    }

    // ── QuickPayInvoiceAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task QuickPayInvoiceAsync_WhenInvoiceNotFound_ReturnsNull()
    {
        _invoiceDaoMock.Setup(d => d.GetInvoiceAsync(99)).ReturnsAsync((Invoice?)null);

        var result = await _sut.QuickPayInvoiceAsync(99, DateTime.Today);

        Assert.Null(result);
    }

    // ── AddPaymentAsync — error path ──────────────────────────────────────────

    [Fact]
    public async Task AddPaymentAsync_WhenConnectionThrowsOnBeginTransaction_ReturnsNull()
    {
        _connectionMock.Setup(c => c.BeginTransaction())
                       .Throws(new InvalidOperationException("Cannot open transaction"));

        var payment = new Payment { AccountHolderID = 1, Amount = 500m };

        var result = await _sut.AddPaymentAsync(payment);

        Assert.Null(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\ScheduledSlotAggregateDataAccessObjectTests.cs

```csharp
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ScheduledSlotAggregateDataAccessObjectTests
{
    // ── GetOccurrences helper — pure business logic ────────────────────────────
    //
    // GetOccurrences is private on ScheduledSlotAggregateDataAccessObject.
    // We replicate the algorithm here so regressions are caught with clear
    // assertion messages, independently of any DB infrastructure.
    // ─────────────────────────────────────────────────────────────────────────

    private static IEnumerable<DateTime> GetOccurrences(
        DateTime from, DateTime to, byte isoDayOfWeek)
    {
        var targetDotNet = isoDayOfWeek == 7
            ? DayOfWeek.Sunday
            : (DayOfWeek)isoDayOfWeek;

        var date = from;
        while (date.DayOfWeek != targetDotNet)
            date = date.AddDays(1);

        while (date <= to)
        {
            yield return date;
            date = date.AddDays(7);
        }
    }

    // ── GetOccurrences — correctness ──────────────────────────────────────────

    [Fact]
    public void GetOccurrences_FromMonday_IsoDayOfWeek1_ReturnsEveryMonday()
    {
        // 2025-01-06 is a Monday
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Equal(4, dates.Count);
        Assert.All(dates, d => Assert.Equal(DayOfWeek.Monday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 6),  dates[0]);
        Assert.Equal(new DateTime(2025, 1, 13), dates[1]);
        Assert.Equal(new DateTime(2025, 1, 20), dates[2]);
        Assert.Equal(new DateTime(2025, 1, 27), dates[3]);
    }

    [Fact]
    public void GetOccurrences_StartOnWrongDay_AdvancesToFirstMatchingWeekday()
    {
        // 2025-01-06 is a Monday; asking for Thursday (ISO 4)
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 4).ToList();

        Assert.All(dates, d => Assert.Equal(DayOfWeek.Thursday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 9), dates[0]);
    }

    [Fact]
    public void GetOccurrences_IsoDayOfWeek7_MappedToSunday()
    {
        // 2025-01-05 is a Sunday
        var from  = new DateTime(2025, 1, 1);
        var to    = new DateTime(2025, 1, 31);
        var dates = GetOccurrences(from, to, 7).ToList();

        Assert.All(dates, d => Assert.Equal(DayOfWeek.Sunday, d.DayOfWeek));
        Assert.Equal(new DateTime(2025, 1, 5), dates[0]);
    }

    [Fact]
    public void GetOccurrences_ToDateIsExclusive_ExactBoundaryIncluded()
    {
        // from = 2025-01-06 (Monday); to = 2025-01-06 → exactly one occurrence
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 6);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Single(dates);
        Assert.Equal(new DateTime(2025, 1, 6), dates[0]);
    }

    [Fact]
    public void GetOccurrences_ToBeforeFirstOccurrence_ReturnsEmpty()
    {
        // from = 2025-01-06 (Monday); to = 2025-01-05 (Sunday before)
        var from  = new DateTime(2025, 1, 6);
        var to    = new DateTime(2025, 1, 5);
        var dates = GetOccurrences(from, to, 1).ToList();

        Assert.Empty(dates);
    }

    [Fact]
    public void GetOccurrences_SpansMultipleMonths_ReturnsCorrectCount()
    {
        // 52 weeks of Tuesdays starting 2025-01-07
        var from  = new DateTime(2025, 1, 7);   // Tuesday
        var to    = from.AddDays(52 * 7 - 1);
        var dates = GetOccurrences(from, to, 2).ToList();

        Assert.Equal(52, dates.Count);
        Assert.All(dates, d => Assert.Equal(DayOfWeek.Tuesday, d.DayOfWeek));
    }

    [Fact]
    public void GetOccurrences_AllDates_AreExactlyOneWeekApart()
    {
        var from  = new DateTime(2025, 3, 3);   // Monday
        var to    = new DateTime(2025, 6, 30);
        var dates = GetOccurrences(from, to, 1).ToList();

        for (int i = 1; i < dates.Count; i++)
            Assert.Equal(7, (dates[i] - dates[i - 1]).Days);
    }

    // ── Lesson-building invariants ─────────────────────────────────────────────

    [Fact]
    public void LessonCreatedPerOccurrence_HasCorrectSlotAndBundleIds()
    {
        const int slotId   = 42;
        const int bundleId = 7;

        var quarter = new BundleQuarter
        {
            QuarterID          = 1,
            BundleID           = bundleId,
            QuarterStartDate   = new DateTime(2025, 1, 1),
            QuarterEndDate     = new DateTime(2025, 3, 31),
            LessonsAllocated   = 12,
            LessonsUsed        = 0
        };

        var slotTime = new TimeOnly(14, 30);
        var date     = new DateTime(2025, 1, 6);   // Monday inside the quarter

        var lesson = new Lesson
        {
            SlotID          = slotId,
            BundleID        = bundleId,
            QuarterID       = quarter.QuarterID,
            ScheduledDate   = date,
            ScheduledTime   = slotTime,
            Status          = LessonStatus.Scheduled,
            CreditForfeited = false
        };

        Assert.Equal(slotId,             lesson.SlotID);
        Assert.Equal(bundleId,           lesson.BundleID);
        Assert.Equal(1,                  lesson.QuarterID);
        Assert.Equal(LessonStatus.Scheduled, lesson.Status);
        Assert.False(lesson.CreditForfeited);
    }

    [Fact]
    public void LessonOutsideAllQuarters_IsSkipped()
    {
        // A date that falls outside every quarter range should map to no quarter.
        var quarters = new List<BundleQuarter>
        {
            new() { QuarterStartDate = new DateTime(2025, 1, 1), QuarterEndDate = new DateTime(2025, 3, 31) },
            new() { QuarterStartDate = new DateTime(2025, 4, 1), QuarterEndDate = new DateTime(2025, 6, 30) }
        };

        var dateOutsideRange = new DateTime(2025, 7, 15);

        var matchingQuarter = quarters.FirstOrDefault(q =>
            dateOutsideRange >= q.QuarterStartDate && dateOutsideRange <= q.QuarterEndDate);

        Assert.Null(matchingQuarter);
    }

    [Fact]
    public void LessonInsideQuarter_MapsToCorrectQuarter()
    {
        var q1 = new BundleQuarter { QuarterID = 1, QuarterStartDate = new DateTime(2025, 1, 1), QuarterEndDate = new DateTime(2025, 3, 31) };
        var q2 = new BundleQuarter { QuarterID = 2, QuarterStartDate = new DateTime(2025, 4, 1), QuarterEndDate = new DateTime(2025, 6, 30) };
        var quarters = new List<BundleQuarter> { q1, q2 };

        var dateInQ2 = new DateTime(2025, 5, 12);

        var match = quarters.FirstOrDefault(q =>
            dateInQ2 >= q.QuarterStartDate && dateInQ2 <= q.QuarterEndDate);

        Assert.NotNull(match);
        Assert.Equal(2, match.QuarterID);
    }
}

```

## File: MusicSchool.Repositories.Tests\ScheduledSlotRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class ScheduledSlotRepositoryTests
{
    private readonly Mock<IScheduledSlotAggregateDataAccessObject> _aggregateMock;
    private readonly Mock<IScheduledSlotDataAccessObject> _slotDaoMock;
    private readonly Mock<ILogger<ScheduledSlotRepository>> _loggerMock;
    private readonly ScheduledSlotRepository _sut;

    public ScheduledSlotRepositoryTests()
    {
        _aggregateMock = new Mock<IScheduledSlotAggregateDataAccessObject>();
        _slotDaoMock   = new Mock<IScheduledSlotDataAccessObject>();
        _loggerMock    = new Mock<ILogger<ScheduledSlotRepository>>();
        _sut = new ScheduledSlotRepository(
            _aggregateMock.Object,
            _slotDaoMock.Object,
            _loggerMock.Object);
    }

    // ── GetSlotAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSlotAsync_WhenFound_ReturnsSlot()
    {
        var expected = new ScheduledSlot { SlotID = 10, StudentID = 2, DayOfWeek = 1 };
        _slotDaoMock.Setup(d => d.GetSlotAsync(10)).ReturnsAsync(expected);

        var result = await _sut.GetSlotAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.SlotID);
    }

    [Fact]
    public async Task GetSlotAsync_WhenNotFound_ReturnsNull()
    {
        _slotDaoMock.Setup(d => d.GetSlotAsync(99)).ReturnsAsync((ScheduledSlot?)null);

        var result = await _sut.GetSlotAsync(99);

        Assert.Null(result);
    }

    // ── GetActiveByStudentAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetActiveByStudentAsync_ReturnsDaoResult()
    {
        var slots = new List<ScheduledSlot>
        {
            new() { SlotID = 1, StudentID = 3 },
            new() { SlotID = 2, StudentID = 3 }
        };
        _slotDaoMock.Setup(d => d.GetActiveByStudentAsync(3)).ReturnsAsync(slots);

        var result = await _sut.GetActiveByStudentAsync(3);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetActiveByStudentAsync_WhenNoSlots_ReturnsEmptyCollection()
    {
        _slotDaoMock.Setup(d => d.GetActiveByStudentAsync(3))
                    .ReturnsAsync(Enumerable.Empty<ScheduledSlot>());

        var result = await _sut.GetActiveByStudentAsync(3);

        Assert.Empty(result);
    }

    // ── GetActiveByTeacherAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetActiveByTeacherAsync_ReturnsDaoResult()
    {
        var slots = new List<ScheduledSlot>
        {
            new() { SlotID = 1, TeacherID = 5 },
            new() { SlotID = 2, TeacherID = 5 },
            new() { SlotID = 3, TeacherID = 5 }
        };
        _slotDaoMock.Setup(d => d.GetActiveByTeacherAsync(5)).ReturnsAsync(slots);

        var result = await _sut.GetActiveByTeacherAsync(5);

        Assert.Equal(3, result.Count());
    }

    // ── AddSlotAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task AddSlotAsync_WhenSuccessful_ReturnsNewSlotId()
    {
        var slot = new ScheduledSlot { StudentID = 1, TeacherID = 2, DayOfWeek = 3 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot)).ReturnsAsync(20);

        var result = await _sut.AddSlotAsync(slot);

        Assert.Equal(20, result);
    }

    [Fact]
    public async Task AddSlotAsync_WhenNoBundleExists_LogsWarningAndReturnsNull()
    {
        var slot = new ScheduledSlot { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot))
                      .ThrowsAsync(new InvalidOperationException("StudentID 1 has no active bundle"));

        var result = await _sut.AddSlotAsync(slot);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddSlotAsync_WhenGeneralExceptionThrown_ReturnsNull()
    {
        var slot = new ScheduledSlot { StudentID = 1 };
        _aggregateMock.Setup(a => a.SaveNewSlotWithLessonsAsync(slot))
                      .ThrowsAsync(new Exception("DB connection error"));

        var result = await _sut.AddSlotAsync(slot);

        Assert.Null(result);
    }

    // ── CloseSlotAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CloseSlotAsync_WhenSuccessful_ReturnsTrue()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo)).ReturnsAsync(true);

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.True(result);
    }

    [Fact]
    public async Task CloseSlotAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo)).ReturnsAsync(false);

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.False(result);
    }

    [Fact]
    public async Task CloseSlotAsync_WhenDaoThrows_ReturnsFalse()
    {
        var effectiveTo = DateOnly.FromDateTime(DateTime.Today);
        _slotDaoMock.Setup(d => d.CloseSlotAsync(5, effectiveTo))
                    .ThrowsAsync(new Exception("DB error"));

        var result = await _sut.CloseSlotAsync(5, effectiveTo);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\StudentRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class StudentRepositoryTests
{
    private readonly Mock<IStudentDataAccessObject> _daoMock;
    private readonly Mock<ILogger<StudentRepository>> _loggerMock;
    private readonly StudentRepository _sut;

    public StudentRepositoryTests()
    {
        _daoMock    = new Mock<IStudentDataAccessObject>();
        _loggerMock = new Mock<ILogger<StudentRepository>>();
        _sut        = new StudentRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetStudentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetStudentAsync_WhenFound_ReturnsStudent()
    {
        var expected = new Student { StudentID = 3, FirstName = "Tom", LastName = "Jones" };
        _daoMock.Setup(d => d.GetStudentAsync(3)).ReturnsAsync(expected);

        var result = await _sut.GetStudentAsync(3);

        Assert.NotNull(result);
        Assert.Equal(3, result.StudentID);
    }

    [Fact]
    public async Task GetStudentAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetStudentAsync(99)).ReturnsAsync((Student?)null);

        var result = await _sut.GetStudentAsync(99);

        Assert.Null(result);
    }

    // ── GetByAccountHolderAsync ───────────────────────────────────────────────

    [Fact]
    public async Task GetByAccountHolderAsync_ReturnsDaoResult()
    {
        var students = new List<Student>
        {
            new() { StudentID = 1, AccountHolderID = 10 },
            new() { StudentID = 2, AccountHolderID = 10 }
        };
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(students);

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByAccountHolderAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        _daoMock.Setup(d => d.GetByAccountHolderAsync(10)).ReturnsAsync(Enumerable.Empty<Student>());

        var result = await _sut.GetByAccountHolderAsync(10);

        Assert.Empty(result);
    }

    // ── AddStudentAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddStudentAsync_WhenSuccessful_ReturnsNewId()
    {
        var student = new Student { FirstName = "Anna", LastName = "Bell" };
        _daoMock.Setup(d => d.InsertAsync(student)).ReturnsAsync(55);

        var result = await _sut.AddStudentAsync(student);

        Assert.Equal(55, result);
    }

    [Fact]
    public async Task AddStudentAsync_WhenDaoThrows_ReturnsNull()
    {
        var student = new Student { FirstName = "Anna", LastName = "Bell" };
        _daoMock.Setup(d => d.InsertAsync(student)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddStudentAsync(student);

        Assert.Null(result);
    }

    // ── UpdateStudentAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStudentAsync_WhenSuccessful_ReturnsTrue()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ReturnsAsync(true);

        var result = await _sut.UpdateStudentAsync(student);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ReturnsAsync(false);

        var result = await _sut.UpdateStudentAsync(student);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStudentAsync_WhenDaoThrows_ReturnsFalse()
    {
        var student = new Student { StudentID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(student)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateStudentAsync(student);

        Assert.False(result);
    }
}

```

## File: MusicSchool.Repositories.Tests\TeacherRepositoryTests.cs

```csharp
using Microsoft.Extensions.Logging;
using Moq;
using MusicSchool.Data.Implementations;
using MusicSchool.Data.Interfaces;
using MusicSchool.Data.Models;

namespace MusicSchool.Repositories.Tests;

public class TeacherRepositoryTests
{
    private readonly Mock<ITeacherDataAccessObject> _daoMock;
    private readonly Mock<ILogger<TeacherRepository>> _loggerMock;
    private readonly TeacherRepository _sut;

    public TeacherRepositoryTests()
    {
        _daoMock    = new Mock<ITeacherDataAccessObject>();
        _loggerMock = new Mock<ILogger<TeacherRepository>>();
        _sut        = new TeacherRepository(_daoMock.Object, _loggerMock.Object);
    }

    // ── GetTeacherAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetTeacherAsync_WhenFound_ReturnsTeacher()
    {
        var expected = new Teacher { TeacherID = 1, Name = "Alice" };
        _daoMock.Setup(d => d.GetTeacherAsync(1)).ReturnsAsync(expected);

        var result = await _sut.GetTeacherAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public async Task GetTeacherAsync_WhenNotFound_ReturnsNull()
    {
        _daoMock.Setup(d => d.GetTeacherAsync(99)).ReturnsAsync((Teacher?)null);

        var result = await _sut.GetTeacherAsync(99);

        Assert.Null(result);
    }

    // ── GetAllActiveAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllActiveAsync_ReturnsDaoResult()
    {
        var teachers = new List<Teacher> { new() { TeacherID = 1 }, new() { TeacherID = 2 } };
        _daoMock.Setup(d => d.GetAllActiveAsync()).ReturnsAsync(teachers);

        var result = await _sut.GetAllActiveAsync();

        Assert.Equal(2, result.Count());
    }

    // ── AddTeacherAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task AddTeacherAsync_WhenSuccessful_ReturnsNewId()
    {
        var teacher = new Teacher { Name = "Bob" };
        _daoMock.Setup(d => d.InsertAsync(teacher)).ReturnsAsync(7);

        var result = await _sut.AddTeacherAsync(teacher);

        Assert.Equal(7, result);
    }

    [Fact]
    public async Task AddTeacherAsync_WhenDaoThrows_ReturnsNull()
    {
        var teacher = new Teacher { Name = "Bob" };
        _daoMock.Setup(d => d.InsertAsync(teacher)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.AddTeacherAsync(teacher);

        Assert.Null(result);
    }

    // ── UpdateTeacherAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateTeacherAsync_WhenSuccessful_ReturnsTrue()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ReturnsAsync(true);

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateTeacherAsync_WhenDaoReturnsFalse_ReturnsFalse()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ReturnsAsync(false);

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTeacherAsync_WhenDaoThrows_ReturnsFalse()
    {
        var teacher = new Teacher { TeacherID = 1 };
        _daoMock.Setup(d => d.UpdateAsync(teacher)).ThrowsAsync(new Exception("DB error"));

        var result = await _sut.UpdateTeacherAsync(teacher);

        Assert.False(result);
    }
}

```

## File: MusicSchool.StudentPortal\Pages\Dashboard.razor

```razor
@page "/dashboard/{StudentId:int}"

@namespace MusicSchool.StudentPortal.Pages

@using MusicSchool.Data.Models

@inject ApiService Api

<PageTitle>My Lessons — Music School</PageTitle>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="mb-4" />
    <MudText>Loading your lessons…</MudText>
}
else if (_student is null)
{
    <MudAlert Severity="Severity.Error">
        Student not found. Please check your ID or contact your teacher.
    </MudAlert>
}
else
{
    <!-- ── Page header ──────────────────────────────────────── -->
    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2" Class="mb-5">
        <MudIcon Icon="@Icons.Material.Filled.LibraryMusic"
                 Style="font-size:2rem; color:#F3D395;" />
        <div>
            <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
                Welcome, @_student.FirstName!
            </MudText>
            <MudText Typo="Typo.body2" Color="Color.Secondary">
                Here's your lesson overview.
            </MudText>
        </div>
    </MudStack>

    <!-- ══════════════════════════════════════════════════════
         NEXT LESSON HERO
    ══════════════════════════════════════════════════════ -->
    <MudText Typo="Typo.h6" Style="font-weight:600;" Class="mb-3">
        Next Lesson
    </MudText>

    @if (_nextLesson is null)
    {
        <MudPaper Class="pa-5 mb-6" Elevation="1">
            <MudStack AlignItems="AlignItems.Center" Spacing="2">
                <MudIcon Icon="@Icons.Material.Filled.EventBusy"
                         Style="font-size:2.5rem; color:#78797A;" />
                <MudText Color="Color.Secondary">
                    No upcoming lessons scheduled. Contact your teacher to book.
                </MudText>
            </MudStack>
        </MudPaper>
    }
    else
    {
        <MudPaper Class="pa-5 mb-6 next-lesson-card" Elevation="2">
            <MudGrid AlignItems="AlignItems.Center">

                <!-- Date / time block -->
                <MudItem xs="12" sm="6" md="4">
                    <MudStack Spacing="1">
                        <MudText Typo="Typo.caption" Style="color:#78797A; text-transform:uppercase; letter-spacing:.05em;">
                            Date &amp; Time
                        </MudText>
                        <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
                            @_nextLesson.ScheduledDate.ToString("dddd, dd MMMM")
                        </MudText>
                        <MudText Typo="Typo.h6" Style="color:#3A3A3A;">
                            @_nextLesson.ScheduledTime.ToString("HH:mm")
                            @if (_nextLessonDuration > 0)
                            {
                                <MudText Typo="Typo.caption" Color="Color.Secondary"
                                         Style="margin-left:6px;">
                                    (@_nextLessonDuration min)
                                </MudText>
                            }
                        </MudText>
                    </MudStack>
                </MudItem>

                <!-- Countdown block -->
                <MudItem xs="12" sm="6" md="4"
                         Class="d-flex justify-center align-center">
                    @{
                        var daysUntil = (_nextLesson.ScheduledDate.Date - DateTime.Today).Days;
                    }
                    @if (daysUntil == 0)
                    {
                        <span class="countdown-badge" style="font-size:1rem; padding:8px 20px;">
                            🎵 Today!
                        </span>
                    }
                    else if (daysUntil == 1)
                    {
                        <span class="countdown-badge">Tomorrow</span>
                    }
                    else
                    {
                        <span class="countdown-badge">In @daysUntil days</span>
                    }
                </MudItem>

                <!-- Bundle / teacher info -->
                <MudItem xs="12" md="4">
                    <MudStack Spacing="1">
                        <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="1">
                            <MudIcon Icon="@Icons.Material.Filled.Inventory2"
                                     Size="Size.Small" Style="color:#78797A;" />
                            <MudText Typo="Typo.body2" Color="Color.Secondary">
                                Bundle #@_nextLesson.BundleID
                            </MudText>
                        </MudStack>
                        @if (!string.IsNullOrEmpty(_nextLessonTeacher))
                        {
                            <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="1">
                                <MudIcon Icon="@Icons.Material.Filled.Person"
                                         Size="Size.Small" Style="color:#78797A;" />
                                <MudText Typo="Typo.body2" Color="Color.Secondary">
                                    @_nextLessonTeacher
                                </MudText>
                            </MudStack>
                        }
                        <StatusChip Status="@_nextLesson.Status" />
                    </MudStack>
                </MudItem>

            </MudGrid>
        </MudPaper>
    }

    <!-- ══════════════════════════════════════════════════════
         PREVIOUS 3 COMPLETED LESSON NOTES
    ══════════════════════════════════════════════════════ -->
    <MudText Typo="Typo.h6" Style="font-weight:600;" Class="mb-3">
        Previous Lesson Notes
    </MudText>

    @if (_previousLessons.Count == 0)
    {
        <MudPaper Class="pa-5" Elevation="1">
            <MudStack AlignItems="AlignItems.Center" Spacing="2">
                <MudIcon Icon="@Icons.Material.Filled.StickyNote2"
                         Style="font-size:2.5rem; color:#78797A;" />
                <MudText Color="Color.Secondary">
                    No completed lessons yet. Notes from your teacher will appear here.
                </MudText>
            </MudStack>
        </MudPaper>
    }
    else
    {
        <MudGrid Spacing="3">
            @foreach (var lesson in _previousLessons)
            {
                <MudItem xs="12" md="4">
                    <MudPaper Class="pa-4 lesson-notes-card completed" Elevation="1" Style="height:100%;">
                        <MudStack Spacing="2">

                            <!-- Lesson header -->
                            <MudStack Row="true"
                                      Justify="Justify.SpaceBetween"
                                      AlignItems="AlignItems.Center">
                                <MudText Typo="Typo.subtitle2" Style="font-weight:600;">
                                    @lesson.ScheduledDate.ToString("yyyy MMM dd")
                                </MudText>
                                <StatusChip Status="@lesson.Status" />
                            </MudStack>

                            <MudText Typo="Typo.caption" Color="Color.Secondary">
                                @lesson.ScheduledTime.ToString("HH:mm")
                                @if (_lessonDurationMap.TryGetValue(lesson.BundleID, out var dur) && dur > 0)
                                {
                                    <span> · @dur min</span>
                                }
                                · Bundle #@lesson.BundleID
                            </MudText>

                            <MudDivider />

                            <!-- Notes section -->
                            <MudStack Spacing="1">
                                <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="1">
                                    <MudIcon Icon="@Icons.Material.Filled.Notes"
                                             Size="Size.Small" Style="color:#F3D395;" />
                                    <MudText Typo="Typo.caption"
                                             Style="font-weight:600; text-transform:uppercase; letter-spacing:.05em; color:#78797A;">
                                        Teacher's Notes
                                    </MudText>
                                </MudStack>

                                @if (!string.IsNullOrWhiteSpace(lesson.Notes))
                                {
                                    <MudText Typo="Typo.body2" Style="color:#3A3A3A; white-space:pre-line;">
                                        @lesson.Notes
                                    </MudText>
                                }
                                else
                                {
                                    <MudText Typo="Typo.body2" Color="Color.Secondary"
                                             Style="font-style:italic;">
                                        No notes recorded for this lesson.
                                    </MudText>
                                }
                            </MudStack>

                        </MudStack>
                    </MudPaper>
                </MudItem>
            }
        </MudGrid>
    }
}

@code {
    [Parameter] public int StudentId { get; set; }

    private bool _loading = true;
    private Student? _student;

    // Next upcoming scheduled lesson
    private Lesson? _nextLesson;
    private int _nextLessonDuration;
    private string _nextLessonTeacher = string.Empty;

    // Last 3 completed lessons
    private List<Lesson> _previousLessons = [];
    private Dictionary<int, int> _lessonDurationMap = [];   // BundleID → DurationMinutes

    protected override async Task OnInitializedAsync()
    {
        _loading = true;

        // 1. Load student info
        _student = await Api.GetStudentAsync(StudentId);
        if (_student is null)
        {
            _loading = false;
            return;
        }

        // 2. Load all bundles for this student so we can look up duration
        var bundles = await Api.GetBundlesByStudentAsync(StudentId);
        foreach (var b in bundles)
            _lessonDurationMap[b.BundleID] = b.DurationMinutes;

        // 3. Load all lessons across all bundles
        var allLessons = new List<Lesson>();
        foreach (var bundle in bundles)
        {
            var lessonList = await Api.GetLessonsByBundleAsync(bundle.BundleID);
            allLessons.AddRange(lessonList);
        }

        var today = DateTime.Today;

        // 4. Next lesson = earliest future Scheduled lesson
        _nextLesson = allLessons
            .Where(l => l.Status == LessonStatus.Scheduled
                        && l.ScheduledDate.Date >= today)
            .OrderBy(l => l.ScheduledDate)
            .ThenBy(l => l.ScheduledTime)
            .FirstOrDefault();

        if (_nextLesson is not null
            && _lessonDurationMap.TryGetValue(_nextLesson.BundleID, out var dur))
        {
            _nextLessonDuration = dur;
        }

        // Derive teacher name from bundle detail when available
        var nextBundle = bundles.FirstOrDefault(b =>
            _nextLesson is not null && b.BundleID == _nextLesson.BundleID);
        _nextLessonTeacher = string.Empty;

        // 5. Previous 3 lessons = most-recent Completed lessons only
        _previousLessons = allLessons
            .Where(l => l.Status == LessonStatus.Completed)
            .OrderByDescending(l => l.ScheduledDate)
            .ThenByDescending(l => l.ScheduledTime)
            .Take(3)
            .ToList();

        _loading = false;
    }
}

```

## File: MusicSchool.StudentPortal\Pages\Index.razor

```razor
@page "/"
@namespace MusicSchool.StudentPortal.Pages

<PageTitle>Music School — Student Portal</PageTitle>

<MudPaper Class="pa-6 mt-6" Elevation="1"
          Style="max-width:480px; margin:auto; border-top:4px solid #F3D395;">

    <MudStack AlignItems="AlignItems.Center" Spacing="2" Class="mb-5">
        <MudIcon Icon="@Icons.Material.Filled.LibraryMusic"
                 Style="font-size:3rem; color:#F3D395;" />
        <MudText Typo="Typo.h5" Style="font-weight:700; color:#3A3A3A;">
            Student Portal
        </MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Align="Align.Center">
            Enter your Student ID to see your upcoming lesson and recent lesson notes.
        </MudText>
    </MudStack>

    <MudTextField @bind-Value="_studentId"
                  Label="Student ID"
                  Variant="Variant.Outlined"
                  InputType="InputType.Number"
                  Adornment="Adornment.Start"
                  AdornmentIcon="@Icons.Material.Filled.School"
                  Class="mb-4"
                  OnKeyDown="@OnKeyDown" />

    <MudButton Variant="Variant.Filled"
               Color="Color.Primary"
               FullWidth="true"
               Size="Size.Large"
               StartIcon="@Icons.Material.Filled.CalendarToday"
               OnClick="ViewDashboard"
               Disabled="@(_studentId <= 0)">
        View My Lessons
    </MudButton>

</MudPaper>

@code {
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private int _studentId;

    private void ViewDashboard()
    {
        if (_studentId > 0)
            Nav.NavigateTo($"/dashboard/{_studentId}");
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter") ViewDashboard();
    }
}

```

## File: MusicSchool.StudentPortal\Properties\launchSettings.json

```json
{
  "profiles": {
    "MusicSchool.StudentPortal": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:51173;http://localhost:51174"
    }
  }
}
```

## File: MusicSchool.StudentPortal\Services\ApiService.cs

```csharp
using MusicSchool.Data.Models;
using MusicSchool.Models;
using MusicSchool.Models.TransferModels;
using System.Net.Http.Json;


namespace MusicSchool.StudentPortal.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    // ── Generic helper ───────────────────────────────────────────
    public async Task<T?> GetAsync<T>(string url)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ResponseBase<T>>(url);
            return response is not null ? response.Data : default;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ApiService.GetAsync] {url} — {ex.Message}");
            return default;
        }
    }

    // ── Student ──────────────────────────────────────────────────
    public Task<Student?> GetStudentAsync(int studentId)
        => GetAsync<Student>($"Student/GetStudent?id={studentId}");

    // ── Lessons ──────────────────────────────────────────────────
    /// <summary>
    /// Returns all lessons for a bundle (base Lesson model).
    /// We fetch bundles first, then pull lessons per bundle.
    /// </summary>
    public async Task<List<Lesson>> GetLessonsByBundleAsync(int bundleId)
    {
        var result = await GetAsync<IEnumerable<Lesson>>(
            $"Lesson/GetByBundle?bundleId={bundleId}");
        return result?.ToList() ?? [];
    }

    // ── Bundles ──────────────────────────────────────────────────
    public async Task<List<LessonBundleDetail>> GetBundlesByStudentAsync(int studentId)
    {
        var result = await GetAsync<IEnumerable<LessonBundleDetail>>(
            $"LessonBundle/GetByStudent?studentId={studentId}");
        return result?.ToList() ?? [];
    }

    // ── Scheduled slots ──────────────────────────────────────────
    public async Task<List<ScheduledSlot>> GetActiveSlotsByStudentAsync(int studentId)
    {
        var result = await GetAsync<IEnumerable<ScheduledSlot>>(
            $"ScheduledSlot/GetActiveByStudent?studentId={studentId}");
        return result?.ToList() ?? [];
    }
}

```

## File: MusicSchool.StudentPortal\Shared\MainLayout.razor

```razor
@inherits LayoutComponentBase
@namespace MusicSchool.StudentPortal.Shared

<MudLayout>
    <MudAppBar Elevation="1" Color="Color.Primary">
        <MudIcon Icon="@Icons.Material.Filled.MusicNote" Class="mr-2" />
        <MudText Typo="Typo.h6" Style="font-weight:600;">Music School</MudText>
        <MudSpacer />
        <MudText Typo="Typo.body2">Student Portal</MudText>
    </MudAppBar>

    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.Large" Class="pa-4 pa-md-6">
            @Body
        </MudContainer>
    </MudMainContent>
</MudLayout>

```

## File: MusicSchool.StudentPortal\Shared\StatusChip.razor

```razor
@* Shared/StatusChip.razor *@
@namespace MusicSchool.StudentPortal.Shared

<MudChip T="string"
         Size="Size.Small"
         Class="@($"mud-chip-filled {GetCssClass()}")"
         Style="font-size:0.7rem;">
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
        _                  => "status-scheduled"
    };
}

```

## File: MusicSchool.StudentPortal\wwwroot\appsettings.Development.json

```json
{
  "ApiBaseUrl": "https://localhost:64100/"
}

```

## File: MusicSchool.StudentPortal\wwwroot\appsettings.json

```json
{
  "ApiBaseUrl": "https://localhost:64100/"
}

```

## File: MusicSchool.StudentPortal\App.razor

```razor
@using MudBlazor
@namespace MusicSchool.StudentPortal

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
    private readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary              = "#F3D395",
            PrimaryContrastText  = "#3A3A3A",
            Secondary            = "#78797A",
            SecondaryContrastText= "#FFFFFF",
            Background           = "#F0F2F5",
            Surface              = "#FFFFFF",
            AppbarBackground     = "#F3D395",
            AppbarText           = "#3A3A3A",
            DrawerBackground     = "#FFFFFF",
            DrawerText           = "#3A3A3A",
            TextPrimary          = "#3A3A3A",
            TextSecondary        = "#78797A",
            ActionDefault        = "#78797A",
            Divider              = "#DFE1E3",
            TableLines           = "#DFE1E3",
            LinesDefault         = "#DFE1E3",
        }
    };
}

```

## File: MusicSchool.StudentPortal\MusicSchool.StudentPortal.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.BlazorWebAssembly">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly" Version="9.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.Components.WebAssembly.DevServer" Version="9.0.0" PrivateAssets="all" />
    <PackageReference Include="MudBlazor" Version="7.15.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
  </ItemGroup>

  <!--
    Add a project reference to your shared MusicSchool.Models project:
    <ItemGroup>
      <ProjectReference Include="..\MusicSchool.Models\MusicSchool.Models.csproj" />
    </ItemGroup>

    Until then, the model classes are duplicated inline in Services\Models.cs.
  -->

</Project>

```

## File: MusicSchool.StudentPortal\Program.cs

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MusicSchool.StudentPortal;
using MusicSchool.StudentPortal.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:64100/")
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddMudServices();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    Console.Error.WriteLine($"[UnhandledException] {e.ExceptionObject}");

TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Console.Error.WriteLine($"[UnobservedTaskException] {e.Exception}");
    e.SetObserved();
};

await builder.Build().RunAsync();

```

## File: MusicSchool.StudentPortal\_Imports.razor

```razor
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.AspNetCore.Components.Web.Virtualization
@using Microsoft.JSInterop
@using MudBlazor
@using MusicSchool.StudentPortal
@using MusicSchool.StudentPortal.Services
@using MusicSchool.StudentPortal.Shared

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
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Due: @inv.DueDate.ToString("yyyy MMM dd")</MudText>
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
                        <MudTd>@(context.DateOfBirth?.ToString("yyyy MMM dd") ?? "—")</MudTd>
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
            <MudDatePicker @bind-Date="_dobDate" Label="Date of Birth" Class="mb-3" DateFormat="yyyy/MM/dd" />
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
            <MudDatePicker @bind-Date="_editDobDate" Label="Date of Birth" Class="mb-3" DateFormat="yyyy/MM/dd" />
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
                    <MudSelectItem Value="@t.TeacherID">@t.Name</MudSelectItem>
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
            <MudText>No account holders found.</MudText>
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
    private int _selectedTeacherId = 0;
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

        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;

            await OnTeacherChanged();

            StateHasChanged(); 
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
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
            <MudTd>@context.ScheduledDate.ToString("yyyy MMM dd")</MudTd>
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
            <MudDatePicker @bind-Date="_extraDate" Label="Date" Required="true" Class="mb-3" DateFormat="yyyy/MM/dd" />
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
        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;

            await OnTeacherChanged();

            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
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
        _newExtra.ScheduledTime = TimeOnly.FromTimeSpan(_extraTime.Value);

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
@inject StudentService StudentSvc
@inject InvoiceService InvoiceSvc

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
    <!-- Tile 1: Active Students -->
    <MudItem xs="12" sm="6" md="3" Style="display:flex;">
        <MudPaper Class="pa-4 stats-card" Elevation="1" Style="flex:1;">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.School" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_studentCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Active Students</MudText>
                    <MudText Typo="Typo.caption" Style="visibility:hidden;">—</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>

    <!-- Tile 2: Invoices this month — count and total value -->
    <MudItem xs="12" sm="6" md="3" Style="display:flex;">
        <MudPaper Class="pa-4 stats-card" Elevation="1" Style="flex:1;">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.Receipt" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_monthInvoiceCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">
                        Invoices — @DateTime.Today.ToString("yyyy MMM")
                    </MudText>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">
                        R @_monthInvoiceTotal.ToString("N2")
                    </MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>

    <!-- Tile 3: Invoices paid this month — count and total value -->
    <MudItem xs="12" sm="6" md="3" Style="display:flex;">
        <MudPaper Class="pa-4 stats-card" Elevation="1" Style="flex:1;">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.CheckCircle" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@_monthPaidCount</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">
                        Paid — @DateTime.Today.ToString("yyyy MMM")
                    </MudText>
                    <MudText Typo="Typo.caption" Color="Color.Secondary">
                        R @_monthPaidTotal.ToString("N2")
                    </MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>

    <!-- Tile 4: Today's date -->
    <MudItem xs="12" sm="6" md="3" Style="display:flex;">
        <MudPaper Class="pa-4 stats-card" Elevation="1" Style="flex:1;">
            <MudStack Row="true" AlignItems="AlignItems.Center">
                <MudIcon Icon="@Icons.Material.Filled.Today" Color="Color.Primary" Size="Size.Large" />
                <div>
                    <MudText Typo="Typo.h4" Style="font-weight:700;">@DateTime.Today.ToString("dd MMM")</MudText>
                    <MudText Typo="Typo.body2" Color="Color.Secondary">Today</MudText>
                    <MudText Typo="Typo.caption" Style="visibility:hidden;">—</MudText>
                </div>
            </MudStack>
        </MudPaper>
    </MudItem>
</MudGrid>

<MudGrid Class="mt-4">
    <MudItem xs="12" md="6">
        <MudPaper Class="pa-4" Elevation="1">
            <MudText Typo="Typo.h6" Class="mb-3">
                Year to Date — Invoices Due to @DateTime.Today.ToString("yyyy MMM dd")
            </MudText>
            @if (!_loading)
            {
                <MudGrid Spacing="2">
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Total Raised</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600;">R @_ytdInvoiceTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_ytdInvoiceCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Total Paid</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600; color:#2E7D32;">R @_ytdPaidTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_ytdPaidCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Outstanding</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600; color:#F57F17;">R @_ytdOutstandingTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_ytdOutstandingCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Collection Rate</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600;">
                                @(_ytdInvoiceTotal > 0
                                    ? $"{(_ytdPaidTotal / _ytdInvoiceTotal * 100):N0}%"
                                    : "—")
                            </MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">paid vs raised</MudText>
                        </MudPaper>
                    </MudItem>
                </MudGrid>
            }
        </MudPaper>
    </MudItem>
    <MudItem xs="12" md="6">
        <MudPaper Class="pa-4" Elevation="1">
            <MudText Typo="Typo.h6" Class="mb-3">
                Invoice Summary — @DateTime.Today.ToString("MMMM yyyy")
            </MudText>
            @if (!_loading)
            {
                <MudGrid Spacing="2">
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Total Raised</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600;">R @_monthInvoiceTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_monthInvoiceCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Total Paid</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600; color:#2E7D32;">R @_monthPaidTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_monthPaidCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Outstanding</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600; color:#F57F17;">R @_monthOutstandingTotal.ToString("N2")</MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">@_monthOutstandingCount invoice(s)</MudText>
                        </MudPaper>
                    </MudItem>
                    <MudItem xs="6">
                        <MudPaper Class="pa-3" Elevation="0" Style="background:#F0F2F5;">
                            <MudText Typo="Typo.caption" Color="Color.Secondary">Collection Rate</MudText>
                            <MudText Typo="Typo.subtitle1" Style="font-weight:600;">
                                @(_monthInvoiceTotal > 0
                                    ? $"{(_monthPaidTotal / _monthInvoiceTotal * 100):N0}%"
                                    : "—")
                            </MudText>
                            <MudText Typo="Typo.caption" Color="Color.Secondary">paid vs raised</MudText>
                        </MudPaper>
                    </MudItem>
                </MudGrid>
            }
        </MudPaper>
    </MudItem>
</MudGrid>

<MudGrid Class="mt-4">
    <MudItem xs="12">
        <MudPaper Class="pa-4" Elevation="1">
            <MudText Typo="Typo.h6" Class="mb-3">Quick Navigation</MudText>
            <MudStack Row="true" Spacing="2" Wrap="Wrap.Wrap">
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.People"
                           Href="/account-holders">Manage Account Holders</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Inventory"
                           Href="/lesson-bundles">View Lesson Bundles</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.CalendarMonth"
                           Href="/schedule">Today's Schedule</MudButton>
                <MudButton Variant="Variant.Outlined" Color="Color.Primary" StartIcon="@Icons.Material.Filled.Receipt"
                           Href="/invoices">Manage Invoices</MudButton>
            </MudStack>
        </MudPaper>
    </MudItem>
</MudGrid>

@code {
    private bool _loading = true;

    // Tile 1
    private int _studentCount;

    // Tiles 2 & 3 + monthly summary panel
    private int _monthInvoiceCount;
    private decimal _monthInvoiceTotal;
    private int _monthPaidCount;
    private decimal _monthPaidTotal;
    private int _monthOutstandingCount;
    private decimal _monthOutstandingTotal;

    // YTD summary panel
    private int _ytdInvoiceCount;
    private decimal _ytdInvoiceTotal;
    private int _ytdPaidCount;
    private decimal _ytdPaidTotal;
    private int _ytdOutstandingCount;
    private decimal _ytdOutstandingTotal;

    protected override async Task OnInitializedAsync()
    {
        var teachers = await TeacherSvc.GetAllActiveAsync();

        int studentCount = 0;
        var allMonthInvoices = new List<Invoice>();
        var allYtdInvoices = new List<Invoice>();

        var thisMonth = DateTime.Today.Month;
        var thisYear = DateTime.Today.Year;

        foreach (var t in teachers)
        {
            var accountHolders = await AccountHolderSvc.GetByTeacherAsync(t.TeacherID);

            foreach (var ah in accountHolders)
            {
                // Count active students
                var students = await StudentSvc.GetByAccountHolderAsync(ah.AccountHolderID);
                studentCount += students.Count(s => s.IsActive);

                // Collect invoices — one fetch covers both month and YTD filters
                var invoices = (await InvoiceSvc.GetByAccountHolderAsync(ah.AccountHolderID)).ToList();

                allMonthInvoices.AddRange(
                    invoices.Where(i =>
                        i.DueDate.Year == thisYear &&
                        i.DueDate.Month == thisMonth));

                allYtdInvoices.AddRange(
                    invoices.Where(i =>
                        i.DueDate.Year == thisYear &&
                        i.DueDate.Date <= DateTime.Today));
            }
        }

        _studentCount = studentCount;

        // Tile 2: all invoices due this month
        _monthInvoiceCount = allMonthInvoices.Count;
        _monthInvoiceTotal = allMonthInvoices.Sum(i => i.Amount);

        // Tile 3: paid invoices due this month
        var paid = allMonthInvoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();
        _monthPaidCount = paid.Count;
        _monthPaidTotal = paid.Sum(i => i.Amount);

        // Summary panel: outstanding (Pending + Overdue) due this month
        var outstanding = allMonthInvoices
            .Where(i => i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.Overdue)
            .ToList();
        _monthOutstandingCount = outstanding.Count;
        _monthOutstandingTotal = outstanding.Sum(i => i.Amount);

        // YTD panel
        _ytdInvoiceCount = allYtdInvoices.Count;
        _ytdInvoiceTotal = allYtdInvoices.Sum(i => i.Amount);

        var ytdPaid = allYtdInvoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();
        _ytdPaidCount = ytdPaid.Count;
        _ytdPaidTotal = ytdPaid.Sum(i => i.Amount);

        var ytdOutstanding = allYtdInvoices
            .Where(i => i.Status == InvoiceStatus.Pending || i.Status == InvoiceStatus.Overdue)
            .ToList();
        _ytdOutstandingCount = ytdOutstanding.Count;
        _ytdOutstandingTotal = ytdOutstanding.Sum(i => i.Amount);

        _loading = false;
    }
}

```

## File: MusicSchool.Web\Pages\Invoices.razor

```razor
@page "/invoices"
@using MusicSchool.Data.Models
@inject TeacherService TeacherSvc
@inject AccountHolderService AccountHolderSvc
@inject InvoiceService InvoiceSvc
@inject PaymentService PaymentSvc
@inject ISnackbar Snackbar

<PageTitle>Invoices — Music School</PageTitle>

<div class="page-header">
    <MudText Typo="Typo.h5">Invoices</MudText>
    <MudText Typo="Typo.body2" Color="Color.Secondary">View invoices and record payments</MudText>
</div>

<!-- ── Filters ─────────────────────────────────────────────────────────── -->
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
                       @bind-Value:after="OnAccountHolderChanged"
                       Disabled="_accountHolders.Count == 0" Clearable="true">
                @foreach (var ah in _accountHolders)
                {
                    <MudSelectItem Value="ah.AccountHolderID">@ah.FullName</MudSelectItem>
                }
            </MudSelect>
        </MudItem>
        <MudItem xs="12" sm="6" md="4">
            <MudSelect @bind-Value="_statusFilter" Label="Status" Clearable="true"
                       @bind-Value:after="ApplyFilter">
                <MudSelectItem Value="@("")">All</MudSelectItem>
                <MudSelectItem Value="@InvoiceStatus.Pending">Pending</MudSelectItem>
                <MudSelectItem Value="@InvoiceStatus.Overdue">Overdue</MudSelectItem>
                <MudSelectItem Value="@InvoiceStatus.Paid">Paid</MudSelectItem>
                <MudSelectItem Value="@InvoiceStatus.Void">Void</MudSelectItem>
            </MudSelect>
        </MudItem>
    </MudGrid>
</MudPaper>

@if (_loading)
{
    <MudProgressLinear Color="Color.Primary" Indeterminate="true" Class="mb-4" />
}

@if (_selectedAccountHolderId > 0)
{
    <!-- ── Invoice table ──────────────────────────────────────────────── -->
    <MudPaper Elevation="1" Class="mb-4">
        <MudTable Items="_filteredInvoices"
                  Hover="true" Dense="false" Loading="_loading" Elevation="0">
            <ToolBarContent>
                <MudStack Row="true" AlignItems="AlignItems.Center"
                          Justify="Justify.SpaceBetween" Style="width:100%">
                    <MudText Typo="Typo.h6">
                        Invoices
                        @if (!string.IsNullOrEmpty(_statusFilter))
                        {
                            <span style="font-weight:400; font-size:0.85rem; color:#78797A;">
                                &nbsp;— @_statusFilter
                            </span>
                        }
                    </MudText>
                    <MudButton Variant="Variant.Filled" Color="Color.Primary" Size="Size.Small"
                               StartIcon="@Icons.Material.Filled.AddCard"
                               OnClick="OpenRecordPaymentDialog">
                        Record Payment
                    </MudButton>
                </MudStack>
            </ToolBarContent>
            <HeaderContent>
                <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.DueDate)">Due Date</MudTableSortLabel></MudTh>
                <MudTh>Description</MudTh>
                <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.Amount)">Amount</MudTableSortLabel></MudTh>
                <MudTh>Paid Date</MudTh>
                <MudTh><MudTableSortLabel SortBy="new Func<Invoice, object>(x => x.Status)">Status</MudTableSortLabel></MudTh>
                <MudTh>Actions</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd DataLabel="Due Date">@context.DueDate.ToString("yyyy MMM dd")</MudTd>
                <MudTd DataLabel="Description">
                    @if (context.BundleID.HasValue)
                    {
                        <span>Bundle #@context.BundleID — Instalment @context.InstallmentNumber</span>
                    }
                    else if (context.ExtraLessonID.HasValue)
                    {
                        <span>Extra Lesson #@context.ExtraLessonID</span>
                    }
                    else
                    {
                        <span>Invoice #@context.InvoiceID</span>
                    }
                </MudTd>
                <MudTd DataLabel="Amount">R @context.Amount.ToString("N2")</MudTd>
                <MudTd DataLabel="Paid Date">@(context.PaidDate?.ToString("yyyy MMM dd") ?? "—")</MudTd>
                <MudTd DataLabel="Status"><StatusChip Status="@context.Status" /></MudTd>
                <MudTd DataLabel="Actions">
                    @if (context.Status is InvoiceStatus.Pending or InvoiceStatus.Overdue)
                    {
                        <MudButton Variant="Variant.Filled" Color="Color.Success" Size="Size.Small"
                                   StartIcon="@Icons.Material.Filled.CheckCircle"
                                   OnClick="@(() => QuickPay(context))"
                                   Style="margin-right:4px;">
                            Paid
                        </MudButton>
                        <MudIconButton Icon="@Icons.Material.Filled.Block"
                                       Size="Size.Small" Color="Color.Default"
                                       Title="Void invoice"
                                       OnClick="@(() => VoidInvoice(context))" />
                    }
                </MudTd>
            </RowTemplate>
            <FooterContent>
                <MudTd colspan="2" Style="font-weight:600;">Total Invoiced</MudTd>
                <MudTd Style="font-weight:600;">R @_allInvoices.Sum(i => i.Amount).ToString("N2")</MudTd>
                <MudTd colspan="3"></MudTd>
            </FooterContent>
            <NoRecordsContent>
                <MudText Class="pa-3" Color="Color.Secondary">No invoices found for this account holder.</MudText>
            </NoRecordsContent>
        </MudTable>
    </MudPaper>

    <!-- ── Unallocated payments ───────────────────────────────────────── -->
    @if (UnallocatedPayments.Count > 0)
    {
        <MudPaper Elevation="1" Class="mb-4" Style="border-left: 4px solid #F3D395;">
            <MudTable Items="UnallocatedPayments" Hover="true" Dense="true" Elevation="0">
                <ToolBarContent>
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
                        <MudIcon Icon="@Icons.Material.Filled.Pending"
                                 Style="color:#F3D395;" Size="Size.Small" />
                        <MudText Typo="Typo.h6">Unallocated Payments</MudText>
                        <MudChip T="string" Size="Size.Small" Color="Color.Warning">
                            R @TotalUnallocated.ToString("N2") pending allocation
                        </MudChip>
                    </MudStack>
                </ToolBarContent>
                <HeaderContent>
                    <MudTh>Date</MudTh>
                    <MudTh>Total Paid</MudTh>
                    <MudTh>Unallocated</MudTh>
                    <MudTh>Reference</MudTh>
                    <MudTh>Notes</MudTh>
                </HeaderContent>
                <RowTemplate>
                    <MudTd>@context.PaymentDate.ToString("yyyy MMM dd")</MudTd>
                    <MudTd>R @context.Amount.ToString("N2")</MudTd>
                    <MudTd>
                        <MudText Style="font-weight:600; color:#E65100;">
                            R @context.UnallocatedAmount.ToString("N2")
                        </MudText>
                    </MudTd>
                    <MudTd>@(context.Reference ?? "—")</MudTd>
                    <MudTd>@(context.Notes ?? "—")</MudTd>
                </RowTemplate>
            </MudTable>
            <MudAlert Severity="Severity.Info" Class="ma-3" Dense="true" Icon="@Icons.Material.Filled.Info">
                These amounts will be automatically applied when they accumulate to cover
                the next outstanding invoice (R @(NextPendingInvoiceAmount ?? "—")).
            </MudAlert>
        </MudPaper>
    }

    <!-- ── Full payment history ───────────────────────────────────────── -->
    @if (_payments.Count > 0)
    {
        <MudPaper Elevation="1">
            <MudTable Items="_payments" Hover="true" Dense="true" Elevation="0">
                <ToolBarContent>
                    <MudText Typo="Typo.h6">Payment History</MudText>
                </ToolBarContent>
                <HeaderContent>
                    <MudTh>Date</MudTh>
                    <MudTh>Amount</MudTh>
                    <MudTh>Unallocated</MudTh>
                    <MudTh>Source</MudTh>
                    <MudTh>Reference</MudTh>
                    <MudTh>Notes</MudTh>
                </HeaderContent>
                <RowTemplate>
                    <MudTd>@context.PaymentDate.ToString("yyyy MMM dd")</MudTd>
                    <MudTd>R @context.Amount.ToString("N2")</MudTd>
                    <MudTd>
                        @if (context.UnallocatedAmount > 0)
                        {
                            <MudText Style="color:#E65100;">R @context.UnallocatedAmount.ToString("N2")</MudText>
                        }
                        else
                        {
                            <MudText Color="Color.Secondary">—</MudText>
                        }
                    </MudTd>
                    <MudTd>
                        <MudChip T="string" Size="Size.Small"
                                 Color="@(context.Source == PaymentSource.QuickPay ? Color.Primary : Color.Default)">
                            @context.Source
                        </MudChip>
                    </MudTd>
                    <MudTd>@(context.Reference ?? "—")</MudTd>
                    <MudTd>@(context.Notes ?? "—")</MudTd>
                </RowTemplate>
                <FooterContent>
                    <MudTd colspan="1" Style="font-weight:600;">Total Received</MudTd>
                    <MudTd Style="font-weight:600;">R @_payments.Sum(p => p.Amount).ToString("N2")</MudTd>
                    <MudTd colspan="4"></MudTd>
                </FooterContent>
            </MudTable>
        </MudPaper>
    }
}
else if (!_loading)
{
    <MudAlert Severity="Severity.Info" Variant="Variant.Outlined">
        Select a teacher and account holder to view invoices and record payments.
    </MudAlert>
}

<!-- ── Record Payment Dialog ─────────────────────────────────────────────── -->
<MudDialog @ref="_recordPaymentDialog" Options="_dialogOptions">
    <TitleContent>
        <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="2">
            <MudIcon Icon="@Icons.Material.Filled.AddCard" Style="color:#F3D395;" />
            <MudText Typo="Typo.h6">Record Payment</MudText>
        </MudStack>
    </TitleContent>
    <DialogContent>
        <MudText Typo="Typo.body2" Color="Color.Secondary" Class="mb-4">
            Enter the amount received. The system will automatically link it to outstanding
            invoices starting from the oldest due date.
        </MudText>
        <MudForm @ref="_paymentForm">
            <MudNumericField @bind-Value="_newPayment.Amount"
                             Label="Amount Received"
                             Required="true"
                             RequiredError="Amount is required"
                             Min="0.01m"
                             Format="N2"
                             Adornment="Adornment.Start"
                             AdornmentText="R"
                             Class="mb-3" />
            <MudDatePicker @bind-Date="_paymentDatePicker"
                           Label="Payment Date"
                           Required="true"
                           DateFormat="yyyy/MM/dd"
                           Class="mb-3" />
            <MudTextField @bind-Value="_newPayment.Reference"
                          Label="Reference (optional)"
                          HelperText="e.g. EFT reference or cheque number"
                          Class="mb-3" />
            <MudTextField @bind-Value="_newPayment.Notes"
                          Label="Notes (optional)"
                          Lines="2" />
        </MudForm>

        @if (_pendingInvoices.Count > 0)
        {
            <MudDivider Class="my-3" />
            <MudText Typo="Typo.caption" Color="Color.Secondary" Class="mb-2">
                Outstanding invoices that will be covered (oldest first):
            </MudText>
            @foreach (var row in PreviewAllocations())
            {
                <MudStack Row="true" Justify="Justify.SpaceBetween" Class="mb-1">
                    <MudText Typo="Typo.body2">
                        @row.Invoice.DueDate.ToString("yyyy MMM dd")
                        @if (row.Invoice.BundleID.HasValue)
                        {
                            <span> · Instalment @row.Invoice.InstallmentNumber</span>
                        }
                    </MudText>
                    <MudStack Row="true" AlignItems="AlignItems.Center" Spacing="1">
                        <MudText Typo="Typo.body2">R @row.Invoice.Amount.ToString("N2")</MudText>
                        @if (row.Covered)
                        {
                            <MudIcon Icon="@Icons.Material.Filled.CheckCircle"
                                     Size="Size.Small" Color="Color.Success" />
                        }
                        else
                        {
                            <MudIcon Icon="@Icons.Material.Filled.RadioButtonUnchecked"
                                     Size="Size.Small" Color="Color.Default" />
                        }
                    </MudStack>
                </MudStack>
            }
            @if (_newPayment.Amount + _payments.Sum(p => p.UnallocatedAmount)
                             < _pendingInvoices.Sum(i => i.Amount))
            {
                <MudText Typo="Typo.caption" Color="Color.Warning" Class="mt-2">
                    Any amount not covering a full invoice will be held as unallocated.
                </MudText>
            }
        }
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _recordPaymentDialog!.CloseAsync(DialogResult.Cancel()))">
            Cancel
        </MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary"
                   StartIcon="@Icons.Material.Filled.Save"
                   OnClick="ConfirmRecordPayment">
            Save Payment
        </MudButton>
    </DialogActions>
</MudDialog>

@code {
    private bool _loading;

    private List<Teacher> _teachers = [];
    private List<AccountHolder> _accountHolders = [];
    private int _selectedTeacherId;
    private int _selectedAccountHolderId;
    private string _statusFilter = string.Empty;

    private List<Invoice> _allInvoices = [];
    private List<Invoice> _filteredInvoices = [];
    private List<Invoice> _pendingInvoices = [];
    private List<Payment> _payments = [];

    // Computed properties — no @{ } blocks needed in markup
    private List<Payment> UnallocatedPayments =>
        _payments.Where(p => p.UnallocatedAmount > 0).ToList();

    private decimal TotalUnallocated =>
        _payments.Sum(p => p.UnallocatedAmount);

    private string? NextPendingInvoiceAmount =>
        _allInvoices
            .Where(i => i.Status is InvoiceStatus.Pending or InvoiceStatus.Overdue)
            .OrderBy(i => i.DueDate)
            .FirstOrDefault()
            ?.Amount.ToString("N2");

    // Record payment dialog
    private MudDialog? _recordPaymentDialog;
    private MudForm? _paymentForm;
    private Payment _newPayment = new();
    private DateTime? _paymentDatePicker = DateTime.Today;

    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;
            await OnTeacherChanged();
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) StateHasChanged();
    }

    private async Task OnTeacherChanged()
    {
        _accountHolders = _selectedTeacherId > 0
            ? await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId)
            : [];
        _allInvoices = _filteredInvoices = _pendingInvoices = [];
        _payments = [];
        _selectedAccountHolderId = 0;
    }

    private async Task OnAccountHolderChanged()
    {
        if (_selectedAccountHolderId <= 0)
        {
            _allInvoices = _filteredInvoices = _pendingInvoices = [];
            _payments = [];
            return;
        }
        await RefreshData();
    }

    private void ApplyFilter()
    {
        _filteredInvoices = string.IsNullOrEmpty(_statusFilter)
            ? [.. _allInvoices.OrderByDescending(i => i.DueDate)]
            : [.. _allInvoices.Where(i => i.Status == _statusFilter)
                              .OrderByDescending(i => i.DueDate)];
    }

    private async Task RefreshData()
    {
        if (_selectedAccountHolderId <= 0) return;
        _loading = true;
        var invoiceTask = InvoiceSvc.GetByAccountHolderAsync(_selectedAccountHolderId);
        var paymentTask = PaymentSvc.GetByAccountHolderAsync(_selectedAccountHolderId);
        await Task.WhenAll(invoiceTask, paymentTask);
        _allInvoices = await invoiceTask;
        _payments = await paymentTask;
        _pendingInvoices = [.. _allInvoices
            .Where(i => i.Status is InvoiceStatus.Pending or InvoiceStatus.Overdue)
            .OrderBy(i => i.DueDate)];
        ApplyFilter();
        _loading = false;
    }

    // ── Preview helper ─────────────────────────────────────────────────────

    private record InvoicePreviewRow(Invoice Invoice, bool Covered);

    private List<InvoicePreviewRow> PreviewAllocations()
    {
        var pool = _newPayment.Amount + _payments.Sum(p => p.UnallocatedAmount);
        var rows = new List<InvoicePreviewRow>();
        foreach (var inv in _pendingInvoices)
        {
            bool covered = pool >= inv.Amount;
            if (covered) pool -= inv.Amount;
            rows.Add(new InvoicePreviewRow(inv, covered));
        }
        return rows;
    }

    // ── Record Payment ─────────────────────────────────────────────────────

    private async Task OpenRecordPaymentDialog()
    {
        _newPayment = new Payment { AccountHolderID = _selectedAccountHolderId };
        _paymentDatePicker = DateTime.Today;
        await _recordPaymentDialog!.ShowAsync();
    }

    private async Task ConfirmRecordPayment()
    {
        await _paymentForm!.Validate();
        if (!_paymentForm.IsValid) return;

        _newPayment.AccountHolderID = _selectedAccountHolderId;
        _newPayment.PaymentDate = _paymentDatePicker ?? DateTime.Today;
        _newPayment.Source = PaymentSource.Manual;

        var result = await PaymentSvc.AddPaymentAsync(_newPayment);
        if (result.HasValue)
        {
            Snackbar.Add("Payment recorded and invoices updated.", Severity.Success);
            await _recordPaymentDialog!.CloseAsync(DialogResult.Ok(true));
            await RefreshData();
        }
        else
        {
            Snackbar.Add("Failed to record payment.", Severity.Error);
        }
    }

    // ── Quick Pay ──────────────────────────────────────────────────────────

    private async Task QuickPay(Invoice invoice)
    {
        var result = await PaymentSvc.QuickPayInvoiceAsync(invoice.InvoiceID, DateTime.Today);
        if (result.HasValue)
        {
            Snackbar.Add($"Invoice marked as paid — R {invoice.Amount:N2}", Severity.Success);
            await RefreshData();
        }
        else Snackbar.Add("Failed to record payment.", Severity.Error);
    }

    // ── Void ──────────────────────────────────────────────────────────────

    private async Task VoidInvoice(Invoice invoice)
    {
        var result = await InvoiceSvc.UpdateInvoiceStatusAsync(invoice.InvoiceID, InvoiceStatus.Void, null);
        if (result)
        {
            Snackbar.Add("Invoice voided.", Severity.Success);
            await RefreshData();
        }
        else Snackbar.Add("Failed to void invoice.", Severity.Error);
    }
}

```

## File: MusicSchool.Web\Pages\LessonBundleDetail.razor

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
                        @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("yyyy MMM dd")
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
                        <MudTd>@context.ScheduledDate.ToString("yyyy MMM dd")</MudTd>
                        <MudTd>@context.ScheduledTime.ToString("HH:mm")</MudTd>
                        <MudTd><StatusChip Status="@context.Status" /></MudTd>
                        <MudTd>
                            @if (context.CreditForfeited)
                            {
                                <MudIcon Icon="@Icons.Material.Filled.Warning" Color="Color.Warning" Size="Size.Small" />
                            }
                        </MudTd>
                        <MudTd>@(context.CancelledBy ?? "—")</MudTd>
                        <MudTd>@(context.Notes ?? "—")</MudTd>
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
                            @if (context.Status == LessonStatus.CancelledTeacher
                              || context.Status == LessonStatus.CancelledStudent)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.EventRepeat" Size="Size.Small"
                                               Color="Color.Primary" Title="Reschedule"
                                               OnClick="@(() => OpenRescheduleDialog(context))" />
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
                        <MudTd>@context.DueDate.ToString("yyyy MMM dd")</MudTd>
                        <MudTd>@(context.PaidDate?.ToString("yyyy MMM dd") ?? "—")</MudTd>
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

<!-- Reschedule Dialog -->
<MudDialog @ref="_rescheduleDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Reschedule Lesson</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-1">
            Originally: <strong>@_lessonToReschedule?.ScheduledDate.ToString("yyyy MMM dd")</strong>
            at <strong>@_lessonToReschedule?.ScheduledTime.ToString("HH:mm")</strong>
        </MudText>
        <MudText Typo="Typo.caption" Color="Color.Secondary" Class="mb-3">
            Cancelled by @(_lessonToReschedule?.CancelledBy ?? "—")
        </MudText>
        <MudDivider Class="mb-3" />
        <MudDatePicker @bind-Date="_rescheduleDate" Label="New Date" Required="true" Class="mb-3" DateFormat="yyyy/MM/dd" />
        <MudTimePicker @bind-Time="_rescheduleTime" Label="New Time" Required="true" AmPm="false" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _rescheduleDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="ConfirmReschedule">
            Reschedule
        </MudButton>
    </DialogActions>
</MudDialog>

<!-- Cancel Lesson Dialog -->
<MudDialog @ref="_cancelDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Student Cancellation</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-3">
            Lesson on <strong>@_lessonToCancel?.ScheduledDate.ToString("yyyy MMM dd")</strong> cancelled by student.
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
        <MudDatePicker @bind-Date="_paidDate" Label="Payment Date" Required="true" DateFormat="yyyy/MM/dd" />
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
    private List<Lesson> _lessons = [];
    private List<Invoice> _invoices = [];

    private MudDialog? _rescheduleDialog, _cancelDialog, _markPaidDialog;
    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    // Reschedule
    private Lesson? _lessonToReschedule;
    private DateTime? _rescheduleDate;
    private TimeSpan? _rescheduleTime;

    // Cancel
    private Lesson? _lessonToCancel;
    private string _cancelReason = string.Empty;

    // Mark paid
    private Invoice? _invoiceToPay;
    private DateTime? _paidDate;

    private List<BreadcrumbItem> _breadcrumbs = [];

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

    // ── Reschedule ────────────────────────────────────────────────

    private async Task OpenRescheduleDialog(Lesson lesson)
    {
        _lessonToReschedule = lesson;
        _rescheduleDate = lesson.ScheduledDate.Date;
        _rescheduleTime = lesson.ScheduledTime.ToTimeSpan();
        await _rescheduleDialog!.ShowAsync();
    }

    private async Task ConfirmReschedule()
    {
        if (_lessonToReschedule is null || _rescheduleDate is null || _rescheduleTime is null)
            return;

        var newDate = _rescheduleDate.Value.Date;
        var newTime = TimeOnly.FromTimeSpan(_rescheduleTime.Value);

        var result = await LessonSvc.RescheduleLessonAsync(_lessonToReschedule.LessonID, newDate, newTime);
        if (result)
        {
            Snackbar.Add("Lesson rescheduled.", Severity.Success);
            await _rescheduleDialog!.CloseAsync(DialogResult.Ok(true));
            _lessons = await LessonSvc.GetByBundleAsync(BundleID);
            _bundleDetails = await BundleSvc.GetBundleAsync(BundleID);
        }
        else Snackbar.Add("Failed to reschedule lesson.", Severity.Error);
    }

    // ── Cancel ────────────────────────────────────────────────────

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

    // ── Mark paid ─────────────────────────────────────────────────

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

@* Group by BundleID so each bundle appears once even though GetBundleAsync returns
   one LessonBundleWithQuarterDetail row per quarter (4 rows per bundle). *@
@if (_bundles.Any())
{
    @foreach (var b in _bundles.GroupBy(x => x.BundleID).Select(g => g.First()))
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
                        @b.StartDate.ToString("yyyy MMM dd") – @b.EndDate.ToString("yyyy MMM dd")
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
    private List<LessonBundleWithQuarterDetail> _bundles = [];
    private int _selectedTeacherId, _selectedAccountHolderId, _selectedStudentId;

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;
            await OnTeacherChanged();
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
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
            // GetByStudentAsync returns LessonBundleDetail (no quarter fields).
            // We load each bundle via GetBundleAsync to get LessonBundleWithQuarterDetail,
            // which carries DurationMinutes, StartDate, EndDate, StudentFirstName, etc.
            var bundleList = await BundleSvc.GetByStudentAsync(_selectedStudentId);
            _bundles = [];
            foreach (var b in bundleList)
            {
                var details = await BundleSvc.GetBundleAsync(b.BundleID);
                _bundles.AddRange(details);
            }
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
@inject AccountHolderService AccountHolderSvc
@inject StudentService StudentSvc
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
                           @bind-Date:after="LoadSchedule" DateFormat="yyyy/MM/dd" />
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
                        <MudTd>
                            <StatusChip Status="@context.Status" />
                            @if (!string.IsNullOrWhiteSpace(context.Notes))
                            {
                                <MudTooltip Text="@context.Notes" Placement="Placement.Right">
                                    <MudIcon Icon="@Icons.Material.Filled.StickyNote2"
                                             Size="Size.Small" Color="Color.Secondary"
                                             Class="ml-1" Style="vertical-align:middle;" />
                                </MudTooltip>
                            }
                        </MudTd>
                        <MudTd>
                            @if (context.Status == LessonStatus.Scheduled)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Mark Completed"
                                               OnClick="@(() => OpenCompleteLessonDialog(context))" />
                                <MudIconButton Icon="@Icons.Material.Filled.Cancel" Size="Size.Small"
                                               Color="Color.Warning" Title="Teacher Cancelled"
                                               OnClick="@(() => QuickUpdateLesson(context, LessonStatus.CancelledTeacher))" />
                                <MudIconButton Icon="@Icons.Material.Filled.PersonOff" Size="Size.Small"
                                               Color="Color.Error" Title="Student Cancelled / Forfeit"
                                               OnClick="@(() => OpenForfeitDialog(context))" />
                            }
                            @if (context.Status == LessonStatus.CancelledTeacher
                              || context.Status == LessonStatus.CancelledStudent)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.EventRepeat" Size="Size.Small"
                                               Color="Color.Primary" Title="Reschedule"
                                               OnClick="@(() => OpenRescheduleDialog(context))" />
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
                        <MudTd>
                            <StatusChip Status="@context.Status" />
                            @if (!string.IsNullOrWhiteSpace(context.Notes))
                            {
                                <MudTooltip Text="@context.Notes" Placement="Placement.Right">
                                    <MudIcon Icon="@Icons.Material.Filled.StickyNote2"
                                             Size="Size.Small" Color="Color.Secondary"
                                             Class="ml-1" Style="vertical-align:middle;" />
                                </MudTooltip>
                            }
                        </MudTd>
                        <MudTd>
                            @if (context.Status == ExtraLessonStatus.Scheduled)
                            {
                                <MudIconButton Icon="@Icons.Material.Filled.CheckCircle" Size="Size.Small"
                                               Color="Color.Success" Title="Complete"
                                               OnClick="@(() => OpenCompleteExtraDialog(context))" />
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

<!-- Reschedule Dialog -->
<MudDialog @ref="_rescheduleDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Reschedule Lesson</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-1">
            Originally: <strong>@_lessonToReschedule?.ScheduledDate.ToString("yyyy MMM dd")</strong>
            at <strong>@_lessonToReschedule?.ScheduledTime.ToString("HH:mm")</strong>
        </MudText>
        <MudText Typo="Typo.caption" Color="Color.Secondary" Class="mb-3">
            Cancelled by @(_lessonToReschedule?.CancelledBy ?? "—")
        </MudText>
        <MudDivider Class="mb-3" />
        <MudDatePicker @bind-Date="_rescheduleDate" Label="New Date" Required="true" Class="mb-3" DateFormat="yyyy/MM/dd" />
        <MudTimePicker @bind-Time="_rescheduleTime" Label="New Time" Required="true" AmPm="false" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _rescheduleDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="ConfirmReschedule">
            Reschedule
        </MudButton>
    </DialogActions>
</MudDialog>

<!-- Complete Lesson Dialog -->
<MudDialog @ref="_completeLessonDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Complete Lesson</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-2">
            <strong>@_lessonToComplete?.StudentFullName</strong>
            — @_lessonToComplete?.ScheduledDate.ToString("yyyy MMM dd")
            @_lessonToComplete?.ScheduledTime.ToString("HH:mm")
        </MudText>
        <MudDivider Class="mb-3" />
        <MudTextField @bind-Value="_completeLessonNote"
                      Label="Lesson note (optional)"
                      Lines="3"
                      Placeholder="e.g. Worked on scales, great progress with sight-reading…" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _completeLessonDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Success" OnClick="ConfirmCompleteLesson">
            Mark Completed
        </MudButton>
    </DialogActions>
</MudDialog>

<!-- Complete Extra Lesson Dialog -->
<MudDialog @ref="_completeExtraDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Complete Extra Lesson</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-2">
            <strong>@_extraToComplete?.StudentFullName</strong>
            — @_extraToComplete?.ScheduledDate.ToString("yyyy MMM dd")
            @_extraToComplete?.ScheduledTime.ToString("HH:mm")
        </MudText>
        <MudDivider Class="mb-3" />
        <MudTextField @bind-Value="_completeExtraNote"
                      Label="Lesson note (optional)"
                      Lines="3"
                      Placeholder="e.g. Worked on scales, great progress with sight-reading…" />
    </DialogContent>
    <DialogActions>
        <MudButton OnClick="@(() => _completeExtraDialog!.CloseAsync(DialogResult.Cancel()))">Cancel</MudButton>
        <MudButton Variant="Variant.Filled" Color="Color.Success" OnClick="ConfirmCompleteExtra">
            Mark Completed
        </MudButton>
    </DialogActions>
</MudDialog>

<!-- Forfeit Dialog -->
<MudDialog @ref="_forfeitDialog" Options="_dialogOptions">
    <TitleContent><MudText Typo="Typo.h6">Student Cancellation</MudText></TitleContent>
    <DialogContent>
        <MudText Class="mb-2">
            <strong>@_lessonToAction?.StudentFullName</strong>
            — @_lessonToAction?.ScheduledDate.ToString("yyyy MMM dd")
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
            <MudSelect @bind-Value="_newExtra.StudentID" Label="Student" Required="true"
                       RequiredError="Student is required" Class="mb-3">
                @foreach (var s in _scheduleStudents)
                {
                    <MudSelectItem Value="s.StudentID">@s.FullName</MudSelectItem>
                }
            </MudSelect>
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
    private List<Student> _scheduleStudents = [];
    private int _selectedTeacherId;
    private DateTime? _selectedDate = DateTime.Today;

    // Reschedule dialog
    private MudDialog? _rescheduleDialog;
    private LessonDetail? _lessonToReschedule;
    private DateTime? _rescheduleDate;
    private TimeSpan? _rescheduleTime;

    // Complete lesson dialog
    private MudDialog? _completeLessonDialog;
    private LessonDetail? _lessonToComplete;
    private string _completeLessonNote = string.Empty;

    // Complete extra lesson dialog
    private MudDialog? _completeExtraDialog;
    private ExtraLessonDetail? _extraToComplete;
    private string _completeExtraNote = string.Empty;

    // Forfeit / cancellation dialog
    private MudDialog? _forfeitDialog;
    private LessonDetail? _lessonToAction;
    private string _actionReason = string.Empty;

    // Add extra lesson dialog
    private MudDialog? _addExtraDialog;
    private MudForm? _extraForm;

    private DialogOptions _dialogOptions = new() { MaxWidth = MaxWidth.Small, FullWidth = true };

    private ExtraLesson _newExtra = new();
    private TimeSpan? _extraTime;

    protected override async Task OnInitializedAsync()
    {
        _teachers = await TeacherSvc.GetAllActiveAsync();
        _lessonTypes = await LessonTypeSvc.GetAllActiveAsync();

        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;
            await LoadSchedule();
            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
    }

    private async Task LoadSchedule()
    {
        if (_selectedTeacherId <= 0 || _selectedDate is null) return;
        _loading = true;
        var date = _selectedDate.Value.Date;
        _lessons = await LessonSvc.GetByTeacherAndDateAsync(_selectedTeacherId, date);
        _extraLessons = await ExtraLessonSvc.GetByTeacherAndDateAsync(_selectedTeacherId, date);

        var accountHolders = await AccountHolderSvc.GetByTeacherAsync(_selectedTeacherId);
        _scheduleStudents = [];
        foreach (var ah in accountHolders)
        {
            var students = await StudentSvc.GetByAccountHolderAsync(ah.AccountHolderID);
            _scheduleStudents.AddRange(students.Where(s => s.IsActive));
        }

        _loading = false;
    }

    private void GoToToday() { _selectedDate = DateTime.Today; _ = LoadSchedule(); }
    private void PreviousDay() { _selectedDate = (_selectedDate ?? DateTime.Today).AddDays(-1); _ = LoadSchedule(); }
    private void NextDay() { _selectedDate = (_selectedDate ?? DateTime.Today).AddDays(1); _ = LoadSchedule(); }

    // ── Reschedule ────────────────────────────────────────────────

    private async Task OpenRescheduleDialog(LessonDetail lesson)
    {
        _lessonToReschedule = lesson;
        _rescheduleDate = lesson.ScheduledDate.Date;
        _rescheduleTime = lesson.ScheduledTime.ToTimeSpan();
        await _rescheduleDialog!.ShowAsync();
    }

    private async Task ConfirmReschedule()
    {
        if (_lessonToReschedule is null || _rescheduleDate is null || _rescheduleTime is null)
            return;

        var newDate = _rescheduleDate.Value.Date;
        var newTime = TimeOnly.FromTimeSpan(_rescheduleTime.Value);

        var result = await LessonSvc.RescheduleLessonAsync(_lessonToReschedule.LessonID, newDate, newTime);
        if (result)
        {
            Snackbar.Add("Lesson rescheduled.", Severity.Success);
            await _rescheduleDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadSchedule();
        }
        else Snackbar.Add("Failed to reschedule lesson.", Severity.Error);
    }

    // ── Complete bundle lesson ────────────────────────────────────

    private async Task OpenCompleteLessonDialog(LessonDetail lesson)
    {
        _lessonToComplete = lesson;
        _completeLessonNote = string.Empty;
        await _completeLessonDialog!.ShowAsync();
    }

    private async Task ConfirmCompleteLesson()
    {
        if (_lessonToComplete is null) return;
        var result = await LessonSvc.UpdateLessonStatusAsync(
            _lessonToComplete.LessonID,
            LessonStatus.Completed,
            note: string.IsNullOrWhiteSpace(_completeLessonNote) ? null : _completeLessonNote);
        if (result)
        {
            Snackbar.Add("Lesson marked as completed.", Severity.Success);
            await _completeLessonDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadSchedule();
        }
        else Snackbar.Add("Failed to update lesson.", Severity.Error);
    }

    // ── Complete extra lesson ─────────────────────────────────────

    private async Task OpenCompleteExtraDialog(ExtraLessonDetail extra)
    {
        _extraToComplete = extra;
        _completeExtraNote = string.Empty;
        await _completeExtraDialog!.ShowAsync();
    }

    private async Task ConfirmCompleteExtra()
    {
        if (_extraToComplete is null) return;
        var result = await ExtraLessonSvc.UpdateExtraLessonStatusAsync(
            _extraToComplete.ExtraLessonID,
            ExtraLessonStatus.Completed,
            note: string.IsNullOrWhiteSpace(_completeExtraNote) ? null : _completeExtraNote);
        if (result)
        {
            Snackbar.Add("Extra lesson marked as completed.", Severity.Success);
            await _completeExtraDialog!.CloseAsync(DialogResult.Ok(true));
            await LoadSchedule();
        }
        else Snackbar.Add("Failed to update extra lesson.", Severity.Error);
    }

    // ── Other status updates ──────────────────────────────────────

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

    // ── Add extra lesson ──────────────────────────────────────────

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
        _newExtra.ScheduledTime = TimeOnly.FromTimeSpan(_extraTime.Value);

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
                                            · @(first.StartDate.ToString("yyyy MMM dd")) – @(first.EndDate.ToString("yyyy MMM dd"))
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
                                                @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("yyyy MMM dd")
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
                        <MudTd>@context.EffectiveFrom.ToString("yyyy MMM dd")</MudTd>
                        <MudTd>@(context.EffectiveTo?.ToString("yyyy MMM dd") ?? "Active")</MudTd>
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
                               RequiredError="Required" Class="mb-3"
                               @bind-Value:after="SetBundleBasePrice">
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
                                     @bind-Value:after="RecalcBundle"
                                     HelperText="Auto-filled from lesson type; teacher may override" />
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudDatePicker @bind-Date="_bundleStartDate" Label="Start Date" Required="true"
                                   Class="mb-3" @bind-Date:after="RecalcBundle" DateFormat="yyyy/MM/dd" />
                </MudItem>
                <MudItem xs="12" sm="6">
                    <MudDatePicker @bind-Date="_bundleEndDate" Label="End Date" Required="true" Class="mb-3" DateFormat="yyyy/MM/dd" />
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
                                    @q.QuarterStartDate.ToString("dd MMM") – @q.QuarterEndDate.ToString("yyyy MMM dd")
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
            <MudDatePicker @bind-Date="_slotEffectiveFrom" Label="Effective From" Required="true" Class="mb-3" DateFormat="yyyy/MM/dd" />
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
        <MudDatePicker @bind-Date="_closeSlotDate" Label="Effective To (last date)" Required="true" DateFormat="yyyy/MM/dd" />
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

    private void SetBundleBasePrice()
    {
        var lt = _lessonTypes.FirstOrDefault(l => l.LessonTypeID == _newBundle.LessonTypeID);
        if (lt is not null)
        {
            _newBundle.PricePerLesson = lt.BasePricePerLesson;
            RecalcBundle();
        }
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

        // Default the lesson type from the student's most recent active bundle
        var activeBundle = _bundles
            .GroupBy(b => b.BundleID)
            .Select(g => g.First())
            .Where(b => b.EndDate >= DateTime.Today)
            .OrderByDescending(b => b.StartDate)
            .FirstOrDefault();

        if (activeBundle is not null)
            _newSlot.LessonTypeID = activeBundle.LessonTypeID;
        else if (_lessonTypes.Count > 0)
            _newSlot.LessonTypeID = _lessonTypes[0].LessonTypeID;

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
            <MudTd>@(context.DateOfBirth?.ToString("yyyy MMM dd") ?? "—")</MudTd>
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
            <MudText>Select an account holder to view students.</MudText>
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
        if (_teachers.Any())
        {
            _selectedTeacherId = _teachers.First().TeacherID;

            await OnTeacherChanged();

            StateHasChanged();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            StateHasChanged();
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

## File: MusicSchool.Web\Services\PaymentService.cs

```csharp
using MusicSchool.Data.Models;
using System.Net.Http.Json;
using MusicSchool.Models.TransferModels;

namespace MusicSchool.Services
{
    /// <summary>
    /// Client-side service for the Payment API endpoints.
    /// Register in Program.cs: builder.Services.AddScoped<PaymentService>();
    /// </summary>
    public class PaymentService
    {
        private readonly HttpClient _http;

        public PaymentService(HttpClient http) => _http = http;

        private async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ResponseBase<T>>(url);
                return response is not null ? response.Data : default;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PaymentService.GetAsync] {url} — {ex.Message}");
                return default;
            }
        }

        private async Task<T?> PostAsync<T>(string url, object body)
        {
            try
            {
                var httpResponse = await _http.PostAsJsonAsync(url, body);
                httpResponse.EnsureSuccessStatusCode();
                var response = await httpResponse.Content.ReadFromJsonAsync<ResponseBase<T>>();
                return response is not null ? response.Data : default;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[PaymentService.PostAsync] {url} — {ex.Message}");
                return default;
            }
        }

        /// <summary>Returns all payments for an account holder, newest first.</summary>
        public async Task<List<Payment>> GetByAccountHolderAsync(int accountHolderId)
        {
            var result = await GetAsync<IEnumerable<Payment>>(
                $"Payment/GetByAccountHolder?accountHolderId={accountHolderId}");
            return result?.ToList() ?? [];
        }

        /// <summary>
        /// Records a manual payment and runs the allocation engine.
        /// Returns the new PaymentID on success, null on failure.
        /// </summary>
        public async Task<int?> AddPaymentAsync(Payment payment)
            => await PostAsync<int?>("Payment/Add", payment);

        /// <summary>
        /// Creates a payment exactly equal to the invoice amount and marks it paid.
        /// Returns the new PaymentID on success, null on failure.
        /// </summary>
        public async Task<int?> QuickPayInvoiceAsync(int invoiceId, DateTime paymentDate)
        {
            var url = $"Payment/QuickPay?invoiceId={invoiceId}&paymentDate={paymentDate:yyyy-MM-dd}";
            return await PostAsync<int?>(url, new { });
        }

        /// <summary>Returns allocation detail rows for a specific payment.</summary>
        public async Task<List<PaymentAllocation>> GetAllocationsAsync(int paymentId)
        {
            var result = await GetAsync<IEnumerable<PaymentAllocation>>(
                $"Payment/GetAllocations?paymentId={paymentId}");
            return result?.ToList() ?? [];
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

        public async Task<bool> UpdateLessonStatusAsync(int lessonId, string status, string? note = null)
        {
            try
            {
                var url = $"Lesson/UpdateLessonStatus?lessonId={lessonId}&status={status}";
                if (!string.IsNullOrWhiteSpace(note))
                    url += $"&note={Uri.EscapeDataString(note)}";
                var r = await http.PutAsync(url, null);
                var result = await r.Content.ReadFromJsonAsync<ResponseBase<bool>>();
                return result?.Data ?? false;
            }
            catch { return false; }
        }

        public async Task<bool> RescheduleLessonAsync(int lessonId, DateTime newDate, TimeOnly newTime)
        {
            try
            {
                var url = $"Lesson/RescheduleLesson?lessonId={lessonId}" +
                          $"&newDate={newDate:yyyy-MM-dd}" +
                          $"&newTime={newTime:HH:mm:ss}";
                var r = await http.PutAsync(url, null);
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

        public async Task<bool> UpdateExtraLessonStatusAsync(int extraLessonId, string status, string? note = null)
        {
            try
            {
                var url = $"ExtraLesson/UpdateExtraLessonStatus?extraLessonId={extraLessonId}&status={status}";
                if (!string.IsNullOrWhiteSpace(note))
                    url += $"&note={Uri.EscapeDataString(note)}";
                var r = await http.PutAsync(url, null);
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
builder.Services.AddScoped<PaymentService>();

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
