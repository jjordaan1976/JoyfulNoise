# DemoRecorder

Records the product demo as a video using [Playwright](https://playwright.dev) (headless Chromium, 1280×720 WebM).
It is a standalone tool and is **not** part of `Tutor.slnx`.

The demo creates a parent (Thandi Mokoena) with two children on different bundles, shows the invoices
that creates, captures a payment, and shows the parent's statement. It writes real rows to whatever
database the API points at — use a development database. All demo emails are `@example.com`.

## One-time setup

```bash
dotnet build Tutor.slnx
dotnet build tools/DemoRecorder
powershell -File tools/DemoRecorder/bin/Debug/net9.0/playwright.ps1 install chromium
```

## Recording

From the repo root:

```bash
powershell -File tools/DemoRecorder/record.ps1
```

`record.ps1` starts `Tutor.Api`, `Tutor.Web` and `Tutor.AccountHolderPortal` (http ports 64100, 64315 and 57350),
runs the recorder, and stops the apps when it is done. The result is `docs/demo/tutor-demo.webm`.

The statement is shown as the parent. With authentication switched off (`Auth:Enabled=false`) the API's user
comes from `Auth:BypassEmail`, so halfway through the recorder writes `switch-user.request` next to the video,
and `record.ps1` restarts the API with `Auth__BypassEmail=thandi.mokoena@example.com` and answers with
`switch-user.done`. A title card covers the gap, so the whole demo is one continuous take — no video joining needed.

The recorder can also be run on its own for one part at a time (`dotnet run --project tools/DemoRecorder -- 1 docs/demo`
or `-- 2 docs/demo`, producing `part1.webm` / `part2.webm`) with the apps already running as the right user.

## Re-recording

The demo family must not exist yet. Remove the previous take first:

```bash
sqlcmd -S . -d JoyfullMusicSchool -E -i tools/DemoRecorder/remove-demo-family.sql
```

The video is WebM (VP8). Playwright's bundled ffmpeg cannot convert it; if an MP4 is needed, use a full
ffmpeg (`winget install Gyan.FFmpeg`) and `ffmpeg -i tutor-demo.webm -c:v libx264 -pix_fmt yuv420p tutor-demo.mp4`.
