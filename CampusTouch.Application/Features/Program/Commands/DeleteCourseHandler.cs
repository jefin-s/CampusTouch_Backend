//using CampusTouch.Application.Common.Exceptions;
//using CampusTouch.Application.Interfaces;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CampusTouch.Application.Features.Program.Commands
//{
//    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, int>
//    {
//        private readonly IProgramRepository _programRepository;
//        private readonly ICurrentUserService _currentUserService;

//        public DeleteCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService)
//        {
//            _programRepository = programRepository;
//            _currentUserService = currentUserService;
//        }

//        public async Task<int> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
//        {

//            if (!_currentUserService.IsAdmin)
//                throw new UnauthorizedException("Only Admin can Delete the Programs");
//            var userid = _currentUserService.UserId;
//            var ProgramIsExist = await _programRepository.GetByIdAsync(request.Id);
//            if (ProgramIsExist == null)
//               throw new NotFoundException("Program is not Exist");
//            ProgramIsExist.IsActive=false;
//            ProgramIsExist.IsDeleted = true;
//            ProgramIsExist.DeletedAt = DateTime.UtcNow;
//            ProgramIsExist.DeletedBy=userid;
//            return await _programRepository.DeleteAsync(request.Id);
//        }
//    }
//}

using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, int>
    {
        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteCourseHandler> _logger;

        public DeleteCourseHandler(
            IProgramRepository programRepository,
            ICurrentUserService currentUserService,
            ILogger<DeleteCourseHandler> logger)
        {
            _programRepository = programRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

        
            _logger.LogInformation("User {UserId} attempting to delete program {ProgramId}", userId, request.Id);

      
            if (!_currentUserService.IsAdmin)
            {
                _logger.LogWarning("Unauthorized delete attempt by User {UserId} for Program {ProgramId}", userId, request.Id);
                throw new UnauthorizedException("Only admin can delete programs");
            }

            var program = await _programRepository.GetByIdAsync(request.Id);

            if (program == null)
            {
                _logger.LogWarning("Delete failed: Program {ProgramId} not found (User {UserId})", request.Id, userId);
                throw new NotFoundException("Program not found");
            }

     
            program.IsActive = false;
            program.IsDeleted = true;
            program.DeletedAt = DateTime.UtcNow;
            program.DeletedBy = userId;

            await _programRepository.DeleteAsync(request.Id);

            
            _logger.LogInformation(
                "User {UserId} deleted program {ProgramId} ({ProgramName}) successfully",
                userId, request.Id, program.Name);

            return request.Id;
        }
    }
}