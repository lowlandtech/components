namespace LowlandTech.Components.App.Showcode.Examples.Data;

public class CustomerModel
{
    [Display(Name = "Customer name", Order = 10)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Email address", Order = 20)]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [Range(0, 10000)]
    public decimal CreditLimit { get; set; }

    public CustomerStatus Status { get; set; }

    public int Age { get; set; }
}
