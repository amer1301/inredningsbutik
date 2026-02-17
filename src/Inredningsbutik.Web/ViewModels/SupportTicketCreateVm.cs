using System.ComponentModel.DataAnnotations;

namespace Inredningsbutik.Web.ViewModels;

public class SupportTicketCreateVm
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Subject { get; set; } = "";

    [Required]
    public string Message { get; set; } = "";
}
