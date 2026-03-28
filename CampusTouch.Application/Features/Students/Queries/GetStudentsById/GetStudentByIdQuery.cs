using CampusTouch.Application.Features.Students.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Students.Queries.GetStudentsById
{
    public  record GetStudentByIdQuery(int id):IRequest<StudentResponseDTO>;

    
}
    