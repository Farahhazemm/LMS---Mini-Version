namespace LMS___Mini_Version.features.TrackFeature.CheckAvailability.Dtos
{
    public record TrackAvailabilityDto(
       string TrackName,
       bool IsActive,
       int MaxCapacity,
       int ReservedSeats,
       int AvailableSeats);
}
