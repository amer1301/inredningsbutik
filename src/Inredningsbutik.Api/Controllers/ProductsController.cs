using Inredningsbutik.Api.Dtos;
using Inredningsbutik.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inredningsbutik.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/products?q=...&categoryId=...&page=1&pageSize=25
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll(
        [FromQuery] string? q,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        // Guards (skyddar mot konstiga värden)
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 5 or > 200 ? 25 : pageSize;

        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p =>
                p.Name.Contains(term) ||
                p.Description.Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        // Total count innan paging
        var total = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageUrl,
                p.StockQuantity,
                p.CategoryId,
                p.Category!.Name
            ))
            .ToListAsync();

        return Ok(new PagedResponse<ProductDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    // GET /api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.ImageUrl,
                p.StockQuantity,
                p.CategoryId,
                p.Category!.Name
            ))
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found.",
                Type = "https://httpstatuses.com/404",
                Detail = $"Product with id {id} was not found.",
                Instance = HttpContext.Request.Path
            });
        }

        return Ok(product);
    }
}
