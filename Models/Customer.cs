using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [StringLength(100)]
    public string CustomerCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer name is required.")]
    [StringLength(120, ErrorMessage = "Customer name cannot exceed 120 characters.")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(120)]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [StringLength(80)]
    public string City { get; set; } = string.Empty;

    [StringLength(80)]
    public string State { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = "Active";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string CreatedBy { get; set; } = string.Empty;
}