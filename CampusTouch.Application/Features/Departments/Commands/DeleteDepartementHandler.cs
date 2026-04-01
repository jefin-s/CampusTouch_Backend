using CampusTouch.Application.Common.Exceptions;
using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public  class DeleteDepartementHandler:IRequestHandler<DeleteDepartmentCommand,int>
    {

        private readonly IDepartementRepository _departementRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteDepartementHandler(IDepartementRepository DeptRepository,ICurrentUserService currentUserService)
        {
            _departementRepository = DeptRepository;
            _currentUserService = currentUserService;
        }
        
       public async Task<int> Handle(DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
        {
            var userid = _currentUserService.UserId;

            var Existing = await _departementRepository.GetByIdAsync(request.Id);
            if (Existing == null)
            {
                throw new NotFoundException("Departement Is Not Found");
            }

            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedException("only Admin can delete the departement");
            }
            Existing.DeletedAt = DateTime.UtcNow;
            Existing.DeletedBy=userid;

            return await _departementRepository.DeleteAsync(request.Id);
        }
    }
}
