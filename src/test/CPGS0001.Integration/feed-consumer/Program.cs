using LowlandTech.Components.Core;
using LowlandTech.Components.Features.Pages;
using LowlandTech.FeedConsumer;
using LowlandTech.Plugins.Shell;

// The shared feed consumption proof of the CPGS0001 page adapter.
//
// It compiles against the published packages only, never against the
// Components source tree, and drives the documented consumption path of a host
// such as the Coder lane: a trusted startup map, a published page binding, the
// Plugins route and page catalogs built from the bridge projection, a
// withdrawal and a reactivation.
//
// The exit code is the gate. A failure prints the failing check and returns 1.

const string OwnerKey = "coder.owner";
const string SymbolicIdentity = "Coder.Page";
const string RoutePath = "/plugin/coder";

var failures = new List<string>();

void Check(string what, bool ok)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {what}");

    if (!ok)
    {
        failures.Add(what);
    }
}

// The trusted startup map of the host.
var startupMap = new ComponentsStartupMap(
    [new ComponentsStartupBinding(OwnerKey, SymbolicIdentity, typeof(CoderPage))],
    [OwnerKey]);

// The bridge of the page adapter, with the route the host reserved.
var bridge = new ComponentPageOwnerBridge(
    startupMap,
    new ComponentPageOwnerSnapshot(1, [OwnerKey]),
    ["/"]);

// The owner publishes its page.
var validation = bridge.Publish(new ComponentPageBinding(OwnerKey, SymbolicIdentity, RoutePath));

Check("the published candidate is valid", validation.IsValid);
Check(
    "a refused candidate names its refusal",
    validation.Errors.Count == 0 || validation.Errors[0].Code != ComponentPageBindingErrorCode.DuplicateBinding);

// The host builds the Plugins catalogs from the bridge projection.
PluginPageCatalog Pages() => new(bridge.PageCatalog().Entries);

PluginComponentCatalog Components() => new(
    bridge.Enumerate().Select(binding => new KeyValuePair<(string, string), Type>(
        (binding.OwnerKey, binding.SymbolicIdentity),
        typeof(CoderPage))));

Check("the active owner contributes one page", bridge.Enumerate().Count == 1);
Check("the page catalog carries one entry", Pages().Entries.Count == 1);
Check("the component catalog carries one entry", Components().Entries.Count == 1);

var metadata = Pages().Entries[(OwnerKey, SymbolicIdentity)];

Check("the metadata keeps the declared label", metadata.Label == "Coder reports");
Check("the metadata keeps the declared icon", metadata.Icon == "table");
Check("the metadata keeps the declared order", metadata.Order == 30);
Check("the metadata keeps the declared group", metadata.Group == "Operations");
Check("the metadata keeps the declared subgroup", metadata.SubGroup == "Daily");
Check("the navigation offers the page", bridge.Navigation().Count == 1);
Check("the route resolves the page", bridge.Resolve(RoutePath) is not null);

// The host withdraws the owner.
bridge.Deliver(new ComponentPageOwnerSnapshot(2, []));

Check("the withdrawn owner contributes no page", bridge.Enumerate().Count == 0);
Check("the page catalog is empty after the withdrawal", Pages().Entries.Count == 0);
Check("the component catalog is empty after the withdrawal", Components().Entries.Count == 0);
Check("the navigation is empty after the withdrawal", bridge.Navigation().Count == 0);
Check("the route resolves nothing after the withdrawal", bridge.Resolve(RoutePath) is null);
Check("the owner is not active", !bridge.IsOwnerActive(OwnerKey));
Check("the bridge holds the new revision", bridge.Revision == 2);

// The host activates the owner again.
bridge.Deliver(new ComponentPageOwnerSnapshot(3, [OwnerKey]));

Check("the reactivated owner contributes the page again", bridge.Enumerate().Count == 1);
Check("the page catalog carries the page again", Pages().Entries.Count == 1);
Check("the route resolves the page again", bridge.Resolve(RoutePath) is not null);

Console.WriteLine(failures.Count == 0
    ? "FEED CONSUMER OK"
    : $"FEED CONSUMER FAILED: {string.Join("; ", failures)}");

return failures.Count == 0 ? 0 : 1;
