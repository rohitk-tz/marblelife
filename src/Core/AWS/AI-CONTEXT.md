# Core/AWS - AI Context

## Purpose

The **AWS** module provides integration with Amazon Web Services including S3 for file storage, SES for email, and other AWS services used by the MarbleLife platform.

## Key Components

### AWS Services
- S3: File and document storage
- SES: Email sending service
- CloudFront: CDN for static assets
- SNS: Push notifications

## Service Interfaces

### Storage Services
- **IS3FileService**: S3 file operations (upload, download, delete)
- **IS3BucketService**: Bucket management
- **IFileUploadService**: Multipart upload handling
- **IFileUrlService**: Presigned URL generation

### Email Services
- **ISESEmailService**: Email sending via SES
- **ISESTemplateService**: Email template management
- **ISESBounceService**: Bounce and complaint handling

## Implementations (Impl/)

Business logic for:
- S3 file upload/download with progress tracking
- Presigned URL generation for secure access
- SES email sending with templates
- CloudFront URL generation
- Error handling and retry logic

## ViewModels (ViewModel/)

- **S3FileViewModel**: S3 file metadata
- **FileUploadViewModel**: Upload configuration
- **SESEmailViewModel**: Email via SES
- **PresignedUrlViewModel**: Temporary URL data

## Business Rules

1. **File Storage**: All job photos and documents stored in S3
2. **Security**: Use presigned URLs for temporary access
3. **Email**: All transactional emails via SES
4. **CDN**: Static assets served via CloudFront
5. **Backup**: S3 versioning enabled for critical data

## Dependencies

- **AWS SDK for .NET**: AWS service clients
- **Core/Application**: File service abstraction
- **Core/Notification**: Email integration

## For AI Agents

### Uploading to S3
```csharp
// Upload file to S3
var result = await _s3FileService.Upload(new FileUploadViewModel
{
    FileName = "job-photo-before.jpg",
    FileStream = fileStream,
    ContentType = "image/jpeg",
    BucketName = "marblelife-job-photos",
    Folder = $"franchisee-{franchiseeId}/jobs/{jobId}",
    MakePublic = false
});

// Generate presigned URL for secure access (valid 1 hour)
var url = _fileUrlService.GeneratePresignedUrl(result.S3Key, expiryMinutes: 60);
```

### Sending Email via SES
```csharp
// Send transactional email
await _sesEmailService.Send(new SESEmailViewModel
{
    To = "customer@example.com",
    Subject = "Appointment Confirmation",
    TemplateName = "appointment-confirmation",
    TemplateData = new
    {
        CustomerName = "John Smith",
        AppointmentDate = "March 15, 2024",
        TechnicianName = "Mike Johnson"
    }
});
```

### Managing S3 Files
```csharp
// Download file from S3
var fileStream = await _s3FileService.Download(s3Key);

// Delete file
await _s3FileService.Delete(s3Key);

// Copy file
await _s3FileService.Copy(sourceKey, destinationKey);

// List files in folder
var files = await _s3FileService.ListFiles("franchisee-123/jobs/456/");
```

## For Human Developers

### Configuration
```csharp
// appsettings.json or web.config
{
  "AWS": {
    "Region": "us-east-1",
    "S3": {
      "BucketName": "marblelife-production",
      "CloudFrontDomain": "d1234abcd.cloudfront.net"
    },
    "SES": {
      "FromEmail": "noreply@marblelife.com",
      "FromName": "MarbleLife"
    }
  }
}
```

### Best Practices
- Use IAM roles for EC2/ECS (not access keys)
- Enable S3 server-side encryption
- Use S3 lifecycle policies for cost optimization
- Monitor SES reputation and bounce rates
- Implement retry logic with exponential backoff
- Use CloudFront for static asset delivery
- Enable S3 versioning for critical files
- Tag S3 objects for organization and billing
- Monitor AWS costs regularly
- Use presigned URLs instead of making objects public
- Implement multipart upload for large files
- Handle SES bounce and complaint notifications
- Use SES configuration sets for analytics
