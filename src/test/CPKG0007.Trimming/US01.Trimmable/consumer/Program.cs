using LowlandTech.Components;

// The consumer uses one type of each shared assembly, so the trimmer has a reason to
// keep the assemblies and the measurement sees what remains of them.
var registry = new ComponentDescriptorRegistry();
var grid = new ContainerGrid();
grid.SetLayout(new ContainerGridLayout(2, 2, [new ContainerSectionLayout("s1", 1, 1, 1, 1)]));
var descriptor = ReflectionDynamicBlockDescriptor.Create(typeof(ProbeModel));

Console.WriteLine("COLUMNS=" + grid.Columns);
Console.WriteLine("DESCRIPTORS=" + registry.All.Count);
Console.WriteLine("FIELDS=" + descriptor.Fields.Count);

/// <summary>The entity model the descriptor reads.</summary>
public sealed class ProbeModel
{
    public string Name { get; set; } = "Ada";
}
