using System.Text.Json.Serialization;
using LMS___Mini_Version.Domain.Enums;

namespace LMS___Mini_Version.features.Orchestrators.ApproveCreateInternOrchestrator.ViewModels
{
    public record ApproveAndRegisterViewModel([property: JsonConverter(typeof(JsonStringEnumConverter))] PaymentMethod PaymentMethod);
}