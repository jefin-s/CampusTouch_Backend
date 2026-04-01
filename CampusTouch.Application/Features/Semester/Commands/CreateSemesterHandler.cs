using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Semester.Commands
{
    public class CreateSemesterHandler:IRequestHandler<CreateSemesterCommand,int>
    {

        private readonly ISemsterRepository _semsterRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProgramRepository _programRepository;
        public CreateSemesterHandler(ISemsterRepository semsterRepository,ICurrentUserService currentUserService,IProgramRepository programRepository)
        {
            _semsterRepository = semsterRepository; 
            _currentUserService = currentUserService;
            _programRepository = programRepository;
                
        }
        public async Task<int> Handle(CreateSemesterCommand request, CancellationToken cancellationToken)
        {

            var userid = _currentUserService.UserId;
            if (!_currentUserService.IsAdmin)
                throw new UnauthorizedException("Only Admin Can CreateSemester");
            var exsiting = await _programRepository.GetByIdAsync(request.CourseId);
            if (exsiting == null||exsiting.IsDeleted)
            {
                throw new NotFoundException("Cousre is Not  Found ");
            }

            var SemExist = await _semsterRepository.SemExist(request.CourseId, request.orderNumber);
            if (SemExist)
                throw new BuisnessRuleException("Semster is already exist");
            var semester = new Semesters
            {
                Name = request.Name,
                OrderNumber = request.orderNumber,
                CourseId = request.CourseId,
                CreatedAt=DateTime.UtcNow,  
                CreatedBy=userid,
                
            };

            return await _semsterRepository.CreateAsync(semester);
        }
    }
}
