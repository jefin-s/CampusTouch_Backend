using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public record DeleteSemesterCommand(int Id) : IRequest<bool>;
}
    