using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Services.TokenProviders;

namespace Services {
    public class JwtTokenService : ITokenService
    {

        private ITokenProvider TokenProvider { get; set; }
        private IConfiguration Configuration { get; set; }

        public JwtTokenService(IConfiguration configuration) {
            TokenProvider = new JWTTokenProvider(configuration);
            Configuration = configuration;
        }

        public async Task<string> GenerateByCredentials(string username, string password)
        {
            // должны быть заданы в конфигах
            var requiredUserName = Configuration["Username"];
            var requiredPassword = Configuration["Password"];

            // TODO: добавить проверку подлинности юзера
            return await Task.FromResult<string>(TokenProvider.Generate());
        }

        public async Task<string> GenerateByFirebaseRequest(string firebaseToken)
        {
            // TODO: сделать акторизацию через firebase
            throw new NotImplementedException();
        }


    }
}
