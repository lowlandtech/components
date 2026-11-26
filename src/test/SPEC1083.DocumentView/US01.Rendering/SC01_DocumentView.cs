using LowlandTech.Components.Core.Documents;

namespace LowlandTech.Components.Tests.SPEC1083.DocumentView.US01.Rendering;

using DocumentViewSut = LowlandTech.Components.DocumentView;

[Trait(Spec.SPEC, "1083")]
[Trait(Spec.US, "01")]
[Trait(Spec.SC, "01")]
[UserStory("01", "A document renders from its definition")]
public class SC01_DocumentView : ViewScenario
{
    private static DocumentSnapshot Snapshot(
        IReadOnlyList<DocumentAction>? actions = null,
        IReadOnlyList<DocumentPropertyDefinition>? properties = null,
        IReadOnlyDictionary<string, string?>? values = null)
    {
        var declared = properties ??
        [
            new DocumentPropertyDefinition("headline", "Title", Required: true),
            new DocumentPropertyDefinition("detail", "Body"),
            new DocumentPropertyDefinition("record-id", "Identifier", ReadOnly: true)
        ];

        return new DocumentSnapshot(
            "document.record",
            "3f2a9c11-0000-4000-8000-000000000007",
            "Cold open",
            "sha256:abc",
            new DocumentDefinition("document.record", declared),
            values ?? new Dictionary<string, string?>
            {
                ["headline"] = "The tide turns",
                ["detail"] = "A quiet street",
                ["record-id"] = "doc-7",
            },
            Relations:
            [
                new DocumentRelation("parent", "Belongs to", "Episode 1", "outgoing"),
            ],
            Actions: actions ??
            [
                new DocumentAction("approve", "Approve", Primary: true),
                new DocumentAction("withdraw", "Withdraw"),
            ]);
    }

    [Fact(DisplayName = "UAC01: declared labels and values render and no key is shown")]
    [Trait(Spec.UAC, "01")]
    public void UAC01_LabelsAndValuesRender()
    {
        var cut = Ctx.Render<DocumentViewSut>(p => p.Add(x => x.Document, Snapshot()));

        cut.Find("[data-slot='document-view']").GetAttribute("data-document-type").ShouldBe("document.record");
        cut.Find("[data-slot='document-view']").GetAttribute("data-document-id").ShouldBe("3f2a9c11-0000-4000-8000-000000000007");
        var headline = cut.Find("[data-field='headline']");
        headline.TextContent.ShouldContain("Title");
        headline.TextContent.ShouldContain("The tide turns");
        headline.TextContent.ShouldNotContain("headline");
        cut.Find("[data-document-key]").TextContent.Trim().ShouldBe("Cold open");
        cut.Find("[data-document-head]").GetAttribute("data-head").ShouldBe("sha256:abc");
    }

    [Fact(DisplayName = "UAC02: relations render as read-only text")]
    [Trait(Spec.UAC, "02")]
    public void UAC02_RelationsAreReadOnlyText()
    {
        var cut = Ctx.Render<DocumentViewSut>(p => p
            .Add(x => x.Document, Snapshot())
            .Add(x => x.RelationsLabel, "Related"));

        var relation = cut.Find("[data-field='parent']");
        relation.TextContent.ShouldContain("Belongs to");
        relation.TextContent.ShouldContain("Episode 1");
        relation.TextContent.ShouldContain("outgoing");
        cut.FindAll("[data-field-input]").Count.ShouldBe(0);
        cut.Find("[data-document-relations-label]").TextContent.Trim().ShouldBe("Related");
    }

    [Fact(DisplayName = "UAC03: a required property is marked and a read-only one offers no edit")]
    [Trait(Spec.UAC, "03")]
    public void UAC03_RequiredAndReadOnly()
    {
        var cut = Ctx.Render<DocumentViewSut>(p => p.Add(x => x.Document, Snapshot()));

        cut.Find("[data-field-label='headline']").TextContent.ShouldContain("*");
        cut.Find("[data-field='record-id'] [data-field-readonly='record-id']").ShouldNotBeNull();
        cut.FindAll("[data-field='record-id'] input").Count.ShouldBe(0);
    }

    [Fact(DisplayName = "UAC04: declared actions render with their label and emphasis")]
    [Trait(Spec.UAC, "04")]
    public void UAC04_ActionsRender()
    {
        var cut = Ctx.Render<DocumentViewSut>(p => p.Add(x => x.Document, Snapshot()));

        cut.Find("[data-document-action='approve']").TextContent.Trim().ShouldBe("Approve");
        cut.Find("[data-document-action='approve']").GetAttribute("data-primary").ShouldBe("true");
        cut.Find("[data-document-action='withdraw']").GetAttribute("data-primary").ShouldBe("false");
    }

    [Fact(DisplayName = "UAC05: a mapped label wins; an unresolvable one shows no key")]
    [Trait(Spec.UAC, "05")]
    public void UAC05_LabelResolution()
    {
        var properties = new List<DocumentPropertyDefinition>
        {
            new("headline", "Title"),
            new("note", ""),
        };
        var snapshot = Snapshot(properties: properties, values: new Dictionary<string, string?>
        {
            ["headline"] = "The tide turns",
            ["note"] = "Only a value",
        });
        var map = new Dictionary<string, string> { ["headline"] = "Caption" };

        var cut = Ctx.Render<DocumentViewSut>(p => p
            .Add(x => x.Document, snapshot)
            .Add(x => x.LabelMap, map));

        cut.Find("[data-field='headline']").TextContent.ShouldContain("Caption");
        cut.Find("[data-field='headline']").TextContent.ShouldNotContain("Title");
        var note = cut.Find("[data-field='note']");
        note.TextContent.Trim().ShouldBe("Only a value");
        note.TextContent.ShouldNotContain("note");
    }

    [Fact(DisplayName = "UAC06: a document with no actions invites no edit")]
    [Trait(Spec.UAC, "06")]
    public void UAC06_NoActionsNoEdit()
    {
        var cut = Ctx.Render<DocumentViewSut>(p => p.Add(x => x.Document, Snapshot(actions: [])));

        cut.FindAll("[data-document-action]").Count.ShouldBe(0);
        cut.FindAll("[data-slot='data-form-actions']").Count.ShouldBe(0);
        cut.FindAll("form button[type='submit']").Count.ShouldBe(0);
        cut.FindAll("[data-field-input]").Count.ShouldBe(0);
    }
}
