using CampusTouch.Application.Features.Applicants.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Applicants.Queries
{
    public record GetApplicantQuery() : IRequest<IEnumerable<ApplicantDto>>;

}