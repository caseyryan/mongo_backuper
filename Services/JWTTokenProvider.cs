using System;
using System.Threading.Tasks;
using Services;

public class JWTTokenProvider : ITokenProvider
{
    public async Task<string> GenerateByCredentials(string username, string password)
    {
        await Task.Delay(1000);
        return "temp token by credentials";
    }

    public async Task<string> GenerateByFirebaseRequest(string firebaseToken)
    {
        // TODO: сделать акторизацию через firebase
        throw new NotImplementedException();
    }
}