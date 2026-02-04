<!-- AUTO-GENERATED: Header -->
# Franchisee Migration
> One-time Static Data Seeder
<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Overview -->
## Overview

This tool seeds the database with the initial set of MarbleLife Franchisees. It defines their contact info, territories, fee structures, and services offered.

**It is effectively a hardcoded database dump.**

<!-- END AUTO-GENERATED -->

<!-- AUTO-GENERATED: Usage -->
## 🚀 Usage

### When to run?
-   Only on **Project Initialization**.
-   Do **NOT** run this against an existing production database; it may create duplicates.

### Logic
-   Iterates through the hardcoded list in `FranchiseeFromFileCollection`.
-   Calls `Core.Organizations` services to save them.

<!-- END AUTO-GENERATED -->
