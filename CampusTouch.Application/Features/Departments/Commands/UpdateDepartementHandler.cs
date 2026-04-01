using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using CampusTouch.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Commands
{
     public class UpdateDepartementHandler:IRequestHandler<UpdateDepartmentCommand,int>
    {
        private readonly IDepartementRepository _departementRepository;
        private readonly ICurrentUserService _currentUserService;
        public UpdateDepartementHandler(IDepartementRepository deptrep,ICurrentUserService currentUserService)
        {
            _departementRepository = deptrep;
            _currentUserService = currentUserService;   
        }

        public async Task <int>Handle(UpdateDepartmentCommand request,CancellationToken token)
        {
            var userId = _currentUserService.UserId;
            var  ExistingDepartement=   await _departementRepository.GetByIdAsync(request.Id);
            if (ExistingDepartement == null)
                throw new NotFoundException("Departement Is Not Found");
            if (!_currentUserService.IsAdmin)
               throw new UnauthorizedException("Only Admin can Update Departement");
            ExistingDepartement.Name = request.Name;
            ExistingDepartement.Description = request.Description;
            ExistingDepartement.UpdatedAt = DateTime.UtcNow;
            ExistingDepartement.UpdatedBy = userId;

            return await _departementRepository.UpdateAsync(ExistingDepartement);

        }
    }
}
