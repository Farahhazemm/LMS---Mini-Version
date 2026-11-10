using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.InternFeature.Commands;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.Handlers
{
    public class DeleteInternHandler(IGeneralRepository<Intern> internRepo, IGeneralRepository<Enrollment> enrollmentRepo, IUnitOfWork uow)
        : IRequestHandler<DeleteInternCommand, RequestResponse>
    {
        public async Task<RequestResponse> Handle(DeleteInternCommand request, CancellationToken ct)
        {
            static RequestResponse Error(int code, string field, string msg)
                => RequestResponse.Fail(code, new Dictionary<string, string[]> { [field] = new[] { msg } });

            var intern = await internRepo.GetAll().FirstOrDefaultAsync(i => i.Id == request.Id, ct);
            if (intern is null)
                return Error(404, "Id", $"Intern {request.Id} was not found.");

            
            var hasEnrollments = await enrollmentRepo.GetAll().AnyAsync(e => e.InternId == request.Id, ct);
            if (hasEnrollments)
                return Error(409, "Enrollments",
                    "This intern has enrollment history and cannot be deleted. Enrollment and payment records must be kept.");

            internRepo.Delete(intern);

            try
            {
                await uow.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
              
                return Error(409, "Enrollments",
                    "This intern has enrollment history and cannot be deleted. Enrollment and payment records must be kept.");
            }

            return RequestResponse.OK();
        }
    }
}