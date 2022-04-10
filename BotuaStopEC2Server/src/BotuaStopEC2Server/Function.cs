using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;
using Amazon.EC2;
using Amazon.S3;
using Amazon.DynamoDBv2;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BotuaStopEC2Server
{
    public class Function
    {
        private ServiceCollection _serviceCollection;

        public Function()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddDefaultAWSOptions(new AWSOptions());
            _serviceCollection.AddAWSService<IAmazonS3>();
            _serviceCollection.AddAWSService<IAmazonDynamoDB>();
            _serviceCollection.AddAWSService<IAmazonEC2>();
            _serviceCollection.AddTransient<Handler>();
        }

        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest input, ILambdaContext context)
        {
            using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
                return await serviceProvider.GetService<Handler>().Handle(input);
        }
    }
}


