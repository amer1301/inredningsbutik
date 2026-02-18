using Inredningsbutik.Api.Dtos;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
public async Task<ActionResult<List<CategoryDto>>> GetAll()
{
    var categories = await _db.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .Select(c => new CategoryDto(c.Id, c.Name, c.Slug))
        .ToListAsync();

    return Ok(categories);
}
}
