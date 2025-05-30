namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class ConcertDto
    {
        public Guid ConcertId { get; set; }
        public string Title { get; set; } = null!;
        public string? Location { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
    }
}
