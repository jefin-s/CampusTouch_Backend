using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Program.Commands
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, int>
    {
        private readonly IProgramRepository _programRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCourseHandler(IProgramRepository programRepository,ICurrentUserService currentUserService)
        {
            _programRepository = programRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {

            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only Admin can Delete the Programs");
            var userid = _currentUserService.UserId;
            var ProgramIsExist = await _programRepository.GetByIdAsync(request.Id);
            if (ProgramIsExist == null)
               throw new NotFoundException("Program is not Exist");
            ProgramIsExist.IsActive=false;
            ProgramIsExist.IsDeleted = true;
            ProgramIsExist.DeletedAt = DateTime.UtcNow;
            ProgramIsExist.DeletedBy=userid;
            return await _programRepository.DeleteAsync(request.Id);
        }
    }
}
