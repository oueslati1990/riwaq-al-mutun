# Spec 006 — Spaced Review Scheduler
# المواصفة 006 — جدولة المراجعات المتباعدة

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 005 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- جلسة مراجعة تعرض فقط الأبيات التي حان موعد مراجعتها اليوم (NextReviewDate ≤ today)
- الأبيات من جميع المتون (ليس متناً واحداً فقط)
- نفس منطق الكشف والتقييم (0/3/5) من spec 005
- رسالة تهنئة عند عدم وجود مراجعات مستحقة
- عرض اسم المتن والباب كسياق لكل بيت

### خارج النطاق ❌
- تعديل خوارزمية SM-2 (محسومة في spec 005)
- فلترة بمتن محدد (spec 007 — الإحصاءات)
- التبشير بمواعيد المراجعة القادمة (spec 007)

---

## القرارات المغلقة / Decisions

| القرار | الخيار |
|---|---|
| ما يظهر في جلسة المراجعة | NextReviewDate ≤ today فقط (null = لم يُحفظ بعد = خارج المراجعة) |
| مصدر الأبيات | كل المتون، بدون تصفية |
| الترتيب | الأكثر تأخراً أولاً (NextReviewDate تصاعدياً) |

---

## نموذج الاستعلام / Query Model

```
DueUnit
  TextUnitId, Content, SectionTitle, MatnTitle, NextReviewDate
  (JOIN: MemorizationProgress → TextUnit → MatnSection → Matn)
  WHERE NextReviewDate <= today
  ORDER BY NextReviewDate ASC
```

---

## تدفق الاستخدام / Use Flow

```
الشريط الجانبي → "المراجعة"
  → ReviewView
       → تحميل الأبيات المستحقة
       إذا 0: "أحسنت! لا توجد مراجعات اليوم"
       إذا N > 0:
            نص مخفي → "كشف" → نص ظاهر
            → [نسيت | صعب | سهل] → RecordAttempt → بيت تالٍ
       → شاشة إنهاء مع عدد المراجعات المُنجزة
```

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-006-01 | بيت بـ NextReviewDate = null لا يظهر في المراجعة |
| AC-006-02 | بيت بـ NextReviewDate = اليوم يظهر |
| AC-006-03 | بيت بـ NextReviewDate = أمس يظهر (متأخر) |
| AC-006-04 | بيت بـ NextReviewDate = غداً لا يظهر |
| AC-006-05 | عند عدم وجود مراجعات: رسالة تهنئة واضحة |
| AC-006-06 | يظهر اسم المتن والباب فوق كل بيت |
| AC-006-07 | الأبيات مرتبة بـ NextReviewDate تصاعدياً |

---

## مهام التنفيذ / Tasks

T-006-01: Application — DueUnit + GetDueUnits use case + IMemorizationProgressRepository.GetDueUnitsAsync
T-006-02: Infrastructure — EF join query in MemorizationProgressRepository
T-006-03: ReviewViewModel
T-006-04: ReviewView.xaml + code-behind
T-006-05: Tests (GetDueUnits AC-006-01..04)
