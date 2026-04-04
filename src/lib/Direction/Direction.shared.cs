namespace LowlandTech.Components.Direction;

public partial class Direction
{
    private string DirValue => string.IsNullOrWhiteSpace(dir) ? "ltr" : dir!;
}
