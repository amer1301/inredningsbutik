using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inredningsbutik.Web.Areas.Admin.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    public int StockQuantity { get; set; }

    public string? HoverImageUrl { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
}
