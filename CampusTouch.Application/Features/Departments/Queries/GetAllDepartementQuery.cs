using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Queries
{
    public record GetAllDepartementQuery() : IRequest<IEnumerable<Departement>>;
}
