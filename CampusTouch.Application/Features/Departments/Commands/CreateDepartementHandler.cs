//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Departments.Commands
//{
//    public class CreateDepartementHandler:IRequestHandler<CreateDepartementCommand,int>
//    {
//        private readonly IDepartementRepository _repository;
//        private readonly ICurrentUserService _currentUserService;
//        public CreateDepartementHandler(IDepartementRepository repo,ICurrentUserService currentUserService)
//        {
//            _repository = repo;
//            _currentUserService = currentUserService;
//        }


//        public async Task<int> Handle(CreateDepartementCommand request, CancellationToken cancellationToken)
//        {
//            if(!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only admins can create departments.");


//            var DepartmentExists = await _repository.DepartemnetExist(request.Name);
//            if(DepartmentExists)
//                throw new BuisnessRuleException($"A department with the name '{request.Name}' already exists.");
//            var userId = _currentUserService.UserId;
//            var department = new Departement
//            {
//                Name = request.Name,
//                Description = request.Description,
//                IsActive = true,
//                CreatedAt = DateTime.UtcNow,
//                CreatedBy= userId,

//            };

//            return await _repository.CreateAsync(department);
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public class CreateDepartementHandler : IRequestHandler<CreateDepartementCommand, int>
    {
        private readonly IDepartementRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreateDepartementHandler> _logger;

        public CreateDepartementHandler(
            IDepartementRepository repo,
            ICurrentUserService currentUserService,
            ILogger<CreateDepartementHandler> logger)
        {
            _repository = repo;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(CreateDepartementCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("User {UserId} attempting to create department {DepartmentName}", userId, request.Name);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized department creation attempt by User {UserId}", userId);
                throw new UnauthorizedException("Only admins can create departments.");
            }

            // 🔥 Duplicate check
            var departmentExists = await _repository.DepartemnetExist(request.Name);

            if (departmentExists)
            {
                _logger.LogWarning(
                    "Duplicate department creation attempt by User {UserId} for Department {DepartmentName}",
                    userId, request.Name);

                throw new BuisnessRuleException($"A department with the name '{request.Name}' already exists.");
            }

            var department = new Departement
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var departmentId = await _repository.CreateAsync(department);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} created department {DepartmentId} ({DepartmentName}) successfully",
                userId, departmentId, request.Name);

            return departmentId;
        }
    }
}