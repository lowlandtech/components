# Published feature feed dependency closure

Coordinator investigation, 2026-10-24. This is a package-consumer defect report,
not acceptance of CPAN's remaining two-circuit browser integration.

## Reproduction

A separate consumer outside every producer checkout references only:

~~~xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="LowlandTech.Components.Features.Session"
      Version="[2026.10.1-20261024192513816465564]" />
  </ItemGroup>
</Project>
~~~

Its Program.cs reads ComponentsSessionFeature.Registration.GetType().FullName.
NuGet.Config contains only D:/lowlandtech/packages and the standard public
NuGet source. Run dotnet restore Consumer.csproj --configfile NuGet.Config.

The clean reproduction exits 1 with NU1605 before compilation:
- Feature -> Blocks -> Components -> LowlandTech.Core 2026.10.2-202610242
  requires Components.Core exactly 2026.10.2-202610242.
- Feature -> Blocks -> Components -> Components.Core requires the batch
  minimum 2026.10.1-20261024192513816465564.
- NuGet detects the downgrade from the exact upstream 2026.10.2 identity to
  the batch's 2026.10.1 identity.

The reproduction is retained locally at:
C:/Users/wendellmva/.codex/visualizations/2026/10/24/01a16d7d-a44a-7921-a509-460c83411931/feature-feed-consumer

## Artifact inspection

The upstream LowlandTech.Core.2026.10.2-202610242.nupkg nuspec pins
LowlandTech.Components.Core to [2026.10.2-202610242]. That older Components.Core
assembly has no ComponentsStartupMap. The newly produced 2026.10.1 Components.Core
assembly has that startup type. Merely pinning the old higher Core package in a
consumer would not establish feature startup compatibility.

No package, cache entry, producer source, version constraint or compiler
warning was overwritten/weakened in this investigation.

## Required recovery

The owning lane must establish a compatible immutable dependency closure and
record the upstream Core producer/version it requires. Do not overwrite the
existing migration identities, suppress NU1605, remove the compatibility guard,
or shadow a conflicting immutable package identity in a private fixture feed
and call that shared-feed consumption.

Prove an external consumer can restore, compile and execute the feature startup
registration from the actual published feed without producer ProjectReferences.
Then run the independent CPAN RCL/two-circuit consumer proof. Scope semantics
already accepted at CPLG and in-process tests do not certify this artifact
closure; keep dependent integration readiness truthful until repaired.
