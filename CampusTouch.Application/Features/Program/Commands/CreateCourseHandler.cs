//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using CampusTouch.Domain.Entities;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Program.Commands
//{
//    public class CreateCourseHandler:IRequestHandler<CreateCourseCommand,int>
//    {

//        private readonly IProgramRepository _programRepository;
//        private readonly ICurrentUserService _currentUserService;
//        private readonly IDepartementRepository _departementRepository;
//       public CreateCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService,IDepartementRepository departementRepository)
//        {
//            _programRepository = programRepository;
//            _currentUserService = currentUserService;
//            _departementRepository=departementRepository;
//        }

//        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
//        {

//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only admin can Create Programs");
//            var Name = request.Name.Trim();
//            var userId = _currentUserService.UserId;

//            var DeptIsExistorNot = await _departementRepository.GetByIdAsync(request.DepartmentId);
//            if (DeptIsExistorNot == null)
//            {
//                throw new NotFoundException("Departement is Not Found");
//            }
//            var CouseIsExistOrNot = await _programRepository.ProgramIsExist(Name,request.DepartmentId);
//            if (CouseIsExistOrNot)
//                throw new BadRequestException("Course is Already Added");
//            var course = new AcademicProgram
//            {
//                Name= Name,
//                Level= request.Level,
//                Duration= request.Duration,
//                DepartmentId= request.DepartmentId,
//                CreatedAt=DateTime.UtcNow,
//                CreatedBy=userId

//            };
//            return await _programRepository.CreateAsync(course);
//        }
//    }
//}


using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand, int>
    {
        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDepartementRepository _departementRepository;
        private readonly ILogger<CreateCourseHandler> _logger;

        public CreateCourseHandler(
            IProgramRepository programRepository,
            ICurrentUserService currentUserService,
            IDepartementRepository departementRepository,
            ILogger<CreateCourseHandler> logger)
        {
            _programRepository = programRepository;
            _currentUserService = currentUserService;
            _departementRepository = departementRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var name = request.Name.Trim();

            // ✅ Attempt log
            _logger.LogInformation("User {UserId} attempting to create program {ProgramName} in Department {DepartmentId}",
                userId, name, request.DepartmentId);

            // 🔐 Authorization
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized program creation attempt by User {UserId}", userId);
                throw new UnauthorizedException("Only admin can create programs");
            }

            // 🔍 Department check
            var dept = await _departementRepository.GetByIdAsync(request.DepartmentId);

            if (dept == null)
            {
                _logger.LogWarning("Program creation failed: Department {DepartmentId} not found (User {UserId})",
                    request.DepartmentId, userId);

                throw new NotFoundException("Department not found");
            }

            // 🔥 Duplicate check
            var exists = await _programRepository.ProgramIsExist(name, request.DepartmentId);

            if (exists)
            {
                _logger.LogWarning(
                    "Duplicate program creation attempt by User {UserId} for Program {ProgramName} in Department {DepartmentId}",
                    userId, name, request.DepartmentId);

                throw new BadRequestException("Course already exists");
            }

            var course = new AcademicProgram
            {
                Name = name,
                Level = request.Level,
                Duration = request.Duration,
                DepartmentId = request.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var programId = await _programRepository.CreateAsync(course);

            // ✅ Audit log (MOST IMPORTANT)
            _logger.LogInformation(
                "User {UserId} created program {ProgramId} ({ProgramName}) in Department {DepartmentId}",
                userId, programId, name, request.DepartmentId);

            return programId;
        }
    }
}