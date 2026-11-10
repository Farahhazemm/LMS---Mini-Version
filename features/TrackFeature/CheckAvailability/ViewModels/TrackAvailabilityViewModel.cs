namespace LMS___Mini_Version.features.TrackFeature.CheckAvailability.ViewModels
{
    public record TrackAvailabilityViewModel(
        string TrackName,
        bool IsActive,
        int MaxCapacity,
        int ReservedSeats,
        int AvailableSeats);
}
