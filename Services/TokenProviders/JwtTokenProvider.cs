using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Services.TokenProviders {
    
    public class JwtTokenProvider : ITokenProvider
    {

        private byte[] m_SecretBytes;
        private string m_Issuer;
        private string m_Audience;
        private int m_TokenValidHours = 1;
        public JwtTokenProvider(IConfiguration configuration) 
        {
            m_SecretBytes = Encoding.ASCII.GetBytes(configuration["JwtSecret"]);
            m_Issuer = configuration["Issuer"];
            m_Audience = configuration["Audience"];
            int.TryParse(configuration["TokenValidHours"], out m_TokenValidHours);
        }

        public string Generate()
        {
            var issuedAt = new DateTimeOffset(DateTime.UtcNow);
            var expiresAt = issuedAt.AddHours(m_TokenValidHours);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var tokenDescriptor = new SecurityTokenDescriptor{
                // добавляем все нужные свойства токена здесь
                Subject = new ClaimsIdentity(new []{
                    new Claim(JwtRegisteredClaimNames.Iss, m_Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, m_Audience)
                }),
                Expires = expiresAt.DateTime,
                IssuedAt = issuedAt.DateTime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(m_SecretBytes), 
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}