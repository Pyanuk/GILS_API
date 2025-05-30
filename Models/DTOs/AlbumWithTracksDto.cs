using SoundPlayerAPI.Models.DTOs.TrackDTOs;

namespace SoundPlayerAPI.Models.DTOs
{
    public class AlbumWithTracksDto
    {
        public Guid AlbumId { get; set; }
        public string Title { get; set; }
        public string CoverFilePath { get; set; }
        public int TrackCount { get; set; }
        public string TotalDuration { get; set; }
        public string Type { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public List<string> Artists { get; set; } = new List<string>();

        public List<TrackDto> Tracks { get; set; }
    }

}
