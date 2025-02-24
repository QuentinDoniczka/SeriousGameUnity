using Project.Api.Constants;
using Project.Api.Models.Requests;
using Project.Api.Models.Responses;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Project.Api.User
{
    public class UserApiService : IUserApiService
    {
        private UnityWebRequest CreatePostRequest<T>(string url, T data)
        {
            var request = new UnityWebRequest(url, "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            using var webRequest = CreatePostRequest(UserApiEndpoint.Login, request);
            await webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var responseContent = webRequest.downloadHandler.text;
        
                try 
                {
                    var response = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    Debug.Log($"Login response received for: {response.Email}");
                    return response;
                }
                catch (JsonException e)
                {
                    Debug.LogError($"Failed to parse login response: {e.Message}");
                    throw new System.Exception("Invalid response format from server");
                }
            }
            else
            {
                Debug.LogError($"Login request failed: {webRequest.error}");
                throw new System.Exception($"Server error: {webRequest.error}");
            }
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            using var webRequest = CreatePostRequest(UserApiEndpoint.Register, request);
            await webRequest.SendWebRequest();
            
            var responseContent = webRequest.downloadHandler.text;
            var response = JsonConvert.DeserializeObject<RegisterResponse>(responseContent);
            Debug.Log($"Register response: {response.Success}");
            return response;
        }
    }
}
