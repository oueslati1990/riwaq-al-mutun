# Spec 005 — Memorization Sessions
# المواصفة 005 — جلسات الحفظ

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 002 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- جلسة حفظ تعرض أبياتاً من متن محدد، واحداً تلو الآخر
- إخفاء النص ثم كشفه عند طلب المستخدم
- تقييم ثلاثي: نسيت (0) / صعب (3) / سهل (5)
- خوارزمية SM-2 المبسطة لحساب الفاصل الزمني للمراجعة القادمة
- حفظ تقدم كل بيت في DB (MemorizationProgress + ReviewAttempt)
- إنهاء الجلسة عند تقييم الكل + شاشة إنهاء مع إمكانية إعادة البدء
- الحد الأقصى 20 بيتاً لكل جلسة
- زر "بدء الحفظ" في MatnDetailView

### خارج النطاق ❌
- فلترة الأبيات المستحقة للمراجعة (spec 006)
- إحصاءات التقدم (spec 007)
- صوت أو تسجيل صوتي
- جلسة بدون متن محدد

---

## القرارات المغلقة / Decisions

| القرار | الخيار |
|---|---|
| خوارزمية المراجعة | SM-2 مبسطة |
| مقياس التقييم | 3 نقاط: 0 / 3 / 5 |
| حجم الجلسة | 20 بيتاً كحد أقصى |

---

## نموذج البيانات / Domain Model

```
MemorizationProgress
  Id, TextUnitId (unique FK), EaseFactor=2.5, Interval=0,
  ReviewCount=0, NextReviewDate?, LastReviewedAt?

ReviewAttempt
  Id, MemorizationProgressId (FK), Quality (0/3/5), ReviewedAt
```

### SM-2 مبسطة
```
إذا quality < 3:
  interval = 1، EF لا تتغير، ReviewCount = 0
إذا quality >= 3:
  interval[0] = 1، interval[1] = 6، interval[n] = round(prev × EF)
  EF = max(1.3, EF + 0.1 - (5-q)*(0.08 + (5-q)*0.02))
  ReviewCount++
NextReviewDate = اليوم + interval
```

---

## تدفق الاستخدام / Use Flow

```
MatnDetailView → "بدء الحفظ"
  → MemorizationView
       → تحميل أول 20 بيتاً
       → لكل بيت:
            نص مخفي → "كشف" → نص ظاهر
            → [نسيت | صعب | سهل]  → RecordAttempt → بيت تالٍ
       → شاشة إنهاء → "بدء جلسة جديدة"
```

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-005-01 | النص مخفي عند بداية كل بيت، يظهر بعد "كشف" |
| AC-005-02 | كل تقييم يُحدث MemorizationProgress في DB |
| AC-005-03 | بيت جديد لم يُراجَع قبلاً → ينشئ MemorizationProgress جديداً |
| AC-005-04 | NextReviewDate تتحسب صحيحاً (SM-2) |
| AC-005-05 | شاشة إنهاء تظهر بعد آخر بيت |
| AC-005-06 | "بدء جلسة جديدة" يعيد الجلسة من أول البيت الأول |
| AC-005-07 | زر "بدء الحفظ" ظاهر في MatnDetailView |

---

## مهام التنفيذ / Tasks

T-005-01: Domain — MemorizationProgress + ReviewAttempt + Sm2Calculator
T-005-02: Application — IMemorizationProgressRepository + StartSession + RecordAttempt
T-005-03: Infrastructure — EF configurations + migration + repository
T-005-04: MemorizationViewModel + MatnDetailViewModel (StartSessionCommand)
T-005-05: MemorizationView.xaml + MatnDetailView.xaml (بدء الحفظ)
T-005-06: DI registration
T-005-07: Tests (Sm2Calculator + RecordAttempt)
