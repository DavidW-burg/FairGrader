# .NET 10 Upgrade Report

## Project target framework modifications

| Project name    | Old Target Framework | New Target Framework |
|:----------------|:--------------------:|:--------------------:|
| FairGraderApp   | net8.0               | net10.0              |

## NuGet Packages

| Package Name                                          | Old Version | New Version |
|:------------------------------------------------------|:-----------:|:-----------:|
| Microsoft.AspNetCore.Components.WebAssembly           |   8.0.7     |   10.0.9    |
| Microsoft.AspNetCore.Components.WebAssembly.DevServer |   8.0.7     |   10.0.9    |

## Project feature upgrades

### FairGraderApp

Here is what changed for the project during upgrade:

- Target framework upgraded from `net8.0` to `net10.0`.
- Microsoft.AspNetCore.Components.WebAssembly package updated to version 10.0.9.
- Microsoft.AspNetCore.Components.WebAssembly.DevServer package updated to version 10.0.9.

## Next steps

- Build and run the application to verify that everything works correctly with .NET 10.0.
- Test the Blazor WebAssembly functionality in the browser to ensure no runtime issues exist.
- Review the [.NET 10 breaking changes documentation](https://learn.microsoft.com/dotnet/core/compatibility/10.0) for any additional considerations specific to your application.
- Consider updating other third-party NuGet packages (e.g., Blazor.Bootstrap) to versions that explicitly support .NET 10.0 if available.
- Note: .NET 10.0 is currently in Preview. For production use, wait for the official LTS release (scheduled for November 2025) or remain on .NET 8.0 LTS until then.
