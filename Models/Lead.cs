using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Lead
{
    public int LeadId { get; set; }

    [StringLength(100)]
    public string LeadCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lead name is required.")]
    [StringLength(120, ErrorMessage = "Lead name cannot exceed 120 characters.")]
    public string LeadName { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "Source is required.")]
    public string Source { get; set; } = "Website";

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = "New";

    [Required(ErrorMessage = "Priority is required.")]
    public string Priority { get; set; } = "Medium";

    [Range(0, 1000000000, ErrorMessage = "Expected value must be between 0 and 1,000,000,000.")]
    public decimal ExpectedValue { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string AssignedTo { get; set; } = string.Empty;
}