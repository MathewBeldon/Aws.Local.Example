using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace Aws.Local.Example.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;
        public S3Controller(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        [HttpPost("bucket")]
        public async Task<IActionResult> CreateBucket([FromBody] string bucketName)
        {
            var putBucketRequest = new PutBucketRequest()
            {
                BucketName = bucketName,
                UseClientRegion = true
            };
            var putBucketResponse = await _amazonS3.PutBucketAsync(putBucketRequest);

            return Ok(putBucketResponse);
        }

        [HttpGet("bucket")]
        public async Task<IActionResult> GetBuckets(CancellationToken cancellationToken)
        {
            return Ok(await _amazonS3.ListBucketsAsync(cancellationToken));
        }

        [HttpGet("bucket/{bucketName}")]
        public async Task<IActionResult> GetBuckets([FromRoute] string bucketName, CancellationToken cancellationToken)
        {
            return Ok(await _amazonS3.ListObjectsAsync(bucketName,cancellationToken));
        }
    }
}
