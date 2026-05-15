

using CampusTouch.Application.Features.Applicants.DTO;

namespace CampusTouch.Application.Interfaces
{
    public  interface IApplicantRepository
    {
        Task<List<ApplicantDto>> GetAllApplicants();
    }
}

