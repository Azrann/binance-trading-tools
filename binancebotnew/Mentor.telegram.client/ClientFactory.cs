using System;
using System.Threading.Tasks;
using OpenTl.ClientApi;
using OpenTl.ClientApi.MtProto.Exceptions;
using OpenTl.Schema;
using OpenTl.Schema.Auth;

namespace Mentor.telegram.client
{
    public class ClientFactory
    {
        private static IClientApi _client;
        private static DateTime? _lastClientUpdate = null;

        public async Task<IClientApi> GetClient(string filename = "session", bool forceNew = false)
        {
            if (_lastClientUpdate != null && ClientFactory._client != null && 
                _lastClientUpdate.Value.AddMinutes(15) > DateTime.Now)
            {
                if (IsHealthy(_client) && !forceNew)
                {
                    return _client;
                }
            }

            var settings = new FactorySettings
            {
                AppHash = "REPLACE_WITH_YOUR_VALUE",
                AppId = 0,
                ServerAddress = "149.154.167.50",
                ServerPublicKey = GetPublicKey(),
                ServerPort = 443,
                SessionTag = filename,
                Properties = new ApplicationProperties
                {
                    AppVersion = "1.0.0", 
                    DeviceModel = "PC", 
                    LangCode = "en", 
                    LangPack = "tdesktop", 
                    SystemLangCode = "en",
                    SystemVersion = "Win 10 Pro"
                }
            };

            //Create the client
            var clientApi = await OpenTl.ClientApi.ClientFactory.BuildClientAsync(settings).ConfigureAwait(false);

            // If the user is not authenticated
            if (!clientApi.AuthService.CurrentUserId.HasValue)
            {
                var phone = "REPLACE_WITH_YOUR_VALUE";

                ISentCode sentCode;
                try
                {
                     sentCode = await clientApi.AuthService.SendCodeAsync(phone).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return null;
                }

                Console.WriteLine("Write your code:");
                var code = ""; // Sent code

                TUser user;
                try
                {
                    user = await clientApi.AuthService.SignInAsync(phone, sentCode, code).ConfigureAwait(false);
                }
                catch (CloudPasswordNeededException)
                {
                    Console.WriteLine("USER NEED CLOUD PASSWORD");
                    Console.ReadKey();
                    // TODO check code bellow
                    // var passwordStr = "REPLACE_WITH_YOUR_VALUE"; 
                    // user = await clientApi.AuthService.CheckCloudPasswordAsync(passwordStr).ConfigureAwait(false);
                }

                catch (PhoneCodeInvalidException)
                {
                    Console.WriteLine("PHONE CODE INVALID");
                    Console.ReadKey();
                }
            }

            _lastClientUpdate = DateTime.Now;
            _client = clientApi;
            return clientApi;
        }

        public async Task<IClientApi> GetTemporaryClient(string filename = "session")
        {
            var settings = new FactorySettings
            {
                AppHash = "REPLACE_WITH_YOUR_VALUE",
                AppId = 0,
                ServerAddress = "149.154.167.50",
                ServerPublicKey = GetPublicKey(),
                ServerPort = 443,
                SessionTag = filename,
                Properties = new ApplicationProperties
                {
                    AppVersion = "1.0.0",
                    DeviceModel = "PC",
                    LangCode = "en",
                    LangPack = "tdesktop",
                    SystemLangCode = "en",
                    SystemVersion = "Win 10 Pro"
                }
            };

            //Create the client
            var clientApi = await OpenTl.ClientApi.ClientFactory.BuildClientAsync(settings).ConfigureAwait(false);

            // If the user is not authenticated
            if (!clientApi.AuthService.CurrentUserId.HasValue)
            {
                // Auth
                //var phone = "REPLACE_WITH_YOUR_VALUE";
                // Botify Phone
                //var phone = "REPLACE_WITH_YOUR_VALUE";

                var phone = "REPLACE_WITH_YOUR_VALUE";

                ISentCode sentCode;
                try
                {
                    sentCode = await clientApi.AuthService.SendCodeAsync(phone).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return null;
                }

                Console.WriteLine("Write your code:");
                var code = ""; // Sent code

                TUser user;
                try
                {
                    user = await clientApi.AuthService.SignInAsync(phone, sentCode, code).ConfigureAwait(false);
                }
                catch (CloudPasswordNeededException)
                {
                    Console.WriteLine("USER NEED CLOUD PASSWORD");
                    Console.ReadKey();
                    // TODO check code bellow
                    // var passwordStr = "REPLACE_WITH_YOUR_VALUE";
                    //user = await clientApi.AuthService.CheckCloudPasswordAsync(passwordStr).ConfigureAwait(false);
                }

                catch (PhoneCodeInvalidException)
                {
                    Console.WriteLine("PHONE CODE INVALID");
                    Console.ReadKey();
                }
            }

            return clientApi;
        }

        private bool IsHealthy(IClientApi client)
        {
            try
            {
                client.KeepAliveConnection();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetPublicKey()
        {
            return @"-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEAwVACPi9w23mF3tBkdZz+zwrzKOaaQdr01vAbU4E1pvkfj4sqDsm6
lyDONS789sVoD/xCS9Y0hkkC3gtL1tSfTlgCMOOul9lcixlEKzwKENj1Yz/s7daS
an9tqw3bfUV/nqgbhGX81v/+7RFAEd+RwFnK7a+XYl9sluzHRyVVaTTveB2GazTw
Efzk2DWgkBluml8OREmvfraX3bkHZJTKX4EQSjBbbdJ2ZXIsRrYOXfaA+xayEGB+
8hdlLmAjbCVfaigxX0CDqWeR1yFL9kwd9P0NsZRPsmoqVwMbMu7mStFai6aIhc3n
Slv8kg9qv1m6XHVQY3PnEw+QQtqSIXklHwIDAQAB
-----END RSA PUBLIC KEY-----";
        }
    }
}