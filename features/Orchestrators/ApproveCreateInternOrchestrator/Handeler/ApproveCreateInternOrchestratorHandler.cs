using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.Orchestrators.ApproveCreateInternOrchestrator.Commands;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.Orchestrators.ApproveCreateInternOrchestrator.Handeler
{
    public class ApproveCreateInternOrchestratorHandler(  IGeneralRepository<Intern> internRepo, IGeneralRepository<Track> trackRepo, IGeneralRepository<Enrollment> enrollmentRepo, IUnitOfWork uow) : IRequestHandler<ApproveCreateInternOrchestratorCommand, RequestResponse<int>>
    {
        public async Task<RequestResponse<int>> Handle(ApproveCreateInternOrchestratorCommand request, CancellationToken ct)
        {
            static RequestResponse<int> Error(int code, string field, string msg)
                => RequestResponse<int>.Fail(code, new Dictionary<string, string[]> { [field] = new[] { msg } });

            if (!Enum.IsDefined(request.PaymentMethod))
                return Error(400, "PaymentMethod", "PaymentMethod must be Cash, CreditCard, BankTransfer, or Online.");

            var intern = await internRepo.GetAll().FirstOrDefaultAsync(i => i.Id == request.InternId, ct);
            if (intern is null)
                return Error(404, "InternId", $"Intern {request.InternId} was not found.");

            if (intern.Status != InternStatus.Pending)
                return Error(409, "Status", $"Only a Pending intern can be approved. Current status: {intern.Status}.");

            var track = await trackRepo.GetAll().FirstOrDefaultAsync(t => t.Id == intern.TrackId, ct);
            if (track is null)
                return Error(404, "TrackId", $"The assigned track {intern.TrackId} was not found.");

            if (!track.IsActive)
                return Error(409, "Track", "The assigned track is not active.");

            var hasDuplicate = await enrollmentRepo.GetAll().AnyAsync(e =>
                e.InternId == intern.Id &&
                e.TrackId == track.Id &&
                (e.Status == EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Active), ct);
            if (hasDuplicate)
                return Error(409, "Enrollment", "The intern already has a Pending or Active enrollment in this track.");

            var reserved = await enrollmentRepo.GetAll().CountAsync(e =>
                e.TrackId == track.Id &&
                (e.Status == EnrollmentStatus.Pending || e.Status == EnrollmentStatus.Active), ct);
            if (track.MaxCapacity - reserved <= 0)
                return Error(409, "Track", "The assigned track has no available seats.");

         
            var now = DateTime.UtcNow;

            intern.Status = InternStatus.Accepted;

            var enrollment = new Enrollment
            {
                InternId = intern.Id,
                TrackId = track.Id,
                EnrollmentDate = now,
                Status = EnrollmentStatus.Active,
             
                Payment = new Payment
                {
                    Amount = track.Fees,                
                    PaymentDate = now,
                    Method = request.PaymentMethod,
                    Status = PaymentStatus.Completed
                }
            };
            enrollmentRepo.Add(enrollment);

           
            try
            {
                await uow.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Error(500, "Server", "The registration could not be completed. No changes were saved.");
            }

            return RequestResponse<int>.OK(enrollment.Id, 200);
        }
    }
}