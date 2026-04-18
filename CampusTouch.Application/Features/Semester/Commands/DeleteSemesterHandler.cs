//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Semester.Commands
//{
//    public  class DeleteSemesterHandler:IRequestHandler<DeleteSemesterCommand, bool>
//    {
//        private readonly ISemsterRepository _repository;
//        private readonly ICurrentUserService _currentUserService;

//        public DeleteSemesterHandler(ISemsterRepository repository,ICurrentUserService currentUserService)
//        {
//            _repository = repository;
//            _currentUserService = currentUserService;
//        }

//        public async Task<bool> Handle(DeleteSemesterCommand request, CancellationToken cancellationToken)
//        {

//            if (!_currentUserService.IsAdmin)
//            {
//                throw new UnauthorizedException("Only Admin can Delete the  Semster");
//            }
//            var userid = _currentUserService.UserId;

//            var semster = await _repository.GetByIdAsync(request.Id);
//            if (semster == null||semster.IsDeleted)
//            {
//                throw new NotFoundException("Semester is Not found");
//            }
//            return await _repository.DeleteAsync(request.Id,userid);
//        }

//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public class DeleteSemesterHandler : IRequestHandler<DeleteSemesterCommand, bool>
    {
        private readonly ISemsterRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteSemesterHandler> _logger;

        public DeleteSemesterHandler(
            ISemsterRepository repository,
            ICurrentUserService currentUserService,
            ILogger<DeleteSemesterHandler> logger)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteSemesterCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to delete semester {SemesterId}",
                userId, request.Id);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning(
                    "Unauthorized delete attempt by User {UserId} for Semester {SemesterId}",
                    userId, request.Id);

                throw new UnauthorizedException("Only admin can delete semester");
            }

            var semester = await _repository.GetByIdAsync(request.Id);

            if (semester == null || semester.IsDeleted)
            {
                _logger.LogWarning(
                    "Delete failed: Semester {SemesterId} not found (User {UserId})",
                    request.Id, userId);

                throw new NotFoundException("Semester not found");
            }

            var result = await _repository.DeleteAsync(request.Id, userId);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} deleted semester {SemesterId} ({SemesterName}) successfully",
                userId, request.Id, semester.Name);

            return result;
        }
    }
}