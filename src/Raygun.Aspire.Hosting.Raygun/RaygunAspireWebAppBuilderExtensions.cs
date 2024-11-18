using System.Net.Sockets;

namespace Aspire.Hosting.ApplicationModel
{
  public static class RaygunAspireWebAppBuilderExtensions
  {
    public static IResourceBuilder<RaygunAspireWebAppResource> AddRaygun(this IDistributedApplicationBuilder builder, string name = "Raygun", int? port = 24605)
    {
      var raygun = new RaygunAspireWebAppResource(name);
      return builder.AddResource(raygun)
                    .WithAnnotation(new ContainerImageAnnotation { Image = "raygunowner/raygun-aspire-portal", Tag = "2.0.2" })
                    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", port: port, targetPort: 8080))
                    .WithVolume("raygun-data", "/app/raygun")
                    .ExcludeFromManifest()
                    .PublishAsContainer();
    }

    public static IResourceBuilder<RaygunAspireWebAppResource> WithOllamaReference(this IResourceBuilder<RaygunAspireWebAppResource> builder, IResourceBuilder<IResourceWithConnectionString> source, string? model = null)
    {
      var resource = source.Resource;
      var connectionName = resource.Name;

      if (!string.IsNullOrEmpty(model))
      {
        builder.WithEnvironment("Ollama:Model", model);
      }
      return builder
        .WithReference(source, "Ollama")
          .WaitFor(source)
        .WithEnvironment(context =>
        {
          if (!string.IsNullOrEmpty(model))
          {
            context.EnvironmentVariables["Ollama:Model"] = model;
          }
        });
    }
  }
}
