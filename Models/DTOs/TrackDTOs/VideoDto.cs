namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class VideoDto
    {
        public Guid VideoId { get; set; }
        public string? Title { get; set; }
        public string FilePath { get; set; } = null!;
        public string? CoverPath { get; set; }
        public string? VideoType { get; set; }  // "clip" or "sample"
    }
}
