using System.ComponentModel.DataAnnotations;

namespace LowlandTech.Components.Tests.SPEC1072.DataForm.Models;

public enum CustomerStatus
{
    Active,
    Inactive,
    Pending
}

public class CustomerFormModel
{
    [Display(Name = "Customer name", Order = 10)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Email address", Order = 20)]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [Range(0, 10000)]
    public decimal CreditLimit { get; set; }

    public DateTime CreatedOn { get; set; }

    public CustomerStatus Status { get; set; }

    public string InternalCode { get; set; } = string.Empty;

    [StringLength(20)]
    public string AccountNumber { get; set; } = string.Empty;

    public bool IsBusiness { get; set; }

    public string? VatNumber { get; set; }

    public int Discount { get; set; }

    public string? TaxId { get; set; }

    public string? LegacyCode { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }

    public int Age { get; set; }

    public string? Password { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
