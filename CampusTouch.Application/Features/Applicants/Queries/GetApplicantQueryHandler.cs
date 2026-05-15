using CampusTouch.Application.Features.Applicants.DTO;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Applicants.Queries
{
    public class GetApplicantQueryHandler : IRequestHandler<GetApplicantQuery, IEnumerable<ApplicantDto>>
    {
        private readonly IApplicantRepository _ApplicantRepository;
        public GetApplicantQueryHandler(IApplicantRepository applicantRepository)
        {
            _ApplicantRepository = applicantRepository;
        }
        public async Task<IEnumerable<ApplicantDto>> Handle(GetApplicantQuery request, CancellationToken token)
        {
            var applicants = await _ApplicantRepository.GetAllApplicants();
            return applicants;
        }
    }

}