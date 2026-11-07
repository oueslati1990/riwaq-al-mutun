# Spec 008 — Backup & Restore
# المواصفة 008 — النسخ الاحتياطي والاستعادة

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 001 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- إنشاء نسخة احتياطية: `VACUUM INTO` → ملف `.db` يختاره المستخدم
- استعادة: اختيار ملف `.db`، التحقق من صحته (SQLite magic bytes)، استبدال قاعدة البيانات، إعادة تشغيل التطبيق
- واجهة المستخدم في SettingsView: قسم منفصل لكل عملية
- مربع تأكيد قبل الاستعادة (العملية مدمّرة)

### خارج النطاق ❌
- نسخ احتياطي تلقائي بجدول زمني
- تشفير النسخة الاحتياطية
- مزامنة سحابية

---

## القرارات المغلقة / Decisions

| القرار | الخيار |
|---|---|
| آلية الاحتياطي | SQLite `VACUUM INTO` (نسخة نظيفة بدون WAL) |
| التحقق من الملف | 16 بايت أول = "SQLite format 3\0" |
| بعد الاستعادة | إعادة تشغيل التطبيق (ClearAllPools + إعادة الإطلاق) |
| الذرية | VACUUM INTO → ملف مؤقت → نقل إلى الوجهة النهائية |

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-008-01 | الاحتياطي ينشئ ملف `.db` صالحاً |
| AC-008-02 | الاستعادة بملف غير صالح تُظهر رسالة خطأ |
| AC-008-03 | مربع تأكيد يظهر قبل الاستعادة |
| AC-008-04 | التطبيق يُعيد تشغيله بعد الاستعادة الناجحة |

---

## مهام التنفيذ / Tasks

T-008-01: Application — IBackupService + IFileDialogService.SaveFile + CreateBackup + RestoreBackup
T-008-02: Infrastructure — BackupService (VACUUM INTO + SQLite magic bytes validation)
T-008-03: FileDialogService.SaveFile (Presentation)
T-008-04: SettingsViewModel + SettingsView.xaml
T-008-05: DI registration
T-008-06: Tests (CreateBackup + RestoreBackup handlers)
