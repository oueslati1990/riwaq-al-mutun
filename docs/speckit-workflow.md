# دليل سير العمل بـ Spec-Kit — رِواق المتون
# Spec-Kit Workflow Guide — Riwaq al-Mutun

---

## نظرة عامة على الدورة / Lifecycle Overview

```
Specify → Clarify → Plan → Tasks → Implement → Converge
   ↑                                                  |
   └──────────────── next feature ───────────────────┘
```

كل ميزة تمر بهذه المراحل بالترتيب. لا تبدأ Implement قبل اعتماد Tasks.  
Every feature goes through these stages in order. Never start Implement before Tasks are approved.

---

## مرجع الأوامر / Command Reference

| الأمر / Command | ما يفعله / What it does | متى تستخدمه / When to run |
|---|---|---|
| `/speckit.constitution` | ينشئ أو يعدّل دستور المشروع (المبادئ الثابتة) | مرة واحدة عند البداية، أو عند تغيير مبدأ أساسي |
| `/speckit.specify` | يكتب مواصفة ميزة: المشكلة، المستخدم، السلوك المطلوب | أول خطوة لكل ميزة جديدة |
| `/speckit.clarify` | يزيل الغموض، يحدد الحالات الحدية، يراجع المتطلبات | بعد specify، قبل plan |
| `/speckit.plan` | يقرر المعمارية والتقنيات والبيانات والاختبارات للميزة | بعد clarify واعتماد المتطلبات |
| `/speckit.tasks` | يقسم الخطة إلى مهام صغيرة ذات مخرجات قابلة للتحقق | بعد plan واعتماد المعمارية |
| `/speckit.implement` | ينفذ المهام بالترتيب مع البناء والاختبار | بعد اعتماد tasks |
| `/speckit.analyze` | يحلل فجوات بين التنفيذ والمواصفة | عند Converge أو عند الشك |

> **ملاحظة:** بعد تشغيل `specify init`, افحص الأوامر المولودة في `.specify/commands/` — هي المرجع الفعلي لصيغة الأوامر في مشروعك.

---

## أوامر الإعداد الأولي / Initial Setup Commands

```bash
# 1. تثبيت Specify CLI
uv tool install specify-cli

# 2. التحقق من التثبيت
specify --version

# 3. تهيئة المشروع مع تكامل Claude
cd ~/Bureau/Dotnet-Projects/MatnMemorization
specify init . --integration claude

# 4. التحقق من الملفات المنشأة
ls .specify/
ls specs/
```

---

## قائمة المميزات ومراحلها / Feature Checklist

### 000 — Vision & MVP (`specs/000-vision-and-mvp.md`)
**الهدف:** تحديد نطاق المنتج، المستخدم المستهدف، معايير MVP

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | سيناريو استخدام واحد على الأقل واضح |
| Clarify | `/speckit.clarify` | معايير MVP قابلة للاختبار (من §16) |
| ✅ لا plan/tasks | — | هذه ليست ميزة تقنية، بل بوابة رؤية |

**نقطة توقف بشرية:** اعتماد نطاق MVP النهائي قبل الانتقال لـ 001.

---

### 001 — Foundation & Navigation (`specs/001-foundation-and-navigation.md`)
**الهدف:** Solution structure، DI، Logging، WPF shell مع RTL، Navigation، SQLite

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | قائمة الشاشات والتدفق بين الشاشات |
| Clarify | `/speckit.clarify` | هيكل Navigation واضح |
| Plan | `/speckit.plan` | طبقات Solution وأسمائها ونظام DI |
| Tasks | `/speckit.tasks` | مهمة واحدة = وحدة بناء قابلة للاختبار |
| Implement | `/speckit.implement` | `dotnet build` و `dotnet test` ناجحان |
| Converge | `/speckit.analyze` | WPF shell يفتح بـ RTL وNavigation يعمل |

**نقطة توقف بشرية:** اعتماد هيكل Solution واختيار النمط النهائي قبل بدء التنفيذ.

---

### 002 — Matn Library & Editor (`specs/002-matn-library-and-editor.md`)
**الهدف:** إضافة/تعديل/حذف متن، أقسام، وحدات حفظ، محرر عربي، بحث

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | نموذج البيانات (من §8: Matn, MatnSection, TextUnit) |
| Clarify | `/speckit.clarify` | سلوك البحث العربي (أشكال الحروف، التشكيل) |
| Plan | `/speckit.plan` | EF Core migrations، Repository interfaces |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | اختبارات RTL، تشكيل في الخطوط المختارة |

---

### 003 — Document Import & Arabic OCR (`specs/003-document-import-and-arabic-ocr.md`)
**الهدف:** استيراد صورة/PDF، استخراج نص مضمن، تحويل الصفحات، OCR عربي

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | سير عمل الاستيراد (من §9) |
| Clarify | `/speckit.clarify` | **حسم OCR محلي vs سحابي** ← نقطة توقف بشرية |
| Plan | `/speckit.plan` | تصميم IArabicOcrProvider وباقي الـ interfaces |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | دقة OCR على عينات اختبار عربية |

**نقاط توقف بشرية:**
- اختيار OCR سحابي أو محلي (قبل Plan)
- أي مكتبة ترخيصها غير واضح (قبل إضافتها)

---

### 004 — OCR Review & Approval (`specs/004-ocr-review-and-approval.md`)
**الهدف:** شاشة مقارنة النص والصورة، تصحيح وتشكيل، اعتماد النص

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | حالات الحالة: Imported→Extracting→NeedsReview→Approved |
| Clarify | `/speckit.clarify` | سلوك التشكيل غير الواثق |
| Plan | `/speckit.plan` | محرر نص عربي مع تمييز المواضع |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | لا يستبدل نصاً معتمداً بدون موافقة |

---

### 005 — Memorization Sessions (`specs/005-memorization-sessions.md`)
**الهدف:** جلسات حفظ جديدة، إخفاء/إظهار، تسميع ذاتي، تسجيل التقدم

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | أنواع الجلسات وسلوك وحدة الحفظ |
| Clarify | `/speckit.clarify` | **تعريف مقياس التقييم** ← نقطة توقف بشرية |
| Plan | `/speckit.plan` | MemorizationProgress، ReviewAttempt entities |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | تقدم يُحفظ عبر إعادة تشغيل التطبيق |

---

### 006 — Spaced Review Scheduler (`specs/006-spaced-review-scheduler.md`)
**الهدف:** جدولة المراجعة المتباعدة، قائمة «مراجعات اليوم»، إحصاءات

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | سلوك IReviewScheduler |
| Clarify | `/speckit.clarify` | **اعتماد خوارزمية المراجعة (SM-2 أو غيرها)** ← نقطة توقف بشرية |
| Plan | `/speckit.plan` | ReviewSchedule، معاملات الخوارزمية |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | unit tests لحساب المواعيد |

---

### 007 — Progress & Statistics (`specs/007-progress-and-statistics.md`)
**الهدف:** الالتزام، السلاسل الزمنية، نسبة الإتقان، المتأخرات

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | تعريف «محفوظ» و«متقن» ومؤشراتهما |
| Clarify | `/speckit.clarify` | لا تضلّل المستخدم بمؤشرات مبهمة |
| Plan | `/speckit.plan` | Queries محسوبة، لا بيانات مكررة |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | — |

---

### 008 — Backup & Restore (`specs/008-backup-and-restore.md`)
**الهدف:** نسخ احتياطي ذري، استعادة، تصدير واستيراد

| المرحلة | الأمر | بوابة الاعتماد |
|---|---|---|
| Specify | `/speckit.specify` | سياسة النسخ الاحتياطي وصيغ الملف |
| Clarify | `/speckit.clarify` | التحقق من سلامة النسخة |
| Plan | `/speckit.plan` | ذرية العملية، لا تخسر بيانات جزئياً |
| Tasks | `/speckit.tasks` | — |
| Implement | `/speckit.implement` | — |
| Converge | `/speckit.analyze` | اختبار استعادة فعلي، ليس فقط اختبار حفظ |

---

## بوابات المراجعة البشرية الإجبارية / Mandatory Human Review Gates

توقف واطلب اعتماداً صريحاً قبل:

1. **اختيار OCR سحابي** — ينطوي على إرسال ملفات المستخدم خارج الجهاز
2. **اعتماد خوارزمية المراجعة** — يؤثر على تجربة الحفظ كلياً
3. **اعتماد مقياس التقييم** — يؤثر على حساب المواعيد
4. **تغيير نموذج البيانات** بعد وجود بيانات فعلية
5. **إضافة مكتبة ترخيصها غير واضح** أو بها تكلفة تشغيلية
6. **اعتماد نطاق MVP النهائي** (بعد spec 000)
7. **أي migration قد يحذف أو يغير بيانات**

---

## ترتيب التنفيذ المقترح / Recommended Execution Order

```
000-vision-and-mvp          ← بوابة: اعتماد MVP
        ↓
001-foundation-and-navigation  ← بوابة: هيكل Solution
        ↓
002-matn-library-and-editor    ← بوابة: نموذج البيانات
        ↓
003-document-import-and-ocr    ← بوابة: اختيار OCR
        ↓
004-ocr-review-and-approval
        ↓
005-memorization-sessions      ← بوابة: مقياس التقييم
        ↓
006-spaced-review-scheduler    ← بوابة: خوارزمية المراجعة
        ↓
007-progress-and-statistics
        ↓
008-backup-and-restore
```

---

## أوامر التحقق القياسية / Standard Verification Commands

```bash
dotnet --info
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

يجب أن ينجح البناء والاختبارات قبل اعتبار أي مهمة مكتملة.
