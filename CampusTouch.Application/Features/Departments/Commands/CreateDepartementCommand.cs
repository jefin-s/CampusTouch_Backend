using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public record CreateDepartementCommand(string Name,string Description):IRequest<int>;
    
}
