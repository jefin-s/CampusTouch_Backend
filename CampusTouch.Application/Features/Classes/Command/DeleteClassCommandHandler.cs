//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Classes.Command
//{
//    public class DeleteClassCommandHandler:IRequestHandler<DeleteClassCommand,int>
//    {
//        private readonly IClassesRepository _repo;
//        private readonly ICurrentUserService _currentUser;
//        public DeleteClassCommandHandler(IClassesRepository repo, ICurrentUserService currentUser)
//        {
//            _repo = repo;
//            _currentUser = currentUser;
//        }
//        public async Task<int> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
//        {
//            if (!_currentUser.IsAdmin)
//            {
//                throw new UnauthorizedAccessException("Only admins can delete classes");
//            }
//            var existingClass = await _repo.GetByIdAsync(request.Id);
//            if (existingClass == null)
//            {
//                throw new NotFoundException("Class not found");
//            }
//            existingClass.IsActive = false;
//            await _repo.DeleteAsync(request.Id);
//            return request.Id;
//        }
//    }
//}
using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Classes.Command
{
    public class DeleteClassCommandHandler : IRequestHandler<DeleteClassCommand, int>
    {
        private readonly IClassesRepository _repo;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<DeleteClassCommandHandler> _logger;

        public DeleteClassCommandHandler(
            IClassesRepository repo,
            ICurrentUserService currentUser,
            ILogger<DeleteClassCommandHandler> logger)
        {
            _repo = repo;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteClassCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUser.UserId;

            _logger.LogInformation("User {UserId} attempting to delete class {ClassId}", userId, request.Id);

            // 🔐 Authorization
            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning("Unauthorized delete attempt by User {UserId} for Class {ClassId}", userId, request.Id);
                throw new UnauthorizedAccessException("Only admins can delete classes");
            }

            var existingClass = await _repo.GetByIdAsync(request.Id);

            if (existingClass == null)
            {
                _logger.LogWarning("Delete failed: Class {ClassId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Class not found");
            }

            // 👉 Soft delete (recommended)
            existingClass.IsActive = false;

            await _repo.DeleteAsync(request.Id);

            // ✅ Audit log
            _logger.LogInformation("User {UserId} deleted class {ClassId} successfully", userId, request.Id);

            return request.Id;
        }
    }
}
