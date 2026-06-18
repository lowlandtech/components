namespace LowlandTech.Components.Tests.SPEC1078.Session.US04.DynamicBlock;

[Trait(Spec.SPEC, "1078")]
[Trait(Spec.US, "04")]
[Trait(Spec.SC, "01")]
[UserStory("04", "DynamicBlock adapts model descriptors into a SessionBlock state machine")]
public sealed class SC01_DynamicBlock : ViewScenario
{
    [Fact(DisplayName = "UAC01: Reflection descriptor maps a workspace model to node metadata")]
    [Trait(Spec.UAC, "01")]
    [Given("a typed workspace model without a hand-written descriptor")]
    [When("the DynamicBlock discovery resolves the model")]
    [Then("a descriptor supplies a workspace node key, fields, and child collection metadata")]
    public void UAC01_ReflectionDescriptorMapsModel()
    {
        var discovery = Ctx.Services.GetRequiredService<IDynamicBlockDiscovery>();

        var descriptor = discovery.GetDescriptor(typeof(WorkspaceUseCaseModel));

        descriptor.ShouldNotBeNull();
        descriptor.NodeTypeKey.ShouldBe("workspace-use-case-model");
        descriptor.Fields.Select(field => field.Key).ShouldContain("Name");
        descriptor.Collections.Select(collection => collection.Key).ShouldContain("Flows");
        descriptor.Collections.Select(collection => collection.Key).ShouldContain("Workitems");
    }

    [Fact(DisplayName = "UAC02: DynamicBlock renders the SessionBlock chrome from model collections")]
    [Trait(Spec.UAC, "02")]
    [Given("a workspace use case model with flows and workitems")]
    [When("DynamicBlock renders the model")]
    [Then("the SessionBlock left rail contains descriptor-created collection headers and child nodes")]
    public void UAC02_RendersSessionBlockFromModel()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-slot='dynamic-block']").ShouldNotBeNull();
        cut.Find("[data-slot='session-block']").ShouldNotBeNull();
        cut.Find("[data-node-id='flow-onboarding']").ShouldNotBeNull();
        cut.Find("[data-node-id='workitem-setup-ci']").ShouldNotBeNull();
        cut.Find("[data-node-id='workspace-foundry']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC03: Dynamic node projection opens the dynamic form in view mode")]
    [Trait(Spec.UAC, "03")]
    [Given("a DynamicBlock rendered from a workspace model")]
    [When("the user projects a child node")]
    [Then("the projected context card delegates its fields to a read-only DataForm until Edit is chosen")]
    public void UAC03_ProjectionRendersDynamicForm()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workitem-setup-ci']").Click();

        cut.Find("[data-slot='dynamic-node-block']").ShouldNotBeNull();
        cut.Find("[data-slot='dynamic-node-form']").ShouldNotBeNull();
        cut.Find("[data-slot='data-form']").ShouldNotBeNull();
        cut.Find("[data-slot='dynamic-node-block']").GetAttribute("data-form-mode").ShouldBe("view");
        cut.Find("[data-field-readonly='Title']").TextContent.ShouldBe("Set up CI");
        cut.Find("button[data-action='edit']").TextContent.ShouldContain("Edit");
    }

    [Fact(DisplayName = "UAC04: Dynamic reducer arm schedules field mutations through the outbox")]
    [Trait(Spec.UAC, "04")]
    [Given("a DynamicBlockStateMachine with the default reducer arms and an outbox")]
    [When("a DynamicFieldChanged action is dispatched")]
    [Then("state is updated and a DynamicMutationEffect is appended to the outbox")]
    public void UAC04_ReducerArmSchedulesMutationOutbox()
    {
        var outbox = new InMemorySessionOutbox();
        var machine = new DynamicBlockStateMachine(
            [new DynamicInitializerReducerArm(), new DynamicFieldReducerArm()],
            outbox);
        machine.Dispatch(new DynamicBlockInitialized(
            "workspace-foundry",
            new Dictionary<string, IReadOnlyDictionary<string, object?>>
            {
                ["workspace-foundry"] = new Dictionary<string, object?> { ["Name"] = "Foundry" }
            },
            []));

        machine.Dispatch(new DynamicFieldChanged("workspace-foundry", "workspace", "Name", "Foundry Platform"));

        machine.State.EntityPropertiesById["workspace-foundry"]["Name"].ShouldBe("Foundry Platform");
        outbox.Entries.Count.ShouldBe(1);
        outbox.Entries[0].Type.ShouldBe(nameof(DynamicMutationEffect));
        var effect = outbox.Entries[0].Payload.ShouldBeOfType<DynamicMutationEffect>();
        effect.Mutation.ShouldBe("update");
        effect.EntityId.ShouldBe("workspace-foundry");
        effect.NodeTypeKey.ShouldBe("workspace");
        effect.Values["Name"].ShouldBe("Foundry Platform");
    }

    [Fact(DisplayName = "UAC05: DynamicNodeBlock buffers edits until Save dispatches through the outbox")]
    [Trait(Spec.UAC, "05")]
    [Given("a DynamicBlock where a projected node form is visible")]
    [When("the user chooses Edit, changes Title, and explicitly chooses Save")]
    [Then("the DynamicBlock dispatcher records one committed change in dynamic state")]
    public void UAC05_FieldInputDispatchesToDynamicBlock()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("button[data-action='edit']").Click();
        cut.Find("button[data-action='cancel']").TextContent.ShouldContain("Cancel");
        cut.Find("button[data-action='save']").TextContent.ShouldContain("Save");
        cut.Find("[data-field-input='Title']").Input("Set up CI pipeline");

        cut.Instance.DynamicState.EntityPropertiesById["workitem-setup-ci"]["Title"].ShouldBe("Set up CI");

        cut.Find("button[data-action='save']").Click();

        var component = cut.Instance;
        component.DynamicState.EntityPropertiesById["workitem-setup-ci"]["Title"].ShouldBe("Set up CI pipeline");
    }

    [Fact(DisplayName = "UAC06: A node is hydrated through DRD discovery and the node runtime")]
    [Trait(Spec.UAC, "06")]
    [Given("a workspace node, a workspace descriptor registry, and a runtime shape loader")]
    [When("DynamicBlock receives the node without a preloaded model")]
    [Then("the descriptor resolves by node type and the runtime entity renders in SessionBlock")]
    public void UAC06_NodeLoadsThroughDiscoveryAndRuntime()
    {
        var runtime = new RecordingDynamicNodeRuntime(CreateModel());
        var discovery = new DynamicBlockDiscovery([new WorkspaceDynamicBlockRegistry()]);
        var node = new SessionNodeInput("workspace-root", "workspace", "Foundry");

        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Node, node)
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));

        runtime.LoadedNodes.ShouldContain(node);
        cut.Find("[data-slot='session-block']").ShouldNotBeNull();
        cut.Find("[data-node-id='workitem-setup-ci']").ShouldNotBeNull();
        cut.Instance.DynamicState.RootNodeId.ShouldBe("workspace-root");
    }

    [Fact(DisplayName = "UAC07: Create, edit, and delete mutations are scheduled before runtime execution")]
    [Trait(Spec.UAC, "07")]
    [Given("a node-backed DynamicBlock with automatic mutation processing")]
    [When("create, edit, and delete operations are requested")]
    [Then("each CRUD mutation is appended to the outbox and then passed to the node runtime")]
    public async Task UAC07_CrudMutationsScheduleAndExecute()
    {
        var runtime = new RecordingDynamicNodeRuntime(CreateModel());
        var discovery = new DynamicBlockDiscovery([new WorkspaceDynamicBlockRegistry()]);
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Node, new SessionNodeInput("workspace-root", "workspace", "Foundry"))
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));

        await cut.InvokeAsync(() => cut.Instance.CreateAsync(
            "workspace",
            new Dictionary<string, object?> { ["Name"] = "New workspace" }));
        await cut.InvokeAsync(() => cut.Instance.EditAsync(
            new SessionNodeInput("workspace-root", "workspace", "Foundry"),
            new Dictionary<string, object?> { ["Name"] = "Foundry Platform" }));
        await cut.InvokeAsync(() => cut.Instance.DeleteAsync(
            new SessionNodeInput("workspace-root", "workspace", "Foundry Platform")));

        runtime.AppliedMutations.Select(mutation => mutation.Mutation)
            .ShouldBe(["create", "update", "delete"]);
        cut.Instance.OutboxEntries
            .Select(entry => entry.Payload)
            .OfType<DynamicMutationEffect>()
            .Select(mutation => mutation.Mutation)
            .ShouldBe(["create", "update", "delete"]);
        cut.Instance.DynamicState.DeletedEntityIds.ShouldContain("workspace-root");
    }

    [Fact(DisplayName = "UAC08: Session chrome renders before node hydration completes")]
    [Trait(Spec.UAC, "08")]
    [Given("a node runtime whose entity load is still pending")]
    [When("DynamicBlock receives the node")]
    [Then("the provisional SessionBlock is visible without waiting for the runtime response")]
    public void UAC08_RendersBeforeHydrationCompletes()
    {
        var runtime = new DeferredDynamicNodeRuntime();
        var discovery = new DynamicBlockDiscovery([new WorkspaceDynamicBlockRegistry()]);

        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Node, new SessionNodeInput("workspace-root", "workspace", "Foundry"))
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));

        runtime.LoadStarted.ShouldBeTrue();
        runtime.LoadCompleted.ShouldBeFalse();
        cut.Find("[data-slot='session-block']").ShouldNotBeNull();
        cut.Find("[data-slot='session-content-loading']").ShouldNotBeNull();
        cut.Find("[data-slot='dynamic-block']").GetAttribute("data-loading").ShouldBe("true");
    }

    [Fact(DisplayName = "UAC09: Loading spinner clears when node hydration completes")]
    [Trait(Spec.UAC, "09")]
    [Given("a DynamicBlock displaying its provisional session and loading spinner")]
    [When("the node runtime completes hydration")]
    [Then("the spinner is removed and the hydrated context tree is displayed")]
    public void UAC09_LoadingSpinnerClearsAfterHydration()
    {
        var runtime = new DeferredDynamicNodeRuntime();
        var discovery = new DynamicBlockDiscovery([new WorkspaceDynamicBlockRegistry()]);
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Node, new SessionNodeInput("workspace-root", "workspace", "Foundry"))
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));

        runtime.Complete(CreateModel());

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("[data-slot='session-content-loading']").Count.ShouldBe(0);
            cut.Find("[data-slot='dynamic-block']").GetAttribute("data-loading").ShouldBe("false");
            cut.Find("[data-node-id='workitem-setup-ci']").ShouldNotBeNull();
        });
    }

    [Fact(DisplayName = "UAC10: Workspace, Flow, and Workitem project their own field shapes")]
    [Trait(Spec.UAC, "10")]
    [Given("a workspace session whose collections contain distinct domain models")]
    [When("each collection item is projected into the session content")]
    [Then("each dynamic form exposes fields belonging to its own domain shape")]
    public void UAC10_DomainModelsProjectDistinctFields()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workspace-foundry']").Click();
        cut.Find("button[data-action='edit']").Click();
        cut.Find("[data-field-input='Slug']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Priority']").Count.ShouldBe(0);

        cut.Find("[data-node-id='flow-onboarding']").Click();
        cut.Find("button[data-action='edit']").Click();
        cut.Find("[data-field-input='Trigger']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Slug']").Count.ShouldBe(0);

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("button[data-action='edit']").Click();
        cut.Find("[data-field-input='Priority']").ShouldNotBeNull();
        cut.Find("[data-field-input='Assignee']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Trigger']").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC11: Dynamic fields use DataForm control mapping")]
    [Trait(Spec.UAC, "11")]
    [Given("a Flow model with a boolean IsEnabled property")]
    [When("the Flow is projected through DynamicNodeBlock")]
    [Then("DataForm renders the property with the shared Checkbox control")]
    public void UAC11_DynamicFieldsUseDataFormControls()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='flow-onboarding']").Click();
        cut.Find("button[data-action='edit']").Click();

        var checkbox = cut.Find("[data-field-input='IsEnabled']");
        checkbox.GetAttribute("data-slot").ShouldBe("checkbox");
        checkbox.GetAttribute("role").ShouldBe("checkbox");
    }

    [Fact(DisplayName = "UAC12: Dynamic entity annotations validate through DataForm")]
    [Trait(Spec.UAC, "12")]
    [Given("a projected Workitem whose required Title is empty")]
    [When("the embedded DataForm is submitted")]
    [Then("DataForm renders the annotation validation message in the session card")]
    public void UAC12_DynamicEntityAnnotationsValidateThroughDataForm()
    {
        var model = CreateModel();
        model.Workitems =
        [
            new WorkitemModel("workitem-setup-ci", string.Empty, "High", "Platform team")
        ];
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, model));

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("button[data-action='edit']").Click();
        cut.Find("[data-slot='data-form']").Submit();

        cut.Find("[data-field-validation='Title']").TextContent.ShouldContain("required");
    }

    [Fact(DisplayName = "UAC13: Dynamic model presentations showcase distinct component compositions")]
    [Trait(Spec.UAC, "13")]
    [Given("Workspace, Flow, and Workitem descriptors with distinct presentation metadata")]
    [When("each model is projected into the session content")]
    [Then("Workspace uses badges, Flow uses an alert and progress, and Workitem uses an avatar, badge, and progress")]
    public void UAC13_DynamicModelsShowcaseDistinctComponents()
    {
        var discovery = new DynamicBlockDiscovery([new ShowcaseDynamicBlockRegistry()]);
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel())
            .Add(x => x.Discovery, discovery));

        cut.Find("[data-node-id='workspace-foundry']").Click();
        cut.Find("[data-slot='dynamic-workspace-presentation']").ShouldNotBeNull();
        cut.FindAll("[data-slot='badge']").Count.ShouldBeGreaterThanOrEqualTo(2);

        cut.Find("[data-node-id='flow-onboarding']").Click();
        cut.Find("[data-slot='dynamic-flow-presentation']").ShouldNotBeNull();
        cut.Find("[data-slot='alert']").ShouldNotBeNull();
        cut.Find("[data-slot='progress']").ShouldNotBeNull();

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("[data-slot='dynamic-workitem-presentation']").ShouldNotBeNull();
        cut.Find("[data-slot='avatar']").ShouldNotBeNull();
        cut.Find("[data-slot='badge']").ShouldNotBeNull();
        cut.Find("[data-slot='progress']").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC13b: Maximized dynamic presentations fill their context card")]
    [Trait(Spec.UAC, "13b")]
    [Given("a Workitem DynamicNodeBlock rendered by a maximized SessionContextBlock")]
    [When("the session context passes its display mode into the dynamic component")]
    [Then("the dynamic root and presentation card both fill the available height")]
    public void UAC13b_MaximizedDynamicPresentationFillsContextCard()
    {
        var model = CreateModel();
        var descriptor = new ShowcaseDynamicBlockRegistry()
            .GetDescriptors()
            .Single(item => item.ModelType == typeof(WorkitemModel));
        var workitem = model.Workitems[0];
        var node = new SessionNodeInput(
            workitem.Id,
            descriptor.NodeTypeKey,
            workitem.Title,
            "DynamicNodeBlock",
            descriptor.Fields.ToDictionary(
                field => field.Key,
                field => descriptor.GetFieldValue(workitem, field.Key),
                StringComparer.Ordinal)
                .Append(new KeyValuePair<string, object?>("dynamic.model", workitem))
                .Append(new KeyValuePair<string, object?>("dynamic.fields", descriptor.Fields))
                .Append(new KeyValuePair<string, object?>("dynamic.presentation", descriptor.Presentation))
                .ToDictionary());
        var block = new SessionContextBlockModel(
            $"context-block:{node.Id}",
            node,
            node.Properties,
            typeof(DynamicNodeBlock));

        var cut = Ctx.Render<SessionContextBlock>(parameters => parameters
            .Add(component => component.Block, block)
            .Add(component => component.Mode, SessionContextBlock.ModeMaximized));

        var dynamicRoot = cut.Find("[data-slot='dynamic-node-block']");
        dynamicRoot.GetAttribute("data-mode").ShouldBe(SessionContextBlock.ModeMaximized);
        dynamicRoot.ClassList.ShouldContain("h-full");
        dynamicRoot.ClassList.ShouldContain("min-h-0");
        var presentation = cut.Find("[data-slot='dynamic-workitem-presentation']");
        presentation.ClassList.ShouldContain("h-full");
        presentation.ClassList.ShouldContain("min-h-0");

        var content = presentation.QuerySelector("[data-slot='card-content']");
        content.ShouldNotBeNull();
        content!.ClassList.ShouldContain("flex-1");
        content.ClassList.ShouldContain("overflow-auto");

        var footer = presentation.QuerySelector("[data-slot='card-footer']");
        footer.ShouldNotBeNull();
        footer!.ClassList.ShouldContain("mt-auto");
        footer.ClassList.ShouldContain("shrink-0");
        footer.ClassList.ShouldContain("border-t");
    }

    [Theory(DisplayName = "UAC13c: Dynamic presentation height follows the projection mode")]
    [InlineData(SessionContextBlock.ModeNormal, false)]
    [InlineData(SessionContextBlock.ModeMinimized, false)]
    [InlineData(SessionContextBlock.ModeMaximized, true)]
    [InlineData(SessionContextBlock.ModeOverlayPage, true)]
    [InlineData(SessionContextBlock.ModeOverlayApp, true)]
    [Trait(Spec.UAC, "13c")]
    [Given("a DynamicNodeBlock rendered in one of the five SessionContextBlock display modes")]
    [When("the dynamic root and presentation card classes are composed")]
    [Then("only maximized, page-overlay, and app-overlay modes receive the full-height flex contract")]
    public void UAC13c_DynamicPresentationHeightFollowsProjectionMode(string mode, bool expectsFullHeight)
    {
        var model = CreateModel();
        var descriptor = new ShowcaseDynamicBlockRegistry()
            .GetDescriptors()
            .Single(item => item.ModelType == typeof(WorkitemModel));
        var workitem = model.Workitems[0];
        var properties = descriptor.Fields.ToDictionary(
            field => field.Key,
            field => descriptor.GetFieldValue(workitem, field.Key),
            StringComparer.Ordinal);
        properties["dynamic.model"] = workitem;
        properties["dynamic.fields"] = descriptor.Fields;
        properties["dynamic.presentation"] = descriptor.Presentation;
        var node = new SessionNodeInput(
            workitem.Id,
            descriptor.NodeTypeKey,
            workitem.Title,
            "DynamicNodeBlock",
            properties);
        var block = new SessionContextBlockModel(
            $"context-block:{node.Id}",
            node,
            properties,
            typeof(DynamicNodeBlock));

        var cut = Ctx.Render<DynamicNodeBlock>(parameters => parameters
            .Add(component => component.Block, block)
            .Add(component => component.Context, node)
            .Add(component => component.Data, properties)
            .Add(component => component.Mode, mode));

        var dynamicRoot = cut.Find("[data-slot='dynamic-node-block']");
        var presentation = cut.Find("[data-slot='dynamic-workitem-presentation']");

        dynamicRoot.ClassList.Contains("h-full").ShouldBe(expectsFullHeight);
        dynamicRoot.ClassList.Contains("min-h-0").ShouldBe(expectsFullHeight);
        presentation.ClassList.Contains("h-full").ShouldBe(expectsFullHeight);
        presentation.ClassList.Contains("min-h-0").ShouldBe(expectsFullHeight);
    }

    [Fact(DisplayName = "UAC14: Collection headers render grid mode and row selection opens form mode")]
    [Trait(Spec.UAC, "14")]
    [Given("a workspace session with a Workspaces collection header and workspace children")]
    [When("the header is projected and a workspace row is selected")]
    [Then("the header shows a DataGrid and the selected workspace is projected in read-only form mode")]
    public void UAC14_CollectionGridSelectsEntityForm()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        cut.Find("[data-slot='dynamic-node-block']").GetAttribute("data-view-mode").ShouldBe("grid");
        cut.Find("[data-slot='dynamic-node-grid']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid']").ShouldNotBeNull();

        cut.FindAll("[data-slot='data-grid-row']").First().Click();

        cut.Find("[data-slot='dynamic-node-block']").GetAttribute("data-view-mode").ShouldBe("form");
        cut.Find("[data-slot='dynamic-node-block']").GetAttribute("data-form-mode").ShouldBe("view");
        cut.Find("[data-field-readonly='Name']").TextContent.ShouldBe("Foundry");
    }

    [Fact(DisplayName = "UAC20: Collection grid fills the available height and keeps its pager at the bottom")]
    [Trait(Spec.UAC, "20")]
    [Given("a projected collection header rendered in grid mode")]
    [When("the collection grid is displayed inside the full-height session content")]
    [Then("the grid and DataGrid participate in the flex height chain while the toolbar remains fixed")]
    public void UAC20_CollectionGridUsesFullHeightLayout()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        var grid = cut.Find("[data-slot='dynamic-node-grid']");
        grid.ClassList.ShouldContain("flex");
        grid.ClassList.ShouldContain("h-full");
        grid.ClassList.ShouldContain("min-h-0");
        grid.ClassList.ShouldContain("flex-col");

        var toolbar = grid.FirstElementChild!;
        toolbar.ClassList.ShouldContain("shrink-0");

        var dataGrid = cut.Find("[data-slot='data-grid']");
        dataGrid.ClassList.ShouldContain("min-h-0");
        dataGrid.ClassList.ShouldContain("flex-1");
    }

    [Fact(DisplayName = "UAC15: Collection search filters while typing and Enter quick-creates a missing entity")]
    [Trait(Spec.UAC, "15")]
    [Given("a projected Workspaces collection grid")]
    [When("the user searches for a missing workspace and presses Enter")]
    [Then("the grid filters immediately and a create mutation is appended to the outbox")]
    public void UAC15_SearchFiltersAndEnterQuickCreates()
    {
        var discovery = new DynamicBlockDiscovery([new ShowcaseDynamicBlockRegistry()]);
        var runtime = new RecordingDynamicNodeRuntime(CreateModel());
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel())
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));
        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        var search = cut.Find("input[type='search']");
        search.Input("Nonexistent");
        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(0);

        search.KeyUp(new KeyboardEventArgs { Key = "Enter" });

        var effect = cut.Instance.OutboxEntries
            .Select(entry => entry.Payload)
            .OfType<DynamicMutationEffect>()
            .Single();
        effect.Mutation.ShouldBe("create");
        effect.NodeTypeKey.ShouldBe("workspace");
        effect.ParentEntityId.ShouldBe("workspace-root");
        effect.Values["Name"].ShouldBe("Nonexistent");
        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(3);
        cut.Markup.ShouldContain("Nonexistent");
    }

    [Fact(DisplayName = "UAC16: Collection grids preserve the shared DataGrid paging contract")]
    [Trait(Spec.UAC, "16")]
    [Given("a Workspaces collection containing more rows than the default page size")]
    [When("the collection header is projected and the user advances the pager")]
    [Then("the grid shows ten rows on the first page and the remaining rows on the second page")]
    public void UAC16_CollectionGridSupportsPaging()
    {
        var model = CreateModel();
        model.Workspaces = Enumerable.Range(1, 12)
            .Select(index => new WorkspaceModel(
                $"workspace-{index}",
                $"Workspace {index}",
                $"workspace-{index}.example",
                "Europe"))
            .ToList();
        var discovery = new DynamicBlockDiscovery([new ShowcaseDynamicBlockRegistry()]);
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, model)
            .Add(x => x.Discovery, discovery));

        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(10);
        cut.Find("[data-slot='data-grid-pager']").ShouldNotBeNull();

        cut.Find("[data-slot='data-grid-page-next']").Click();

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(2);
        cut.Find("[data-slot='data-grid-page-summary']").TextContent.ShouldContain("2 of 2");
    }

    [Fact(DisplayName = "UAC17: Collection grid always shows its pagination footer and Add creates a visible row")]
    [Trait(Spec.UAC, "17")]
    [Given("a projected Workspaces collection with fewer rows than one page")]
    [When("the user enters a workspace name and clicks Add")]
    [Then("the persistent pagination footer remains visible and the created workspace appears in the grid")]
    public void UAC17_PersistentFooterAndAddButtonCreateVisibleRow()
    {
        var discovery = new DynamicBlockDiscovery([new ShowcaseDynamicBlockRegistry()]);
        var runtime = new RecordingDynamicNodeRuntime(CreateModel());
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel())
            .Add(x => x.Discovery, discovery)
            .Add(x => x.Runtime, runtime));
        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        cut.Find("[data-slot='data-grid-pager']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-page-size']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-page-summary']").TextContent.ShouldContain("1 of 1");

        cut.Find("input[type='search']").Input("Northwind");
        cut.Find("button[data-action='add']").Click();

        cut.FindAll("[data-slot='data-grid-row']").Count.ShouldBe(3);
        cut.Markup.ShouldContain("Northwind");
    }

    [Fact(DisplayName = "UAC18: Collection grid pager uses LowlandTech Select and Button components")]
    [Trait(Spec.UAC, "18")]
    [Given("a projected Workspaces collection")]
    [When("the pager is rendered")]
    [Then("page size is a Select and previous/next actions are Button components")]
    public void UAC18_CollectionGridPagerUsesSharedControls()
    {
        var discovery = new DynamicBlockDiscovery([new ShowcaseDynamicBlockRegistry()]);
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel())
            .Add(x => x.Discovery, discovery));
        cut.Find("[data-node-id='workspace-root:Workspaces']").Click();

        cut.Find("[data-slot='data-grid-pager'] [data-slot='data-grid-page-size'] [data-slot='select']").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-pager'] [data-slot='data-grid-page-previous']").GetAttribute("data-variant").ShouldNotBeNull();
        cut.Find("[data-slot='data-grid-pager'] [data-slot='data-grid-page-next']").GetAttribute("data-variant").ShouldNotBeNull();
    }

    [Fact(DisplayName = "UAC19: Descriptor-generated form renders shared controls for representative fields")]
    [Trait(Spec.UAC, "19")]
    [Given("a projected Flow with boolean and text fields")]
    [When("the edit form is rendered")]
    [Then("boolean uses Checkbox and text uses LowlandTech Input")]
    public void UAC19_FormUsesSharedControlsForRepresentativeFields()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p.Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='flow-onboarding']").Click();
        cut.Find("button[data-action='edit']").Click();

        cut.Find("[data-field-input='IsEnabled']").GetAttribute("data-slot").ShouldBe("checkbox");
        cut.Find("[data-field-input='Name']").GetAttribute("data-slot").ShouldBe("input");
    }

    private static WorkspaceUseCaseModel CreateModel() => new()
    {
        Id = "workspace-root",
        Name = "Foundry",
        Workspaces =
        [
            new WorkspaceModel("workspace-foundry", "Foundry", "foundry.dev", "Europe"),
            new WorkspaceModel("workspace-acme", "Acme", "acme.example", "North America")
        ],
        Flows =
        [
            new FlowModel("flow-onboarding", "Onboarding", "workspace.created", true),
            new FlowModel("flow-approval", "Approval", "workitem.submitted", false)
        ],
        Workitems =
        [
            new WorkitemModel("workitem-setup-ci", "Set up CI", "High", "Platform team"),
            new WorkitemModel("workitem-write-docs", "Write docs", "Medium", "Documentation team")
        ]
    };

    private sealed class WorkspaceUseCaseModel
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public IReadOnlyList<WorkspaceModel> Workspaces { get; set; } = [];

        public IReadOnlyList<FlowModel> Flows { get; set; } = [];

        public IReadOnlyList<WorkitemModel> Workitems { get; set; } = [];
    }

    private sealed record WorkspaceModel(string Id, string Name, string Slug, string Region);

    private sealed record FlowModel(string Id, string Name, string Trigger, bool IsEnabled);

    private sealed record WorkitemModel(
        string Id,
        [property: Required] string Title,
        string Priority,
        string Assignee);

    private sealed class WorkspaceDynamicBlockRegistry : IDynamicBlockRegistry
    {
        public IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors()
        {
            var reflected = ReflectionDynamicBlockDescriptor.Create(typeof(WorkspaceUseCaseModel));
            return
            [
                new DynamicBlockDescriptor(
                    typeof(WorkspaceUseCaseModel),
                    "workspace",
                    "Workspace",
                    reflected.GetId,
                    reflected.GetLabel,
                    reflected.GetFieldValue,
                    reflected.Fields,
                    reflected.Collections)
            ];
        }
    }

    private sealed class ShowcaseDynamicBlockRegistry : IDynamicBlockRegistry
    {
        public IReadOnlyList<IDynamicBlockDescriptor> GetDescriptors()
        {
            var root = ReflectionDynamicBlockDescriptor.Create(typeof(WorkspaceUseCaseModel));
            var workspace = ReflectionDynamicBlockDescriptor.Create(typeof(WorkspaceModel));
            var flow = ReflectionDynamicBlockDescriptor.Create(typeof(FlowModel));
            var workitem = ReflectionDynamicBlockDescriptor.Create(typeof(WorkitemModel));

            return
            [
                new DynamicBlockDescriptor(
                    typeof(WorkspaceUseCaseModel),
                    "workspace",
                    "Workspace",
                    root.GetId,
                    root.GetLabel,
                    root.GetFieldValue,
                    root.Fields,
                    root.Collections),
                new DynamicBlockDescriptor(
                    typeof(WorkspaceModel),
                    "workspace",
                    "Workspace",
                    workspace.GetId,
                    workspace.GetLabel,
                    workspace.GetFieldValue,
                    workspace.Fields,
                    workspace.Collections,
                    Presentation: "workspace"),
                new DynamicBlockDescriptor(
                    typeof(FlowModel),
                    "flow",
                    "Flow",
                    flow.GetId,
                    flow.GetLabel,
                    flow.GetFieldValue,
                    flow.Fields,
                    flow.Collections,
                    Presentation: "flow"),
                new DynamicBlockDescriptor(
                    typeof(WorkitemModel),
                    "workitem",
                    "Workitem",
                    workitem.GetId,
                    workitem.GetLabel,
                    workitem.GetFieldValue,
                    workitem.Fields,
                    workitem.Collections,
                    Presentation: "workitem")
            ];
        }
    }

    private sealed class RecordingDynamicNodeRuntime(WorkspaceUseCaseModel model) : IDynamicNodeRuntime
    {
        public List<SessionNodeInput> LoadedNodes { get; } = [];

        public List<DynamicMutationEffect> AppliedMutations { get; } = [];

        public Task<DynamicEntity?> LoadAsync(
            SessionNodeInput node,
            IDynamicBlockDescriptor descriptor,
            CancellationToken cancellationToken = default)
        {
            LoadedNodes.Add(node);
            return Task.FromResult<DynamicEntity?>(new DynamicEntity(
                node,
                typeof(WorkspaceUseCaseModel),
                model,
                descriptor.Fields.ToDictionary(
                    field => field.Key,
                    field => descriptor.GetFieldValue(model, field.Key),
                    StringComparer.Ordinal)));
        }

        public Task<DynamicEntity?> ApplyAsync(
            DynamicMutationEffect mutation,
            IDynamicBlockDescriptor descriptor,
            CancellationToken cancellationToken = default)
        {
            AppliedMutations.Add(mutation);
            if (mutation.Mutation != "create" || descriptor.ModelType != typeof(WorkspaceModel))
                return Task.FromResult<DynamicEntity?>(null);

            var name = Convert.ToString(mutation.Values.GetValueOrDefault("Name"), CultureInfo.InvariantCulture)
                ?? "New Workspace";
            var slug = ReflectionDynamicBlockDescriptor.ToNodeTypeKey(name);
            var workspace = new WorkspaceModel($"workspace-{slug}", name, $"{slug}.example", "Unassigned");
            var node = new SessionNodeInput(workspace.Id, descriptor.NodeTypeKey, workspace.Name, "DynamicNodeBlock");
            return Task.FromResult<DynamicEntity?>(new(
                node,
                typeof(WorkspaceModel),
                workspace,
                descriptor.Fields.ToDictionary(
                    field => field.Key,
                    field => descriptor.GetFieldValue(workspace, field.Key),
                    StringComparer.Ordinal)));
        }
    }

    private sealed class DeferredDynamicNodeRuntime : IDynamicNodeRuntime
    {
        private readonly TaskCompletionSource<DynamicEntity?> _load =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool LoadStarted { get; private set; }

        public bool LoadCompleted => _load.Task.IsCompleted;

        public Task<DynamicEntity?> LoadAsync(
            SessionNodeInput node,
            IDynamicBlockDescriptor descriptor,
            CancellationToken cancellationToken = default)
        {
            LoadStarted = true;
            cancellationToken.Register(() => _load.TrySetCanceled(cancellationToken));
            return _load.Task;
        }

        public Task<DynamicEntity?> ApplyAsync(
            DynamicMutationEffect mutation,
            IDynamicBlockDescriptor descriptor,
            CancellationToken cancellationToken = default)
            => Task.FromResult<DynamicEntity?>(null);

        public void Complete(WorkspaceUseCaseModel model)
        {
            var descriptor = new WorkspaceDynamicBlockRegistry().GetDescriptors().Single();
            _load.TrySetResult(new DynamicEntity(
                new SessionNodeInput("workspace-root", "workspace", "Foundry"),
                typeof(WorkspaceUseCaseModel),
                model,
                descriptor.Fields.ToDictionary(
                    field => field.Key,
                    field => descriptor.GetFieldValue(model, field.Key),
                    StringComparer.Ordinal)));
        }
    }
}
