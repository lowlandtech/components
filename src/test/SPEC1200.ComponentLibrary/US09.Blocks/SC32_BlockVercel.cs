namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US09.Blocks;

/// <summary>
/// Tests for vercel block porting.
/// SC32 - Port block vercel
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "32")]
public class SC32_BlockVercel : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "vercel block not yet implemented", DisplayName = "Then it should render the block layout using existing library components")]
    [Trait(Spec.UAC, "32")]
    public async Task UAC32_ShouldRenderBlockLayout()
    {
        await ArrangeAndActAsync();
        // TODO: Implement test when vercel block is ported
    }
}
