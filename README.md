Helper libraries for tokens and cryptography in .NET.

- Branca tokens with JWT style validation

## Branca Tokens

[Branca](https://branca.io/) is token construct suitable for internal systems. The payload is encrypted using XChaCha20-Poly1305, using a 32-byte symmetric key.

This library supports the creation of Branca tokens with an arbitrary payload or using a JWT-style payload.

- NuGet: [ScottBrady.IdentityModel.Tokens.Branca](https://www.nuget.org/packages/ScottBrady.IdentityModel.Tokens.Branca)
- [Test vectors](https://github.com/scottbrady91/IdentityModel/tree/master/test/ScottBrady.IdentityModel.Tests/Tokens/Branca/TestVectors)

```csharp
var handler = new BrancaTokenHandler();
var key = Encoding.UTF8.GetBytes("supersecretkeyyoushouldnotcommit");

// JWT-style payload
string token = handler.CreateToken(new SecurityTokenDescriptor
{
    Issuer = "me",
    Audience = "you",
    Expires = DateTime.UtcNow.AddMinutes(5),
    NotBefore = DateTime.UtcNow,
    Claims = new Dictionary<string, object> {{"sub", "123"}},
    EncryptingCredentials = new EncryptingCredentials(
        new SymmetricSecurityKey(key), ExtendedSecurityAlgorithms.XChaCha20Poly1305)
});

ClaimsPrincipal principal = handler.ValidateToken(
    token,
    new TokenValidationParameters
    {
        ValidIssuer = "me",
        ValidAudience = "you",
        TokenDecryptionKey = new SymmetricSecurityKey(key)
    }, out SecurityToken parsedToken);
```

## Base62 Encoding

Base62 encoding uses the `0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz` character set.

```csharp
var plaintext = "hello world"; // encoded = AAwf93rvy4aWQVw
string encoded = Base62.Encode(Encoding.UTF8.GetBytes(plaintext));
```
