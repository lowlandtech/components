using Bunit;
using SliderSut = LowlandTech.Components.Slider.Slider;
using LowlandTech.Components.Slider;

namespace LowlandTech.Components.Tests.SPEC1045.Slider.US01.View;

[Trait(Spec.SPEC, "1045")]
[Trait(Spec.SC, "02")]
[UserStory("01", "Render Slider subcomponents")]
public class SC02_RenderSliderSubcomponents : ViewScenario
{
    [Fact(DisplayName = "UAC01: SliderTrack renders data-slot attribute")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_SliderTrackRendersDataSlot()
    {
        var cut = Ctx.Render<SliderTrack>();
        cut.Find("[data-slot='slider-track']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: SliderTrack inherits orientation from parent")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_SliderTrackInheritsOrientation()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Orientation, "vertical");
            p.AddChildContent<SliderTrack>();
        });
        cut.Find("[data-orientation='vertical']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: SliderTrack renders ChildContent")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_SliderTrackRendersChildContent()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent("Track Content");
            });
        });
        cut.Markup.ShouldContain("Track Content");
    }

    [Fact(DisplayName = "UAC04: SliderRange renders data-slot attribute")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_SliderRangeRendersDataSlot()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent<SliderRange>();
            });
        });
        cut.Find("[data-slot='slider-range']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: SliderRange inherits orientation from parent")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_SliderRangeInheritsOrientation()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Orientation, "vertical");
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent<SliderRange>();
            });
        });
        var range = cut.Find("[data-slot='slider-range']");
        range.GetAttribute("data-orientation").ShouldBe("vertical");
    }

    [Fact(DisplayName = "UAC06: SliderRange renders with width style based on value")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SliderRangeRendersWithWidthStyle()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Value, 50);
            p.Add(x => x.Min, 0);
            p.Add(x => x.Max, 100);
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent<SliderRange>();
            });
        });
        var range = cut.Find("[data-slot='slider-range']");
        range.GetAttribute("style").ShouldContain("width: 50%");
    }

    [Fact(DisplayName = "UAC07: SliderRange renders with height style for vertical orientation")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_SliderRangeRendersWithHeightStyleForVertical()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Value, 75);
            p.Add(x => x.Orientation, "vertical");
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent<SliderRange>();
            });
        });
        var range = cut.Find("[data-slot='slider-range']");
        range.GetAttribute("style").ShouldContain("height: 75%");
    }

    [Fact(DisplayName = "UAC08: SliderThumb renders data-slot attribute")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_SliderThumbRendersDataSlot()
    {
        var cut = Ctx.Render<SliderThumb>();
        cut.Find("[data-slot='slider-thumb']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC09: SliderThumb has role=slider attribute")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_SliderThumbHasRoleSlider()
    {
        var cut = Ctx.Render<SliderThumb>();
        cut.Find("[role='slider']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC10: SliderThumb renders with aria attributes")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_SliderThumbRendersWithAriaAttributes()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Min, 0);
            p.Add(x => x.Max, 100);
            p.Add(x => x.Value, 30);
            p.AddChildContent<SliderThumb>();
        });
        var thumb = cut.Find("[data-slot='slider-thumb']");
        thumb.GetAttribute("aria-valuemin").ShouldBe("0");
        thumb.GetAttribute("aria-valuemax").ShouldBe("100");
        thumb.GetAttribute("aria-valuenow").ShouldBe("30");
    }

    [Fact(DisplayName = "UAC11: SliderThumb renders with tabindex attribute")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_SliderThumbRendersWithTabindex()
    {
        var cut = Ctx.Render<SliderThumb>();
        var thumb = cut.Find("[data-slot='slider-thumb']");
        thumb.GetAttribute("tabindex").ShouldBe("0");
    }

    [Fact(DisplayName = "UAC12: SliderThumb renders with -1 tabindex when disabled")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_SliderThumbRendersWithNegativeTabindexWhenDisabled()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Disabled, true);
            p.AddChildContent<SliderThumb>();
        });
        var thumb = cut.Find("[data-slot='slider-thumb']");
        thumb.GetAttribute("tabindex").ShouldBe("-1");
    }

    [Fact(DisplayName = "UAC13: Complete Slider renders all subcomponents")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_CompleteSliderRendersAllSubcomponents()
    {
        var cut = Ctx.Render<SliderSut>(p =>
        {
            p.Add(x => x.Value, 50);
            p.AddChildContent<SliderTrack>(t =>
            {
                t.AddChildContent<SliderRange>();
            });
            p.AddChildContent<SliderThumb>();
        });

        cut.Find("[data-slot='slider']").ShouldNotBeNull();
        cut.Find("[data-slot='slider-track']").ShouldNotBeNull();
        cut.Find("[data-slot='slider-range']").ShouldNotBeNull();
        cut.Find("[data-slot='slider-thumb']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC14: SliderThumb Class parameter is applied")]
    [Trait(Spec.UAC, "14")]
    public void UAC14_SliderThumbClassParameterApplied()
    {
        var cut = Ctx.Render<SliderThumb>(p => p.Add(x => x.Class, "custom-thumb"));
        cut.Find("[data-slot='slider-thumb']").ClassList.ShouldContain("custom-thumb");
    }
}
