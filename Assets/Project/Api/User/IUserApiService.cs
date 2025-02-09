// User/IUserApiService.cs
using Project.Api.Models.Requests;
using Project.Api.Models.Responses;
using System.Threading.Tasks;

namespace Project.Api.User
{
    public interface IUserApiService
    {
        Task<LoginResponse> Login(LoginRequest request);
    }
}