using CampusTouch.Application.Features.Classes.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/classes")]
public class ClassesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClassesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateClassCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _mediator.Send(new GetAllClassesQuery()));
    }
    [HttpPut("{id}")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> Update(int id, UpdateClassCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteClassCommand { Id = id });
        return NoContent();
    }
}