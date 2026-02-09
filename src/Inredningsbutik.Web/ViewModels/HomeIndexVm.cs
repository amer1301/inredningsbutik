using Inredningsbutik.Core.Entities;

namespace Inredningsbutik.Web.ViewModels;

public class HomeIndexVm
{
    public string? Q { get; set; }
    public int? CategoryId { get; set; }

    public List<Category> Categories { get; set; } = [];
    public List<Product> Products { get; set; } = [];
}
