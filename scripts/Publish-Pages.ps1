param(
    [Parameter(Mandatory = $true)]
    [string] $PublishedWwwroot
)
$ErrorActionPreference = 'Stop'
$published = (Resolve-Path -LiteralPath $PublishedWwwroot).Path
if (!(Test-Path -LiteralPath (Join-Path $published 'index.html'))) { throw 'Expected a published Blazor wwwroot containing index.html.' }
if (!(Test-Path -LiteralPath (Join-Path $published '_framework'))) { throw 'Missing Blazor framework files.' }
$staging = Join-Path ([System.IO.Path]::GetTempPath()) ('components-pages-' + [guid]::NewGuid())
git clone --single-branch --branch gh-pages git@github.com:lowlandtech/components.git $staging
if ($LASTEXITCODE -ne 0) { throw 'Could not clone the deployment branch.' }
# Only remove tracked site files inside this fresh temporary clone.
git -C $staging rm -r -q -- .
if ($LASTEXITCODE -ne 0) { throw 'Could not clear the previous deployment.' }
Copy-Item -Path (Join-Path $published '*') -Destination $staging -Recurse -Force
Set-Content -LiteralPath (Join-Path $staging 'CNAME') -Value 'components.lowlandtech.com'
New-Item -ItemType File -Path (Join-Path $staging '.nojekyll') -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $staging 'index.html') -Destination (Join-Path $staging '404.html') -Force
git -C $staging config core.autocrlf false
git -C $staging add .
git -C $staging diff --cached --quiet
if ($LASTEXITCODE -eq 0) { Write-Output 'The published site is already current.'; return }
git -C $staging commit -m 'deploy: update Blazor component demo'
if ($LASTEXITCODE -ne 0) { throw 'Deployment commit failed.' }
git -C $staging push origin gh-pages
if ($LASTEXITCODE -ne 0) { throw "Push failed; deployment remains in $staging. No force push was attempted." }
Write-Output "Deployment pushed. Temporary checkout: $staging"
