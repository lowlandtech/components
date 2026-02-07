using Xunit;
using Bunit;
using LowlandTech.Components.Components.DataGrid;
using Shouldly;
using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Tests.SPEC1200.ComponentLibrary.US12.DataPresentation;

[Trait(Spec.SPEC, "1200")]
[Trait(Spec.US, "12")]
[Trait(Spec.SC, "41")]
public class SC41_DataGridServerSideTotalCount : Scenario
{
    private record Person(int Id, string Name);

    protected override Task GivenAsync() => Task.CompletedTask;
    protected override Task WhenAsync() => Task.CompletedTask;

    [Fact(DisplayName = "Then the pager should show correct page numbers")]
    [Trait(Spec.UAC, "01")]
    public async Task UAC01_PagerShowsCorrectPageNumbers()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        
        // Simulate server returning 2 items per page, but server has 10 total items
        var clientData = new List<Person>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, clientData)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.Count, 10) // Server total
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, args => { }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        var markup = cut.Markup;
        
        // Should show 5 pages (10 items / 2 per page)
        // Look for page number elements or text indicating total pages
        markup.ShouldContain("1"); // Current page
        
        // Verify total pages calculation: 10 items / 2 per page = 5 pages
        var pagerText = cut.Find(".data-grid-pager").TextContent;
        pagerText.ShouldContain("5"); // Should show total page count
    }

    [Fact(DisplayName = "And paging should work with server-side data")]
    [Trait(Spec.UAC, "02")]
    public async Task UAC02_PagingWorksWithServerSideData()
    {
        await ArrangeAndActAsync();

        using var ctx = new TestContext();
        var clientData = new List<Person>
        {
            new(1, "Alice"),
            new(2, "Bob")
        };
        
        var serverData = new List<Person>
        {
            new(1, "Alice"), new(2, "Bob"),
            new(3, "Charlie"), new(4, "David"),
            new(5, "Eve"), new(6, "Frank"),
            new(7, "Grace"), new(8, "Henry"),
            new(9, "Ivy"), new(10, "Jack")
        };

        var cut = ctx.Render<DataGrid<Person>>(parameters => parameters
            .Add(p => p.Data, clientData)
            .Add(p => p.AllowPaging, true)
            .Add(p => p.PageSize, 2)
            .Add(p => p.Count, serverData.Count) // Server total
            .Add(p => p.LoadData, EventCallback.Factory.Create<LoadDataArgs>(this, args =>
            {
                // Simulate server paging
                clientData.Clear();
                clientData.AddRange(serverData.Skip(args.Skip).Take(args.Top));
            }))
            .AddChildContent<DataGridColumn<Person>>(col => col
                .Add(p => p.Property, nameof(Person.Name))
                .Add(p => p.Title, "Name"))
        );

        // Initial state: should show first 2 items
        clientData.Count.ShouldBe(2);
        clientData[0].Name.ShouldBe("Alice");

        // Navigate to next page
        var nextButton = cut.Find("button:contains('Next')");
        await cut.InvokeAsync(() => nextButton.Click());

        // Should now show next 2 items from server
        clientData.Count.ShouldBe(2);
        clientData[0].Name.ShouldBe("Charlie");
        clientData[1].Name.ShouldBe("David");
    }
}
