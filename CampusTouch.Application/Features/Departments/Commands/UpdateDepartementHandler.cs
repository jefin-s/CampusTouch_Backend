using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Commands
{
     public class UpdateDepartementHandler:IRequestHandler<UpdateDepartmentCommand,int>
    {
        private readonly IDepartementRepository _departementRepository;
        public UpdateDepartementHandler(IDepartementRepository deptrep)
        {
            _departementRepository = deptrep;
        }

        public async Task <int>Handle(UpdateDepartmentCommand request,CancellationToken token)
        {
            var dept = new Departement
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
            };
            return await _departementRepository.UpdateAsync(dept);

        }
    }
}
