using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.IO;
using System.Text.Json;

namespace AwsParameterStoreConfig
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tenant = args[0];

            Console.WriteLine($"Getting configuration for tenant {tenant}");

            var config = TryGetFromCache(tenant) ?? await GetFromRemote(tenant);

            foreach (var item in config)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }
        }

        private static Dictionary<string, string> TryGetFromCache(string tenant)
        {
            try
            {
                Console.WriteLine($"Trying to satisfy request from data/{tenant}.json");

                using var r = new StreamReader($"data/{tenant}.json");

                string json = r.ReadToEnd();

                return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<Dictionary<string, string>> GetFromRemote(string tenant)
        {
            var path = $"/my-app/{tenant}/";

            Console.WriteLine($"Calling for Parameter Store by path {path}");

            var client = new AmazonSimpleSystemsManagementClient();

            var parameters = await client.GetParametersByPathAsync(
                new GetParametersByPathRequest { Recursive = true, Path = path });

            var output = new Dictionary<string, string>();

            parameters.Parameters.ForEach(x => output.Add(x.Name.Replace(path, ""), x.Value));

            WriteCache(tenant, output);

            return output;
        }

        private static void WriteCache(string tenant, Dictionary<string, string> config)
        {
            Console.WriteLine($"Caching configuration data to data/{tenant}.json");

            using var writer = new StreamWriter($"data/{tenant}.json");

            writer.Write(JsonSerializer.Serialize(config));
        }
    }
}
