using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;

namespace Aws.Local.Example.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamoController : ControllerBase
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;
        public DynamoController(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        [HttpGet("create")]
        public async Task<IActionResult> GetBuckets(CancellationToken cancellationToken)
        {
            var tableList = await _amazonDynamoDB.ListTablesAsync(cancellationToken);
            if (tableList.TableNames.Contains("TestTable"))
            {
                return Ok("Table Exists Already");
            }

            List<KeySchemaElement> schema = new List<KeySchemaElement>
            {
                new KeySchemaElement
                {
                    AttributeName = "Hash", KeyType = "HASH"
                },
                new KeySchemaElement
                {
                    AttributeName = "Range", KeyType = "RANGE"
                }
            };

            List<AttributeDefinition> definitions = new List<AttributeDefinition>
            {
                new AttributeDefinition
                {
                    AttributeName = "Hash", AttributeType = "S"
                },
                new AttributeDefinition
                {
                    AttributeName = "Range", AttributeType = "S"
                }
            };

            ProvisionedThroughput throughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 20,
                WriteCapacityUnits = 50
            };

            var createTableRequest = new CreateTableRequest()
            {
                TableName = "TestTable",
                KeySchema = schema,
                AttributeDefinitions = definitions,
                ProvisionedThroughput = throughput,
            };

            return Ok(await _amazonDynamoDB.CreateTableAsync(createTableRequest, cancellationToken));
        }
    }
}
