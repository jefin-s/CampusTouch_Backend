//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Semester.Commands
//{
//    public class UpdateSemesterHandler:IRequestHandler<UpdateSemesterCommand,bool>
//    {
//        private readonly ISemsterRepository _repository;
//        private readonly ICurrentUserService _currentUserService;
//        public UpdateSemesterHandler(ISemsterRepository repository,ICurrentUserService currentUserService)
//        {
//            _repository = repository;
//            _currentUserService = currentUserService;
//        }
//        public async Task<bool> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
//        {
//            if (!_currentUserService.IsAdmin)
//            {
//               throw new UnauthorizedException("Only Admin Can Update the Semester");
//            }
//            var userid = _currentUserService.UserId;
//            var name = request.Name.Trim();
//            var existing = await _repository.GetByIdAsync(request.Id);
//            if (existing==null||existing.IsDeleted)
//            {
//                throw new NotFoundException("Semester Is Not Exist");
//            }
//            var SemExist = await _repository.SemExist(request.CourseId,request.OrderNumber);
//            if (SemExist&&existing.OrderNumber!=request.OrderNumber)
//                throw new BuisnessRuleException("Semster is already registered  ");

//            existing.Name = name;
//            existing.CourseId = request.CourseId;   
//            existing.OrderNumber = request.OrderNumber;
//            existing.UpdatedAt = DateTime.UtcNow;
//            existing.UpdatedBy = userid;


//            return await _repository.UpdateAsync(existing);
//        }
//    }
//}
using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public class UpdateSemesterHandler : IRequestHandler<UpdateSemesterCommand, bool>
    {
        private readonly ISemsterRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateSemesterHandler> _logger;

        public UpdateSemesterHandler(
            ISemsterRepository repository,
            ICurrentUserService currentUserService,
            ILogger<UpdateSemesterHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var name = request.Name.Trim();

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to update semester {SemesterId}",
                userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized update attempt by User {UserId} for Semester {SemesterId}",
                    userId, request.Id);

                throw new UnauthorizedException("Only admin can update semester");
            }

            var existing = await _repository.GetByIdAsync(request.Id);

            if (existing == null || existing.IsDeleted)
            {
                _logger.LogWarning(
                    "Update failed: Semester {SemesterId} not found (User {UserId})",
                    request.Id, userId);

                throw new NotFoundException("Semester not found");
            }

            // 🔥 Duplicate check
            var semExist = await _repository.SemExist(request.CourseId, request.OrderNumber);

            if (semExist && existing.OrderNumber != request.OrderNumber)
            {
                _logger.LogWarning(
                    "Duplicate semester update attempt by User {UserId} for Course {CourseId}, Order {Order}",
                    userId, request.CourseId, request.OrderNumber);

                throw new BuisnessRuleException("Semester already exists");
            }

            // 📝 Capture old values (for audit)
            var oldName = existing.Name;
            var oldOrder = existing.OrderNumber;

            // 📝 Update
            existing.Name = name;
            existing.CourseId = request.CourseId;
            existing.OrderNumber = request.OrderNumber;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userId;

            await _repository.UpdateAsync(existing);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} updated semester {SemesterId} successfully. Name: {OldName} → {NewName}, Order: {OldOrder} → {NewOrder}",
                userId, request.Id, oldName, name, oldOrder, request.OrderNumber);

            return true;
        }
    }
}