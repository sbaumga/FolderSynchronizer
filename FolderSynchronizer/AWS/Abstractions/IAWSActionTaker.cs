using Amazon.S3.Model;
using Amazon.SQS.Model;

namespace FolderSynchronizer.AWS.Abstractions
{
    public interface IAWSActionTaker
    {
        Task<ListObjectsV2Response> DoListActionAsync(ListObjectsV2Request request);

        Task<PutObjectResponse> DoUploadActionAsync(PutObjectRequest request);

        Task<GetObjectResponse> DoGetObjectAsync(GetObjectRequest request);

        Task<DeleteObjectResponse> DoDeleteActionAsync(DeleteObjectRequest request);

        Task<ReceiveMessageResponse> DoSQSPollingAsync(ReceiveMessageRequest request);

        Task<DeleteMessageResponse> DoSQSMessageDeletionAsync(DeleteMessageRequest request);
    }
}