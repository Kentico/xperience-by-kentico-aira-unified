using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Aira.Chat.Models;

public class AiraChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<IFormFile> Files { get; set; } = [];
}
