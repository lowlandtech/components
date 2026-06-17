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

    [Fact(DisplayName = "UAC03: Dynamic node projection renders an editable dynamic form")]
    [Trait(Spec.UAC, "03")]
    [Given("a DynamicBlock rendered from a workspace model")]
    [When("the user projects a child node")]
    [Then("the projected context card uses DynamicNodeBlock and exposes field inputs")]
    public void UAC03_ProjectionRendersDynamicForm()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workitem-setup-ci']").Click();

        cut.Find("[data-slot='dynamic-node-block']").ShouldNotBeNull();
        cut.Find("[data-slot='dynamic-node-form']").ShouldNotBeNull();
        cut.Find("[data-field-input='Title']").GetAttribute("value").ShouldBe("Set up CI");
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

    [Fact(DisplayName = "UAC05: DynamicNodeBlock edits dispatch through the DynamicBlock outbox")]
    [Trait(Spec.UAC, "05")]
    [Given("a DynamicBlock where a projected node form is visible")]
    [When("the Title field is changed")]
    [Then("the DynamicBlock dispatcher records the change in dynamic state")]
    public void UAC05_FieldInputDispatchesToDynamicBlock()
    {
        var cut = Ctx.Render<Components.DynamicBlock>(p => p
            .Add(x => x.Model, CreateModel()));

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("[data-field-input='Title']").Input("Set up CI pipeline");

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
        cut.Find("[data-field-input='Slug']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Priority']").Count.ShouldBe(0);

        cut.Find("[data-node-id='flow-onboarding']").Click();
        cut.Find("[data-field-input='Trigger']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Slug']").Count.ShouldBe(0);

        cut.Find("[data-node-id='workitem-setup-ci']").Click();
        cut.Find("[data-field-input='Priority']").ShouldNotBeNull();
        cut.Find("[data-field-input='Assignee']").ShouldNotBeNull();
        cut.FindAll("[data-field-input='Trigger']").Count.ShouldBe(0);
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

    private sealed record WorkitemModel(string Id, string Title, string Priority, string Assignee);

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
            return Task.FromResult<DynamicEntity?>(null);
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
