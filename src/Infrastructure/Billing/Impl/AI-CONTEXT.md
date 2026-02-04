# Infrastructure/Billing/Impl - AI Context

## Purpose

Contains concrete implementations of billing service interfaces, including payment gateway integrations and billing utilities.

## Contents

Implementation classes:
- **AuthorizeNetService**: Authorize.Net payment processing
- **PaymentProcessor**: Payment workflow orchestration
- **InvoiceGenerator**: Invoice creation and formatting
- **TransactionLogger**: Payment transaction logging

## For AI Agents

Implementations integrate with external payment gateways and handle billing operations.

**Key Responsibilities**:
- Process credit card payments
- Handle payment authorization and capture
- Generate invoices
- Log all payment transactions
- Handle payment failures and retries

## For Human Developers

Payment implementations must be:
- PCI compliant (never store raw card data)
- Idempotent (safe to retry)
- Well-logged (audit trail for all transactions)
- Error-tolerant (handle gateway failures)
- Thoroughly tested (use sandbox mode)
