using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Lead
{
    public int LeadId { get; set; }
    [Required, StringLength(100)] public string LeadCode { get; set; } = string.Empty;
    [Required, StringLength(120)] public string LeadName { get; set; } = string.Empty;
    [Required, EmailAddress, StringLength(160)] public string Email { get; set; } = string.Empty;
    [Required, Phone, StringLength(15)] public string Phone { get; set; } = string.Empty;
    [StringLength(120)] public string CompanyName { get; set; } = string.Empty;
    [Required] public string Source { get; set; } = "Website";
    [Required] public string Status { get; set; } = "New";
    [Required] public string Priority { get; set; } = "Medium";
    [Range(0, 1000000000)] public decimal ExpectedValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string AssignedTo { get; set; } = string.Empty;
}
