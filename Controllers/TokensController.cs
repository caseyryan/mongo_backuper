using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Controllers.Dto;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace MongoBackuper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        // для генерации нужного типа токена по запросу
        // по умолчанию инжектится JWTTokenProvider
        private ITokenProvider TokenProvider { get; set; }

        public TokensController(ITokenProvider tokenProvider) {
            TokenProvider = tokenProvider;
        }

        /// позволяет получить токен используя IdToken, получаемый
        /// при смс аутентификации в Firebase
        [HttpGet("SmsToken")]
        public async Task<string> SmsToken([FromQuery] AuthDto dto) {
            return await TokenProvider.GenerateByFirebaseRequest(dto.IdToken);
        }

        [HttpGet("CredentialsToken")]
        public async Task<string> CredentialsToken([FromQuery] AuthDto dto) {
            return await TokenProvider.GenerateByCredentials(dto.Username, dto.Password);
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
