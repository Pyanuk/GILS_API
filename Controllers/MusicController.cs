using Microsoft.AspNetCore.Mvc;
using SoundPlayerAPI.Models;
using Microsoft.EntityFrameworkCore;
using SoundPlayerAPI.Models.DTOs.TrackDTOs;
using System.IO;

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
    public async Task<ActionResult<IEnumerable<TrackDto>>> GetTracks(int page = 1, int pageSize = 9)
    {
        try
        {
            var trackEntities = await _context.Tracks
                .Include(t => t.Artists)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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


    private int? GetMp3DurationInSeconds(string fullPath)
    {
        try
        {
            // Формируем абсолютный путь для NAudio
            var path = fullPath;
            if (!Path.IsPathRooted(fullPath))
            {
                var relativePath = fullPath.TrimStart('/', '\\').Replace('\\', '/');
                const string mediaPrefix = "Media/";
                if (!string.IsNullOrEmpty(relativePath) && relativePath.StartsWith(mediaPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(mediaPrefix.Length);
                }
                path = Path.Combine(_mediaStoragePath, "Media", relativePath.Replace('/', Path.DirectorySeparatorChar));
            }

            using var reader = new NAudio.Wave.Mp3FileReader(path);
            return (int)reader.TotalTime.TotalSeconds;
        }
        catch
        {
            return null;
        }
    }
}
