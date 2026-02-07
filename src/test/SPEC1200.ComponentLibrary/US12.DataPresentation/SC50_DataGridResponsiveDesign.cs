using Xunit;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "50")]
public class SC50_DataGridResponsiveDesign : Scenario
{
    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(Skip = "Feature not yet implemented", DisplayName = "Then the grid should adapt appropriately")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_GridAdaptsAppropriately()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when responsive settings are available
        // Given I have a DataGrid with responsive settings
        // When I view on different screen sizes
        // Then the grid should adapt appropriately
    }

    [Fact(Skip = "Feature not yet implemented", DisplayName = "And I can use ShowOnMobile/ShowOnDesktop per column")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_CanUseShowOnMobileShowOnDesktopPerColumn()
    {
        await ArrangeAndActAsync();
        // TODO: Implement when ShowOnMobile/ShowOnDesktop properties are available
        // When I view on different screen sizes
        // Then I can use ShowOnMobile/ShowOnDesktop per column
    }
}
