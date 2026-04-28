namespace LowlandTech.Components.ContainerGrid;

/// <summary>Layout snapshot for a single ContainerGridSection.</summary>
public record ContainerSectionLayout(string Id, int Row, int Column, int RowSpan, int ColSpan);

/// <summary>Full layout snapshot for a ContainerGrid and all its sections.</summary>
public record ContainerGridLayout(int Columns, int Rows, List<ContainerSectionLayout> Sections);
