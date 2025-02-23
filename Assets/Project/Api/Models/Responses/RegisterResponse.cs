namespace Project.Api.Models.Responses
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }
    }
}