using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.features.EnrollmentFeature.DTOs
{
    public record InternEnrollmentDTO(
     int EnrollmentId,
     int TrackId,
     string TrackName,
     DateTime EnrollmentDate,
     EnrollmentStatus EnrollmentStatus
        );
}
