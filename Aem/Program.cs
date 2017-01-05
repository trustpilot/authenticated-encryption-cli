namespace Aem
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.IO;
    using System.Net;
    using AuthenticatedEncryption;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ParseConfiguration();
            var arguments = ParseArguments(args);

            var cryptKey = Convert.FromBase64String(configuration.CryptKeyBase64);
            var authKey = Convert.FromBase64String(configuration.AuthKeyBase64);

            switch (arguments.Command)
            {
                case Command.Encrypt:

                    var ciphertext = AuthenticatedEncryption.Encrypt(arguments.Message, cryptKey, authKey);

                    if (arguments.UrlEncode)
                    {
                        ciphertext = WebUtility.UrlEncode(ciphertext);
                    }

                    Console.Write(ciphertext);
                    break;
                case Command.Decrypt:

                    Console.Write(AuthenticatedEncryption.Decrypt(arguments.Message, cryptKey, authKey));
                    break;
            }

            Environment.Exit(0);
        }

        private static Arguments ParseArguments(IEnumerable<string> args)
        {
            var urlEncode = false;
            var command = Command.Encrypt;
            ArgumentSyntax.Parse(
                args,
                syntax =>
                    {
                        syntax.DefineCommand("encrypt", ref command, Command.Encrypt, "Encrypt the given plaintext");
                        syntax.DefineOption("urlencode", ref urlEncode, "Url encode the base64 encoded ciphertext");

                        syntax.DefineCommand("decrypt", ref command, Command.Decrypt, "Decrypt the given base64 encoded ciphertext");
                    });

            var message = Console.In.ReadToEnd();

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.Error.WriteLine("error: please supply an input for encryption/decryption");

                Environment.Exit(1);
            }

            return new Arguments(command, message, urlEncode);
        }

        private static Configuration ParseConfiguration()
        {
            var configFileName = "appSettings.json";
            var currentDirectory = Directory.GetCurrentDirectory();
            var appSettingsFilePath = Path.Combine(currentDirectory, configFileName);
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile(configFileName)
                .AddJsonFile("appSettings.local.json", optional: true)
                .Build();
            var configuration = new Configuration
                {
                    AuthKeyBase64 = configurationRoot["authkey"],
                    CryptKeyBase64 = configurationRoot["cryptkey"]
                };

            if (string.IsNullOrWhiteSpace(configuration.CryptKeyBase64))
            {
                Console.Error.WriteLine($"error: please insert your cryptkey in the file: {appSettingsFilePath}");

                Environment.Exit(1);
            }

            if (string.IsNullOrWhiteSpace(configuration.AuthKeyBase64))
            {
                Console.Error.WriteLine($"error: please insert your authkey in the file: {appSettingsFilePath}");

                Environment.Exit(1);
            }

            return configuration;
        }
    }
}
