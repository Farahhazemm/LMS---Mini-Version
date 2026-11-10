using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.EnrollmentFeature.DTOs;
using LMS___Mini_Version.features.EnrollmentFeature.Queries;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.Handlers
{
    public class GetEnrollmentsByTrackIdHandler(IGeneralRepository<Track> trackRepo, IGeneralRepository<Enrollment> enrollmentRepo)
        : IRequestHandler<GetEnrollmentsByTrackIdQuery, RequestResponse<IEnumerable<TrackEnrollmentDTO>>>
    {
        public async Task<RequestResponse<IEnumerable<TrackEnrollmentDTO>>> Handle(GetEnrollmentsByTrackIdQuery request, CancellationToken ct)
        {
            var trackExists = await trackRepo.GetAll().AnyAsync(t => t.Id == request.TrackId, ct);

            if (!trackExists)
            {
                return RequestResponse<IEnumerable<TrackEnrollmentDTO>>.Fail(404,
                    new Dictionary<string, string[]> { ["TrackId"] = new[] { $"Track {request.TrackId} was not found." } });
            }

            var enrollments = await enrollmentRepo.GetAll()
                .AsNoTracking()
                .Where(e => e.TrackId == request.TrackId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ThenByDescending(e => e.Id)
                .Select(e => new TrackEnrollmentDTO(
                    e.Id,
                    e.Intern.FullName,
                    e.EnrollmentDate,
                    e.Status,
                    e.Payment != null ? (decimal?)e.Payment.Amount : null,
                    e.Payment != null ? (PaymentStatus?)e.Payment.Status : null))
                .ToListAsync(ct);

            return RequestResponse<IEnumerable<TrackEnrollmentDTO>>.OK(enrollments);
        }
    }
}