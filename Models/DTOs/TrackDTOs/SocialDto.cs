namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class SocialDto
    {
        public string? Platform { get; set; } = null!;

        public string? Url { get; set; } = null!;

        public string? Info { get; set; }

        public string? Title { get; set; }
    }
}
