using LMS___Mini_Version.features.EnrollmentFeature.Commands;
using LMS___Mini_Version.features.EnrollmentFeature.DTOs;
using LMS___Mini_Version.features.EnrollmentFeature.Queries;
using LMS___Mini_Version.features.EnrollmentFeature.ViewModels;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController(IMediator mediator) : ControllerBase
    {
    
        [HttpGet("intern/{internId:int}")]
        public async Task<ActionResult<EndpointResponse<IEnumerable<InternEnrollmentViewModel>>>> GetByInternId(int internId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetEnrollmentsByInternIdQuery(internId), ct);
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get intern enrollments", result.Errors));

            var enrollmentsVM = result.Data!.Select(x => new InternEnrollmentViewModel(
                x.EnrollmentId,
                x.TrackId,
                x.TrackName,
                x.EnrollmentDate,
                x.EnrollmentStatus));

            return StatusCode(result.StatusCode,
                EndpointResponse<IEnumerable<InternEnrollmentViewModel>>.OK(enrollmentsVM, result.StatusCode, "Success"));
        }

     
        [HttpPost("{id:int}/complete")]
        public async Task<ActionResult<EndpointResponse>> Complete(int id, CancellationToken ct)
        {
            var result = await mediator.Send(new CompleteEnrollmentCommand(id), ct);
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to complete enrollment", result.Errors));

            return StatusCode(result.StatusCode,
                EndpointResponse.OK(result.StatusCode, "Enrollment completed successfully"));
        }

        [HttpGet("track/{trackId:int}")]
        public async Task<ActionResult<EndpointResponse<IEnumerable<TrackEnrollmentViewModel>>>> GetByTrackId(int trackId, CancellationToken ct)
        {
            var result = await mediator.Send(new GetEnrollmentsByTrackIdQuery(trackId), ct);
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get track enrollments", result.Errors));

            var enrollmentsVM = result.Data!.Select(x => new TrackEnrollmentViewModel(
                x.EnrollmentId,
                x.InternFullName,
                x.EnrollmentDate,
                x.EnrollmentStatus,
                x.PaymentAmount,
                x.PaymentStatus));

            return StatusCode(result.StatusCode,
                EndpointResponse<IEnumerable<TrackEnrollmentViewModel>>.OK(enrollmentsVM, result.StatusCode, "Success"));
        }
    }
}
