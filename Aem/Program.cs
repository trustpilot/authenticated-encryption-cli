namespace Aem
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using AuthenticatedEncryption;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = ParseConfiguration();
            var arguments = ParseArguments(args);

            switch (arguments.Command)
            {
                case Command.Encrypt:

                    var ciphertext = AuthenticatedEncryption.Encrypt(arguments.Message, configuration.CryptKey, configuration.AuthKey);

                    if (arguments.UrlEncode)
                    {
                        ciphertext = WebUtility.UrlEncode(ciphertext);
                    }

                    WriteResultToOutput(ciphertext);
                    break;
                case Command.Decrypt:

                    var message = arguments.Message;

                    if (arguments.UrlDecode)
                    {
                        message = WebUtility.UrlDecode(message);
                    }

                    try
                    {
                        WriteResultToOutput(AuthenticatedEncryption.Decrypt(message, configuration.CryptKey, configuration.AuthKey));
                    }
                    catch (FormatException)
                    {
                        Console.Error.WriteLine("error: the message to decrypt is not in a valid format");

                        Environment.Exit(1);
                    }
                    catch (CryptographicException cryptographicException)
                    {
                        Console.Error.WriteLine($"error: {cryptographicException.Message}");

                        Environment.Exit(1);
                    }
                    break;
            }
        }

        private static void WriteResultToOutput(string result)
        {
            using (var streamWriter = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false)))
            {
                streamWriter.WriteLine(result);
            }
        }

        private static Arguments ParseArguments(IEnumerable<string> args)
        {
            var urlEncode = false;
            var urlDecode = false;
            var command = Command.Encrypt;
            ArgumentSyntax.Parse(
                args,
                syntax =>
                    {
                        syntax.DefineCommand("encrypt", ref command, Command.Encrypt, "Encrypt the given plaintext");
                        syntax.DefineOption("urlencode", ref urlEncode, "Url encode the base64 encoded ciphertext");

                        syntax.DefineCommand("decrypt", ref command, Command.Decrypt, "Decrypt the given base64 encoded ciphertext");
                        syntax.DefineOption("urldecode", ref urlDecode, "Url decode the input before decrypting");
                    });

            string message;
            using (var streamReader = new StreamReader(Console.OpenStandardInput()))
            {
                message = streamReader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.Error.WriteLine("error: please supply an input for encryption/decryption");

                Environment.Exit(1);
            }

            return new Arguments(command, message, urlEncode, urlDecode);
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
            var cryptKeyBase64 = configurationRoot["cryptkey"];
            var authKeyBase64 = configurationRoot["authkey"];

            if (string.IsNullOrWhiteSpace(cryptKeyBase64))
            {
                Console.Error.WriteLine($"error: please insert your cryptkey in the file: {appSettingsFilePath}");

                Environment.Exit(1);
            }

            if (string.IsNullOrWhiteSpace(authKeyBase64))
            {
                Console.Error.WriteLine($"error: please insert your authkey in the file: {appSettingsFilePath}");

                Environment.Exit(1);
            }

            byte[] cryptKey = null;
            try
            {
                cryptKey = Convert.FromBase64String(cryptKeyBase64);
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("error: cryptkey is not a valid base64 string");

                Environment.Exit(1);
            }

            byte[] authKey = null;
            try
            {
                authKey = Convert.FromBase64String(authKeyBase64);
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("error: authkey is not a valid base64 string");

                Environment.Exit(1);
            }

            return new Configuration { AuthKey = authKey, CryptKey = cryptKey };
        }
    }
}
