using System.Net.Sockets;

namespace Aspire.Hosting.ApplicationModel
{
  public static class RaygunAspireWebAppBuilderExtensions
  {
    public static IResourceBuilder<RaygunAspireWebAppResource> AddRaygun(this IDistributedApplicationBuilder builder, string name = "Raygun", int? port = 24605)
    {
      var raygun = new RaygunAspireWebAppResource(name);
      return builder.AddResource(raygun)
                    .WithAnnotation(new ContainerImageAnnotation { Image = "raygunowner/raygun-aspire-portal", Tag = "1.0.1" })
                    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", port: port, targetPort: 8080))
                    .WithVolume("raygun-data", "/app/raygun")
                    .ExcludeFromManifest()
                    .PublishAsContainer();
    }
  }
}
