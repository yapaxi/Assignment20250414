using Assignment.LoaderConsole;
using Assignment.LoaderConsole.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Assignment.App.Data;

public interface IRepository
{
    IQueryable<Character> CharactersQuery { get; }

    Task<Character> Add(Character character);

    Task SaveChanges();
}

public class Repository(Context Context, StatisticsContext statisticsContext) : IRepository
{
    public IQueryable<Character> CharactersQuery => Context.Characters.Include(q => q.CharacterEpisodes).ThenInclude(q => q.Episode)
                                                                      .Include(q => q.Origin)
                                                                      .Include(q => q.Location)
                                                                      .AsNoTracking();

    public async Task SaveChanges()
    {
        if (await Context.SaveChangesAsync() is > 0)
        {
            statisticsContext.RegisterDbCall();
        }
    }

    public async Task<Character> Add(Character character)
    {
        return (await Context.Characters.AddAsync(character)).Entity;
    }
}

public class Context() : DbContext
{
    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<LocationInfo> LocationInfos { get; set; } = null!;
    public DbSet<CharacterEpisode> CharacterEpisodes { get; set; } = null!;
    public DbSet<Episode> Episodes { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var path = Path.Combine(Path.GetTempPath(), "rickandmorty_assignment.db");
        optionsBuilder.UseSqlite($"Data Source={path}.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Episode>().HasIndex(q => q.Url).IsUnique();
        modelBuilder.Entity<LocationInfo>().HasIndex(q => q.Url).IsUnique();
        modelBuilder.Entity<Character>().HasIndex(q => q.Url).IsUnique();


        modelBuilder.Entity<LocationInfo>().HasIndex(q => q.SourceId).IsUnique();
        modelBuilder.Entity<LocationInfo>().HasIndex(q => new { q.Type, q.Name });

        modelBuilder.Entity<Character>().HasIndex(q => q.SourceId).IsUnique();
        modelBuilder.Entity<CharacterEpisode>().HasIndex(q => q.EpisodeId);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().UseCollation("NOCASE");
    }
}
