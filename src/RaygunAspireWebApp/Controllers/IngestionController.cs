using Microsoft.AspNetCore.Mvc;
using Mindscape.Raygun4Net.AspNetCore;
using System.Text;
using System.Text.Json;
using System.Web;

namespace RaygunAspireWebApp.Controllers
{
  public class IngestionController : Controller
  {
    public const string ErrorsFolderPath = "/app/raygun/errors";

    // If changing this limit, also update it in the public-site documentation:
    private const int RetentionCount = 1000;

    private readonly RaygunClient _raygunClient;

    public IngestionController(RaygunClient raygunClient)
    {
      _raygunClient = raygunClient;
    }

    public async Task<IActionResult> Entries()
    {
      try
      {
        using var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var requestBody = await reader.ReadToEndAsync();

        var raygunMessage = JsonSerializer.Deserialize<Mindscape.Raygun4Net.RaygunMessage>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new RaygunIdentifierMessageConverter() } });

        if (raygunMessage != null)
        {
          var info = Directory.CreateDirectory(ErrorsFolderPath);

          var message = raygunMessage.Details?.Error?.Message;
          var uniqueSlug = DateTime.UtcNow.Ticks;

          if (string.IsNullOrWhiteSpace(message))
          {
            message = "Unknown error";
          }

          var noAsterisk = message.Replace("*", "");
          var encodedMessage = HttpUtility.UrlEncode(noAsterisk);

          var fileName = BuildFileName(uniqueSlug, encodedMessage);
          if (fileName.Length > 255)
          {
            encodedMessage = encodedMessage.Substring(0, encodedMessage.Length - (fileName.Length - 255) - 3) + "...";
            fileName = BuildFileName(uniqueSlug, encodedMessage);
          }

          System.IO.File.WriteAllText($"{ErrorsFolderPath}/{fileName}", requestBody);

          EnforceRetentionAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        await _raygunClient.SendInBackground(ex);
      }

      return Accepted();
    }

    private string BuildFileName(long uniqueSlug, string encodedMessage)
    {
      return $"{uniqueSlug}-{encodedMessage}.json";
    }

    private static void EnforceRetentionAsync()
    {
      var files = Directory.GetFiles(ErrorsFolderPath)
            .Select(filePath => new FileInfo(filePath))
            .OrderByDescending(filePath => filePath.CreationTime).Skip(RetentionCount).ToList();

      foreach (var file in files)
      {
        System.IO.File.Delete(file.FullName);
      }
    }
  }
}
