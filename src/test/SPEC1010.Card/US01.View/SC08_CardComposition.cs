using Bunit;
using CardSut = LowlandTech.Components.Card.Card;
using LowlandTech.Components.Card;
using LowlandTech.Components.Tests.Infrastructure;

namespace LowlandTech.Components.Tests.SPEC1010.Card.US01.View;

[Trait(Spec.SPEC, "1010")]
[Trait(Spec.SC, "08")]
[UserStory("01", "Render Card component")]
public class SC08_CardComposition : ViewScenario
{
    [Fact(DisplayName = "UAC01: Card composes header, content, and footer")]
    [Trait(Spec.UAC, "01")]
    [Then("all sub-components render inside the card")]
    public void UAC01_ComposesSubComponents()
    {
        var cut = Ctx.Render<CardSut>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<CardHeader>(0);
            builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b =>
            {
                b.OpenComponent<CardTitle>(0);
                b.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(0, "Test Title")));
                b.CloseComponent();
                b.OpenComponent<CardDescription>(0);
                b.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b2 => b2.AddContent(0, "Test Description")));
                b.CloseComponent();
            }));
            builder.CloseComponent();
            builder.OpenComponent<CardContent>(0);
            builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Body content")));
            builder.CloseComponent();
            builder.OpenComponent<CardFooter>(0);
            builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Footer content")));
            builder.CloseComponent();
        }));

        // Verify nesting
        var card = cut.Find("[data-slot='card']");
        card.QuerySelector("[data-slot='card-header']").ShouldNotBeNull();
        card.QuerySelector("[data-slot='card-title']").ShouldNotBeNull();
        card.QuerySelector("[data-slot='card-description']").ShouldNotBeNull();
        card.QuerySelector("[data-slot='card-content']").ShouldNotBeNull();
        card.QuerySelector("[data-slot='card-footer']").ShouldNotBeNull();

        cut.Markup.ShouldContain("Test Title");
        cut.Markup.ShouldContain("Test Description");
        cut.Markup.ShouldContain("Body content");
        cut.Markup.ShouldContain("Footer content");
    }

    [Fact(DisplayName = "UAC02: CardAction renders inside CardHeader")]
    [Trait(Spec.UAC, "02")]
    [Then("card-action is a child of card-header")]
    public void UAC02_CardActionRendersInsideHeader()
    {
        var cut = Ctx.Render<CardHeader>(p => p.AddChildContent(builder =>
        {
            builder.OpenComponent<CardTitle>(0);
            builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Title")));
            builder.CloseComponent();
            builder.OpenComponent<CardAction>(0);
            builder.AddAttribute(1, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)(b => b.AddContent(0, "Action")));
            builder.CloseComponent();
        }));

        var header = cut.Find("[data-slot='card-header']");
        header.QuerySelector("[data-slot='card-action']").ShouldNotBeNull();
        cut.Markup.ShouldContain("Action");
    }
}
