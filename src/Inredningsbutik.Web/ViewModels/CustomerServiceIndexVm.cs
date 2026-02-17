using Inredningsbutik.Core.Entities;

namespace Inredningsbutik.Web.ViewModels;

public class CustomerServiceIndexVm
{
    public SupportTicketCreateVm Ticket { get; set; } = new();
    public Dictionary<string, List<FaqItem>> FaqByCategory { get; set; } = new();
}
