using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using ScottBrady.IdentityModel.Crypto;
using ScottBrady.IdentityModel.Tokens;
using Xunit;

namespace ScottBrady.IdentityModel.Tests.Tokens
{
    public class JwtSecurityTokenHandlerTests
    {
        [Fact]
        public void WhenEdDsaTokenGenerated_ExpectEdDsaTokenVerifiable()
        {
            const string issuer = "me";
            const string audience = "you";
            const string subject = "123";
            
            var keyPairGenerator = new Ed25519KeyPairGenerator();
            keyPairGenerator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            var keyPair = keyPairGenerator.GenerateKeyPair();
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // ffs
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(30),
                Subject = new ClaimsIdentity(new[] {new Claim("sub", subject)}),
                SigningCredentials = new SigningCredentials(
                    new EdDsaSecurityKey((Ed25519PrivateKeyParameters) keyPair.Private),
                    ExtendedSecurityAlgorithms.EdDsa)
            });

            var validationResult = handler.ValidateToken(jwt, new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                IssuerSigningKey = new EdDsaSecurityKey((Ed25519PublicKeyParameters) keyPair.Public)
            }, out _);
            
            validationResult.Claims.Should().Contain(x => x.Type == "sub" && x.Value == subject);
        }
    }
}