using CampusTouch.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Students.Commands
{
    public class DeleteStudentCommandHandler:IRequestHandler<DeleteStudentCommand,bool>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        public DeleteStudentCommandHandler(IStudentRepository studentRepository,ICurrentUserService currentUserService)
        {
           _studentRepository = studentRepository;
            _currentUserService = currentUserService;   
        }

        public async Task<bool> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var userid = _currentUserService.UserId;
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admin can delete student");
            }
            var studentexisting = await _studentRepository.GetStudentsById(request.id);
            if (studentexisting == null || studentexisting.IsDeleted)
            {
                throw new KeyNotFoundException("Student is not exist");
            }
            studentexisting.IsDeleted = true;
            studentexisting.DeletedAt = DateTime.UtcNow;
            studentexisting.DeletedBy = userid;
            studentexisting.IsActive = false;
            return  await _studentRepository.DeleteStudent(request.id,userid);
        }
    }
}
