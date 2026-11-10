
using LMS___Mini_Version.features.InternFeature.Commands;
using LMS___Mini_Version.features.InternFeature.DTOs;
using LMS___Mini_Version.features.InternFeature.Queries;
using LMS___Mini_Version.features.InternFeature.ViewModels;
using LMS___Mini_Version.features.Orchestrators.ApproveCreateInternOrchestrator.Commands;
using LMS___Mini_Version.features.Orchestrators.ApproveCreateInternOrchestrator.ViewModels;
using LMS___Mini_Version.features.shared;
using LMS___Mini_Version.features.TrackFeature.GetAllTracks.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS___Mini_Version.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class InternController(IMediator mediator) : ControllerBase
    {


        [HttpGet]
        public async Task<ActionResult<EndpointResponse<IEnumerable<InternDTO>>>> GetAll(CancellationToken ct)
        {
            var result = await mediator.Send(new GetAllInternsQuery(), ct);
            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to get Interns", result.Errors));

            return StatusCode(result.StatusCode,
                EndpointResponse<IEnumerable<InternDTO>>.OK(result.Data!, result.StatusCode, "Success"));
        }


        [HttpPost("{internId:int}/approve-and-register")]
        public async Task<ActionResult<EndpointResponse<int>>> ApproveAndRegister( int internId, [FromBody] ApproveAndRegisterViewModel vm,CancellationToken ct)
        {
            var result = await mediator.Send( new ApproveCreateInternOrchestratorCommand(internId, vm.PaymentMethod), ct);

            if (!result.Success)
             return StatusCode(result.StatusCode, EndpointResponse.Fail(result.StatusCode, "Failed to approve and register intern", result.Errors));

            return StatusCode(result.StatusCode, EndpointResponse<int>.OK(result.Data, result.StatusCode, "Intern approved and registered successfully"));
        }


        //[HttpGet("{id}")]
        //public async Task<ActionResult<>> GetById(int id)
        //{

        //}
        [HttpPost]
        public async Task<ActionResult<EndpointResponse>> Create([FromBody] InternViewModel vm, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateInternCommand(vm.FullName, vm.Email, vm.BirthYear, vm.TrackId), ct);

            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to create Intern", result.Errors));

            return StatusCode(result.StatusCode,EndpointResponse.OK(  result.StatusCode,"Intern created successfully"));
        }


        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult> Update()
        //{

        //}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<EndpointResponse>> Delete(int id, CancellationToken ct)
        {
            var result = await mediator.Send(new DeleteInternCommand(id), ct);

            if (!result.Success)
                return StatusCode(result.StatusCode,
                    EndpointResponse.Fail(result.StatusCode, "Failed to delete Intern", result.Errors));

            return StatusCode(result.StatusCode,
                EndpointResponse.OK(result.StatusCode, "Intern deleted successfully"));
        }
    }
}
