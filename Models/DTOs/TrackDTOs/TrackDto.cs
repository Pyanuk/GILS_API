namespace SoundPlayerAPI.Models.DTOs.TrackDTOs
{
    public class TrackDto
    {
        public Guid TrackId { get; set; }
        public string Title { get; set; }
        public List<string> Artists { get; set; }
        public string TrackFilePath { get; set; }
        public string CoverFilePath { get; set; }
        public int? Duration { get; set; }
    }

}
