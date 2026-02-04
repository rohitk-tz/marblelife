# Web.UI/reports Module - AI Context

## Purpose

The **reports** AngularJS module handles reports-related functionality in the MarbleLife web application.

## Structure

- **controllers/**: AngularJS controllers for reports views
- **services/**: Data services and API communication
- **directives/**: Custom AngularJS directives (if exists)
- **views/**: HTML templates for reports pages

## For AI Agents

**Module Pattern**:
Controllers handle view logic and user interactions, services communicate with API endpoints, directives provide reusable UI components, and views define the HTML templates.

**Common Operations**:
- Controllers inject services via dependency injection
- Services use HttpWrapper for API calls
- Views use AngularJS data binding
- Follow established routing patterns

## For Human Developers

When working with this module:
- Controllers in controllers/ folder
- API services in services/ folder  
- HTML templates in views/ folder
- Follow AngularJS 1.x best practices
- Use HttpWrapper service for API calls
- Implement proper error handling
