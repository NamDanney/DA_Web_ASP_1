namespace DA_Web.Configurations
{
    public class FileUploadConfig
    {
        public long MaxFileSize { get; set; } = 5242880; // 5MB
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif" };
        public string UploadPath { get; set; } = "wwwroot/uploads/avatars";
    }
}