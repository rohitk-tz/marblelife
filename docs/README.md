# Marblelife Documentation Index

## Overview
Welcome to the comprehensive documentation for the Marblelife Franchise Management System. This documentation is intended for Developers, Architects, and System Administrators maintaining or extending the platform.

---

## Documentation Structure

```
docs/
├── ARCHITECTURE.md          # System stack, diagrams, and deployment topology
├── DATA_FLOW.md             # Diagrams of Lead, Job, and Payment flows
├── SEQUENCE_DIAGRAMS.md     # Detailed interaction diagrams (Auth, API calls)
├── PII_DATA.md              # Privacy, Data Classification, and Compliance
├── DATA_SECURITY.md         # Security Architecture, RBAC, and Encryption
├── FEATURES_LIST.md         # Inventory of system capabilities
├── MODULES_LIST.md          # Guide to the Source Code structure
├── CONFIGURABLE_DESIGN.md   # Configuration keys and environment settings
└── README.md                # This file
```

---

## Quick Reference Guide

### For Developers
| Document | Purpose | When to Use |
|----------|---------|-------------|
| **MODULES_LIST** | Understand Code Organization | When trying to find *where* a feature is implemented. |
| **ARCHITECTURE** | High-level View | Onboarding to the project stack. |
| **SEQUENCE_DIAGRAMS** | Logic Flow | Debugging complex interactions like Payments. |

### For Security/Compliance
| Document | Purpose | When to Use |
|----------|---------|-------------|
| **PII_DATA** | Data Privacy | preparing for an Audit or Privacy Impact Assessment. |
| **DATA_SECURITY** | Controls & Auth | Reviewing access control policies. |

### For Product Owners
| Document | Purpose | When to Use |
|----------|---------|-------------|
| **FEATURES_LIST** | Capability Inventory | Assessing current state vs roadmap. |

---

## Document Summaries

### Architecture
Overview of the ASP.NET + AngularJS stack, SQL Server data layer, and Windows Service background jobs. includes High-Level diagrams.

### Data Flow
Visualizes how information moves from "Lead" to "Paid Invoice". Critical for understanding the lifecycle of business entities.

### PII Data
Classifies sensitive data (Passwords, Credit Card Tokens, Customer Contact Info) and defines encryption/masking requirements.

### Sequence Diagrams
Step-by-step logic flows for Authentication, Dashboard Loading, and Payment Processing.

### Configurable Design
Reference for `Web.config` and `App.config` keys, including Feature Flags and external API credentials.
