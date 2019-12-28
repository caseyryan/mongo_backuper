using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.Packagers;
using Services.TokenProviders;

namespace Services {
    public class JwtTokenService : ITokenService
    {

        private readonly ITokenProvider m_TokenProvider;
        private readonly IConfiguration m_Configuration;

        /// разрешенное количество попыток
        /// после которых аккаунт блокируется на 15 минут
        private int m_MaxLoginAttempts = 0;
        private int m_MaxAccountBlockSeconds = 0;
        private Dictionary<string, AccountBlockData> m_LoginAttemtps 
            = new Dictionary<string, AccountBlockData>();

        public JwtTokenService(IConfiguration configuration) {
            m_TokenProvider = new JwtTokenProvider(configuration);
            m_Configuration = configuration;
            if (!int.TryParse(configuration["MaxLoginAttempts"], out m_MaxLoginAttempts)) {
                m_MaxLoginAttempts = 10;
            }
            if (!int.TryParse(configuration["MaxAccountBlockSeconds"], out m_MaxAccountBlockSeconds)) {
                m_MaxAccountBlockSeconds = 900;
            }
        }

        public async Task<string> GenerateByCredentials(string login, string password)
        {
            AccountBlockData accountBlockData;
            if (!m_LoginAttemtps.ContainsKey(login)) {
                accountBlockData = m_LoginAttemtps[login] = new AccountBlockData();
            } else {
                accountBlockData = m_LoginAttemtps[login];
                accountBlockData.IncreaseCounter(m_MaxLoginAttempts, m_MaxAccountBlockSeconds);
            }
            
            if (accountBlockData.IsLocked) {
                return new Dictionary<string, object>() {
                    { "result", "failure"},
                    { "reason", "Вы исчерпали все попытки входа. Аккаунт заблокирован на несколько минут" }
                }.ToJson();
            }

            // должны быть заданы в конфигах
            var requiredUserLogin = m_Configuration["Login"];
            var requiredPassword = m_Configuration["Password"];
            if (login != requiredUserLogin || password != requiredPassword) {
                return new Dictionary<string, object>() {
                    { "result", "failure"},
                    { "reason", "Имя пользователя или пароль не верны" }
                }.ToJson();
            }
            accountBlockData.Reset();
            var token = m_TokenProvider.Generate();
            return await Task.FromResult<string>(new Dictionary<string, object>() {
                { "token", token }
            }.ToJson());
        }

        public async Task<string> GenerateByFirebaseRequest(string firebaseToken)
        {
            // TODO: сделать акторизацию через firebase
            throw new NotImplementedException();
        }


    }
}
