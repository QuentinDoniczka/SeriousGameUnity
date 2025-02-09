// User/UserApiService.cs
using Project.Api.Models.Requests;
using Project.Api.Models.Responses;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

namespace Project.Api.User
{
    public class UserApiService : IUserApiService
    {
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            using var webRequest = CreatePostRequest($"{ApiConfig.UserEndpoint}/login", request);
            await webRequest.SendWebRequest();

            if (webRequest.responseCode == 400)
                throw new System.Exception("Invalid credentials");

            if (webRequest.result != UnityWebRequest.Result.Success)
                throw new System.Exception("Network error, please try again");

            return JsonUtility.FromJson<LoginResponse>(webRequest.downloadHandler.text);
        }

        private UnityWebRequest CreatePostRequest<T>(string url, T data)
        {
            var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }
    }
}