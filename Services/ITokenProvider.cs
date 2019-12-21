using System.Threading.Tasks;

namespace Services {
    public interface ITokenProvider
    {
        Task<string> GenerateByCredentials(string username, string password);
        Task<string> GenerateByFirebaseRequest(string firebaseToken);
    }
}