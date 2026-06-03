namespace LowlandTech.Components.RepositoryApp;

public partial class RepositoryAppMain
{
    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "flex min-h-0 min-w-0 flex-1 flex-col overflow-hidden"
        : $"flex min-h-0 min-w-0 flex-1 flex-col overflow-hidden {Class}";
}
