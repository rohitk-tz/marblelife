# Billing/Domain - AI Context

## Purpose

This folder contains the domain entities and models for the **Billing** module. These are the core business objects that represent the data structures and business concepts.

## Contents

Domain entities in this folder:
- Entity classes with properties and relationships
- Value objects for complex types
- Domain-specific data structures
- Entity relationships and navigation properties

## For AI Agents

**Entity Classes**:
- All entities inherit from `DomainBase` or implement similar base functionality
- Properties follow C# naming conventions (PascalCase)
- Navigation properties for related entities
- Data annotations for validation and ORM mapping

**Modifying Entities**:
1. Update entity class with new properties
2. Update corresponding ViewModels in ../ViewModel folder
3. Update factory and service interfaces if business logic changes
4. Update database mappings in ORM project
5. Create migration script in DatabaseDeploy project

**Best Practices**:
- Keep domain entities focused on data structure
- Business logic belongs in service implementations (../Impl folder)
- Use value objects for complex types
- Maintain consistency with database schema
- Document complex relationships with XML comments

## For Human Developers

Domain entities represent the core business data model. When adding or modifying entities:
- Ensure proper Entity Framework mappings
- Update related ViewModels for API communication
- Consider impact on existing data (migrations required)
- Maintain referential integrity
- Document entity relationships clearly
