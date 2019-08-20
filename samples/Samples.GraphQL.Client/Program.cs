using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;

namespace Samples.GraphQL.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string port = args.FirstOrDefault(arg => arg.StartsWith("Port="))?.Split('=')[1] ?? "9000";
            Console.WriteLine($"Port {port}");

            string url = $"http://localhost:{port}/graphql";
            Console.WriteLine($"Sending requests to {url}");
            var graphQLClient = new GraphQLHttpClient(url);

            // Launch GraphQL Server at the specified port
            Console.WriteLine();

            // SUCCESS: query using just a query string
            string unnamedHeroQueryString = "query HeroQuery{hero {name appearsIn }}";
            var graphQLResponse = await graphQLClient.SendQueryAsync(unnamedHeroQueryString);

            // SUCCESS: query using a query string and operation name
            var heroQueryRequest = new GraphQLRequest();
            heroQueryRequest.OperationName = "HeroQuery";
            heroQueryRequest.Query = unnamedHeroQueryString;
            graphQLResponse = await graphQLClient.SendQueryAsync(heroQueryRequest);

            // SUCCESS: mutation
            var mutationRequest = new GraphQLRequest();
            mutationRequest.OperationName = "AddBobaFett";
            mutationRequest.Query = "mutation AddBobaFett($human:HumanInput!){createHuman(human: $human){id name}}";
            mutationRequest.Variables = new
            {
                human = new
                {
                    name = "Boba Fett"
                }
            };
            graphQLResponse = await graphQLClient.SendQueryAsync(mutationRequest);

            // FAILURE: query fails 'validate' step
            var humanErrorRequest = new GraphQLRequest();
            humanErrorRequest.Query = "query HumanError{human(id:1){name apearsIn}}";
            graphQLResponse = await graphQLClient.SendQueryAsync(humanErrorRequest);
        }
    }
}
