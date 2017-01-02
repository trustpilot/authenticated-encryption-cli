namespace Aem
{
    using System;
    using System.CommandLine;
    using AuthenticatedEncryption;

    public class Program
    {
        public static void Main(string[] args)
        {
            var cryptKeyBase64 = string.Empty;
            var authKeyBase64 = string.Empty;
            var message = string.Empty;
            var command = Command.Encrypt;
            ArgumentSyntax.Parse(args, syntax =>
                {
                    syntax.DefineCommand("encrypt", ref command, Command.Encrypt, "Encrypt the given plaintext");
                    syntax.DefineOption("c|cryptkey", ref cryptKeyBase64, "The base64 encoded key to use for encryption");
                    syntax.DefineOption("a|authkey", ref authKeyBase64, "The base64 encoded key to use for authentication");
                    syntax.DefineParameter("plaintext", ref message, "The plaintext to encrypt");

                    syntax.DefineCommand("decrypt", ref command, Command.Decrypt, "Decrypt the given base64 encoded ciphertext");
                    syntax.DefineOption("c|cryptkey", ref cryptKeyBase64, "The base64 encoded key to use for decryption");
                    syntax.DefineOption("a|authkey", ref authKeyBase64, "The base64 encoded key to use for authentication");
                    syntax.DefineParameter("ciphertext", ref message, "The base64 encoded ciphertext to decrypt");
                });

            if (string.IsNullOrWhiteSpace(cryptKeyBase64))
            {
                Console.WriteLine("error: cryptkey is required");

                return;
            }

            if (string.IsNullOrWhiteSpace(authKeyBase64))
            {
                Console.WriteLine("error: authkey is required");

                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                switch (command)
                {
                    case Command.Encrypt:

                        Console.WriteLine("error: plaintext is required");
                        break;
                    case Command.Decrypt:

                        Console.WriteLine("error: ciphertext is required");
                        break;
                }

                return;
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
        }

        private enum Command
        {
            Encrypt,
            Decrypt
        }
    }
}
