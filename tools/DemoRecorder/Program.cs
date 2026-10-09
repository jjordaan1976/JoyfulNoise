using Microsoft.Playwright;

// Usage: DemoRecorder <part> <outputDir>
//   part all = the whole demo in one take (default; see record.ps1, which drives the API switch)
//   part 1   = management app (parent, children, bundles, invoices, payment) — API must run as the teacher
//   part 2   = account holder portal (statement)                             — API must run as the parent
//
// In a single take the recorder cannot restart the API itself, so between the parts it writes
// <outputDir>/switch-user.request and waits for <outputDir>/switch-user.done, which record.ps1
// creates once the API is back up as the parent. A title card covers the wait.
var part = args.Length > 0 ? args[0] : "all";
var outDir = Path.GetFullPath(args.Length > 1 ? args[1] : "videos");
Directory.CreateDirectory(outDir);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
var context = await browser.NewContextAsync(new()
{
    ViewportSize = new() { Width = 1280, Height = 720 },
    RecordVideoDir = outDir,
    RecordVideoSize = new() { Width = 1280, Height = 720 },
    Locale = "en-ZA",
    IgnoreHTTPSErrors = true,   // the apps call the API over the local dev certificate
});
await context.AddInitScriptAsync(Overlay.Script);

var page = await context.NewPageAsync();
var demo = new Demo(page);
switch (part)
{
    case "1": await demo.Part1Async(); break;
    case "2": await demo.Part2Async(); break;
    default:
        await demo.Part1Async();
        await demo.SwitchToParentAsync(outDir);
        await demo.Part2Async();
        break;
}

await page.CloseAsync();
var recorded = await page.Video!.PathAsync();
await context.CloseAsync();                       // finalises the video file
var target = Path.Combine(outDir, part is "1" or "2" ? $"part{part}.webm" : "tutor-demo.webm");
File.Move(recorded, target, overwrite: true);
Console.WriteLine($"saved {target}");

/// <summary>Drives the apps slowly enough to follow, with a caption bar and a visible cursor.</summary>
sealed class Demo(IPage page)
{
    const string WebUrl = "http://localhost:64315";
    const string PortalUrl = "http://localhost:57350";

    // Demo family (all @example.com so the records are easy to recognise and remove)
    const string Parent = "Thandi Mokoena";
    const string Child1 = "Lerato";
    const string Child2 = "Kagiso";

    ILocator Dialog => page.Locator(".mud-dialog");

    public async Task Part1Async()
    {
        await BootAsync($"{WebUrl}/account-holders", "Manage account holders");
        await CardAsync("Tutor demo",
            ["A parent with two children on different bundles", "Invoices  ·  capturing a payment  ·  the parent's statement"], 4000);

        // ── Step 1: parent ───────────────────────────────────────────────────
        await CaptionAsync("Step 1 — Set up the parent. Accounts › Account Holders › ADD ACCOUNT HOLDER", 2200);
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = "Add Account Holder" }));
        await CaptionAsync("The parent is the billing contact. Enter the details and Save.", 1000);
        await TypeAsync(Dialog.GetByLabel("First Name"), "Thandi");
        await TypeAsync(Dialog.GetByLabel("Last Name"), "Mokoena");
        await TypeAsync(Dialog.GetByLabel("Email"), "thandi.mokoena@example.com");
        await TypeAsync(Dialog.GetByLabel("Phone"), "083 555 0142");
        await TypeAsync(Dialog.GetByLabel("Billing Address"), "8 Jacaranda Avenue, Centurion");
        await WaitAsync(900);
        await ClickAsync(Dialog.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }));
        await CaptionAsync("Saved. A portal login is created for the parent automatically.", 2600);

        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = Parent }));
        await CaptionAsync("Her page: account details, outstanding invoices and students — none enrolled yet.", 2600);

        // ── Children ────────────────────────────────────────────────────────
        await AddStudentAsync(Child1, "lerato.mokoena@example.com", "2014/03/21",
            "ADD STUDENT — first child, Lerato. The email lets her use the Student Portal.");
        await AddStudentAsync(Child2, "kagiso.mokoena@example.com", "2012/08/15",
            "ADD STUDENT again — second child, Kagiso.");
        await CaptionAsync("Both children are enrolled under the parent. Next: a lesson bundle for each.", 3000);

        // ── Bundle 1: Lerato, 30 min, 32 lessons ─────────────────────────────
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = $"{Child1} Mokoena" }));
        await CaptionAsync("Lerato's student page — no bundle yet (slots can only be scheduled once she has one).", 2600);
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = "New Bundle" }));
        await CaptionAsync("NEW BUNDLE: starts today. Prorata is pre-selected — only the last quarter of the year is left.", 2800);
        await SelectAsync(Dialog.GetByLabel("Lesson Type"), "30 min");
        await CaptionAsync("30-minute lessons: the price fills in. 32 lessons a year → 8 this quarter, R1 520 in 3 monthly instalments.", 4200);
        await ClickAsync(Dialog.GetByRole(AriaRole.Button, new() { Name = "Save Bundle" }));
        await CaptionAsync("Bundle created: 8 lessons at R190, 09 Oct – 31 Dec.", 2600);

        // ── Bundle 2: Kagiso, 45 min, 36 lessons ─────────────────────────────
        await ClickAsync(page.GetByRole(AriaRole.Link, new() { Name = "Account Holder", Exact = true }));
        await page.GetByRole(AriaRole.Button, new() { Name = $"{Child2} Mokoena" }).WaitForAsync();
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = $"{Child2} Mokoena" }));
        await CaptionAsync("Now Kagiso — a different bundle.", 1600);
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = "New Bundle" }));
        await ClickAsync(Dialog.GetByLabel("Total Lessons"), 200);
        await page.Keyboard.PressAsync("Control+a");
        await page.Keyboard.TypeAsync("36", new() { Delay = 90 });
        await page.Keyboard.PressAsync("Tab");
        await WaitAsync(500);
        await SelectAsync(Dialog.GetByLabel("Lesson Type"), "45 min");
        await CaptionAsync("45-minute lessons, 36 a year → 9 this quarter, R2 475 in 3 instalments of R825.", 4200);
        await ClickAsync(Dialog.GetByRole(AriaRole.Button, new() { Name = "Save Bundle" }));
        await CaptionAsync("Both children are set up, each with their own bundle.", 2800);

        // ── Step 2: invoices ─────────────────────────────────────────────────
        await ClickAsync(page.GetByRole(AriaRole.Link, new() { Name = "Invoices" }));
        await page.GetByText("View invoices and record payments").First.WaitForAsync();
        await CaptionAsync("Step 2 — Invoices. Finance › Invoices, filtered to the parent.", 2000);
        await SelectAsync(page.GetByLabel("Account Holder"), Parent);
        await page.GetByText("Total Invoiced").First.WaitForAsync();
        await CaptionAsync("Six invoices were created automatically — one instalment per month, per bundle (Oct, Nov, Dec). All Pending.", 5000);

        // ── Step 3: payment ──────────────────────────────────────────────────
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = "Record Payment" }));
        await CaptionAsync("Step 3 — Capture a payment. The amount is matched to the oldest invoices first.", 2600);
        await ClickAsync(Dialog.GetByLabel("Amount Received"), 200);
        await page.Keyboard.PressAsync("Control+a");
        await page.Keyboard.TypeAsync("1331,67", new() { Delay = 90 });
        await page.Keyboard.PressAsync("Tab");
        await TypeAsync(Dialog.GetByLabel("Reference"), "EFT 51877");
        await TypeAsync(Dialog.GetByLabel("Notes"), "October fees - Lerato and Kagiso");
        await CaptionAsync("R1 331,67 covers both October instalments — the dialog ticks what will be paid. Save.", 3600);
        await ClickAsync(Dialog.GetByRole(AriaRole.Button, new() { Name = "Save Payment" }));
        await page.GetByText("Payment History").First.WaitForAsync();
        await CaptionAsync("Payment recorded: both October instalments are now Paid, with today's date.", 3400);

        var row = page.Locator("tr", new() { HasText = "Instalment 2" }).Filter(new() { HasText = "506,67" }).First;
        await ClickAsync(row.GetByRole(AriaRole.Button, new() { Name = "Paid" }));
        await CaptionAsync("Or mark a single invoice paid with one click — the green PAID button.", 3200);

        await ScrollAsync(page.GetByText("Payment History"));
        await CaptionAsync("Payment History records both: the manual EFT and the quick-pay.", 4000);
        await CaptionAsync("", 300);
    }

    /// <summary>
    /// Asks the outside world (record.ps1) to restart the API as the parent and waits, behind a
    /// title card, until it signals that the API is back.
    /// </summary>
    public async Task SwitchToParentAsync(string outDir)
    {
        var request = Path.Combine(outDir, "switch-user.request");
        var done = Path.Combine(outDir, "switch-user.done");
        File.Delete(done);
        await page.EvaluateAsync("([t, l]) => window.__demoCard(t, l, true)",
            new object[] { "Step 4 — The parent's view", new[] { "Thandi signs in to the Account Holder Portal to see her statement" } });
        File.WriteAllText(request, DateTime.Now.ToString("O"));

        var deadline = DateTime.UtcNow.AddMinutes(3);
        while (!File.Exists(done))
        {
            if (DateTime.UtcNow > deadline) throw new TimeoutException($"Timed out waiting for {done}. Run the demo through record.ps1.");
            await WaitAsync(500);
        }
        await WaitAsync(1500);                            // let the card be read before the portal loads over it
        File.Delete(request);
        File.Delete(done);
    }

    public async Task Part2Async()
    {
        await BootAsync($"{PortalUrl}/statement", "Account Statement");
        await CaptionAsync("Step 4 — The parent's statement in the Account Holder Portal.", 3000);
        await CaptionAsync("Aging summary and status banner at the top; the full invoice history below.", 3200);
        await ScrollAsync(page.GetByText("Invoice History"));
        await CaptionAsync("Three instalments paid, three still to come — exactly what the teacher captured.", 4200);
        await CaptionAsync("", 300);
        await CardAsync("That's the full flow",
            ["Parent  →  children  →  bundles  →  invoices  →  payment  →  statement", "Recorded against a local development database"], 4500);
    }

    // ── Building blocks ─────────────────────────────────────────────────────

    async Task AddStudentAsync(string first, string email, string dob, string caption)
    {
        await ClickAsync(page.GetByRole(AriaRole.Button, new() { Name = "Add Student" }));
        await CaptionAsync(caption, 1000);
        await TypeAsync(Dialog.GetByLabel("First Name"), first);
        await TypeAsync(Dialog.GetByLabel("Last Name"), "Mokoena");
        await TypeAsync(Dialog.GetByLabel("Email"), email);
        await SetDateAsync(Dialog.GetByLabel("Date of Birth"), dob);
        await WaitAsync(800);
        await ClickAsync(Dialog.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }));
        await page.GetByRole(AriaRole.Button, new() { Name = $"{first} Mokoena" }).WaitForAsync();
        await WaitAsync(1200);
    }

    async Task BootAsync(string url, string readyText)
    {
        await page.GotoAsync(url);
        await page.GetByText(readyText).First.WaitForAsync(new() { Timeout = 90_000 });
        await WaitAsync(1000);
    }

    Task WaitAsync(int ms) => page.WaitForTimeoutAsync(ms);

    async Task CaptionAsync(string text, int holdMs)
    {
        await page.EvaluateAsync("t => window.__demoCaption && window.__demoCaption(t)", text);
        await WaitAsync(holdMs);
    }

    async Task CardAsync(string title, string[] lines, int holdMs)
    {
        await page.EvaluateAsync("([t, l]) => window.__demoCard(t, l, true)", new object[] { title, lines });
        await WaitAsync(holdMs);
        await page.EvaluateAsync("() => window.__demoCard('', [], false)");
        await WaitAsync(400);
    }

    /// <summary>Moves the (injected) cursor to the element in visible steps, then clicks it.</summary>
    async Task ClickAsync(ILocator target, int settleMs = 800)
    {
        await target.First.ScrollIntoViewIfNeededAsync();
        var box = await target.First.BoundingBoxAsync() ?? throw new InvalidOperationException("Element has no box: " + target);
        var x = box.X + box.Width / 2;
        var y = box.Y + box.Height / 2;
        await page.Mouse.MoveAsync(x, y, new() { Steps = 28 });
        await WaitAsync(280);
        await page.Mouse.ClickAsync(x, y);
        await WaitAsync(settleMs);
    }

    async Task TypeAsync(ILocator field, string text)
    {
        await ClickAsync(field, 150);
        await field.First.PressSequentiallyAsync(text, new() { Delay = 45 });
        await WaitAsync(250);
    }

    /// <summary>Opens a MudSelect and clicks the item with the given text.</summary>
    async Task SelectAsync(ILocator select, string itemText)
    {
        await ClickAsync(select);
        var item = page.Locator(".mud-popover-open .mud-list-item", new() { HasText = itemText }).First;
        await item.WaitForAsync();
        await ClickAsync(item);
    }

    /// <summary>MudDatePicker's input is read-only; set it the way the picker would and notify Blazor.</summary>
    async Task SetDateAsync(ILocator input, string value)
    {
        try
        {
            await ClickAsync(input, 150);
            await page.Keyboard.PressAsync("Escape");   // close the picker popover
            await input.First.EvaluateAsync(
                "(el, v) => { el.value = v; el.dispatchEvent(new Event('input', { bubbles: true })); el.dispatchEvent(new Event('change', { bubbles: true })); }",
                value);
            await WaitAsync(400);
        }
        catch { /* date of birth is optional; carry on without it */ }
    }

    async Task ScrollAsync(ILocator toward)
    {
        for (var i = 0; i < 5; i++) { await page.Mouse.WheelAsync(0, 120); await WaitAsync(140); }
        await toward.First.ScrollIntoViewIfNeededAsync();
        await WaitAsync(600);
    }
}

/// <summary>Injected into every page: a visible cursor, a caption bar, and full-screen title cards.</summary>
static class Overlay
{
    public const string Script = """
        (() => {
          const LOGO = '/_content/Tutor.Client.Theme/images/logo-full.png';
          function setup() {
            if (document.getElementById('demo-cursor')) return;
            const c = document.createElement('div');
            c.id = 'demo-cursor';
            c.style.cssText = 'position:fixed;left:-60px;top:-60px;width:24px;height:24px;border-radius:50%;background:rgba(239,107,163,.35);border:2px solid #c03276;box-shadow:0 0 0 2px #fff;pointer-events:none;z-index:2147483647;transform:translate(-50%,-50%);transition:transform .1s,background .1s';
            document.body.appendChild(c);
            const cap = document.createElement('div');
            cap.id = 'demo-caption';
            cap.style.cssText = 'position:fixed;left:0;right:0;bottom:0;padding:16px 26px;background:rgba(17,17,17,.93);color:#fff;font:600 19px/1.4 Inter,"Segoe UI",sans-serif;z-index:2147483646;pointer-events:none;display:none;border-top:3px solid #ef6ba3';
            document.body.appendChild(cap);
            document.addEventListener('mousemove', e => { c.style.left = e.clientX + 'px'; c.style.top = e.clientY + 'px'; }, true);
            document.addEventListener('mousedown', () => { c.style.transform = 'translate(-50%,-50%) scale(1.7)'; c.style.background = 'rgba(239,107,163,.75)'; }, true);
            document.addEventListener('mouseup',   () => { c.style.transform = 'translate(-50%,-50%)';            c.style.background = 'rgba(239,107,163,.35)'; }, true);
            window.__demoCaption = t => { cap.textContent = t; cap.style.display = t ? 'block' : 'none'; };
            window.__demoCard = (title, lines, show) => {
              let card = document.getElementById('demo-card');
              if (!show) { if (card) card.remove(); return; }
              if (!card) { card = document.createElement('div'); card.id = 'demo-card'; document.body.appendChild(card); }
              card.style.cssText = 'position:fixed;inset:0;background:#fff;z-index:2147483645;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:12px;font-family:Inter,"Segoe UI",sans-serif;color:#000;text-align:center';
              card.innerHTML = '<img src="' + LOGO + '" style="width:380px;margin-bottom:22px">'
                + '<div style="font:700 36px Inter,\'Segoe UI\',sans-serif">' + title + '</div>'
                + lines.map(l => '<div style="font:400 21px Inter,\'Segoe UI\',sans-serif;color:#5f5f5f">' + l + '</div>').join('')
                + '<div style="position:absolute;left:0;right:0;bottom:0;height:6px;background:linear-gradient(90deg,#93d4c7,#b9cda4,#f6b669,#f79b83,#ef6ba3,#aa77b8)"></div>';
            };
          }
          if (document.body) setup(); else document.addEventListener('DOMContentLoaded', setup);
        })();
        """;
}
