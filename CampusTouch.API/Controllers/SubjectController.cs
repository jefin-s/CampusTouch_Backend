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
        public async Task<ActionResult> Create(CreateSubjectCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Subject added successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetAll()
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
        public async Task<ActionResult<Subject>> GetById(int id)
        {
            var result = await _mediator.Send(new GetSubjectByIdQuery(id));

            if (result == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Subject not found"
                });

            return Ok(new ApiResponse<Subject>
            {
                Success = true,
                Message = "Subject fetched successfully",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateSubjectCommand command)
        {
            if (id != command.Id)
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "ID mismatch"
                });

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Subject not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Subject updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteSubjectCommand(id));

            if (!result)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Subject not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Subject deleted successfully"
            });
        }
    }
}
