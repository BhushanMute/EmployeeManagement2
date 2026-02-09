using EmployeeManagement.API.Models;
using EmployeeManagement.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _repo;

    public EmployeeController(IEmployeeRepository repo)
    {
        _repo = repo;
    }

     
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

     
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _repo.GetById(id);
        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

     
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee emp)
    {
        await _repo.Add(emp);

        // Optional: return the created object
        return CreatedAtAction(nameof(GetById), new { id = emp.Id }, emp);
    }

   
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Employee emp)
    {
        emp.Id = id;
        await _repo.Update(emp);
        return Ok();
    }

     
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.Delete(id);
        return Ok();
    }
}