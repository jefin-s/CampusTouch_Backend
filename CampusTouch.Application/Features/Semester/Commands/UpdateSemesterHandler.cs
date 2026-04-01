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
    public class UpdateSemesterHandler:IRequestHandler<UpdateSemesterCommand,bool>
    {
        private readonly ISemsterRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateSemesterHandler(ISemsterRepository repository,ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
        {
            var userid = _currentUserService.UserId;
            if (!_currentUserService.IsAdmin)
            {
               throw new UnauthorizedException("Only Admin Can Update the Semester");
            }
            var existing = await _repository.GetByIdAsync(request.Id);
            if (existing==null||existing.IsDeleted)
            {
                throw new NotFoundException("Semester Is Not Exist");
            }
            var SemExist = await _repository.SemExist(request.CourseId,request.OrderNumber);
            if (SemExist&&existing.Id!=request.Id)
                throw new BuisnessRuleException("Semster is already registered  ");
             
            existing.Name = request.Name;
            existing.CourseId = request.CourseId;   
            existing.OrderNumber = request.OrderNumber;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = userid;
            

            return await _repository.UpdateAsync(existing);
        }
    }
}
