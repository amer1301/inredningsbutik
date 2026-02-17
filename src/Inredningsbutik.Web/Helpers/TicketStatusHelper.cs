namespace Inredningsbutik.Web.Helpers;

public static class TicketStatusHelper
{
    public static readonly Dictionary<string, string> Labels = new()
    {
        ["New"] = "Ny",
        ["Open"] = "Pågående",
        ["WaitingCustomer"] = "Väntar på kund",
        ["Resolved"] = "Löst",
        ["Closed"] = "Stängt"
    };

    public static string ToLabel(string status)
        => Labels.TryGetValue(status, out var label) ? label : status;
}
