using System.Reflection;
using System.Text.RegularExpressions;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1066.ContainerGrid.US04.DisposeResilience;

/// <summary>
/// SC00 — Drift guard for ContainerGrid / ContainerGridSection dispose
/// resilience against a disconnected Blazor Server circuit.
///
/// Background: when an auth session times out the circuit is torn down and
/// the JS runtime is no longer reachable. Any unguarded JS-interop call in
/// <c>DisposeAsync</c> (including the implicit interop performed by
/// <c>IJSObjectReference.DisposeAsync</c>) bubbles a
/// <see cref="Microsoft.JSInterop.JSDisconnectedException"/> out of the
/// renderer and crashes the application.
///
/// These tests scan the source of both components and assert that every
/// <c>DisposeAsync</c> still wraps its interop calls in the canonical
/// catch set (JSDisconnected / ObjectDisposed / InvalidOperation /
/// TaskCanceled). They are intentionally textual — a behavioural test
/// cannot easily reproduce a torn-down circuit, but a renamed file or a
/// "just clean this up" refactor that drops a catch block must fail loudly
/// and immediately.
/// </summary>
[Trait(Spec.SPEC, "1066")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "00")]
[UserStory("04", "ContainerGrid dispose survives disconnected circuit")]
public class SC00_DisposeSurvivesDisconnectedCircuit
{
    private static readonly string[] RequiredCatches =
    {
        "JSDisconnectedException",
        "ObjectDisposedException",
        "InvalidOperationException",
        "TaskCanceledException",
    };

    /// <summary>
    /// Reads the named source file out of the test assembly's manifest
    /// resources (see LowlandTech.Components.Tests.csproj — EmbeddedResource
    /// rules with LogicalName=fileName). Reading from disk via
    /// [CallerFilePath] does not survive NCrunch's shadow copy, which only
    /// brings the test project's outputs into the workspace, not the lib
    /// project's source tree.
    /// </summary>
    private static string ReadSource(string fileName)
    {
        var asm = typeof(SC00_DisposeSurvivesDisconnectedCircuit).Assembly;
        using var stream = asm.GetManifestResourceStream(fileName);
        stream.ShouldNotBeNull(
            $"Expected '{fileName}' embedded as a manifest resource in the test assembly. " +
            "Wire it up in LowlandTech.Components.Tests.csproj via <EmbeddedResource>.");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Extracts the body of the (single) <c>DisposeAsync</c> method by
    /// matching from the signature through to the matching closing brace.
    /// </summary>
    private static string ExtractDisposeAsyncBody(string source)
    {
        var match = Regex.Match(
            source,
            @"public\s+async\s+ValueTask\s+DisposeAsync\s*\(\s*\)\s*\{",
            RegexOptions.Compiled);
        match.Success.ShouldBeTrue("Could not locate 'public async ValueTask DisposeAsync()' signature");

        var depth = 1;
        var i = match.Index + match.Length;
        var start = i;
        while (i < source.Length && depth > 0)
        {
            var c = source[i];
            if (c == '{') depth++;
            else if (c == '}') depth--;
            i++;
        }

        depth.ShouldBe(0, "Unbalanced braces inside DisposeAsync");
        return source[start..(i - 1)];
    }

    // ── ContainerGrid ───────────────────────────────────────────────────────

    [Fact(DisplayName = "UAC01: ContainerGrid.DisposeAsync catches JSDisconnectedException, ObjectDisposed, InvalidOperation, TaskCanceled")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_ContainerGrid_DisposeCatchesAllRequiredExceptions()
    {
        var body = ExtractDisposeAsyncBody(ReadSource("ContainerGrid.razor.cs"));
        foreach (var catchType in RequiredCatches)
        {
            body.ShouldContain(
                $"catch ({catchType})",
                customMessage: $"DisposeAsync must catch {catchType} so a torn-down circuit can finish disposing without crashing the app");
        }
    }

    [Fact(DisplayName = "UAC02: ContainerGrid.DisposeAsync wraps _module.DisposeAsync() in its own try/catch")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ContainerGrid_ModuleDisposeIsGuarded()
    {
        // The interop bug we are guarding against was 'await _module.DisposeAsync()'
        // sitting outside the try/catch — guard that the call is followed (within a
        // few lines) by a catch (JSDisconnectedException) clause.
        var body = ExtractDisposeAsyncBody(ReadSource("ContainerGrid.razor.cs"));
        var idx = body.IndexOf("_module.DisposeAsync()", StringComparison.Ordinal);
        idx.ShouldBeGreaterThan(-1, "Expected _module.DisposeAsync() to be called from DisposeAsync");

        var tail = body[idx..];
        tail.ShouldContain(
            "catch (JSDisconnectedException)",
            customMessage: "_module.DisposeAsync() must be wrapped in a try/catch that handles JSDisconnectedException");
    }

    // ── ContainerGridSection ────────────────────────────────────────────────

    [Fact(DisplayName = "UAC03: ContainerGridSection.DisposeAsync catches JSDisconnectedException, ObjectDisposed, InvalidOperation, TaskCanceled")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_ContainerGridSection_DisposeCatchesAllRequiredExceptions()
    {
        var body = ExtractDisposeAsyncBody(ReadSource("ContainerGridSection.razor.cs"));
        foreach (var catchType in RequiredCatches)
        {
            body.ShouldContain(
                $"catch ({catchType})",
                Case.Sensitive,
                $"ContainerGridSection.DisposeAsync must catch {catchType} so a torn-down circuit can finish disposing without crashing the app");
        }
    }

    [Fact(DisplayName = "UAC04: ContainerGridSection.DisposeAsync wraps _module.DisposeAsync() in its own try/catch")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ContainerGridSection_ModuleDisposeIsGuarded()
    {
        var body = ExtractDisposeAsyncBody(ReadSource("ContainerGridSection.razor.cs"));
        var idx = body.IndexOf("_module.DisposeAsync()", StringComparison.Ordinal);
        idx.ShouldBeGreaterThan(-1, "Expected _module.DisposeAsync() to be called from DisposeAsync");

        var tail = body[idx..];
        tail.ShouldContain(
            "catch (JSDisconnectedException)",
            customMessage: "_module.DisposeAsync() must be wrapped in a try/catch that handles JSDisconnectedException");
    }

    [Fact(DisplayName = "UAC05: ContainerGridSection.DisposeAsync wraps the unbindSection JS call in try/catch")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ContainerGridSection_UnbindIsGuarded()
    {
        var body = ExtractDisposeAsyncBody(ReadSource("ContainerGridSection.razor.cs"));
        var idx = body.IndexOf("\"containerGrid.unbindSection\"", StringComparison.Ordinal);
        idx.ShouldBeGreaterThan(-1, "Expected containerGrid.unbindSection invocation in DisposeAsync");

        var tail = body[idx..];
        tail.ShouldContain(
            "catch (JSDisconnectedException)",
            customMessage: "containerGrid.unbindSection invocation must be wrapped in a try/catch that handles JSDisconnectedException");
    }
}
