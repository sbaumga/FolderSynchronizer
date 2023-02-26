using Amazon.S3;
using Amazon.S3.Model;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSActionTaker
    {
        Task<ListObjectsV2Response> DoListActionAsync(ListObjectsV2Request request);
        Task<PutObjectResponse> DoUploadActionAsync(PutObjectRequest request);
        Task<DeleteObjectResponse> DoDeleteActionAsync(DeleteObjectRequest request);
    }
}