using LowlandTech.Components.Tests.SPEC1072.DataForm.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US01.ModelBinding;

using DataFormSut = LowlandTech.Components.DataForm.DataForm;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "Model Binding and Field Generation")]
public class SC01_DataFormModelBinding : ViewScenario
{
    [Fact(DisplayName = "UAC01: DataForm creates an EditContext for the supplied model")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_CreatesEditContext()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-slot='data-form']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC02: the EditContext model is the supplied CustomerFormModel instance")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_EditContextModelIsSuppliedInstance()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        var cascading = cut.FindComponent<CascadingValue<EditContext>>();
        cascading.Instance.Value.Model.ShouldBe(model);
    }

    [Fact(DisplayName = "UAC03: DataForm cascades the EditContext to child field components")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_CascadesEditContext()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        var cascading = cut.FindComponent<CascadingValue<EditContext>>();
        cascading.ShouldNotBeNull();
        cascading.Instance.Value.ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC05: fields contain Name, Email, IsActive, CreditLimit, and CreatedOn")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_DiscoversPublicEditableProperties()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='Name']").ShouldNotBeNull();
        cut.Find("[data-field-input='Email']").ShouldNotBeNull();
        cut.Find("[data-field-input='IsActive']").ShouldNotBeNull();
        cut.Find("[data-field-input='CreditLimit']").ShouldNotBeNull();
        cut.Find("[data-field-input='CreatedOn']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC07: fields preserve a deterministic order by metadata order then declaration order")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_FieldOrderByMetadata()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        var labels = cut.FindAll("[data-field-label]");
        labels.Count.ShouldBeGreaterThan(2);
        var nameIndex = labels.ToList().FindIndex(l => l.GetAttribute("data-field-label") == "Name");
        var emailIndex = labels.ToList().FindIndex(l => l.GetAttribute("data-field-label") == "Email");
        nameIndex.ShouldBeLessThan(emailIndex);
    }

    [Fact(DisplayName = "UAC09: string properties render text inputs by default")]
    [Trait(Spec.UAC, "09")]
    public void UAC09_StringPropertyRendersTextInput()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='Name']").GetAttribute("type").ShouldBe("text");
    }

    [Fact(DisplayName = "UAC10: numeric properties render number inputs by default")]
    [Trait(Spec.UAC, "10")]
    public void UAC10_NumericPropertyRendersNumberInput()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='CreditLimit']").GetAttribute("type").ShouldBe("number");
    }

    [Fact(DisplayName = "UAC11: boolean properties render checkbox inputs by default")]
    [Trait(Spec.UAC, "11")]
    public void UAC11_BooleanPropertyRendersCheckbox()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='IsActive']").GetAttribute("type").ShouldBe("checkbox");
    }

    [Fact(DisplayName = "UAC12: enum properties render Select component by default")]
    [Trait(Spec.UAC, "12")]
    public void UAC12_EnumPropertyRendersSelect()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='Status']").GetAttribute("data-slot").ShouldBe("select");
    }

    [Fact(DisplayName = "UAC13: DateTime properties render date inputs by default")]
    [Trait(Spec.UAC, "13")]
    public void UAC13_DateTimePropertyRendersDateInput()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='CreatedOn']").GetAttribute("type").ShouldBe("date");
    }

    [Fact(DisplayName = "UAC15: DisplayAttribute name is used as field label")]
    [Trait(Spec.UAC, "15")]
    public void UAC15_DisplayNameApplied()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-label='Name']").TextContent.ShouldContain("Customer name");
    }

    [Fact(DisplayName = "UAC16: Email display name is applied")]
    [Trait(Spec.UAC, "16")]
    public void UAC16_EmailDisplayNameApplied()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-label='Email']").TextContent.ShouldContain("Email address");
    }

    [Fact(DisplayName = "UAC18: includeProperties shapes generated fields")]
    [Trait(Spec.UAC, "18")]
    public void UAC18_IncludePropertiesShapesFields()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.IncludeProperties, ["Name", "Email"]));
        cut.FindAll("[data-field-input='Name']").Count.ShouldBe(1);
        cut.FindAll("[data-field-input='Email']").Count.ShouldBe(1);
        cut.FindAll("[data-field-input='IsActive']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC19: excludeProperties removes specified fields")]
    [Trait(Spec.UAC, "19")]
    public void UAC19_ExcludePropertiesRemovesFields()
    {
        var model = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.ExcludeProperties, ["InternalCode"]));
        cut.FindAll("[data-field-input='InternalCode']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC21: updating a field writes to the model")]
    [Trait(Spec.UAC, "21")]
    public void UAC21_FieldUpdateWritesToModel()
    {
        var model = new CustomerFormModel { Name = "Old name" };
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='Name']").Input("New name");
        model.Name.ShouldBe("New name");
    }

    [Fact(DisplayName = "UAC22: clearing a numeric field sets it to zero instead of crashing")]
    [Trait(Spec.UAC, "22")]
    public void UAC22_ClearingNumericFieldSetsZero()
    {
        var model = new CustomerFormModel { Age = 42 };
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model));
        cut.Find("[data-field-input='Age']").Input("");
        model.Age.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC25: the EditContext points at the new model instance")]
    [Trait(Spec.UAC, "25")]
    public void UAC25_EditContextPointsAtNewModel()
    {
        var model1 = new CustomerFormModel();
        var cut = Ctx.Render<DataFormSut>(p => p.Add(x => x.Model, model1));
        var cascadingBefore = cut.FindComponent<CascadingValue<EditContext>>().Instance.Value;

        var model2 = new CustomerFormModel();
        cut.Render(p => p.Add(x => x.Model, model2));

        var cascadingAfter = cut.FindComponent<CascadingValue<EditContext>>().Instance.Value;
        cascadingAfter.ShouldNotBe(cascadingBefore);
        cascadingAfter.Model.ShouldBe(model2);
    }

    [Fact(DisplayName = "UAC29: read-only mode renders values without editable inputs")]
    [Trait(Spec.UAC, "29")]
    public void UAC29_ReadOnlyRendersValues()
    {
        var model = new CustomerFormModel { Name = "ReadOnlyValue" };
        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.ReadOnly, true));
        cut.Find("[data-field-readonly='Name']").TextContent.ShouldContain("ReadOnlyValue");
    }

    [Fact(DisplayName = "UAC30: read-only mode does not render editable inputs")]
    [Trait(Spec.UAC, "30")]
    public void UAC30_ReadOnlyDoesNotRenderInputs()
    {
        var model = new CustomerFormModel { Name = "ReadOnlyValue" };
        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.ReadOnly, true));
        cut.FindAll("[data-field-input='Name']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC31: validation does not mark read-only fields dirty on render")]
    [Trait(Spec.UAC, "31")]
    public void UAC31_ReadOnlyFieldsNotMarkedDirty()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.ReadOnly, true));
        var cascading = cut.FindComponent<CascadingValue<EditContext>>().Instance.Value;
        cascading.IsModified(new FieldIdentifier(model, "Name")).ShouldBeFalse();
    }
}
