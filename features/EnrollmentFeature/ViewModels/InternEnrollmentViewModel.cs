using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.features.EnrollmentFeature.ViewModels
{
    public record InternEnrollmentViewModel(
        int EnrollmentId,
        int TrackId,
        string TrackName,
        DateTime EnrollmentDate,
        EnrollmentStatus EnrollmentStatus);
}
