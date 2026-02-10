# Core.Billing Module Documentation

## Documentation Overview

This folder contains comprehensive AI-generated context documentation for the Core.Billing module.

### Files Generated

1. **CONTEXT.md** (649 lines)
   - Architectural mental model
   - Complete file inventory (all 117 files with descriptions)
   - Critical type definitions with code samples
   - Public interface reference with input/output/behavior documentation
   - Dependency mappings
   - Developer insights and best practices

2. **OVERVIEW.md** (437 lines)
   - Human-readable module guide
   - Real-world analogies
   - Usage examples with runnable code
   - API summary tables
   - Troubleshooting section
   - Business rules reference

3. **metadata.json** (33 lines)
   - Version tracking information
   - File count statistics
   - Key components list
   - Critical workflows reference

### Module Summary

**Total Files**: 117 C# files organized into:
- **Root Level**: 25 service and factory interfaces
- **Constants/**: 2 files (service type aliases)
- **Domain/**: 31 entities (Invoice, Payment, ChargeCard, ECheck, audit entities)
- **Enum/**: 13 enumerations (InvoiceStatus, PaymentStatus, InstrumentType, etc.)
- **Impl/**: 21 implementations (services and factories)
- **ViewModel/**: 27 DTOs (input/output models for API layer)

### Key Focus Areas

1. **Payment Processing Workflows**
   - Credit card processing via Authorize.Net
   - eCheck (ACH) processing
   - Paper check recording
   - Saved payment profiles (Customer Information Manager)
   - Account credit application

2. **Authorize.Net Integration**
   - Payment gateway API integration
   - Customer profile management
   - Transaction processing and response handling
   - Rollback/refund capabilities

3. **Invoice Management**
   - Invoice generation from sales data
   - Multi-item invoices (royalty, ad-fund, service fees)
   - Status tracking (Paid, Unpaid, PartialPaid, Canceled, ZeroDue)
   - QuickBooks export functionality

4. **Late Fee Automation**
   - Scheduled polling agent (background service)
   - Configurable grace periods
   - Percentage or flat-rate calculations
   - Email notifications

5. **Royalty Calculations**
   - Percentage-based royalty fees
   - Ad-fund contributions
   - Service type categorization
   - Currency exchange rate support

### Critical Components

- **PaymentService**: Orchestrates all payment processing
- **InvoiceService**: Core invoice CRUD and export operations
- **InvoiceLateFeePollingAgent**: Automated late fee generation
- **ChargeCardPaymentService**: Authorize.Net credit card integration
- **ECheckPaymentService**: Authorize.Net ACH integration
- **ProcessorResponse**: Critical DTO for payment gateway responses

### Documentation Standards

This documentation follows the DevMind module-context skill format:
- **AUTO-GENERATED sections**: Reflects current codebase state
- **CUSTOM SECTION markers**: Preserved across regenerations for manual notes
- **Per-file inventory**: Every file documented with purpose
- **No fluff**: Concrete examples, real type definitions, actual behavior descriptions

### Staleness Detection

The `metadata.json` tracks the commit hash when documentation was generated. To check if docs are stale:

```bash
git diff 756207f800ce20f013d92c41cb12e0f8a8fbf48e HEAD -- src/Core/Billing/
```

If changes are detected, regenerate docs with:
```bash
@workspace Document Core/Billing folder --force
```

### Usage for AI Agents

When working with Core.Billing code:
1. Read `CONTEXT.md` for architectural understanding
2. Reference type definitions for data structures
3. Use interface documentation for method signatures and behavior
4. Check OVERVIEW.md for usage examples
5. Consult troubleshooting section for common issues

This documentation is designed to **replace** reading source code files directly - all critical information is extracted and organized here.

---
**Generated**: 2025-01-10T00:00:00Z  
**Commit**: 756207f800ce20f013d92c41cb12e0f8a8fbf48e  
**Module**: src/Core/Billing
