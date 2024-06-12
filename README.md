# Aspire.Hosting.Raygun

A locally running [Raygun Crash Reporting](https://raygun.com/platform/crash-reporting) portal for your Aspire orchestrator.

# Installation

## Step 1 - Install the Aspire.Hosting.Raygun NuGet package

Install the [Aspire.Hosting.Raygun NuGet package](https://www.nuget.org/packages/Aspire.Hosting.Raygun) into your **Aspire orchestration project (AppHost)**. Either use the NuGet package management GUI in the IDE you use, OR use the below dotnet command.

```bash
dotnet add package Aspire.Hosting.Raygun
```

## Step 2 - Add Raygun to the orchestration builder

In `Program.cs` of the AppHost project, call `AddRaygun` on the builder (after the builder is initialized and before it is used to build and run).

```csharp
// The distributed application builder is created here

builder.AddRaygun();

// The builder is used to build and run the app somewhere down here
```

The steps so far will cause a Raygun resource to be listed in the orchestration app. Clicking on the URL of that resource will open a local Raygun portal in a new tab where you'll later be able to view crash reports captured in your local development environment. Find out everything you need to know about using this local Raygun portal in the [documentation here](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/aspire/#locally-running-raygun-portal).

## Step 3 - Instrument your .NET projects

In each of the projects in your Aspire app where you'll be capturing exceptions from, install the [Raygun4Aspire NuGet package](https://www.nuget.org/packages/Raygun4Aspire/).

Then, in `Program.cs`, add a `using Raygun4Aspire;` statement, call `AddRaygun` on the WebApplicationBuilder followed by calling `UseRaygun` on the created application.

```csharp
using Raygun4Aspire;

// The WebApplicationBuilder is created somewhere here

builder.AddRaygun();

// The builder is used to create the application a little later on

app.UseRaygun();

// Then at the end of the file, the app is commanded to run
```

Those are the minimal steps to get Raygun4Aspire capturing unhandled exceptions that occur during web requests in your local development environment. See the [Raygun4Aspire documentation](https://github.com/MindscapeHQ/raygun4aspire?tab=readme-ov-file#manually-sending-exceptions) to log exceptions from try/catch blocks and other features.

## Step 4 - Optionally send crash reports in production to the Raygun cloud service

In production `appsettings` files of each of your .NET projects, add the below `RaygunSettings` section. Substitute in your application API key that Raygun provides when you create a new Application in Raygun.

```json
"RaygunSettings": {
  "ApiKey": "YOUR_APP_API_KEY"
}
```

# Enable AI Error Resolution (optional)

Get AI suggestions to resolve exceptions from a locally running LLM!

## Step 1 - Install the Aspire Hosting Ollama NuGet package

Install the [Raygun.Aspire.Hosting.Ollama](https://www.nuget.org/packages/Raygun.Aspire.Hosting.Ollama#readme-body-tab) NuGet package into the Aspire orchestration project (AppHost). Either use the NuGet package management GUI in the IDE you use, OR the below dotnet command.

```bash
dotnet add package Raygun.Aspire.Hosting.Ollama
```

## Step 2 - Add Ollama to the orchestration builder

In `Program.cs` of the AppHost project, right above where you added the `builder.AddRaygun();` line in Step 2 of the standard installation, you'll need to add the following line to add the Ollama container.

```csharp
var ollama = builder.AddOllama();
```

## Step 3 - Reference Ollama in the Raygun component

Finally, you will need to modify the `builder.AddRaygun();` line to add a reference to Ollama. Your final code should look like this.

```csharp
// The distributed application builder is created here

var ollama = builder.AddOllama();
builder.AddRaygun().WithReference(ollama);

// The builder is used to build and run the app somewhere down here
```

Now, when you view an exception that's been captured in the locally running Raygun portal, click the "AI Error Respolution" button in the top right corner to get AI suggestions on what to do about it.
The first time you use this feature, the LLM will need to be downloaded. Keep the Aspire orchestration app open until this has completed.
AI responses are not stored, and so they restart everytime you drill in to view an exception and click the button.
[Find out more details about how to use this feature here.](https://raygun.com/documentation/language-guides/dotnet/crash-reporting/aspire#using-ai-error-resolution-within-aspire)
