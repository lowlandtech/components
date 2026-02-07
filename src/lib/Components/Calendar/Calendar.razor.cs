using Microsoft.AspNetCore.Components;
using LowlandTech.Components.Utilities;

namespace LowlandTech.Components.Components.Calendar;

public partial class Calendar : ComponentBase
{
    /// <summary>
    /// The currently selected date.
    /// </summary>
    [Parameter]
    public DateTime? Value { get; set; }

    /// <summary>
    /// Event callback invoked when the selected date changes.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime?> ValueChanged { get; set; }

    /// <summary>
    /// Predicate function to determine if a date should be disabled.
    /// </summary>
    [Parameter]
    public Func<DateTime, bool>? DisabledDates { get; set; }

    /// <summary>
    /// Additional CSS class for the calendar root element.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Additional attributes to be applied to the calendar root element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// The month currently being displayed.
    /// </summary>
    private DateTime CurrentMonth { get; set; } = DateTime.Today;

    private string RootClass => CssClassUtils.Merge("calendar", Class);

    protected override void OnInitialized()
    {
        // Initialize CurrentMonth to the selected value's month, or today's month
        if (Value.HasValue)
        {
            CurrentMonth = new DateTime(Value.Value.Year, Value.Value.Month, 1);
        }
        else
        {
            CurrentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
    }

    private void NavigatePreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
    }

    private void NavigateNextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
    }

    private async Task HandleDayClick(CalendarDay day)
    {
        if (day.IsDisabled || day.IsOutsideMonth)
            return;

        Value = day.Date;
        await ValueChanged.InvokeAsync(Value);
    }

    private IEnumerable<CalendarDay> GetCalendarDays()
    {
        var days = new List<CalendarDay>();
        
        // Get the first day of the current month
        var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        
        // Get the last day of the current month
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        
        // Calculate how many days from the previous month to show
        // (0 = Sunday, 1 = Monday, etc.)
        var startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
        
        // Add days from previous month
        for (int i = startDayOfWeek - 1; i >= 0; i--)
        {
            var date = firstDayOfMonth.AddDays(-i - 1);
            days.Add(new CalendarDay
            {
                Date = date,
                IsOutsideMonth = true,
                IsDisabled = IsDateDisabled(date)
            });
        }
        
        // Add days of current month
        for (int day = 1; day <= lastDayOfMonth.Day; day++)
        {
            var date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, day);
            days.Add(new CalendarDay
            {
                Date = date,
                IsOutsideMonth = false,
                IsDisabled = IsDateDisabled(date)
            });
        }
        
        // Calculate how many days from next month to show (to fill the grid)
        var totalDaysShown = days.Count;
        var remainingDays = 42 - totalDaysShown; // 6 rows × 7 columns = 42 cells
        
        // If we already have more than 35 days (5 rows), only show 42 total
        // Otherwise show 35 (5 rows)
        if (totalDaysShown <= 35)
        {
            remainingDays = 35 - totalDaysShown;
        }
        
        // Add days from next month
        for (int i = 1; i <= remainingDays; i++)
        {
            var date = lastDayOfMonth.AddDays(i);
            days.Add(new CalendarDay
            {
                Date = date,
                IsOutsideMonth = true,
                IsDisabled = IsDateDisabled(date)
            });
        }
        
        return days;
    }

    private string GetDayClass(CalendarDay day)
    {
        var classes = new List<string> { "calendar-day" };
        
        if (day.IsOutsideMonth)
        {
            classes.Add("calendar-day-outside");
        }
        
        if (day.IsDisabled)
        {
            classes.Add("calendar-day-disabled");
        }
        
        if (Value.HasValue && day.Date.Date == Value.Value.Date)
        {
            classes.Add("calendar-day-selected");
        }
        
        if (day.Date.Date == DateTime.Today)
        {
            classes.Add("calendar-day-today");
        }
        
        return string.Join(" ", classes);
    }

    private bool IsDateDisabled(DateTime date)
    {
        return DisabledDates?.Invoke(date) ?? false;
    }

    /// <summary>
    /// Represents a single day in the calendar grid.
    /// </summary>
    private class CalendarDay
    {
        public DateTime Date { get; set; }
        public bool IsOutsideMonth { get; set; }
        public bool IsDisabled { get; set; }
    }
}
