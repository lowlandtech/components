namespace LowlandTech.Components.Work;

public partial class WorkList : TinyComponentBase
{
    private static string SafeId(string id) => id.Length > 8 ? id[..8] + "..." : id;
}
