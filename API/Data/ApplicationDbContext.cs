using API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Artist> Artists { get; init; }
    public DbSet<Song> Songs { get; init; }
    public DbSet<Track> Tracks { get; init; }
    public DbSet<UserScore> UserScores { get; init; }
    public DbSet<SongOfTheDay> SongsOfTheDay { get; init; }
    public DbSet<UserGuess> UserGuesses { get; init; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Artist>()
            .HasMany(a => a.Songs)
            .WithOne(s => s.Artist)
            .HasForeignKey(s => s.ArtistId);

        builder.Entity<Song>()
            .HasMany(s => s.Tracks)
            .WithOne(t => t.Song)
            .HasForeignKey(t => t.SongId);

        builder.Entity<UserScore>()
            .HasOne(us => us.User)
            .WithMany(u => u.UserScores)
            .HasForeignKey(us => us.UserId)
            .IsRequired();

        builder.Entity<Track>()
            .HasOne(t => t.WaveformData)
            .WithOne(wf => wf.Track)
            .HasForeignKey<Track>(t => t.WaveformId);
    }
}