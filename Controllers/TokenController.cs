using System;
using System.Net;
using System.Threading.Tasks;
using Controllers.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController
    {
        // для генерации нужного типа токена по запросу
        // по умолчанию инжектится JWTTokenProvider
        private ITokenService TokenService { get; set; }

        public TokenController(ITokenService tokenService) {
            TokenService = tokenService;
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
            return await TokenService.GenerateByCredentials(dto.Username, dto.Password);
        }
    }
}
