    using CampusTouch.Application.Common;
    using CampusTouch.Domain.Entities;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    namespace CampusTouch.API.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class SubjectController : ControllerBase
        {
            private readonly IMediator _mediator;
            public SubjectController(IMediator mediator)
            {
                _mediator = mediator;
            }
           
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create(CreateSubjectCommand command)
        {
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = result },
                new ApiResponse<int>
                {
                    Success = true,
                    Message = "Subject created successfully",
                    Data = result
                });
        }

        [HttpGet]
      
        public async Task<ActionResult<ApiResponse<IEnumerable<Subject>>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllSubjectQuery());

            return Ok(new ApiResponse<IEnumerable<Subject>>
            {
                Success = true,
                Message = "Subjects fetched successfully",
                Data = result
            });
        }

        [HttpGet("{id}")]
       
        public async Task<ActionResult<ApiResponse<Subject>>> GetById(int id)
        {
            var result = await _mediator.Send(new GetSubjectByIdQuery(id));

            return Ok(new ApiResponse<Subject>
            {
                Success = true,
                Message = "Subject fetched successfully",
                Data = result
            });
        }

        [HttpPut("{id}")]
      
        public async Task<IActionResult> Update(int id, UpdateSubjectCommand command)
        {
            if (id != command.Id)
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "ID mismatch"
                });

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
       
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteSubjectCommand(id));
            return NoContent();
        }
    }
    }
