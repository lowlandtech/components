namespace LowlandTech.Components.NativeSelect;

public partial class NativeSelect
{
    private string SizeValue => string.IsNullOrWhiteSpace(size) ? "default" : size!;

    private string WrapperClass
    {
        get
        {
            const string baseClass = "cn-native-select-wrapper group/native-select relative w-fit has-[select:disabled]:opacity-50";
            return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
        }
    }
}
