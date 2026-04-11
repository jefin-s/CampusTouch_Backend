using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Classes.Command
{
    public  class UpdateClassCommandHandler:IRequestHandler<UpdateClassCommand,int>
    {

        private readonly IClassesRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public UpdateClassCommandHandler(IClassesRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }


        public async Task<int> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can update classes");
            }
            var existingClass = await _repo.GetByIdAsync(request.Id);
            if (existingClass == null)
            {
                throw new NotFoundException("Class not found");
            }
            if (await _repo.ExistsAsync(request.Year, request.Semester, request.Id))
            {
                throw new BuisnessRuleException("Class already exists");
            }
            existingClass.Name = request.Name;
            existingClass.DepartmentId = request.DepartmentId;
            existingClass.CourseId = request.CourseId;
            existingClass.Year = request.Year;
            existingClass.Semester = request.Semester;
            await _repo.UpdateAsync(request.Id, existingClass);
            return request.Id;
        }
    }
}
