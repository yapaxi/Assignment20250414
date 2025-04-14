using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.LoaderConsole.Data.Models;

public class Episode
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(256)]
    [Required]
    public string Name { get; set; } = null!;

    public int? SourceId { get; set; }

    [MaxLength(256)]
    public string? Url { get; set; }

    public DateTime? AirDate { get; set; }

    public DateTime Created { get; set; }
}
