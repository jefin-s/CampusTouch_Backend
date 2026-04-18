//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Departments.Commands
//{
//    public  class DeleteDepartementHandler:IRequestHandler<DeleteDepartmentCommand,int>
//    {

//        private readonly IDepartementRepository _departementRepository;
//        private readonly ICurrentUserService _currentUserService;
//        public DeleteDepartementHandler(IDepartementRepository DeptRepository,ICurrentUserService currentUserService)
//        {
//            _departementRepository = DeptRepository;
//            _currentUserService = currentUserService;
//        }

//       public async Task<int> Handle(DeleteDepartmentCommand request,
//        CancellationToken cancellationToken)
//        {
//            var userid = _currentUserService.UserId;

//            if (!_currentUserService.IsAdmin)
//            {
//                throw new UnauthorizedException("only Admin can delete the departement");
//            }
//            var Existing = await _departementRepository.GetByIdAsync(request.Id);
//            if (Existing == null)
//            {
//                throw new NotFoundException("Departement Is Not Found");
//            }
//            Existing.isDeleted = true;
//            Existing.IsActive=false;
//            Existing.DeletedAt = DateTime.UtcNow;
//            Existing.DeletedBy = userid;

//            return await _departementRepository.DeleteAsync(request.Id);
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public class DeleteDepartementHandler : IRequestHandler<DeleteDepartmentCommand, int>
    {
        private readonly IDepartementRepository _departementRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteDepartementHandler> _logger;

        public DeleteDepartementHandler(
            IDepartementRepository deptRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteDepartementHandler> logger)
        {
            _departementRepository = deptRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            _logger.LogInformation("User {UserId} attempting to delete department {DepartmentId}", userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized department delete attempt by User {UserId}", userId);
                throw new UnauthorizedException("Only admin can delete department");
            }

            var existing = await _departementRepository.GetByIdAsync(request.Id);

            if (existing == null)
            {
                _logger.LogWarning("Delete failed: Department {DepartmentId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Department not found");
            }

            // 🗑 Soft delete
            existing.isDeleted = true;
            existing.IsActive = false;
            existing.DeletedAt = DateTime.UtcNow;
            existing.DeletedBy = userId;

            await _departementRepository.DeleteAsync(request.Id);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} deleted department {DepartmentId} ({DepartmentName}) successfully",
                userId, request.Id, existing.Name);

            return request.Id;
        }
    }
}