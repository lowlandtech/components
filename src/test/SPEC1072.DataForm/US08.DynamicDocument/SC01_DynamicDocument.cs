using LowlandTech.Components.Tests.SPEC1072.DataForm.Models;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US08.DynamicDocument;

using DataFormSut = LowlandTech.Components.DataForm;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "08")]
[Trait(Spec.SC, "01")]
[UserStory("08", "A document whose fields are declared at runtime renders through the form")]
public class SC01_DynamicDocument : ViewScenario
{
    private static DataFormField Field(string name, string label, string kind = "text", bool readOnly = false, int order = 0)
        => new(name, label, typeof(string), kind, Required: false, ReadOnly: readOnly, Order: order);

    [Fact(DisplayName = "UAC01: declared fields render without a model")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_DeclaredFieldsRenderWithoutModel()
    {
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Fields,
        [
            Field("title", "Title"),
            Field("body", "Body")
        ]));

        cut.FindAll("[data-field]").Count.ShouldBe(2);
        cut.Find("[data-field='title']").ShouldNotBeNull();
        cut.Find("[data-field-input='title']").ShouldNotBeNull();
        cut.Find("[data-field-label='body']").TextContent.ShouldContain("Body");
    }

    [Fact(DisplayName = "UAC02: the values dictionary is what the controls show")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_ValuesDictionaryIsShown()
    {
        var values = new Dictionary<string, string?> { ["title"] = "Cold open", ["body"] = "A quiet street" };

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Fields, [Field("title", "Title"), Field("body", "Body")])
            .Add(x => x.Values, values));

        cut.Find("[data-field-input='title']").GetAttribute("value").ShouldBe("Cold open");
        cut.Find("[data-field-input='body']").GetAttribute("value").ShouldBe("A quiet street");
    }

    [Fact(DisplayName = "UAC03: editing writes the dictionary and reports the change")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_EditWritesDictionaryAndRaisesCallback()
    {
        var values = new Dictionary<string, string?> { ["title"] = "Cold open" };
        var changed = new List<FormFieldChangeArgs>();

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Fields, [Field("title", "Title")])
            .Add(x => x.Values, values)
            .Add(x => x.OnFieldValueChanged, (FormFieldChangeArgs args) => changed.Add(args)));

        cut.Find("[data-field-input='title']").Input("Cold close");

        values["title"].ShouldBe("Cold close");
        changed.ShouldContain(a => a.FieldName == "title" && (string?)a.NewValue == "Cold close");
    }

    [Fact(DisplayName = "UAC04: a declared read-only field shows its value without a control")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ReadOnlyDeclaredFieldShowsText()
    {
        var values = new Dictionary<string, string?> { ["id"] = "doc-7" };

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Fields, [Field("id", "Identifier", readOnly: true)])
            .Add(x => x.Values, values));

        cut.Find("[data-field-readonly='id']").TextContent.Trim().ShouldBe("doc-7");
        cut.FindAll("[data-field-input='id']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC05: a model-based form still takes its fields from the model")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_ModelPathUnchanged()
    {
        var model = new CustomerFormModel { Name = "Ada" };

        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));

        cut.Find("[data-field='Name']").ShouldNotBeNull();
        cut.Find("[data-field-label='Name']").TextContent.ShouldContain("Customer name");
        cut.Find("[data-field-input='Name']").GetAttribute("value").ShouldBe("Ada");
    }
}
