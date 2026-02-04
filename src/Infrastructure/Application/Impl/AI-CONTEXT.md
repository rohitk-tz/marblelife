# Infrastructure/Application/Impl - AI Context

## Purpose

Contains concrete implementations of application infrastructure services defined in Core/Application.

## Contents

Implementation classes:
- **Repository<T>**: Generic repository implementation
- **UnitOfWork**: Transaction management
- **LogService**: Logging implementation
- **FileService**: File operations
- **CryptographyService**: Password hashing
- **PdfGenerator**: PDF generation
- **ExcelCreator**: Excel file creation

## For AI Agents

Implementations use Entity Framework, external libraries, and system APIs to provide concrete functionality for Core service interfaces.

## For Human Developers

Follow dependency injection pattern:
1. Implement interface from Core/Application
2. Register in DependencyInjection module
3. Inject dependencies via constructor
4. Use async/await for I/O operations
5. Log operations and errors
