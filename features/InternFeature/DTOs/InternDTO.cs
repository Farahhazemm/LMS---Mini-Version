using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.features.InternFeature.DTOs
{
    public record InternDTO(int Id, string FullName, string Email, InternStatus Status, string? TrackName);
}
