using Inredningsbutik.Web.ViewModels;
using Xunit;

public class PagedListVmTests
{
    [Fact]
    public void TotalPages_Calculates_Correctly()
    {
        var vm = new PagedListVm<int>
        {
            Items = new() { 1, 2 },
            Page = 1,
            PageSize = 25,
            TotalCount = 51
        };

        Assert.Equal(3, vm.TotalPages);
    }

    [Fact]
    public void HasNext_And_HasPrevious_Work()
    {
        var vm = new PagedListVm<int>
        {
            Items = new() { 1 },
            Page = 2,
            PageSize = 10,
            TotalCount = 25
        };

        Assert.True(vm.HasPrevious);
        Assert.True(vm.HasNext);
    }
}
