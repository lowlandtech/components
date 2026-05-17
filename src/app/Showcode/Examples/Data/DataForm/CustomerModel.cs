namespace LowlandTech.Components.App.Showcode.Examples.Data;

public class CustomerModel
{
    [System.ComponentModel.DataAnnotations.Display(Name = "Customer name", Order = 10)]
    public string Name { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Display(Name = "Email address", Order = 20)]
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [System.ComponentModel.DataAnnotations.Range(0, 10000)]
    public decimal CreditLimit { get; set; }

    public CustomerStatus Status { get; set; }

    public int Age { get; set; }
}
