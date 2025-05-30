namespace SoundPlayerAPI.Models.DTOs.ArtistDTOs
{
    public class AlbumDto
    {
        public Guid AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public string CoverPath { get; set; }
        public string Type { get; set; }
        public DateOnly? ReleaseDate { get; set; }
    }
}
