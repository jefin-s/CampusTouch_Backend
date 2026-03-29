using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public record DeleteCourseCommand(int Id) : IRequest<int>;
}
