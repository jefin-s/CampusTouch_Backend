using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Queries
{
    public record GetCourseByIdQuery(int Id) : IRequest<AcademicProgram?>;
}
