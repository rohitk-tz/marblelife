# Marblelife Features List

## Overview
This document inventories the features of the Marblelife Franchise Management System.

---

## Feature Status Legend

| Status | Description |
|--------|-------------|
| ✅ IMPLEMENTED | Feature is complete and in production |
| 🚧 IN PROGRESS | Partially implemented or prototype |
| ⏳ PLANNED | Roadmap item |
| ❌ DEPRECATED | Legacy feature being removed |

---

## 1. Customer Relationship Management (CRM)

### Lead Management
| Feature | Status | Description |
|---------|--------|-------------|
| **HomeAdvisor Integration** | ✅ IMPLEMENTED | Auto-ingest leads from HA emails via IMAP polling. |
| **Manual Lead Entry** | ✅ IMPLEMENTED | Form for dispatchers to enter phone-in leads. |
| **Lead Assignment** | ✅ IMPLEMENTED | Auto-route leads to Franchisees based on Zip Code. |
| **Lead Status Pipeline** | ✅ IMPLEMENTED | Track New, Attempted Contact, Appt Set, Sold, Lost. |

### Customer Data
| Feature | Status | Description |
|---------|--------|-------------|
| **Customer Profiling** | ✅ IMPLEMENTED | Store Name, Address, Phone, Email history. |
| **De-duplication** | 🚧 IN PROGRESS | Logic to merge duplicate customer records on import. |
| **Excel Import** | ✅ IMPLEMENTED | Bulk upload of customer lists (`CustomerDataUpload`). |

---

## 2. Field Service Operations

### Scheduling
| Feature | Status | Description |
|---------|--------|-------------|
| **Visual Calendar** | ✅ IMPLEMENTED | Day/Week/Month view of assigned jobs. |
| **Job Creation** | ✅ IMPLEMENTED | Convert Lead/Estimate into a scheduled Job. |
| **Tech Assignment** | ✅ IMPLEMENTED | Assign specific technicians to job slots. |
| **Conflict Detection** | ⏳ PLANNED | Warn if technician is double-booked. |

### Estimation
| Feature | Status | Description |
|---------|--------|-------------|
| **Create Estimate** | ✅ IMPLEMENTED | Build line-item quotes for services (StoneLife, etc.). |
| **Email Estimate** | ✅ IMPLEMENTED | Send PDF quote to customer. |
| **E-Signature** | ⏳ PLANNED | Allow customer to sign estimate online. |

### Job Execution
| Feature | Status | Description |
|---------|--------|-------------|
| **Work Orders** | ✅ IMPLEMENTED | Generate printable work orders for techs. |
| **Before/After Photos** | ✅ IMPLEMENTED | Upload photos to job record. |
| **Tech Feedback** | ✅ IMPLEMENTED | Capture notes from the field. |

---

## 3. Financials & Billing

### Invoicing
| Feature | Status | Description |
|---------|--------|-------------|
| **Generate Invoice** | ✅ IMPLEMENTED | Create invoice from completed Job. |
| **Email Invoice** | ✅ IMPLEMENTED | Send PDF invoice to customer. |
| **Recurring Invoices** | ✅ IMPLEMENTED | Auto-bill for maintenance plans. |
| **Late Fees** | ✅ IMPLEMENTED | Auto-apply fees for overdue invoices. |

### Payments
| Feature | Status | Description |
|---------|--------|-------------|
| **Credit Card Processing** | ✅ IMPLEMENTED | Integration with Authorize.Net. |
| **eCheck / ACH** | ✅ IMPLEMENTED | Bank account draft support. |
| **Payment Profiles** | ✅ IMPLEMENTED | Save card on file (Tokenized). |
| **Refunds** | ✅ IMPLEMENTED | Process refunds via UI. |

---

## 4. Administration

### Franchisee Management
| Feature | Status | Description |
|---------|--------|-------------|
| **Onboarding** | ✅ IMPLEMENTED | Create new Franchisee, assign territory. |
| **Service Config** | ✅ IMPLEMENTED | Toggle available services (TileLok, StoneLife) per Franchisee. |
| **Royalty Settings** | ✅ IMPLEMENTED | Configure royalty % and fee structures. |

### Reporting
| Feature | Status | Description |
|---------|--------|-------------|
| **Growth Dashboard** | ✅ IMPLEMENTED | Visual charts of revenue growth. |
| **Product Mix** | ✅ IMPLEMENTED | Breakdown of sales by service type. |
| **Royalty Reports** | ✅ IMPLEMENTED | Monthly aggregation for HQ billing. |

---

## Feature Roadmap Summary

### Implemented
-   Core ERP Loop (Lead -> Estimate -> Job -> Invoice -> Payment).
-   Multi-Tenant Franchisee Isolation.
-   Background Email Dispatch.

### In Progress
-   Customer De-duplication Logic improvement.
-   Calendar Import Service (Prototype).

### Planned
-   Mobile App for Technicians.
-   Real-time notifications (SignalR).
-   Customer Portal for Self-Service Payment.
