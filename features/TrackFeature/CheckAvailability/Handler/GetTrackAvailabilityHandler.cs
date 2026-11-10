using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.shared;
using LMS___Mini_Version.features.TrackFeature.CheckAvailability.Dtos;
using LMS___Mini_Version.features.TrackFeature.CheckAvailability.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.TrackFeature.CheckAvailability.Handler
{
    public class GetTrackAvailabilityHandler(IGeneralRepository<Track> trackRepo)
        : IRequestHandler<GetTrackAvailabilityQuery, RequestResponse<TrackAvailabilityDto>>
    {
        public async Task<RequestResponse<TrackAvailabilityDto>> Handle(GetTrackAvailabilityQuery request, CancellationToken ct)
        {
            var track = await trackRepo.GetAll()
                .AsNoTracking()
                .Where(t => t.Id == request.TrackId)
                .Select(t => new
                {
                    t.Name,
                    t.IsActive,
                    t.MaxCapacity,
                    Reserved = t.Enrollments.Count(e =>
                    e.Status == EnrollmentStatus.Pending ||
                        e.Status == EnrollmentStatus.Active)
                }) .FirstOrDefaultAsync(ct);

            if (track is null)
            {
                return RequestResponse<TrackAvailabilityDto>.Fail(404,

                    new Dictionary<string, string[]> { ["Id"] = new[] { $"Track {request.TrackId} was not found." }});
            }
            var available = Math.Max(0, track.MaxCapacity - track.Reserved);

            return RequestResponse<TrackAvailabilityDto>.OK(
                new TrackAvailabilityDto(track.Name, track.IsActive, track.MaxCapacity, track.Reserved, available));
        }
    }
}