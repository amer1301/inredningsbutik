using System.ComponentModel.DataAnnotations;

namespace Inredningsbutik.Web.Areas.Admin.ViewModels;

public class SupportReplyVm
{
    [Required]
    public int TicketId { get; set; }

    [Required]
    public string Message { get; set; } = "";
}
