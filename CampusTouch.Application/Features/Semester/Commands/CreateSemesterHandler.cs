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
//    public class CreateSemesterHandler:IRequestHandler<CreateSemesterCommand,int>
//    {

//        private readonly ISemsterRepository _semsterRepository;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IProgramRepository _programRepository;
//        public CreateSemesterHandler(ISemsterRepository semsterRepository,ICurrentUserService currentUserService,IProgramRepository programRepository)
//        {
//            _semsterRepository = semsterRepository; 
//            _currentUserService = currentUserService;
//            _programRepository = programRepository;

//        }
//        public async Task<int> Handle(CreateSemesterCommand request, CancellationToken cancellationToken)
//        {

//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only Admin Can CreateSemester");

//            var userid = _currentUserService.UserId;
//            var name = request.Name.Trim();
//            var exsiting = await _programRepository.GetByIdAsync(request.CourseId);
//            if (exsiting == null||exsiting.IsDeleted)
//            {

//                throw new NotFoundException("Cousre is Not  Found ");
//            }

//            var SemExist = await _semsterRepository.SemExist(request.CourseId, request.orderNumber);
//            if (SemExist)
//                throw new BuisnessRuleException("Semster is already exist");
//            var semester = new Semesters
//            {
//                Name = name,
//                OrderNumber = request.orderNumber,
//                CourseId = request.CourseId,
//                CreatedAt=DateTime.UtcNow,  
//                CreatedBy=userid,

//            };

//            return await _semsterRepository.CreateAsync(semester);
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public class CreateSemesterHandler : IRequestHandler<CreateSemesterCommand, int>
    {
        private readonly ISemsterRepository _semsterRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProgramRepository _programRepository;
        private readonly ILogger<CreateSemesterHandler> _logger;

        public CreateSemesterHandler(
            ISemsterRepository semsterRepository,
            ICurrentUserService currentUserService,
            IProgramRepository programRepository,
            ILogger<CreateSemesterHandler> logger)
        {
            _semsterRepository = semsterRepository;
            _currentUserService = currentUserService;
            _programRepository = programRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateSemesterCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var name = request.Name.Trim();

            // ✅ Attempt log
            _logger.LogInformation(
                "User {UserId} attempting to create semester {SemesterName} (Order {Order}) for Course {CourseId}",
                userId, name, request.orderNumber, request.CourseId);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized semester creation attempt by User {UserId}", userId);
                throw new UnauthorizedException("Only admin can create semester");
            }

            // 🔍 Course validation
            var course = await _programRepository.GetByIdAsync(request.CourseId);

            if (course == null || course.IsDeleted)
            {
                _logger.LogWarning(
                    "Semester creation failed: Course {CourseId} not found (User {UserId})",
                    request.CourseId, userId);

                throw new NotFoundException("Course not found");
            }

            // 🔥 Duplicate check
            var exists = await _semsterRepository.SemExist(request.CourseId, request.orderNumber);

            if (exists)
            {
                _logger.LogWarning(
                    "Duplicate semester creation attempt by User {UserId} for Course {CourseId}, Order {Order}",
                    userId, request.CourseId, request.orderNumber);

                throw new BuisnessRuleException("Semester already exists");
            }

            var semester = new Semesters
            {
                Name = name,
                OrderNumber = request.orderNumber,
                CourseId = request.CourseId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var semesterId = await _semsterRepository.CreateAsync(semester);

            // ✅ Audit log (VERY IMPORTANT)
            _logger.LogInformation(
                "User {UserId} created semester {SemesterId} ({SemesterName}) for Course {CourseId}",
                userId, semesterId, name, request.CourseId);

            return semesterId;
        }
    }
}