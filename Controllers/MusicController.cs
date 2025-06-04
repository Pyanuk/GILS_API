using Microsoft.AspNetCore.Mvc;
using SoundPlayerAPI.Models;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models.DTOs.TrackDTOs;
using SoundPlayerAPI.Models.DTOs.ArtistDTOs;
using SoundPlayerAPI.Models.DTOs;

[Route("api/[controller]")]
[ApiController]
public class MusicController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _mediaStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    public MusicController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks()
    {
        try
        {
            var trackEntities = await _context.Tracks
                .Include(t => t.Artists)
                .ToListAsync();

            var trackDtos = trackEntities.Select(track => new TrackDto
            {
                TrackId = track.TrackId,
                Title = track.Title,
                TrackFilePath = track.FilePath,
                Artists = track.Artists.Select(a => a.Name).ToList(),
                CoverFilePath = track.CoverPath,
                Duration = GetMp3DurationInSeconds(track.FilePath)
            }).ToList();

            return Ok(trackDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Server error", message = ex.Message });
        }
    }

    [HttpGet("{trackId}")]
    public async Task<IActionResult> GetTrack(Guid trackId)
    {
        try
        {
            var track = await _context.Tracks.Include(t => t.Artists).FirstOrDefaultAsync(t => t.TrackId == trackId);

            if (track == null)
                return NotFound(new { error = "Track not found" });

            var relativePath = track.FilePath?.TrimStart('/', '\\').Replace('\\', '/');
            const string mediaPrefix = "Media/";
            if (!string.IsNullOrEmpty(relativePath) && relativePath.StartsWith(mediaPrefix, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Substring(mediaPrefix.Length);
            }

            var fullFilePath = Path.Combine(_mediaStoragePath, "Media", relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(fullFilePath))
                return NotFound(new { error = $"File not found: {fullFilePath}" });

            var memoryStream = new MemoryStream();
            using (var fileStream = System.IO.File.OpenRead(fullFilePath))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, "audio/mpeg")
            {
                EnableRangeProcessing = true
            };
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Server error", message = ex.Message });
        }
    }
    [HttpGet("sidebar/{trackId}")]
    public async Task<ActionResult<TrackSidebarInfoDto>> GetTrackSidebarInfo(Guid trackId)
    {
        try
        {
            var track = await _context.Tracks
                .Include(t => t.Artists)
                    .ThenInclude(a => a.Concerts)
                .Include(t => t.Videos)
                .FirstOrDefaultAsync(t => t.TrackId == trackId);

            if (track == null)
                return NotFound(new { error = "Track not found" });

            var trackDto = new TrackSidebarInfoDto
            {
                TrackId = track.TrackId,
                Title = track.Title,
                Duration = track.Duration,
                Lyrics = track.Lyrics,
                CoverPath = track.CoverPath,
                Produsser = track.Produsser,
                LrcLyrics = track.LrcLyrics,
                Artists = track.Artists.Select(artist => new ArtistDto
                {
                    ArtistId = artist.ArtistId,
                    Name = artist.Name,
                    Bio = artist.Bio,
                    Concerts = artist.Concerts.Select(concert => new ConcertDto
                    {
                        ConcertId = concert.ConcertId,
                        Title = concert.Title,
                        Location = concert.Location,
                        Date = concert.Date,
                        Description = concert.Description
                    }).ToList(),
                    Photo = artist.PhotoPath
                }).ToList()
            };

            foreach (var video in track.Videos)
            {
                var artistName = track.Artists.FirstOrDefault()?.Name?.ToLower().Replace(" ", "") ?? "unknown";

                var videoDto = new VideoDto
                {
                    VideoId = video.VideoId,
                    Title = video.Title,
                    FilePath = video.VideoType?.ToLower() == "clip"
                        ? $"/Media/Clips/Videos/{artistName}/{System.IO.Path.GetFileName(video.FilePath)}"
                        : $"/Media/Samples/{artistName}/{System.IO.Path.GetFileName(video.FilePath)}",
                    CoverPath = video.VideoType?.ToLower() == "clip"
                        ? $"/Media/Clips/Cover/{artistName}/{System.IO.Path.GetFileName(video.CoverPath)}"
                        : null,
                    VideoType = video.VideoType
                };

                if (video.VideoType?.ToLower() == "clip")
                    trackDto.Clips.Add(videoDto);
                else if (video.VideoType?.ToLower() == "sample")
                    trackDto.Samples.Add(videoDto);
            }

            return Ok(trackDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Server error", message = ex.Message });
        }
    }


    [HttpGet("infoartist/{name}")]
    public async Task<ActionResult<ArtistDetailsDto>> GetArtistByName(string name)
    {
        var artist = await _context.Artists
            .Include(a => a.Albums)
            .Include(a => a.Tracks)
                .ThenInclude(t => t.Videos)
            .Include(a => a.Tracks)
                .ThenInclude(t => t.Artists)
                .Include(a => a.ArtistSocials)
            .Include(a => a.ArtistPhotos)
            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

        if (artist == null)
            return NotFound($"Artist with name '{name}' not found.");

        var dto = new ArtistDetailsDto
        {
            ArtistId = artist.ArtistId,
            Name = artist.Name,
            Bio = artist.Bio,
            Photos = artist.ArtistPhotos.Select(p => p.PhotoPath).ToList(),
            Photo = artist.PhotoPath ?? artist.ArtistPhotos.FirstOrDefault(p => p.IsPrimary == true)?.PhotoPath,

            Albums = artist.Albums.Select(al => new AlbumDto
            {
                AlbumId = al.AlbumId,
                Title = al.Title,
                CoverPath = al.CoverPath,
                Type = al.Type,
                ReleaseDate = al.ReleaseDate
            }).ToList(),

            Tracks = artist.Tracks.Select(t => new TrackDto
            {
                TrackId = t.TrackId,
                Title = t.Title,
                Duration = t.Duration ?? GetMp3DurationInSeconds(t.FilePath),
                TrackFilePath = t.FilePath ?? string.Empty,
                CoverFilePath = t.CoverPath ?? string.Empty,
                Artists = t.Artists.Select(a => a.Name).ToList()
            }).ToList(),

            Videos = artist.Tracks
                .SelectMany(t => t.Videos)
                .Select(v => new VideoDto
                {
                    VideoId = v.VideoId,
                    Title = v.Title,
                    FilePath = v.FilePath ?? string.Empty,
                    CoverPath = v.CoverPath,
                    VideoType = v.VideoType
                }).ToList(),
            Type = artist.TypeArtist,
            Social = artist.ArtistSocials.Select(i => new SocialDto
            {
                Platform = i.Platform,
                Url = i.Url,
                Info = i.Info,
                Title = i.Title
            }).ToList(),
        };

        return Ok(dto);
    }
    [HttpGet("by-album/{albumId}")]
    public async Task<ActionResult<AlbumWithTracksDto>> GetTracksByAlbum(Guid albumId)
    {
        try
        {
            var album = await _context.Albums
                .Include(a => a.Tracks)
                    .ThenInclude(t => t.Artists)
                .FirstOrDefaultAsync(a => a.AlbumId == albumId);

            if (album == null)
                return NotFound(new { error = "Album not found" });

            var trackDtos = album.Tracks.Select(track => new TrackDto
            {
                TrackId = track.TrackId,
                Title = track.Title,
                TrackFilePath = track.FilePath,
                Artists = track.Artists.Select(a => a.Name).ToList(),
                CoverFilePath = track.CoverPath,
                Duration = GetMp3DurationInSeconds(track.FilePath)
            }).ToList();

            // Собираем всех уникальных артистов из треков альбома
            var allArtists = album.Tracks
                .SelectMany(t => t.Artists.Select(a => a.Name))
                .Distinct()
                .ToList();

            var totalDurationSeconds = trackDtos.Sum(t => (t.Duration ?? 0));
            var ts = TimeSpan.FromSeconds(totalDurationSeconds);
            var totalDurationFormatted = ts.Hours > 0 ? ts.ToString(@"h\:mm\:ss") : ts.ToString(@"mm\:ss");


            var result = new AlbumWithTracksDto
            {
                AlbumId = album.AlbumId,
                Title = album.Title,
                CoverFilePath = GetAbsolutePath(album.CoverPath),
                TrackCount = trackDtos.Count,
                TotalDuration = totalDurationFormatted,
                Tracks = trackDtos,
                Type = album.Type,
                Artists = allArtists,
                ReleaseDate = album.ReleaseDate
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Server error", message = ex.Message });
        }
    }





    private int? GetMp3DurationInSeconds(string fullPath)
    {
        try
        {
            var path = GetAbsolutePath(fullPath);

            var duration = TagLib.File.Create(path).Properties.Duration;
            return (int)duration.TotalSeconds;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading duration: {ex.Message}");
            return null;
        }
    }


    private string GetAbsolutePath(string fullPath)
    {
        // Убираем ведущий слеш, если есть
        var relativePath = fullPath.TrimStart('/', '\\');

        // Объединяем с корнем
        var absolutePath = Path.Combine(_mediaStoragePath, relativePath.Replace('/', Path.DirectorySeparatorChar));

        return absolutePath;
    }



}
