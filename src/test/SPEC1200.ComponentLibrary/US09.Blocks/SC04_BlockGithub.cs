namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US09.Blocks;

/// <summary>
/// Tests for github block porting.
/// SC04 - Port block github
/// </summary>
[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "09")]
[Trait(Spec.SC, "04")]
public class SC04_BlockGithub : Scenario
{
    protected override Task GivenAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task WhenAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(Skip = "github block not yet implemented", DisplayName = "Then it should render the block layout using existing library components")]
    [Trait(Spec.UAC, "04")]
    public async Task UAC04_ShouldRenderBlockLayout()
    {
        await ArrangeAndActAsync();
        // TODO: Implement test when github block is ported
    }
}
