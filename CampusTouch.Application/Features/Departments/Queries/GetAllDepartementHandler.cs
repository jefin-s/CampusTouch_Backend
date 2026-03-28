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
    public class GetAllDepartementHandler:IRequestHandler<GetAllDepartementQuery,IEnumerable<Deparetment_Response_DTO>>
    {

        private readonly IDepartementRepository _departementRepository;


        public GetAllDepartementHandler(IDepartementRepository departementRepository)
        {
            _departementRepository = departementRepository;
        }

        public async Task<IEnumerable<Deparetment_Response_DTO>> Handle(GetAllDepartementQuery  query,CancellationToken token)
        {
           var departement= await _departementRepository.GetAllAsync();
            return departement.Select(x => new Deparetment_Response_DTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });
        }
    }
}
