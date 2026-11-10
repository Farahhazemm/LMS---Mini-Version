using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.EnrollmentFeature.Commands;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.EnrollmentFeature.Handlers
{
    public class CompleteEnrollmentHandler(IGeneralRepository<Enrollment> enrollmentRepo, IGeneralRepository<Payment> paymentRepo, IUnitOfWork uow)
        : IRequestHandler<CompleteEnrollmentCommand, RequestResponse>
    {
        public async Task<RequestResponse> Handle(CompleteEnrollmentCommand request, CancellationToken ct)
        {
            static RequestResponse Error(int code, string field, string msg)
                => RequestResponse.Fail(code, new Dictionary<string, string[]> { [field] = new[] { msg } });

            var enrollment = await enrollmentRepo.GetAll()
                .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

            if (enrollment is null)
                return Error(404, "Id", $"Enrollment {request.Id} was not found.");

            if (enrollment.Status != EnrollmentStatus.Active)
                return Error(409, "Status", $"Only an Active enrollment can be completed. Current status: {enrollment.Status}.");

            var paymentStatus = await paymentRepo.GetAll()
                .Where(p => p.EnrollmentId == request.Id)
                .Select(p => (PaymentStatus?)p.Status)
                .FirstOrDefaultAsync(ct);

            if (paymentStatus is null)
                return Error(409, "Payment", "The enrollment has no payment, so it cannot be completed.");

            if (paymentStatus != PaymentStatus.Completed)
                return Error(409, "Payment", $"The payment must be Completed. Current payment status: {paymentStatus}.");

            
            enrollment.Status = EnrollmentStatus.Completed;
            await uow.SaveChangesAsync();

            return RequestResponse.OK();
        }
    }
}
