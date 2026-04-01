using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public record CreateSemesterCommand(string Name, int orderNumber, int CourseId) : IRequest<int>;
    };
