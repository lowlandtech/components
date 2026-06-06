namespace LowlandTech.Components;

public partial class WorkQueue : TinyComponentBase
{
    private static string SafeId(string id) => id.Length > 8 ? id[..8] + "..." : id;
}
