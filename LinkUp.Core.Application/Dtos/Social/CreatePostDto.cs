namespace LinkUp.Core.Application.Dtos.Social
{
    public class CreatePostDto
    {
        public string Content { get; set; } = string.Empty;
        public string MediaKind { get; set; } = string.Empty; // image | youtube
        public string? YouTubeUrl { get; set; }
        public string? SavedImagePath { get; set; } // ruta relativa guardada en capa web
    }
}
