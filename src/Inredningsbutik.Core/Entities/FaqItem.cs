namespace Inredningsbutik.Core.Entities;

public class FaqItem
{
    public int Id { get; set; }

    // "populara" / "leveranser" / "retur" / "reklamation" / "betalning"
    public string CategoryKey { get; set; } = "";

    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";

    public bool IsPublished { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
