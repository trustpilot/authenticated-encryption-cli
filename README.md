# Authenticated Encryption CLI
A command line interface for encrypting and decrypting data using authenticated encryption

## Installation

1. [Download](https://www.microsoft.com/net/download/core) and install .NET Core for your operating system
2. [Download](https://github.com/trustpilot/authenticated-encryption-cli/releases) and unzip Authenticated Encryption Manager

## Usage

Before you can use the Authenticated Encryption Manager (AEM), you need to insert your base64 encoded cryptkey and authkey in the appSettings.json file:

```
{
  "cryptkey": "<insert your cryptkey>",
  "authkey": "<insert your authkey>"
}
```

### Encrypt

To encrypt a string, call AEM like this:

```
echo "<text to encrypt>" | dotnet Aem.dll encrypt
```

You can also encrypt the contents of a file, like this:

```
dotnet Aem.dll encrypt < <path to file>
```

If you want to paste or write a multiline text to encrypt, just call AEM like this, and you will be able to write or paste whatever text you want to encrypt. When you are done writing, type CTRL+Z in Windows or CTRL+D in unix systems and then ENTER to send EOF:

```
dotnet Aem.dll encrypt
```

No matter which way you decide to run AEM, it will output a base64 encoded string with the encrypted message (the ciphertext). If you want to store the ciphertext in a file, you can easily do that, like this:

```
echo "<text to encrypt>" | dotnet Aem.dll encrypt > <path to file>
```

### Decrypt

Decryption can be done in the same ways as encryption explained above. You just need to replace the "encrypt" command with "decrypt":

```
echo "<text to decrypt>" | dotnet Aem.dll decrypt
```
