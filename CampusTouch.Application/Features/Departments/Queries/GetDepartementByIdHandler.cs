using CampusTouch.Application.Features.Departments.DTOs;
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
    public class GetDepartementByIdHandler : IRequestHandler<GetDepartementByIdQuery, Deparetment_Response_DTO?>
    {
        private readonly IDepartementRepository _repository;
        public GetDepartementByIdHandler(IDepartementRepository repository)
        {
            _repository = repository;
        }
        public async Task<Deparetment_Response_DTO?> Handle(
      GetDepartementByIdQuery request,
      CancellationToken cancellationToken)
        {
           var dept=await _repository.GetByIdAsync(request.Id);
            if (dept == null)
                return null;

            return new Deparetment_Response_DTO
            {
                Id = dept.Id,
                Name = dept.Name,
                Description = dept.Description,
            };
           
        }
    }
    
}
