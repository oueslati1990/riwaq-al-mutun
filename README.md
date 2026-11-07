# رِواق المتون — Riwaq al-Mutun

تطبيق سطح مكتب لحفظ ومراجعة المتون العلمية العربية، مبني بـ WPF / .NET 10.

---

## لقطات الشاشة / Screenshots

| لوحة التحكم | المكتبة |
|---|---|
| ![Dashboard](docs/screenshots/dashboard.png) | ![Library](docs/screenshots/library.png) |

| جلسة الحفظ | المراجعة اليومية |
|---|---|
| ![Memorization](docs/screenshots/memorization.png) | ![Review](docs/screenshots/review.png) |

| الإحصائيات | الإعدادات |
|---|---|
| ![Statistics](docs/screenshots/statistics.png) | ![Settings](docs/screenshots/settings.png) |

> **To add screenshots:** run the app on Windows, take screenshots of each screen, and save them to `docs/screenshots/` using the filenames above.

---

## المتطلبات / Prerequisites

| المتطلب | الإصدار |
|---|---|
| OS | Windows 10 / 11 (x64) |
| .NET SDK | 10.0.113 or later patch (`global.json` pins this) |

Install the SDK from <https://dotnet.microsoft.com/download/dotnet/10.0>.

---

## الإعداد والتشغيل / Setup & Run

```bash
# 1. Clone
git clone https://github.com/oueslati1990/riwaq-al-mutun.git
cd riwaq-al-mutun

# 2. Restore dependencies
dotnet restore

# 3. Build
dotnet build --no-restore

# 4. Run
dotnet run --project src/MatnMemorization.Presentation.Wpf
```

The app creates its data directory on first launch:

```
%APPDATA%\MatnMemorization\
  data\matn.db      ← SQLite database (auto-migrated)
  logs\             ← rolling log files
```

---

## ملاحظة للمطورين على Linux / Note for Linux developers

The WPF project targets `net10.0-windows`. It **builds** on Linux thanks to `EnableWindowsTargeting=true`, but the resulting `.exe` must be **run on Windows**.

```bash
# Build on Linux (CI-safe)
dotnet build --no-restore

# Run tests on Linux
dotnet test --no-build
```

To run the app, copy the publish output to a Windows machine (see **Publish** below) or use a Windows VM.

---

## النشر / Publish

```bash
dotnet publish src/MatnMemorization.Presentation.Wpf \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  --output ./publish
```

This produces a single `MatnMemorization.Presentation.Wpf.exe` in `./publish/`.  
Copy the entire `publish/` folder (including `tessdata/`) to the target machine and run the `.exe` directly — no installer needed.

---

## الميزات / Features

| الرقم | الميزة |
|---|---|
| 001 | Foundation & navigation |
| 002 | Matn library & editor |
| 003 | Document import & Arabic OCR |
| 004 | OCR review & text-unit editor |
| 005 | Memorization sessions |
| 006 | Spaced-review scheduler (SM-2) |
| 007 | Progress & statistics |
| 008 | Backup & restore |

---

## البنية / Architecture

```
Domain → Application → Infrastructure ← Presentation.Wpf
```

Clean Architecture — domain and application layers have no dependency on WPF or EF Core.  
See `docs/speckit-workflow.md` and `specs/` for feature specs.

---

## الاختبارات / Tests

```bash
dotnet test
```

57 tests across Domain, Application, and Infrastructure test projects.
