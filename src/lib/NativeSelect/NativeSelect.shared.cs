namespace LowlandTech.Components;

public partial class NativeSelect
{
    private string SizeValue => string.IsNullOrWhiteSpace(size) ? "default" : size!;

    private string WrapperClass
    {
        get
        {
            // w-full so the wrapper fills the column it lives in (form
            // rows, grid cells, flex children). Was w-fit which made
            // the wrapper content-sized — fine for "shrink to label"
            // contexts but broken in "fill the row's value cell" layouts
            // where the chevron icon would clip awkwardly.
            const string baseClass = "cn-native-select-wrapper group/native-select relative inline-flex w-full has-[select:disabled]:opacity-50";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
