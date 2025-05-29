namespace SoundPlayerAPI.Models.DTOs
{
    public class RegisterDto
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateOnly Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Cityid { get; set; }
        public int Countryid { get; set; }
    }
}
