using LMS___Mini_Version.features.shared;
using LMS___Mini_Version.features.TrackFeature.CheckAvailability.Dtos;
using MediatR;

namespace LMS___Mini_Version.features.TrackFeature.CheckAvailability.Queries
{
    public record GetTrackAvailabilityQuery(int TrackId) : IRequest<RequestResponse<TrackAvailabilityDto>>;
}