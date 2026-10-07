using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Customer
{
    public int CustomerId { get; set; }
    [Required, StringLength(100)] public string CustomerCode { get; set; } = string.Empty;
    [Required, StringLength(120)] public string CustomerName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(160)] public string Email { get; set; } = string.Empty;
    [Required, Phone, StringLength(15)] public string Phone { get; set; } = string.Empty;
    [StringLength(120)] public string CompanyName { get; set; } = string.Empty;
    [StringLength(250)] public string Address { get; set; } = string.Empty;
    [StringLength(80)] public string City { get; set; } = string.Empty;
    [StringLength(80)] public string State { get; set; } = string.Empty;
    [Required] public string Status { get; set; } = "Active";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}
