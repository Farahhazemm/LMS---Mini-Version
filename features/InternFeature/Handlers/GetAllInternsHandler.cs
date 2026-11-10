using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.InternFeature.DTOs;
using LMS___Mini_Version.features.InternFeature.Queries;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.Handlers
{
    public class GetAllInternsHandler
    {

        public class GetAllInternsQueryHandler(IGeneralRepository<Intern> repo) : IRequestHandler<GetAllInternsQuery, RequestResponse<IEnumerable<InternDTO>>>
        {
            public async Task<RequestResponse<IEnumerable<InternDTO>>> Handle(GetAllInternsQuery request, CancellationToken ct)
            {
                var interns = await repo.GetAll().OrderBy(x => x.Id).Select(x => new InternDTO(
                x.Id,
                x.FullName,
                x.Email,
                x.Status,
                x.Track != null ? x.Track.Name : null))
            .ToListAsync(ct);

                return RequestResponse<IEnumerable<InternDTO>>.OK(interns);

            }



        }
    }
}
