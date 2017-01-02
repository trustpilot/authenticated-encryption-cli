# Authenticated Encryption CLI
A command line interface for encrypting and decrypting data using authenticated encryption

## Installation

1. [Download](https://www.microsoft.com/net/download/core) and install .NET Core for your operating system
2. [Download](https://github.com/trustpilot/authenticated-encryption-cli/releases) the CLI
3. Unzip the CLI

## Usage

To encrypt a string, call the Authenticated Encryption Manager like this:

```
dotnet Aem.dll encrypt -c <crypt key> -a <auth key> <text to encrypt>
```

The tool will output a base64 encoded string with the encrypted message (the ciphertext).

To decrypt a base64 encoded string, do this:

```
dotnet Aem.dll decrypt -c <cryptkey> -a <authkey> <base 64 encoded ciphertext>
```

The tool will output the decrypted message.
