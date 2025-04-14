using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment.LoaderConsole.Data.Models;

[PrimaryKey(nameof(CharacterId), nameof(EpisodeId))]
public class CharacterEpisode
{
    public int CharacterId { get; set; }

    [MaxLength(256)]
    public int EpisodeId { get; set; }

    [ForeignKey("CharacterId")]
    public Character Character { get; set; } = null!;

    [ForeignKey("EpisodeId")]
    public Episode Episode { get; set; } = null!;
}
