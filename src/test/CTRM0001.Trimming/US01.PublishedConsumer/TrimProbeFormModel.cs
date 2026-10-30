using System.ComponentModel.DataAnnotations;

namespace LowlandTech.Components.Tests.CTRM0001.Trimming.US01.PublishedConsumer;

/// <summary>
/// The model the trim preservation probe binds to the DataForm. It carries a
/// required password-style property that starts empty and a ranged property that
/// starts out of range, so the probe observes both a set/read of a value and a
/// validation failure that the DataForm still reports.
/// </summary>
public sealed class TrimProbeFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Rating { get; set; } = 99;
}
