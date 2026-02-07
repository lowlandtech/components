namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US09.Blocks;

/// <summary>
/// Tests for preview block porting.
/// SC10 - Port block preview
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "10")]
public class SC10_BlockPreview : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "preview block not yet implemented", DisplayName = "Then it should render the block layout using existing library components")]
    [Trait(Spec.UAC, "10")]
    public async Task UAC10_ShouldRenderBlockLayout()
    {
        await ArrangeAndActAsync();
        // TODO: Implement test when preview block is ported
    }
}
