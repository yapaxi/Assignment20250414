using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.LoaderConsole.Data.Models;

public class LocationInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? SourceId { get; set; }

    [MaxLength(256)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public string? Url { get; set; }

    [MaxLength(64)]
    public string? Type { get; set; }

    [MaxLength(256)]
    public string? Dimension { get; set; }

    public DateTime Created { get; set; }
}
