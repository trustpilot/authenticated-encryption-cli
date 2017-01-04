namespace Aem
{
    using System;
    using System.CommandLine;
    using System.IO;
    using AuthenticatedEncryption;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            var message = string.Empty;
            var command = Command.Encrypt;
            ArgumentSyntax.Parse(args, syntax =>
                {
                    syntax.DefineCommand("encrypt", ref command, Command.Encrypt, "Encrypt the given plaintext");
                    syntax.DefineParameter("plaintext", ref message, "The plaintext to encrypt");

                    syntax.DefineCommand("decrypt", ref command, Command.Decrypt, "Decrypt the given base64 encoded ciphertext");
                    syntax.DefineParameter("ciphertext", ref message, "The base64 encoded ciphertext to decrypt");
                });

            var configFileName = "appSettings.json";
            var currentDirectory = Directory.GetCurrentDirectory();
            var appSettingsFilePath = Path.Combine(currentDirectory, configFileName);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile(configFileName)
                .AddJsonFile("appSettings.local.json", optional: true)
                .Build();
            var cryptKeyBase64 = configuration["cryptkey"];
            var authKeyBase64 = configuration["authkey"];

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

            if (string.IsNullOrWhiteSpace(message))
            {
                switch (command)
                {
                    case Command.Encrypt:

                        Console.Error.WriteLine("error: plaintext is required");
                        break;
                    case Command.Decrypt:

                        Console.Error.WriteLine("error: ciphertext is required");
                        break;
                }

                Environment.Exit(1);
            }

            var cryptKey = Convert.FromBase64String(cryptKeyBase64);
            var authKey = Convert.FromBase64String(authKeyBase64);

            switch (command)
            {
                case Command.Encrypt:

                    Console.Write(AuthenticatedEncryption.Encrypt(message, cryptKey, authKey));
                    break;
                case Command.Decrypt:

                    Console.Write(AuthenticatedEncryption.Decrypt(message, cryptKey, authKey));
                    break;
            }

            Environment.Exit(0);
        }

        private enum Command
        {
            Encrypt,
            Decrypt
        }
    }
}
