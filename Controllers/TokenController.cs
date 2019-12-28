using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Controllers.Dto;
using Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController
    {
        // для генерации нужного типа токена по запросу
        // по умолчанию инжектится JWTTokenProvider
        private readonly ITokenService m_TokenService;

        public TokenController(ITokenService tokenService) {
            m_TokenService = tokenService;
            Console.WriteLine("TOKEN SERVICE " + tokenService);
        }

        /// позволяет получить токен используя IdToken, получаемый
        /// при смс аутентификации в Firebase
        [HttpPost("SmsToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<string> SmsToken([FromBody] AuthDto dto) {
            // return await TokenService.GenerateByFirebaseRequest(dto.IdToken);
            // TODO сгенерировать токен по результатам проверки в Firebase
            return "AUTHORIZED";
        }

        [HttpPost("CredentialsToken")]
        public async Task<string> CredentialsToken([FromBody] AuthDto dto) {
            if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password)) {
                return new Dictionary<string, object>() {
                    { "result", "failure"},
                    { "reason", "Логин или пароль не верны" }
                }.ToJson();
            }
            return await m_TokenService.GenerateByCredentials(dto.Login, dto.Password);
        }
    }
}
