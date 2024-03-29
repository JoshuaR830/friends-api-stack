using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using BotuaGetFriendTimes.Models;
using BotuaGetFriendTimes.Repositories;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BotuaGetFriendTimes
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
            _serviceCollection.AddAWSService<IAmazonDynamoDB>();
            _serviceCollection.AddAWSService<IAmazonSimpleSystemsManagement>();
            _serviceCollection.AddTransient<Handler>();
            _serviceCollection.AddTransient<ITimeRepository, TimeRepository>();
            _serviceCollection.AddTransient<INameHelper, NameHelper>();
        }

        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest input, ILambdaContext context)
        {
            using (ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider())
                return await serviceProvider.GetService<Handler>().Handle(input);
        }
    }
}
