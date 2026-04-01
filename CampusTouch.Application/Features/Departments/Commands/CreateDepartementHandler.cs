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
        private readonly ICurrentUserService _currentUserService;
        public CreateDepartementHandler(IDepartementRepository repo,ICurrentUserService currentUserService)
        {
            _repository = repo;
            _currentUserService = currentUserService;
        }


        public async Task<int> Handle(CreateDepartementCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var department = new Departement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy= userId,
                
            };

            return await _repository.CreateAsync(department);
        }
    }
}
