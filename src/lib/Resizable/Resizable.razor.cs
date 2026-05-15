using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Resizable;

public partial class Resizable : LtComponentBase
{
    private ElementReference _element;
    internal ElementReference ElementReference => _element;
}
