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
        public async Task<ActionResult> Create(CreateSemesterCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = null,
                    Message = "Semester added successfully"
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

        // ✅ GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Semesters>>> GetAllSemesters()
        {
            var result = await _mediator.Send(new GetAllSemesterQuery());

            return Ok(new ApiResponse<IEnumerable<Semesters>>
            {
                Success = true,
                Message = "Semesters fetched successfully",
                Data = result
            });
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Semesters>> GetById(int id)
        {
            var result = await _mediator.Send(new GetSemesterByIdQuery(id));

            if (result == null)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Semester not found"
                });

            return Ok(new ApiResponse<Semesters>
            {
                Success = true,
                Message = "Semester fetched successfully",
                Data = result
            });
        }

        // ✅ DELETE (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteSemesterCommand(id));

            if (!result)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Semester not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Semester deleted successfully",
                Data = "Deleted"
            });
        }

        // ✅ UPDATE
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

            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Semester not found"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Semester updated successfully",
                Data = "Updated"
            });
        }
    }
}
