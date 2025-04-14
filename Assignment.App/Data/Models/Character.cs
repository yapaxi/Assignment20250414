using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.LoaderConsole.Data.Models;

public class Character
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int? SourceId { get; set; }

    [MaxLength(128)]
    public required string Name { get; set; }

    [MaxLength(128)]
    public required string Status { get; set; }

    [MaxLength(128)]
    public string? Species { get; set; }

    [MaxLength(128)]
    public string? Type { get; set; }

    [MaxLength(128)]
    public string? Gender { get; set; }

    public int? OriginId { get; set; }

    [ForeignKey("OriginId")]
    public LocationInfo? Origin { get; set; }

    public int? LocationId { get; set; }

    [ForeignKey("LocationId")]
    public LocationInfo? Location { get; set; }

    public string? Image { get; set; }

    [MaxLength(256)]
    public string? Url { get; set; }

    public DateTime Created { get; set; }

    public ICollection<CharacterEpisode>? CharacterEpisodes { get; set; }
}
