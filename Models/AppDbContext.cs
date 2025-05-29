using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SoundPlayerAPI.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<RecentlyPlayed> RecentlyPlayed { get; set; }  // <-- вот это строка

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Concert> Concerts { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<FavoriteTrack> FavoriteTracks { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistTrack> PlaylistTracks { get; set; }

    public virtual DbSet<RecentlyPlayed> RecentlyPlayeds { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userauthprovider> Userauthproviders { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=77.95.203.81;Port=5432;Database=soundplayer;Username=super;Password=super");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("albums_pkey");

            entity.ToTable("albums");

            entity.Property(e => e.AlbumId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("album_id");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("artists_pkey");

            entity.ToTable("artists");

            entity.Property(e => e.ArtistId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("artist_id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PhotoPath).HasColumnName("photo_path");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasMany(d => d.Albums).WithMany(p => p.Artists)
                .UsingEntity<Dictionary<string, object>>(
                    "ArtistAlbum",
                    r => r.HasOne<Album>().WithMany()
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artist_album_album_id_fkey"),
                    l => l.HasOne<Artist>().WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artist_album_artist_id_fkey"),
                    j =>
                    {
                        j.HasKey("ArtistId", "AlbumId").HasName("artist_album_pkey");
                        j.ToTable("artist_album");
                        j.IndexerProperty<Guid>("ArtistId").HasColumnName("artist_id");
                        j.IndexerProperty<Guid>("AlbumId").HasColumnName("album_id");
                    });

            entity.HasMany(d => d.Concerts).WithMany(p => p.Artists)
                .UsingEntity<Dictionary<string, object>>(
                    "ArtistConcert",
                    r => r.HasOne<Concert>().WithMany()
                        .HasForeignKey("ConcertId")
                        .HasConstraintName("artist_concerts_concert_id_fkey"),
                    l => l.HasOne<Artist>().WithMany()
                        .HasForeignKey("ArtistId")
                        .HasConstraintName("artist_concerts_artist_id_fkey"),
                    j =>
                    {
                        j.HasKey("ArtistId", "ConcertId").HasName("artist_concerts_pkey");
                        j.ToTable("artist_concerts");
                        j.IndexerProperty<Guid>("ArtistId").HasColumnName("artist_id");
                        j.IndexerProperty<Guid>("ConcertId").HasColumnName("concert_id");
                    });

            entity.HasMany(d => d.Tracks).WithMany(p => p.Artists)
                .UsingEntity<Dictionary<string, object>>(
                    "ArtistTrack",
                    r => r.HasOne<Track>().WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artist_track_track_id_fkey"),
                    l => l.HasOne<Artist>().WithMany()
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("artist_track_artist_id_fkey"),
                    j =>
                    {
                        j.HasKey("ArtistId", "TrackId").HasName("artist_track_pkey");
                        j.ToTable("artist_track");
                        j.HasIndex(new[] { "TrackId" }, "idx_artist_track_track_id");
                        j.IndexerProperty<Guid>("ArtistId").HasColumnName("artist_id");
                        j.IndexerProperty<Guid>("TrackId").HasColumnName("track_id");
                    });
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.IdCity).HasName("cities_pkey");

            entity.ToTable("cities");

            entity.Property(e => e.IdCity).HasColumnName("id_city");
            entity.Property(e => e.Cityname)
                .HasMaxLength(100)
                .HasColumnName("cityname");
            entity.Property(e => e.Countryid).HasColumnName("countryid");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cities_countryid_fkey");
        });

        modelBuilder.Entity<Concert>(entity =>
        {
            entity.HasKey(e => e.ConcertId).HasName("concerts_pkey");

            entity.ToTable("concerts");

            entity.Property(e => e.ConcertId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("concert_id");
            entity.Property(e => e.Date)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry).HasName("countries_pkey");

            entity.ToTable("countries");

            entity.Property(e => e.IdCountry).HasColumnName("id_country");
            entity.Property(e => e.Countryname)
                .HasMaxLength(100)
                .HasColumnName("countryname");
        });

        modelBuilder.Entity<FavoriteTrack>(entity =>
        {
            entity.HasKey(e => e.IdFavorite); // IdFavorite как первичный ключ
            entity.ToTable("favorite_tracks");

            entity.Property(e => e.IdFavorite).HasColumnName("id_favorite").ValueGeneratedOnAdd(); // Автоинкремент (SERIAL)
            entity.Property(e => e.TrackId).HasColumnName("track_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Track)
                .WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_tracks_track_id_fkey");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_tracks_user_id_fkey");

            entity.HasIndex(e => new { e.UserId, e.TrackId }).IsUnique();
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("playlists_pkey");

            entity.ToTable("playlists");

            entity.Property(e => e.PlaylistId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("playlist_id");
            entity.Property(e => e.CoverPath)
                .HasMaxLength(255)
                .HasColumnName("cover_path");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TotalDuration)
                .HasDefaultValueSql("'00:00:00'::interval")
                .HasColumnName("total_duration");
            entity.Property(e => e.TotalTracks)
                .HasDefaultValue(0)
                .HasColumnName("total_tracks");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("playlists_created_by_fkey");
        });

        modelBuilder.Entity<PlaylistTrack>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("playlist_tracks");

            entity.Property(e => e.PlaylistId).HasColumnName("playlist_id");
            entity.Property(e => e.TrackId).HasColumnName("track_id");

            entity.HasOne(d => d.Playlist).WithMany()
                .HasForeignKey(d => d.PlaylistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playlist_tracks_playlist_id_fkey");

            entity.HasOne(d => d.Track).WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playlist_tracks_track_id_fkey");
        });

        modelBuilder.Entity<RecentlyPlayed>(entity =>
        {
            entity.HasKey(e => e.RecentlyId).HasName("recently_played_pkey");

            entity.ToTable("recently_played");

            entity.Property(e => e.RecentlyId)
                .HasColumnName("recently_id") // Проверь точное имя столбца в БД
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TrackId).HasColumnName("track_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Track).WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recently_played_track_id_fkey");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("recently_played_user_id_fkey");
        });


        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasKey(e => e.TrackId).HasName("tracks_pkey");

            entity.ToTable("tracks");

            entity.HasIndex(e => e.AlbumId, "idx_tracks_album_id");

            entity.Property(e => e.TrackId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("track_id");
            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.CoverPath)
                .HasMaxLength(255)
                .HasColumnName("cover_path");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.Lyrics).HasColumnName("lyrics");
            entity.Property(e => e.Produsser).HasMaxLength(255);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Album).WithMany(p => p.Tracks)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("tracks_album_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.PasswordUsers)
                .HasMaxLength(256)
                .HasColumnName("password_users");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Roleid)
                .HasDefaultValue(1)
                .HasColumnName("roleid");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.Cityid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_cityid_fkey");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_countryid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roleid_fkey");
        });

        modelBuilder.Entity<Userauthprovider>(entity =>
        {
            entity.HasKey(e => e.IdUserauthproviders).HasName("userauthproviders_pkey");

            entity.ToTable("userauthproviders");

            entity.Property(e => e.IdUserauthproviders).HasColumnName("id_userauthproviders");
            entity.Property(e => e.Providerid)
                .HasMaxLength(100)
                .HasColumnName("providerid");
            entity.Property(e => e.Providername)
                .HasMaxLength(50)
                .HasColumnName("providername");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Userauthproviders)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userauthproviders_userid_fkey");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("videos_pkey");

            entity.ToTable("videos");

            entity.Property(e => e.VideoId)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("video_id");
            entity.Property(e => e.CoverPath).HasColumnName("cover_path");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.TrackId).HasColumnName("track_id");
            entity.Property(e => e.VideoType)
                .HasMaxLength(10)
                .HasDefaultValueSql("'clip'::character varying")
                .HasColumnName("video_type");

            entity.HasOne(d => d.Track).WithMany(p => p.Videos)
                .HasForeignKey(d => d.TrackId)
                .HasConstraintName("videos_track_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
