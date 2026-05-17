namespace LowlandTech.Components.App.Showcode.Examples.Data;

public sealed record OrderRow(string OrderNumber, CustomerInfo Customer, string Status, decimal Total, DateOnly DueDate);
