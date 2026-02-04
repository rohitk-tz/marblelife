# API/Areas/Organizations - AI Context

## Purpose

The **Organizations** area handles all franchisee-related operations, including franchisee management, fee structures, documents, and organizational hierarchies. This area exposes the Core/Organizations business logic through RESTful endpoints.

## Structure

```
Organizations/
├── OrganizationsAreaRegistration.cs     # Area route registration
└── Controllers/
    ├── FranchiseeController.cs          # Main franchisee CRUD operations
    └── FranchiseeDocumentController.cs   # Document management
```

## Controllers

### FranchiseeController.cs
**Purpose**: Manages franchisee entities, fee profiles, and franchisee-user assignments.

**Key Endpoints**:

#### Get Single Franchisee
```
GET /Organizations/Franchisee/{id}
Response: FranchiseeEditModel
```

#### Get Franchisee List
```
GET /Organizations/Franchisee?pageNumber=1&pageSize=25&status=1
Response: FranchiseeListModel (paginated)
```

#### Create/Update Franchisee
```
POST /Organizations/Franchisee
Body: FranchiseeEditModel
Response: boolean (success)
```

#### Delete Franchisee
```
GET /Organizations/Franchisee/DeleteFranchisee?id={id}
Response: boolean (success if no dependencies)
```

#### Get Fee Profile
```
GET /Organizations/Franchisee/GetFeeProfile?id={id}
Response: FeeProfileViewModel
```

#### Get Franchisees for User
```
GET /Organizations/Franchisee/GetFranchiseeListForLogin?userId={id}
Response: FranchiseeInfoListModel (franchisees user can access)
```

#### Manage Franchisee Assignment
```
POST /Organizations/Franchisee/ManageFranchisee
Body: ManageFranchiseeAccountModel
Response: boolean (success)
```

**Business Rules**:
- Super admins can manage all franchisees
- Franchisee admins can only manage their own franchisee
- Cannot delete franchisee with existing sales data or active jobs
- Fee profiles must be configured for billing
- Geographic territories must not overlap

### FranchiseeDocumentController.cs
**Purpose**: Manages franchisee-related documents (contracts, licenses, insurance, etc.)

**Key Endpoints**:

#### Upload Document
```
POST /Organizations/FranchiseeDocument/Upload
Body: Multipart form data with file + metadata
Response: FileUploadResult
```

#### Download Document
```
GET /Organizations/FranchiseeDocument/Download/{id}
Response: File stream
```

#### Get Document List
```
GET /Organizations/FranchiseeDocument/GetDocuments?franchiseeId={id}
Response: List<FranchiseeDocumentViewModel>
```

#### Delete Document
```
DELETE /Organizations/FranchiseeDocument/{id}
Response: boolean
```

**Document Types**:
- Franchise Agreement
- Business License
- Insurance Certificate
- Tax Forms (W9, etc.)
- Operating Permits
- Training Certificates

## Key ViewModels

### FranchiseeEditModel
```csharp
public class FranchiseeEditModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    
    // Business Information
    public DateTime? StartDate { get; set; }
    public bool IsActive { get; set; }
    public string TerritoryDescription { get; set; }
    
    // Fee Structure
    public long? FeeProfileId { get; set; }
    public decimal? RoyaltyPercentage { get; set; }
    
    // Contact Information
    public string PrimaryContactName { get; set; }
    public string PrimaryContactPhone { get; set; }
    public string PrimaryContactEmail { get; set; }
    
    // Audit Fields
    public long UserId { get; set; }  // Current user making changes
}
```

### FranchiseeListModel
```csharp
public class FranchiseeListModel
{
    public List<FranchiseeListItem> Franchisees { get; set; }
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class FranchiseeListItem
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }
    public decimal YearToDateRevenue { get; set; }
    public int ActiveJobs { get; set; }
}
```

### FranchiseeListFilter
```csharp
public class FranchiseeListFilter
{
    public int? FranchiseeStatus { get; set; }  // 1 = Active, 0 = Inactive
    public bool? status { get; set; }           // Converted from FranchiseeStatus
    public string SearchTerm { get; set; }
    public string State { get; set; }
    public long LoggedInRoleId { get; set; }    // Populated from session
    public long LoggedInUserId { get; set; }    // Populated from session
}
```

### FeeProfileViewModel
```csharp
public class FeeProfileViewModel
{
    public long Id { get; set; }
    public long FranchiseeId { get; set; }
    
    // Royalty Fees
    public List<RoyaltyFeeSlabViewModel> RoyaltySlabs { get; set; }
    
    // Late Fees
    public int LateFeeGraceDays { get; set; }
    public decimal LateFeePercentage { get; set; }
    public decimal LateFeeFixed { get; set; }
    
    // Service Fees
    public List<ServiceFeeViewModel> ServiceFees { get; set; }
}
```

### ManageFranchiseeAccountModel
```csharp
public class ManageFranchiseeAccountModel
{
    public long UserId { get; set; }
    public long FranchiseeId { get; set; }
    public long CurrentFranchiseeId { get; set; }
    public bool AddToFranchisee { get; set; }  // true = add, false = remove
}
```

## For AI Agents

### Adding New Franchisee Operation

1. **Add method to controller**:
```csharp
[HttpPost]
public bool UpdateFeeProfile([FromBody] FeeProfileViewModel model)
{
    model.UserId = _sessionContext.UserSession.UserId;
    _franchiseeService.UpdateFeeProfile(model);
    PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage(
        "Fee profile updated successfully."
    );
    return true;
}
```

2. **Implement business logic in Core**:
```csharp
// In Core/Organizations/IFranchiseeService
void UpdateFeeProfile(FeeProfileViewModel model);
```

3. **Test endpoint**:
```bash
curl -X POST http://localhost/Organizations/Franchisee/UpdateFeeProfile \
  -H "token: $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"franchiseeId":1,"royaltySlabs":[...]}'
```

### Authorization Pattern

All operations check user permissions:
```csharp
[HttpGet]
public FranchiseeEditModel Get(long id)
{
    // Super admin can access any franchisee
    if (_sessionContext.UserSession.RoleId == (long)RoleType.SuperAdmin)
        return _franchiseeService.Get(id);
    
    // Franchisee users can only access their own
    if (_sessionContext.UserSession.OrganizationId != id)
        throw new UnauthorizedAccessException("Access denied");
    
    return _franchiseeService.Get(id);
}
```

## For Human Developers

### Common Operations

#### Get Franchisee with Authorization
```csharp
[HttpGet]
public FranchiseeEditModel Get(long id)
{
    var model = _franchiseeService.Get(id);
    
    // Franchisee users can only see their own data
    if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin &&
        _sessionContext.UserSession.OrganizationId != id)
    {
        throw new UnauthorizedAccessException("Access denied to this franchisee");
    }
    
    return model;
}
```

#### Get Filtered List
```csharp
[HttpGet]
public FranchiseeListModel Get(
    [FromUri] FranchiseeListFilter filter, 
    [FromUri] int pageNumber, 
    [FromUri] int pageSize)
{
    // Convert status filter
    if (filter.FranchiseeStatus != null)
    {
        filter.status = filter.FranchiseeStatus == 1;
    }
    
    // Populate from session
    filter.LoggedInRoleId = _sessionContext.UserSession.RoleId;
    filter.LoggedInUserId = _sessionContext.UserSession.UserId;
    
    // Service applies additional filtering based on role
    return _franchiseeService.GetFranchiseeCollection(filter, pageNumber, pageSize);
}
```

#### Safe Delete with Dependencies Check
```csharp
[HttpGet]
public bool DeleteFranchisee([FromUri] long id)
{
    var result = _franchiseeService.Delete(id);
    
    if (result == true)
    {
        ResponseModel.SetSuccessMessage("Franchisee deleted successfully.");
        return true;
    }
    else
    {
        ResponseModel.SetErrorMessage(
            "Can't delete Franchisee, as SalesData is uploaded"
        );
        return false;
    }
}
```

#### Managing User-Franchisee Assignments
```csharp
[HttpPost]
public bool ManageFranchisee([FromBody] ManageFranchiseeAccountModel model)
{
    model.UserId = _sessionContext.UserSession.UserId;
    
    // Franchisee admins can only manage their own franchisee
    if (_sessionContext.UserSession.RoleId == (long)RoleType.FranchiseeAdmin)
    {
        model.CurrentFranchiseeId = model.CurrentFranchiseeId <= 1 
            ? _sessionContext.UserSession.OrganizationId 
            : model.CurrentFranchiseeId;
    }
    
    bool result = _organizationRoleUserInfoServiceService.ManageFranchisee(model);
    
    if (result)
    {
        PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage(
            "Franchisee assignment updated successfully."
        );
    }
    
    return result;
}
```

### Best Practices

#### Data Validation
- Validate franchisee name is unique
- Ensure email format is valid
- Validate territory doesn't overlap with existing franchisees
- Check fee percentages are reasonable (0-100%)
- Validate contact information is complete

#### Authorization
- Always check role before allowing operations
- Franchisee users can only access their own data
- Super admins have full access
- Log all administrative operations for audit

#### Business Rules
- Cannot delete franchisee with:
  - Active jobs
  - Historical sales data
  - Outstanding invoices
  - Active users
- Fee profile changes require approval workflow
- Territory changes must be reviewed for conflicts

#### Performance
- Use pagination for franchisee lists
- Cache fee profiles (change infrequently)
- Index franchisee searches by name, location, status
- Load documents lazily (don't include in main query)

### Testing

```csharp
[Test]
public void GetFranchisee_SuperAdmin_ReturnsAnyFranchisee()
{
    // Arrange
    SetupSession(RoleType.SuperAdmin, organizationId: 1);
    var controller = new FranchiseeController(_service, _sessionContext, ...);
    
    // Act
    var result = controller.Get(franchiseeId: 999);
    
    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(999, result.Id);
}

[Test]
public void GetFranchisee_FranchiseeAdmin_AccessDenied()
{
    // Arrange
    SetupSession(RoleType.FranchiseeAdmin, organizationId: 1);
    var controller = new FranchiseeController(_service, _sessionContext, ...);
    
    // Act & Assert
    Assert.Throws<UnauthorizedAccessException>(() => 
        controller.Get(franchiseeId: 999)  // Different franchisee
    );
}

[Test]
public void DeleteFranchisee_WithSalesData_ReturnsFalse()
{
    // Arrange
    var franchiseeWithSales = CreateFranchiseeWithSales();
    var controller = new FranchiseeController(_service, _sessionContext, ...);
    
    // Act
    var result = controller.DeleteFranchisee(franchiseeWithSales.Id);
    
    // Assert
    Assert.IsFalse(result);
    Assert.IsTrue(controller.ResponseModel.Message.Contains("SalesData"));
}
```

## Related Areas

- **Sales**: Franchisee-specific sales and customer data
- **Scheduler**: Franchisee job scheduling and dispatch
- **Reports**: Franchisee performance metrics
- **Payment**: Franchisee billing and payments
- **Users**: Franchisee user management

## Security Considerations

- **Role-Based Access**: Enforce role-based permissions
- **Data Isolation**: Franchisee users can only see their data
- **Audit Logging**: Log all franchisee changes
- **Document Security**: Validate document ownership before download
- **PII Protection**: Handle franchisee contact information securely

## Business Impact

This area is critical for:
- Franchisee onboarding and setup
- Fee calculation and billing
- Territory management
- Franchisee performance tracking
- Compliance and documentation
