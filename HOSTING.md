# Components demo hosting

The public demo is hosted at https://components.lowlandtech.com by GitHub Pages.

- Pages publishes the root of the `gh-pages` branch.
- Cloudflare DNS: CNAME `components` to `lowlandtech.github.io`, DNS only.
- The custom domain is configured in repository Settings > Pages.
- Enable Enforce HTTPS there after GitHub provisions the domain certificate.
- `.nojekyll` preserves `_framework` assets. `404.html` loads the Blazor app for direct route navigation; GitHub returns HTTP 404 for these fallback requests.

## Updating the deployed demo

The initial deployment was built from Foundry revision c7cb4e12d48173f13dd40bc94c97623372cec4b4, whose component tree matches this repository's extraction. The demo still requires Foundry core, so pushes to `main` do not automatically rebuild it.

Until that dependency is packaged independently, build from a compatible Foundry checkout with the current component sources under `src/components`:

```powershell
dotnet publish src/components/app/LowlandTech.Components.App.csproj -c Release -o artifacts/components-pages -p:SkipTailwindBuild=true -p:GenerateEmbeddedValidatableTypeAttribute=false
```

That command uses the checked-in CSS. If styles changed, rebuild the library Tailwind CSS first. Then, from this repository:

```powershell
./scripts/Publish-Pages.ps1 -PublishedWwwroot D:/path/to/foundry/artifacts/components-pages/wwwroot
```

The script updates `gh-pages` using a normal push from an isolated temporary checkout. GitHub then publishes the site. Verify the published demo after deployment.

## Sidebar navigation

Groups start collapsed in a fresh browser. Group expansion and the last component route are stored in localStorage under `showcode-navigation-v1`. Opening the app restores that route; an explicit component URL takes precedence. Returning to All Components saves the overview as the current route. Clearing browser site data resets these preferences.

Browser regression check against a published build (requires Chrome and the app's npm dependencies):

```powershell
node src/app/wwwroot/visual/test-navigation.mjs D:/path/to/published/wwwroot
```
