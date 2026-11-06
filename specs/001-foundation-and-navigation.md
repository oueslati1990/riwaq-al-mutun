# Spec 001 — Foundation & Navigation
# المواصفة 001 — الأساس والتنقل

**الحالة / Status:** ✅ Approved — ready for tasks  
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**  
**يسبقه / Depends on:** spec 000 ✅

---

## المشكلة / Problem

قبل بناء أي ميزة، يجب أن يكون هناك:
- هيكل Solution نظيف يطبق Clean Architecture
- تهيئة DI وLogging وConfiguration
- نافذة WPF أساسية بـ RTL وNavigation تعمل
- قاعدة بيانات SQLite مهيأة بـ EF Core migrations
- بيئة اختبار جاهزة

---

## نطاق هذه المواصفة / Scope

### داخل النطاق ✅
- إنشاء Solution بمشاريعها الأربعة + مشاريع الاختبار
- تهيئة DI (Microsoft.Extensions.DependencyInjection + Hosting)
- تهيئة Logging (Microsoft.Extensions.Logging + Serilog ملف محلي)
- تهيئة Configuration (appsettings.json)
- WPF shell: نافذة رئيسية، RTL، Navigation بين الشاشات
- صفحات placeholder للشاشات التسع (بدون محتوى)
- SQLite + EF Core: DbContext، أول migration فارغ، اختبار الاتصال
- Directory.Build.props لإعدادات مشتركة (Nullable، TreatWarningsAsErrors)
- global.json لتثبيت إصدار SDK
- مشروع Domain.Tests + Application.Tests + Infrastructure.Tests فارغة جاهزة

### خارج النطاق ❌
- أي منطق عمل
- أي شاشة بمحتوى حقيقي
- الكيانات الكاملة (تأتي في spec 002)
- OCR أو PDF

---

## الشاشات والتنقل / Screens & Navigation

```
MainWindow
├── DashboardPage         (الرئيسية)
├── LibraryPage           (مكتبتي)
├── ImportPage            (استيراد متن)
├── OcrReviewPage         (مراجعة النص المستخرج)
├── MatnDetailPage        (تفاصيل المتن)
├── MemorizationPage      (جلسة الحفظ)
├── ReviewPage            (جلسة المراجعة)
├── StatisticsPage        (الإحصاءات)
└── SettingsPage          (الإعدادات)
```

نمط التنقل: `INavigationService` في Application، تنفيذه في Presentation.Wpf.  
استخدم Frame + Page أو ContentControl + DataTemplate — الاختيار يُحسم في Plan.

---

## هيكل Solution / Solution Structure

```
MatnMemorization.sln
├── src/
│   ├── MatnMemorization.Domain/
│   │   └── (فارغ في هذه المرحلة — سيُملأ في spec 002)
│   ├── MatnMemorization.Application/
│   │   └── Contracts/Navigation/INavigationService.cs
│   ├── MatnMemorization.Infrastructure/
│   │   └── Persistence/AppDbContext.cs
│   │   └── Persistence/Migrations/
│   └── MatnMemorization.Presentation.Wpf/
│       └── App.xaml / App.xaml.cs (Composition Root)
│       └── MainWindow.xaml
│       └── Views/ (placeholder pages)
│       └── Services/Navigation/NavigationService.cs
├── tests/
│   ├── MatnMemorization.Domain.Tests/
│   ├── MatnMemorization.Application.Tests/
│   └── MatnMemorization.Infrastructure.Tests/
├── global.json
├── Directory.Build.props
└── CLAUDE.md
```

---

## القرارات التقنية / Technical Decisions

| الموضوع | القرار | السبب |
|---|---|---|
| Navigation | `ContentControl` + `DataTemplate` بدل `Frame`/`Page` | أسهل مع MVVM وDI؛ Frame يحمل state داخلي يتعارض مع ViewModel |
| Logging | Serilog مع sink لملف محلي | يدعم structured logging، ترخيص Apache 2.0 |
| DI container | `Microsoft.Extensions.DependencyInjection` عبر `IHostBuilder` | معيار .NET، متوافق مع EF Core وLogging وConfiguration |
| DB location | `%APPDATA%\MatnMemorization\` | مجلد تطبيق قياسي على Windows، لا يحتاج صلاحيات admin |
| SDK | .NET 10 — يُثبَّت عبر `global.json` | متوافق مع WPF الحديث |

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-01 | `dotnet build` ينجح بلا تحذيرات (TreatWarningsAsErrors=true) |
| AC-02 | `dotnet test` ينجح (حتى لو المشاريع فارغة) |
| AC-03 | التطبيق يفتح نافذة رئيسية بـ RTL، الخط العربي يظهر في العنوان |
| AC-04 | Navigation بين صفحتين على الأقل يعمل (Dashboard ↔ Library) |
| AC-05 | EF Core ينشئ ملف SQLite في `%APPDATA%\MatnMemorization\` عند أول تشغيل |
| AC-06 | Logging يكتب ملف `.log` في مجلد البيانات عند بدء التطبيق |
| AC-07 | لا يوجد قواعد عمل أو منطق تطبيق في Presentation layer |

---

## الحزم المطلوبة / Required Packages

```xml
<!-- Domain — لا حزم -->

<!-- Application — لا حزم خارجية -->

<!-- Infrastructure -->
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Design
Serilog.Extensions.Logging
Serilog.Sinks.File

<!-- Presentation.Wpf -->
CommunityToolkit.Mvvm
Microsoft.Extensions.Hosting

<!-- Tests -->
xunit
xunit.runner.visualstudio
Microsoft.NET.Test.Sdk
```

---

## المهام / Tasks

### T-001-01: إنشاء هيكل Solution
- إنشاء `MatnMemorization.sln`
- إنشاء 4 مشاريع src + 3 مشاريع tests بالأسماء الصحيحة
- إضافة project references (Domain ← Application ← Infrastructure، Application ← Presentation)
- **تحقق:** `dotnet build` ينجح

### T-001-02: Directory.Build.props و global.json
- `Nullable=enable`، `ImplicitUsings=enable`، `TreatWarningsAsErrors=true`
- `net10.0-windows` و `<UseWpf>true</UseWpf>` في Presentation فقط
- `global.json` يثبت إصدار SDK الفعلي المثبت
- **تحقق:** `dotnet --info` يتوافق مع global.json

### T-001-03: تهيئة DI وHosting وConfiguration
- `IHostBuilder` في `App.xaml.cs`
- `appsettings.json` مضاف كـ embedded resource أو copy-to-output
- تسجيل placeholder services
- **تحقق:** التطبيق يبدأ بدون استثناء

### T-001-04: Logging بـ Serilog
- تسجيل Serilog مع File sink في `%APPDATA%\MatnMemorization\logs\`
- لا يُسجَّل محتوى نصوص المتون
- **تحقق:** ملف `.log` ينشأ عند التشغيل

### T-001-05: WPF Shell وRTL
- `MainWindow.xaml` مع `FlowDirection="RightToLeft"`
- خط Noto Naskh Arabic أو Segoe UI مع Arabic fallback
- `ContentControl` مركزي لاستضافة الصفحات
- **تحقق:** النافذة تفتح، النص العربي يظهر صحيحاً

### T-001-06: INavigationService + تنفيذه
- `INavigationService` في Application.Contracts
- `NavigationService` في Presentation.Wpf
- تسجيله في DI
- **تحقق:** Navigation بين Dashboard وLibrary يعمل

### T-001-07: صفحات Placeholder (9 صفحات)
- كل صفحة: ViewModel فارغ، View بعنوان عربي فقط
- **تحقق:** كل صفحة قابلة للتنقل إليها دون استثناء

### T-001-08: AppDbContext وأول Migration
- `AppDbContext` في Infrastructure مع connection string من Configuration
- ملف SQLite في `%APPDATA%\MatnMemorization\data\`
- `dotnet ef migrations add InitialCreate`
- EnsureCreated أو Migrate عند بدء التطبيق
- **تحقق:** ملف `.db` ينشأ عند أول تشغيل

### T-001-09: Integration Test لـ DbContext
- اختبار يتحقق أن AppDbContext يتصل بـ SQLite In-Memory أو ملف مؤقت
- **تحقق:** `dotnet test` ينجح

---

**الخطوة التالية:** T-001-01 — إنشاء هيكل Solution
