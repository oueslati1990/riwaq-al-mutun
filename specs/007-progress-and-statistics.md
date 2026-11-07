# Spec 007 — Progress & Statistics
# المواصفة 007 — التقدم والإحصاءات

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 005 ✅, spec 006 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- إحصاءات إجمالية: عدد المتون، الأبيات، المحفوظة، المراجعات الكلية، المستحقة اليوم
- إحصاءات لكل متن: شريط تقدم، X/Y بيت محفوظ، عدد المستحق اليوم
- التحديث التلقائي عند فتح الشاشة

### خارج النطاق ❌
- مخططات بيانية أو رسوم (قد تُضاف لاحقاً)
- سجل يومي أو تسلسل أيام الحفظ (streak)
- تصدير الإحصاءات

---

## نموذج البيانات / Data Model

```
OverallStats
  TotalMatns, TotalUnits, StartedUnits, DueTodayCount, TotalReviews

MatnStats
  MatnId, Title, Author?, TotalUnits, StartedUnits, DueTodayCount, CompletionPercent

StatisticsReport = { Overall, IReadOnlyList<MatnStats> }
```

**StartedUnits** = MemorizationProgress.ReviewCount > 0  
**CompletionPercent** = round(100 × StartedUnits / TotalUnits), 0 إذا TotalUnits = 0

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-007-01 | شريط التقدم لمتن 0 بيت يبقى 0% |
| AC-007-02 | متن بـ 10 أبيات محفوظة من 10 يظهر 100% |
| AC-007-03 | الإجمالي يجمع من كل المتون |
| AC-007-04 | الشاشة تُحدَّث بكل مرة تُفتح (Loaded) |

---

## مهام التنفيذ / Tasks

T-007-01: Application — StatisticsReport + IStatisticsRepository + GetStatistics use case
T-007-02: Infrastructure — StatisticsRepository (EF queries + in-memory aggregation)
T-007-03: StatisticsViewModel
T-007-04: StatisticsView.xaml + code-behind
T-007-05: DI registration
T-007-06: Tests (GetStatistics — fake repository)
