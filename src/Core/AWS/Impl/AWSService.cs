using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.AWS.Impl
{
    [DefaultImplementation]
    public class AWSService : IAWSService
    {

        private ISettings _settings;
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        public AWSService(ISettings settings, ILogService logService, IUnitOfWork unitOfWork)
        {
            _settings = settings;
            _logService = logService;
            _unitOfWork = unitOfWork;
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
        }

        public async Task<bool> CreateSubFolderAsync(string SubFolder)
        {
            using (IAmazonS3 client = new AmazonS3Client(_settings.AWSAccessKey, _settings.AWSSecreatKey, RegionEndpoint.USEast1))
            {
                try
                {
                    PutObjectRequest request = new PutObjectRequest()
                    {
                        BucketName = _settings.AWSBucketName,
                        Key = SubFolder // <-- in S3 key represents a path
                     };
                        PutObjectResponse response = client.PutObject(request);
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.Error("Starting services", ex);
                    return false;
                }
            }
        }

        public async Task<bool> UploadPartRequestFileInSubFolder(string SubFolderName, string Key, string filePath)
        {
            using (IAmazonS3 client = new AmazonS3Client(_settings.AWSAccessKey, _settings.AWSSecreatKey, RegionEndpoint.USEast1))
            {
                try
                {
                    var filePath1 = @filePath;
                    string path = SubFolderName + "/" + Key;
                    var fileTransferUtility = new TransferUtility(client);
                    fileTransferUtility.Upload(filePath1, _settings.AWSBucketName, path);
                    
                    
                    _logService.Info(string.Format("File Uploaded to AWS(S3 Bucket) Successfully! - {0} {1}", SubFolderName, Key));
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.Error(string.Format("Error in uploading in AWS S3 Bucket - {0} {1} with exception {2} {3}", SubFolderName, Key,ex.InnerException, ex));
                    return false;
                }
            }
        }

        public async Task<bool> DoesFolderexists(string subFolderName)
        {
            //ListObjectsV2Response response;
            try
            {
                IAmazonS3 client = new AmazonS3Client(_settings.AWSAccessKey, _settings.AWSSecreatKey, RegionEndpoint.USEast1);
               
                ListObjectsRequest request = new ListObjectsRequest 
                { 
                    BucketName = _settings.AWSBucketName,  
                    Prefix = subFolderName
                }; 
                ListObjectsResponse response = client.ListObjects(request);
                return (response != null && response.S3Objects != null && response.S3Objects.Count > 0);
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error in uploading in AWS S3 Bucket - {0} {1}", subFolderName, ex));
            }

            return false;
        }
    }
}
