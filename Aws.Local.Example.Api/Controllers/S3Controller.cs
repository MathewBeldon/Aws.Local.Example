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

        [HttpPost("bucket/{bucketName}")]
        public async Task<IActionResult> AddItemToBucket([FromRoute] string bucketName, IFormFile file, CancellationToken cancellationToken)
        {
            var bucketExists = await _amazonS3.DoesS3BucketExistAsync(bucketName);

            if (!bucketExists)
            {
                return BadRequest($"Bucket {bucketName} does not exists.");
            }

            using var fileStream = file.OpenReadStream();

            var putObjectRequest = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = file.FileName,
                InputStream = fileStream
            };

            putObjectRequest.Metadata.Add("Content-Type", file.ContentType);

            var putResult = await _amazonS3.PutObjectAsync(putObjectRequest);

            return Ok($"File {file.FileName} uploaded to S3 successfully!");
        }

        [HttpGet("bucket/{bucketName}/{key}")]
        public async Task<IActionResult> GeneratePresignedUrl([FromRoute] string bucketName, [FromRoute] string key, CancellationToken cancellationToken)
        {
            var bucketExists = await _amazonS3.DoesS3BucketExistAsync(bucketName);

            if (!bucketExists)
            {
                return BadRequest($"Bucket {bucketName} does not exists.");
            }

            var url = _amazonS3.GeneratePreSignedURL(bucketName, key, DateTime.UtcNow.AddMinutes(1), null);

            return Ok(url);
        }
    }
}
