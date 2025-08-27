using System.ComponentModel.DataAnnotations;

namespace RESTfullAPI.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    public DateTime CreatedOn { get; set; }
    
    [MaxLength(100)]
    public string? ModifiedBy { get; set; }
    
    public DateTime? ModifiedOn { get; set; }
    
    // Navigation property for related items
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

