using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Features.Classes.Command
{
    public class DeleteClassCommand:IRequest<int>
     {
        public int Id { get; set; }
    
    }
}
