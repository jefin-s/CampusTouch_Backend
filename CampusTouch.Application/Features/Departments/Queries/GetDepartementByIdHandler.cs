using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Queries
{
    public class GetDepartementByIdHandler:IRequestHandler<GetAllDepartementQuery,IEnumerable<Departement>>
    {
        private readonly IDepartementRepository _repository;
        public GetDepartementByIdHandler(IDepartementRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Departement>> Handle(
      GetAllDepartementQuery request,
      CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
    
}
