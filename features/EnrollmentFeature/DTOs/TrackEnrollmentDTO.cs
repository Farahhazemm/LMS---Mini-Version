using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.features.EnrollmentFeature.DTOs
{
    public record TrackEnrollmentDTO(
        int EnrollmentId,
        string InternFullName,
        DateTime EnrollmentDate,
        EnrollmentStatus EnrollmentStatus,
        decimal? PaymentAmount,
        PaymentStatus? PaymentStatus
        );
}