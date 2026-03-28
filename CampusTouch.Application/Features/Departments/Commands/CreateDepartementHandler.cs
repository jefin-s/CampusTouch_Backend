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
    public class CreateDepartementHandler:IRequestHandler<CreateDepartementCommand,int>
    {
        private readonly IDepartementRepository _repository;
        public CreateDepartementHandler(IDepartementRepository repo)
        {
            _repository = repo;
        }


        public async Task<int> Handle(CreateDepartementCommand request, CancellationToken cancellationToken)
        {
            var department = new Departement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.CreateAsync(department);
        }
    }
}
