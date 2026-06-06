using LowlandTech.Components.Core;
using LowlandTech.Components.Tests.SPEC1072.DataForm.Models;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.US03.StoreIntegration;

using DataFormSut = LowlandTech.Components.DataForm;

[Trait(Spec.SPEC, "1072")]
[Trait(Spec.US, "03")]
[Trait(Spec.SC, "01")]
[UserStory("03", "DataForm Store Integration")]
public class SC01_StoreIntegration : ViewScenario
{
    [Fact(DisplayName = "UAC01: UseStore=true initializes Store with field metadata")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_StoreInitializesWithFields()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        store.State.Fields.Count.ShouldBeGreaterThan(0);
        store.State.Fields.ShouldContain(f => f.PropertyName == "Name");
    }

    [Fact(DisplayName = "UAC02: Store captures initial model values")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_StoreCapturesInitialValues()
    {
        var model = new CustomerFormModel { Name = "Initial" };
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        store.State.Values.ContainsKey("Name").ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC03: FormField input marks state dirty")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_InputMarksStateDirty()
    {
        var model = new CustomerFormModel();
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        var input = cut.Find("[data-field-input='Name']");
        input.Input("Hello");

        store.State.IsDirty.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC04: FormField blur marks field as touched")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_BlurMarksFieldTouched()
    {
        var model = new CustomerFormModel();
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        cut.Find("[data-field-input='Name']").Blur();

        store.State.Touched.ShouldContain("Name");
    }

    [Fact(DisplayName = "UAC05: Store validation errors reflect in FormField")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_StoreErrorsReflectInFormField()
    {
        var model = new CustomerFormModel();
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        store.Dispatch(new DataFormActions.ValidateField("Name", new[] { "Name is required" }));

        cut.Find("[data-field-validation='Name']").TextContent.ShouldContain("required");
    }

    [Fact(DisplayName = "UAC06: Submit dispatches SubmitForm action")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_SubmitDispatchesAction()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        cut.Find("form").Submit();

        store.State.IsSubmitting.ShouldBeTrue();
    }

    [Fact(DisplayName = "UAC07: ResetForm clears state")]
    [Trait(Spec.UAC, "07")]
    public void UAC07_ResetFormClearsState()
    {
        var model = new CustomerFormModel { Name = "Test" };
        var store = new Store<DataFormState>(new DataFormState(), DataFormReducer.Reduce);

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, true)
            .Add(x => x.Store, store));

        store.Dispatch(new DataFormActions.ResetForm());

        store.State.IsDirty.ShouldBeFalse();
        store.State.Errors.Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC08: UseStore=false preserves legacy EditContext behavior")]
    [Trait(Spec.UAC, "08")]
    public void UAC08_LegacyModePreservesEditContext()
    {
        var model = new CustomerFormModel { Name = "Test" };

        var cut = Ctx.Render<DataFormSut>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.UseStore, false));

        cut.Find("[data-field-input='Name']").GetAttribute("type").ShouldBe("text");
    }
}
