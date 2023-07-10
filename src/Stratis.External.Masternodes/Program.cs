using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using NBitcoin;
using Newtonsoft.Json;
using Stratis.Bitcoin.Utilities.JsonConverters;

namespace Stratis.External.Masternodes
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome to the Stratis Masternode Registration application.");
            Console.WriteLine("Please press any key to start.");
            Console.ReadKey();

            SetupJsonConverters();
            
            var service = new RegistrationService();

            NetworkType networkType = NetworkType.Mainnet;

            if (args.Contains("-testnet"))
                networkType = NetworkType.Testnet;

            if (args.Contains("-regtest"))
                networkType = NetworkType.Regtest;

            await service.StartAsync(networkType);
        }

        private static void SetupJsonConverters()
        {
            FlurlHttp.Configure(settings => {
                var jsonSettings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter>()
                    {
                        new DateTimeToUnixTimeConverter()
                    }
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }
    }
}
