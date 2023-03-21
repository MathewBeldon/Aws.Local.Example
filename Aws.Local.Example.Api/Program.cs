using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.S3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddSingleton<IAmazonS3>(sc =>
{
    var awsS3Config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]),
        ServiceURL = Environment.GetEnvironmentVariable("AWS_SERVICEURL") ?? builder.Configuration["AWS:ServiceURL"],
        ForcePathStyle = bool.Parse(builder.Configuration["AWS:ForcePathStyle"]!)
    };

    return new AmazonS3Client(awsS3Config);
});

builder.Services.AddSingleton<IAmazonDynamoDB>(_ =>
{
    var awsDynamoConfig = new AmazonDynamoDBConfig
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"]),
        ServiceURL = Environment.GetEnvironmentVariable("AWS_SERVICEURL") ?? builder.Configuration["AWS:ServiceURL"],
    };

    return new AmazonDynamoDBClient(awsDynamoConfig);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
