using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.EnrollmentFeature.DTOs;
using LMS___Mini_Version.features.EnrollmentFeature.Queries;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.Handlers
{
    public class GetEnrollmentsByInternIdHandler(IGeneralRepository<Intern> internRepo, IGeneralRepository<Enrollment> enrollmentRepo) : IRequestHandler<GetEnrollmentsByInternIdQuery, RequestResponse<IEnumerable<InternEnrollmentDTO>>>
    {
        public async Task<RequestResponse<IEnumerable<InternEnrollmentDTO>>> Handle(GetEnrollmentsByInternIdQuery request, CancellationToken ct)
        {
            var internExists = await internRepo.GetAll().AnyAsync(i => i.Id == request.InternId, ct); 

            if (!internExists)
            {
                return RequestResponse<IEnumerable<InternEnrollmentDTO>>.Fail(404); // Handel masseage
            }

            var enrollments = await enrollmentRepo.GetAll()
                .Where(e => e.InternId == request.InternId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ThenByDescending(e => e.Id)
                .Select(e => new InternEnrollmentDTO(
                    e.Id,
                    e.TrackId,
                    e.Track.Name,
                    e.EnrollmentDate,
                    e.Status))
                .ToListAsync(ct);   

            return RequestResponse<IEnumerable<InternEnrollmentDTO>>.OK(enrollments);
        }
    }
}
