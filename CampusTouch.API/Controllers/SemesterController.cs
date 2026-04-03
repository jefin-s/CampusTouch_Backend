using CampusTouch.Application.Common;
using CampusTouch.Application.Features.Semester.Commands;
using CampusTouch.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CampusTouch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SemesterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int>>> Create(CreateSemesterCommand command)
        {
            var result=await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = result },
                new ApiResponse<int>
                {
                    Success = true,
                    Message = "Semester added successfully",
                    Data = result
                });

        }

       
        [HttpGet]
        public async Task<ActionResult<ApiResponse< IEnumerable<Semesters>>>> GetAllSemesters()
        {
            var result = await _mediator.Send(new GetAllSemesterQuery());

            return Ok(new ApiResponse<IEnumerable<Semesters>>
            {
                Success = true,
                Message = "Semesters fetched successfully",
                Data = result
            });
        }

      
        [HttpGet("{id}")]
        public async Task<ActionResult< ApiResponse<Semesters>>> GetById(int id)
        {
            var result = await _mediator.Send(new GetSemesterByIdQuery(id));

          

            return Ok(new ApiResponse<Semesters>
            {
                Success = true,
                Message = "Semester fetched successfully",
                Data = result
            });
        }

       
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
           await _mediator.Send(new DeleteSemesterCommand(id));
            return NoContent();

            
        }

      
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, UpdateSemesterCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "ID mismatch"
                });
            }

             await _mediator.Send(command);

            return NoContent();
        }
    }
}
