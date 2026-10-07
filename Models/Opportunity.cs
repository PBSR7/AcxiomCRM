using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Opportunity
{
    public int OpportunityId { get; set; }
    [Required, StringLength(160)] public string OpportunityName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? LeadId { get; set; }
    [Range(typeof(decimal), "0.01", "100000000000")] public decimal Amount { get; set; }
    [Required] public string Stage { get; set; } = "Qualification";
    [Range(0, 100)] public int Probability { get; set; }
    [DataType(DataType.Date)] public DateTime ExpectedCloseDate { get; set; } = DateTime.UtcNow.Date.AddDays(30);
    [Required] public string Status { get; set; } = "Open";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string AssignedTo { get; set; } = string.Empty;
    [StringLength(500)] public string Notes { get; set; } = string.Empty;
    public decimal WeightedValue => Amount * Probability / 100m;
}
