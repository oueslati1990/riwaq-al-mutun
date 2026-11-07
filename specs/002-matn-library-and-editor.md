# Spec 002 — Matn Library & Editor
# المواصفة 002 — مكتبة المتون والمحرر

**الحالة / Status:** ✅ Approved — ready for tasks
**المرحلة / Stage:** Specify → Clarify → Plan → **Tasks**
**يسبقه / Depends on:** spec 001 ✅

---

## النطاق / Scope

### داخل النطاق ✅
- Domain entities: Matn, MatnSection, TextUnit (with invariants)
- EF Core configuration + migration for the 3 entities
- Repository interfaces in Application layer
- Repository implementations in Infrastructure
- Use cases: CreateMatn, UpdateMatn, DeleteMatn (soft delete), AddSection, AddTextUnit
- LibraryPage: list matns, search by title/author, filter by category/status
- MatnDetailPage: view matn sections and text units
- LibraryView + MatnDetailView wired to real ViewModels with DI
- Arabic search: diacritic-stripped search index alongside original text

### خارج النطاق ❌
- OCR / PDF import (spec 003)
- Memorization progress or review scheduling (specs 005–006)
- Backup/restore (spec 008)
- Full-text search with FTS5 (can be added later)

---

## Domain Entities

### Matn
```
Id: Guid
Title: string (required, 1–200 chars)
Author: string? 
Category: string?
Description: string?
Status: MatnStatus (Draft | Active | Archived)
CreatedAt: DateTimeOffset
UpdatedAt: DateTimeOffset
Sections: IReadOnlyList<MatnSection>
```

### MatnSection
```
Id: Guid
MatnId: Guid (FK)
Title: string (required, 1–200 chars)
Order: int
TextUnits: IReadOnlyList<TextUnit>
```

### TextUnit
```
Id: Guid
SectionId: Guid (FK)
Content: string (required, 1–2000 chars)  ← original with diacritics
SearchIndex: string                         ← stripped for search, never displayed
Order: int
CreatedAt: DateTimeOffset
UpdatedAt: DateTimeOffset
```

**Invariants:**
- Order values are unique within their parent (Section within Matn, TextUnit within Section)
- Content cannot be whitespace-only
- Deleting a Matn soft-deletes it (Status = Archived, IsDeleted flag); no cascade hard delete
- SearchIndex is always derived from Content — never set directly by caller

---

## معايير القبول / Acceptance Criteria

| # | المعيار |
|---|---|
| AC-01 | المستخدم ينشئ متناً بعنوان وكاتب، يظهر في قائمة المكتبة |
| AC-02 | المستخدم يضيف باباً وأبياتاً لمتن موجود |
| AC-03 | البحث بالعنوان يعمل مع وبدون تشكيل (تَحْفِيظ ← تحفيظ) |
| AC-04 | الحذف الناعم: المتن المحذوف لا يظهر في القائمة الافتراضية |
| AC-05 | الترتيب محفوظ عند الاسترجاع |
| AC-06 | `dotnet build` و`dotnet test` ينجحان بدون تحذيرات |
| AC-07 | اختبارات وحدة تغطي: invariants الكيانات، تطبيع البحث العربي |

---

## Tasks

### T-002-01: Domain entities + enums
- `MatnStatus` enum في Domain
- `Matn`, `MatnSection`, `TextUnit` entities مع factory methods وinvariants
- `ArabicNormalizer` utility في Domain لاستخراج SearchIndex

### T-002-02: Repository interfaces في Application
- `IMatnRepository`: GetAll, GetById, Add, Update, SoftDelete
- `IMatnSectionRepository`: Add, Update, Delete (hard — section under draft matn)
- `ITextUnitRepository`: Add, Update, Delete, Reorder

### T-002-03: EF Core configuration + migration
- `MatnConfiguration`, `MatnSectionConfiguration`, `TextUnitConfiguration`
- Migration: `AddMatnEntities`
- Register DbSets في AppDbContext

### T-002-04: Repository implementations
- `MatnRepository`, `MatnSectionRepository`, `TextUnitRepository` في Infrastructure

### T-002-05: Use cases في Application
- `CreateMatnCommand` + handler
- `UpdateMatnCommand` + handler
- `SoftDeleteMatnCommand` + handler
- `AddSectionCommand` + handler
- `AddTextUnitCommand` + handler
- `GetMatnListQuery` + handler (يدعم search + filter)
- `GetMatnDetailQuery` + handler

### T-002-06: LibraryViewModel + LibraryView
- قائمة المتون مع pagination أو virtualization
- حقل بحث يفلتر في الوقت الفعلي
- زر «إضافة متن» يفتح dialog
- تنقل إلى MatnDetailPage عند النقر على متن

### T-002-07: MatnDetailViewModel + MatnDetailView
- عرض أبواب المتن وأبياته
- زر إضافة باب، زر إضافة بيت
- عرض النص بخط عربي مناسب، RTL

### T-002-08: Unit tests
- Invariants: Content فارغ، Order مكرر، SearchIndex مشتق
- ArabicNormalizer: أشكال الألف، التشكيل، التاء المربوطة، الياء
- Use case handlers مع in-memory repositories

**الخطوة التالية:** T-002-01
