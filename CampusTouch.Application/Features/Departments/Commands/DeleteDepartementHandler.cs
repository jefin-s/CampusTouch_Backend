using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Departments.Commands
{
    public  class DeleteDepartementHandler:IRequestHandler<DeleteDepartmentCommand,int>
    {

        private readonly IDepartementRepository _departementRepository;
        public DeleteDepartementHandler(IDepartementRepository DeptRepository)
        {
            _departementRepository = DeptRepository;
        }
        
       public async Task<int> Handle(DeleteDepartmentCommand request,
        CancellationToken cancellationToken)
        {

            return await _departementRepository.DeleteAsync(request.Id);
        }
    }
}
