# Spec 003 — Document Import & Arabic OCR
# المواصفة 003 — استيراد المستندات والتعرف الضوئي العربي

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 002 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- استيراد PDF متعدد الصفحات وصور (PNG/JPEG/TIFF)
- OCR بالتعرف على النص العربي عبر Tesseract (محلي، بدون إنترنت)
- معاينة نتيجة OCR صفحة بصفحة وتصحيح النص قبل الحفظ
- إنشاء Matn جديد من نتيجة OCR (أو إضافة أبواب لمتن موجود — خارج نطاق MVP)
- تقسيم النص تلقائياً إلى أبيات/مقاطع بحسب الأسطر
- واجهة مراجعة OCR (OcrReviewView) مع دعم RTL
- إظهار تقدم العملية، إمكانية الإلغاء، وعرض الأخطاء
- حد أقصى لحجم الملف (50 MB) وعدد الصفحات (200)

### خارج النطاق ❌
- OCR سحابي (Azure/Google Vision)
- إضافة صفحات OCR لمتن موجود (نسخة مستقبلية)
- تحرير التخطيط أو تحديد مناطق OCR يدوياً
- دعم صيغ DOCX/ODT
- تصحيح إملائي تلقائي

---

## القرارات المغلقة / Decisions

| القرار | الخيار المختار | السبب |
|---|---|---|
| محرك OCR | Tesseract 5 (local) | مجاني، offline، دعم عربي جيد مع tessdata-fast |
| صيغ الملفات | PDF متعدد الصفحات + PNG/JPEG/TIFF | الأكثر شيوعاً لمصادر المتون |
| مكتبة PDF → صور | PDFium (PdfiumViewer) أو Pdfium.NET | تحويل الصفحات إلى bitmap لـ Tesseract |
| مكتبة Tesseract .NET | Tesseract NuGet (charlesw/tesseract) | wrapper رسمي، متوافق مع .NET |
| تقسيم النص | بالأسطر الفارغة كحد للمقطع | بسيط وفعّال للمتون الشعرية |

---

## نموذج البيانات / Domain Model

لا كيانات domain جديدة. OCR هو عملية Import تُنتج Matn+Sections+TextUnits.

### حالة OCR (ViewModel state فقط، لا تخزين)

```
OcrImportState:
  FilePath: string
  TotalPages: int
  ProcessedPages: int
  PageResults: List<OcrPageResult>

OcrPageResult:
  PageNumber: int
  RawText: string          ← نص Tesseract الأصلي
  EditedText: string       ← النص بعد تعديل المستخدم
  IsApproved: bool
```

---

## واجهات التجريد / Abstractions

```csharp
// Application layer
public interface IOcrService
{
    Task<string> RecognizeAsync(byte[] imageBytes, CancellationToken ct);
}

public interface IPdfService
{
    int GetPageCount(string filePath);
    Task<byte[]> RenderPageAsync(string filePath, int pageNumber, int dpi, CancellationToken ct);
}
```

---

## تدفق الاستخدام / Use Flow

```
ImportView
  → اختر ملف (PDF أو صورة)
  → تحقق من الحجم والنوع
  → إطار تقدم: OCR صفحة بصفحة
  → OcrReviewView
       → قائمة الصفحات + نص قابل للتحرير
       → "اعتماد الكل" أو اعتماد صفحة بصفحة
  → إنشاء Matn من النص المعتمد
       → حوار: عنوان المتن + الكاتب
       → كل صفحة معتمدة → Section
       → كل سطر غير فارغ → TextUnit
  → انتقال تلقائي إلى MatnDetailView للمتن الجديد
```

---

## معايير القبول / Acceptance Criteria

| # | المعيار | كيفية التحقق |
|---|---|---|
| AC-003-01 | يمكن استيراد PDF من 1 إلى 200 صفحة دون تعليق UI | اختبار بملف 10 صفحات، تقدم ظاهر |
| AC-003-02 | يمكن استيراد صورة PNG/JPEG/TIFF مفردة | اختبار بصورة عربية |
| AC-003-03 | Tesseract يستخرج نصاً عربياً مقبولاً | مقارنة بنص معروف |
| AC-003-04 | المستخدم يستطيع تعديل نص كل صفحة قبل الحفظ | تعديل في OcrReviewView |
| AC-003-05 | الحفظ ينشئ Matn + Sections + TextUnits في DB | تحقق بـ MatnDetailView |
| AC-003-06 | عرض تقدم OCR يتحدث تلقائياً | مشاهدة ProgressBar |
| AC-003-07 | زر إلغاء يوقف العملية نظيفاً | ضغط إلغاء أثناء OCR |
| AC-003-08 | رفض ملفات > 50 MB برسالة واضحة | اختبار بملف كبير |
| AC-003-09 | Tesseract يعمل بدون إنترنت | قطع الشبكة والاختبار |
| AC-003-10 | OCR لا يخزن الملف الأصلي في DB | تحقق من schema |

---

## مهام التنفيذ / Tasks

### T-003-01: NuGet packages + tessdata
- أضف `Tesseract` (charlesw) إلى Infrastructure
- أضف `PdfiumViewer` أو بديلاً لتحويل PDF → bitmap إلى Infrastructure
- أضف `Microsoft.Win32.Registry` أو بديلاً لـ FileDialog في Presentation
- حمّل `tessdata-fast/ara.traineddata` من repository رسمي → `src/MatnMemorization.Presentation.Wpf/tessdata/`
- أضف tessdata إلى `.gitignore` (ثنائية كبيرة)
- وثّق في `docs/decisions/ADR-003-ocr-engine.md`

### T-003-02: Domain value objects + use cases
- أضف `OcrPageResult` record في Application (ليس Domain)
- Use case: `ImportMatnFromOcrCommand(Title, Author, List<OcrPageResult>)` + handler
  - ينشئ Matn، يضيف Section لكل صفحة، يضيف TextUnit لكل سطر غير فارغ

### T-003-03: Infrastructure — IPdfService + IOcrService implementations
- `TesseractOcrService : IOcrService` في Infrastructure
  - يقرأ tessdata من `tessdata/` بجانب executable
  - يعيد النص العربي بـ RTL
- `PdfiumPdfService : IPdfService` في Infrastructure
  - `GetPageCount(filePath)` → int
  - `RenderPageAsync(filePath, pageNumber, dpi=300)` → byte[] (PNG)
- تسجيل الخدمتين في DI في `App.xaml.cs`

### T-003-04: ImportViewModel
- يقبل `IPdfService`, `IOcrService`, `ImportMatnFromOcrHandler`, `MatnSelectionService`, `INavigationService`
- Commands: `PickFileCommand`, `StartOcrCommand`, `CancelCommand`
- Properties: `FilePath`, `IsProcessing`, `Progress` (0..100), `StatusMessage`, `ErrorMessage`
- يفتح FileDialog لاختيار PDF/PNG/JPEG/TIFF
- يتحقق من الحجم (≤ 50 MB) وعدد الصفحات (≤ 200)
- يشغل OCR على خيط خلفي (Task.Run)، يبلغ بـ `IProgress<int>`
- يخزن `List<OcrPageResult>` ثم يبحر إلى OcrReviewViewModel

### T-003-05: OcrReviewViewModel
- يقبل نتائج OCR من `ImportViewModel` عبر Service بسيط (OcrResultStore)
- خصائص: `List<OcrPageResult> Pages`, `SelectedPage`, `EditedText`
- Commands: `ApprovePageCommand`, `ApproveAllCommand`, `ConfirmImportCommand`, `CancelCommand`
- `ConfirmImportCommand`: يفتح حوار (Title, Author) ثم يستدعي `ImportMatnFromOcrHandler`
- بعد الحفظ: sets `MatnSelectionService.SelectedMatnId` وينتقل إلى `MatnDetailViewModel`

### T-003-06: ImportView + OcrReviewView (XAML)
- `ImportView.xaml`: زر اختيار ملف، اسم الملف، ProgressBar، رسالة الحالة، زر بدء OCR، زر إلغاء
- `OcrReviewView.xaml`:
  - عمودان: قائمة الصفحات (يسار)، TextBox للتعديل (يمين، RTL، خط عربي)
  - زر "اعتماد الصفحة"، "اعتماد الكل"، "حفظ وإنشاء المتن"
  - حوار الحفظ: حقلا عنوان وكاتب

### T-003-07: تسجيل OcrReviewViewModel في DI + OcrResultStore
- `OcrResultStore` singleton: `List<OcrPageResult> Results { get; set; }`
- أضف `OcrReviewViewModel` إلى DI + إلى DataTemplates في `MainWindow.xaml`

### T-003-08: Integration tests
- `TesseractOcrService` بصورة بسيطة تحتوي كلمة عربية (skip إذا tessdata غير موجود)
- `ImportMatnFromOcrHandler` بـ in-memory DB: تحقق أن Matn+Sections+TextUnits تُنشأ بالعدد الصحيح
- التحقق من رفض ملف > 50 MB (وحدة اختبار)

---

## ملاحظات تقنية

- Tesseract تحتاج `TesseractEngine` مؤقتاً لكل استدعاء (أو singleton مع lock — اختر singleton)
- PDF → bitmap يحتاج DPI لا يقل عن 300 لدقة OCR جيدة
- `PdfiumViewer` تحتاج native DLL لـ Windows — أضف إلى `.gitignore`
- تأكد أن `tessdata/ara.traineddata` موجود أو ابلغ المستخدم برسالة واضحة عند غيابه
- استخدم `PageSegMode.Auto` أو `PageSegMode.SingleBlock` حسب نتيجة الاختبار
- لا تخزن bytes الصورة أو الـ PDF في DB — معالجة في الذاكرة فقط
