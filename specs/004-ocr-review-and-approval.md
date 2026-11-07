# Spec 004 — OCR Review & Approval
# المواصفة 004 — مراجعة OCR واعتماد النص

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 003 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- مشين حالات لكل صفحة OCR: Pending → NeedsReview → Approved
- عرض جانبي: صورة الصفحة الأصلية + محرر النص (side-by-side)
- مؤشر ثقة OCR على مستوى الصفحة (نسبة مئوية)
- حماية النص المعتمد: لا تعديل بدون تأكيد المستخدم
- مؤشرات بصرية لحالة كل صفحة (معلقة / تحتاج مراجعة / معتمدة)
- عرض رقم الصفحة ونسبة الثقة في قائمة الصفحات
- GridSplitter بين الصورة والمحرر

### خارج النطاق ❌
- تمييز كلمات منخفضة الثقة بشكل فردي (يحتاج word-level confidence)
- محرر تشكيل متخصص
- مقارنة نص مضمن PDF مع نص OCR
- حفظ الصور في DB

---

## نموذج الحالة / State Machine

```
Pending ──OCR done──► NeedsReview ──approve──► Approved
                            ▲                      │
                            └──user edits──────────┘
                               (confirmation required)
```

---

## تغييرات نموذج البيانات / Data Changes

لا تغييرات في DB. كل البيانات الجديدة transient في الجلسة.

### تغييرات في Application/Models:
```csharp
// جديد
public enum OcrPageStatus { Pending, NeedsReview, Approved }
public sealed record OcrResult(string Text, float Confidence);

// تعديل OcrPageResult
+ Status: OcrPageStatus
+ Confidence: float        // 0.0 – 1.0
+ PageImageBytes: byte[]   // PNG المُصدَّر، لعرض الصورة الجانبية
```

### تغييرات في IOcrService:
```
- Task<string> RecognizeAsync(...)
+ Task<OcrResult> RecognizeAsync(...)
```

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-004-01 | كل صفحة تبدأ بحالة `Pending`، تنتقل إلى `NeedsReview` بعد OCR |
| AC-004-02 | اعتماد الصفحة ينقلها إلى `Approved`، مع مؤشر بصري واضح |
| AC-004-03 | تعديل نص صفحة `Approved` يطلب تأكيداً قبل إعادتها إلى `NeedsReview` |
| AC-004-04 | عرض صورة الصفحة الأصلية بجانب المحرر |
| AC-004-05 | نسبة ثقة OCR تظهر لكل صفحة في القائمة |
| AC-004-06 | صفحات ثقتها < 60% تظهر بلون تحذيري |
| AC-004-07 | "حفظ وإنشاء المتن" ممكن فقط إذا وجدت صفحة معتمدة واحدة على الأقل |

---

## مهام التنفيذ / Tasks

### T-004-01: Models — OcrPageStatus + OcrResult
- أضف `OcrPageStatus.cs` في `Application/Models/`
- أضف `OcrResult.cs` record في `Application/Models/`
- عدّل `OcrPageResult`: أضف `Status`, `Confidence`, `PageImageBytes`

### T-004-02: تحديث IOcrService + TesseractOcrService
- عدّل `IOcrService.RecognizeAsync` ليعيد `OcrResult`
- عدّل `TesseractOcrService` ليستخرج `page.GetMeanConfidence()` ويعيد `OcrResult`

### T-004-03: تحديث ImportViewModel
- بعد `RenderPageAsync`: احفظ PNG bytes في `OcrPageResult.PageImageBytes`
- بعد `RecognizeAsync`: اقرأ `OcrResult.Confidence` واحفظه
- عيّن `Status = NeedsReview` بعد OCR

### T-004-04: تحديث OcrPageViewModel
- أضف `Status` (observable)، `ConfidencePercent` (int 0-100)، `IsLowConfidence` (< 60%)
- أضف `BitmapSource? PageImage` محسوب من `PageImageBytes`
- في `IsApproved` setter: إذا كانت الحالة `Approved` والقيمة false → حالة `NeedsReview`
- أضف `GuardedEditCommand`: إذا `Approved` → طلب تأكيد، إذا نعم → `NeedsReview`

### T-004-05: تحديث OcrReviewViewModel
- أضف `IsConfirmReEditOpen` + `PageToReEdit` للحوار
- أضف `ConfirmReEditCommand` و `CancelReEditCommand`
- `OpenImportDialogCommand` يتحقق من وجود صفحة `Approved` واحدة

### T-004-06: إعادة كتابة OcrReviewView.xaml
- 3 أعمدة: قائمة الصفحات (160px) | GridSplitter | Image+Editor (*)
- القسم الأيمن: Grid بصفين: صورة (Auto/300) | GridSplitter | TextBox (*)
- مؤشرات الحالة في قائمة الصفحات: دائرة ملونة (رمادي/برتقالي/أخضر)
- نسبة الثقة بالنسبة المئوية بجانب رقم الصفحة
- لون تحذيري (برتقالي) لصفحات ثقتها < 60%
- حوار تأكيد إعادة التعديل

### T-004-07: اختبارات
- وحدة: انتقال الحالة (Pending→NeedsReview→Approved)
- وحدة: حماية النص المعتمد
- وحدة: `ImportMatnFromOcr` يستخدم فقط الصفحات `Approved`
