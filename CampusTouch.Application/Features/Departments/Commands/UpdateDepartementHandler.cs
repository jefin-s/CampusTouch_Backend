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
//     public class UpdateDepartementHandler:IRequestHandler<UpdateDepartmentCommand,int>
//    {
//        private readonly IDepartementRepository _departementRepository;
//        private readonly ICurrentUserService _currentUserService;
//        public UpdateDepartementHandler(IDepartementRepository deptrep,ICurrentUserService currentUserService)
//        {
//            _departementRepository = deptrep;
//            _currentUserService = currentUserService;   
//        }

//        public async Task <int>Handle(UpdateDepartmentCommand request,CancellationToken token)
//        {
//            if (!_currentUserService.IsAdmin)
//               throw new UnauthorizedException("Only Admin can Update Departement");
//            var userId = _currentUserService.UserId;
//            var  ExistingDepartement=   await _departementRepository.GetByIdAsync(request.Id);
//            if (ExistingDepartement == null)
//                throw new NotFoundException("Departement Is Not Found");
//            ExistingDepartement.Name = request.Name;
//            ExistingDepartement.Description = request.Description;
//            ExistingDepartement.UpdatedAt = DateTime.UtcNow;
//            ExistingDepartement.UpdatedBy = userId;

//            return await _departementRepository.UpdateAsync(ExistingDepartement);

//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public class UpdateDepartementHandler : IRequestHandler<UpdateDepartmentCommand, int>
    {
        private readonly IDepartementRepository _departementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateDepartementHandler> _logger;

        public UpdateDepartementHandler(
            IDepartementRepository deptrep,
            ICurrentUserService currentUserService,
            ILogger<UpdateDepartementHandler> logger)
        {
            _departementRepository = deptrep;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateDepartmentCommand request, CancellationToken token)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation("User {UserId} attempting to update department {DepartmentId}", userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized update attempt by User {UserId} for Department {DepartmentId}", userId, request.Id);
                throw new UnauthorizedException("Only Admin can update department");
            }

            var existingDepartment = await _departementRepository.GetByIdAsync(request.Id);

            if (existingDepartment == null)
            {
                _logger.LogWarning("Update failed: Department {DepartmentId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Department not found");
            }

            // (Optional) capture old values for audit
            var oldName = existingDepartment.Name;
            var oldDescription = existingDepartment.Description;

            // 📝 Update
            existingDepartment.Name = request.Name;
            existingDepartment.Description = request.Description;
            existingDepartment.UpdatedAt = DateTime.UtcNow;
            existingDepartment.UpdatedBy = userId;

            await _departementRepository.UpdateAsync(existingDepartment);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} updated department {DepartmentId} successfully. Name: {OldName} → {NewName}",
                userId, request.Id, oldName, request.Name);

            return request.Id;
        }
    }
}