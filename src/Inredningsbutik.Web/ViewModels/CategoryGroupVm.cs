namespace Inredningsbutik.Web.ViewModels;

public class CategoryGroupVm
{
    public string Title { get; set; } = "";
    public List<CategoryItemVm> Items { get; set; } = [];
}

public class CategoryItemVm
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public int ProductCount { get; set; }
}
