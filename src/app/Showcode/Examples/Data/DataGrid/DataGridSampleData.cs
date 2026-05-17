namespace LowlandTech.Components.App.Showcode.Examples.Data;

public static class DataGridSampleData
{
    public static readonly IReadOnlyList<OrderRow> Orders =
    [
        new("ORD-1042", new CustomerInfo("Brightline Studios", "West"), "Paid", 18420m, new DateOnly(2026, 4, 21)),
        new("ORD-1043", new CustomerInfo("Northstar Labs", "North"), "Review", 9250m, new DateOnly(2026, 4, 22)),
        new("ORD-1044", new CustomerInfo("Harbor & Finch", "East"), "Shipped", 31200m, new DateOnly(2026, 4, 23)),
        new("ORD-1045", new CustomerInfo("Fieldstone Supply", "South"), "Paid", 7200m, new DateOnly(2026, 4, 24)),
        new("ORD-1046", new CustomerInfo("Morrow Works", "West"), "Review", 14780m, new DateOnly(2026, 4, 25)),
        new("ORD-1047", new CustomerInfo("Arcadia Goods", "East"), "Shipped", 22190m, new DateOnly(2026, 4, 26))
    ];

    public static string GetStatusVariant(string status)
    {
        return status switch
        {
            "Paid" => "default",
            "Shipped" => "secondary",
            "Review" => "outline",
            _ => "secondary"
        };
    }
}
