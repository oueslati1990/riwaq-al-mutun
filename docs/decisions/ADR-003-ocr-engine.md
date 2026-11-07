# ADR-003 — OCR Engine Selection

**Date:** 2026-11-07
**Status:** Accepted

## Context

The app needs to extract Arabic text from PDF files and scanned images (PNG/JPEG/TIFF) to allow importing scholarly texts without manual typing.

## Decision

Use **Tesseract 5** (via `Tesseract` NuGet by charlesw) as the OCR engine, with **Docnet.Core** (pdfium wrapper) for PDF page rendering.

## Consequences

**Positive:**
- Fully offline — no data leaves the user's machine
- MIT/Apache licensed — no cost or licensing risk
- Tesseract 5 has significantly improved Arabic support vs. v4
- `tessdata-fast/ara.traineddata` is small (~6 MB) and covers most Arabic text well
- Docnet.Core bundles native pdfium DLLs for Windows and Linux

**Negative:**
- OCR accuracy is lower than cloud services (Google Vision, Azure)
- User must place `ara.traineddata` in the `tessdata/` folder (documented in README)
- No GPU acceleration — large PDFs may be slow on old hardware

## Alternatives Rejected

| Option | Reason |
|---|---|
| Azure AI Vision | Requires internet, costs money, violates local-first principle |
| Google Vision AI | Same as above |
| IronOCR | Commercial license, expensive |
| PaddleOCR | Complex setup, no good .NET wrapper |

## Notes

- tessdata download: `https://github.com/tesseract-ocr/tessdata_fast/raw/main/ara.traineddata`
- DPI for PDF rendering: 300 — good balance of quality vs. speed
- `PageSegMode.Auto` chosen — best for mixed layout Arabic text
