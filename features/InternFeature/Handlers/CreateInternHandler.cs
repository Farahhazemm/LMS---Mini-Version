using Azure.Core;
using LMS___Mini_Version.Domain.Entities;
using LMS___Mini_Version.Domain.Enums;
using LMS___Mini_Version.Domain.Repositories;
using LMS___Mini_Version.features.InternFeature.Commands;
using LMS___Mini_Version.features.shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS___Mini_Version.features.InternFeature.Handlers
{

    public class CreateInternHandler(IGeneralRepository<Intern> internRepo, IGeneralRepository<Track> trackRepo, IUnitOfWork uow) : IRequestHandler<CreateInternCommand, RequestResponse<int>>
    {
        public async Task<RequestResponse<int>> Handle(CreateInternCommand request, CancellationToken ct)
        {
            var isTrackActive = await trackRepo.GetAll().Where(t => t.Id == request.TrackId).Select(t => (bool?)t.IsActive).FirstOrDefaultAsync(ct);

            if (isTrackActive is null)
                return RequestResponse<int>.Fail(404);  // TODO : Handel SpMasseage

            if (isTrackActive == false)
                return RequestResponse<int>.Fail(400);  // TODO : Handel SpMasseage

            // Email

            var email = request.Email.Trim();
            var normalizedEmail = email.ToLower();
            var emailExists = await internRepo.GetAll().AnyAsync(i => i.Email.ToLower() == normalizedEmail, ct);
            if (emailExists)
                return RequestResponse<int>.Fail(409, new Dictionary<string, string[]> { ["Email"] = new[] { "An intern with this email already exists." } });

            var intern = new Intern
            {
                FullName = request.FullName.Trim(),
                Email = email,
                BirthYear = request.BirthYear,
                TrackId = request.TrackId,
                Status = InternStatus.Pending
            };

            internRepo.Add(intern);
            await uow.SaveChangesAsync();
            return RequestResponse<int>.OK(intern.Id, 201);
        }
    }
}
