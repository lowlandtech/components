using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Calendar;

/// <summary>
/// Represents a date range with start and end dates.
/// </summary>
public class DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public bool IsInRange(DateTime date)
    {
        return date.Date >= Start.Date && date.Date <= End.Date;
    }

    public bool IsStart(DateTime date)
    {
        return date.Date == Start.Date;
    }

    public bool IsEnd(DateTime date)
    {
        return date.Date == End.Date;
    }
}

/// <summary>
/// Calendar selection modes.
/// </summary>
public enum CalendarMode
{
    Single,
    Range,
    Multiple
}

/// <summary>
/// Extends the auto-generated Calendar partial class with mode support.
/// </summary>
public partial class Calendar
{
    private static readonly string[] MonthNames =
    {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    private CalendarMode _mode = CalendarMode.Single;
    private DateTime? _selectedDate;
    private DateRange? _selectedRange;
    private List<DateTime> _selectedDates = new();
    private DateTime? _rangeStart;

    /// <summary>
    /// The selection mode: "single" (default), "range", or "multiple".
    /// </summary>
    [Parameter]
    public CalendarMode Mode
    {
        get => _mode;
        set => _mode = value;
    }

    /// <summary>
    /// The selected date in single mode.
    /// </summary>
    [Parameter]
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set => _selectedDate = value;
    }

    /// <summary>
    /// Event callback when selected date changes in single mode.
    /// </summary>
    [Parameter] public EventCallback<DateTime?> SelectedDateChanged { get; set; }

    /// <summary>
    /// The selected date range in range mode.
    /// </summary>
    [Parameter]
    public DateRange? SelectedRange
    {
        get => _selectedRange;
        set => _selectedRange = value;
    }

    /// <summary>
    /// Event callback when selected range changes in range mode.
    /// </summary>
    [Parameter] public EventCallback<DateRange?> SelectedRangeChanged { get; set; }

    /// <summary>
    /// The selected dates in multiple mode.
    /// </summary>
    [Parameter]
    public List<DateTime> SelectedDates
    {
        get => _selectedDates;
        set => _selectedDates = value ?? new List<DateTime>();
    }

    /// <summary>
    /// Event callback when selected dates change in multiple mode.
    /// </summary>
    [Parameter] public EventCallback<List<DateTime>> SelectedDatesChanged { get; set; }

    /// <summary>
    /// Event callback when any date selection changes (works for all modes).
    /// </summary>
    [Parameter] public EventCallback<DateChangedEventArgs> DateChanged { get; set; }

    /// <summary>
    /// Whether to show days outside the current month.
    /// </summary>
    [Parameter] public bool ShowOutsideDays { get; set; } = true;

    /// <summary>
    /// Whether to show preset quick-select buttons.
    /// </summary>
    [Parameter] public bool ShowPresets { get; set; }

    /// <summary>
    /// The list of preset items to display for quick selection.
    /// </summary>
    [Parameter] public List<CalendarPresetItem>? Presets { get; set; }

    /// <summary>
    /// Orientation of preset buttons: "horizontal" or "vertical".
    /// </summary>
    [Parameter] public string PresetsOrientation { get; set; } = "vertical";

    /// <summary>
    /// Variant style for preset buttons.
    /// </summary>
    [Parameter] public string PresetsVariant { get; set; } = "ghost";

    /// <summary>
    /// Size of preset buttons.
    /// </summary>
    [Parameter] public string PresetsSize { get; set; } = "sm";

    /// <summary>
    /// Whether to always show 6 weeks (fixed height).
    /// </summary>
    [Parameter] public bool FixedWeeks { get; set; } = true;

    /// <summary>
    /// Whether to render a month dropdown in the header.
    /// </summary>
    [Parameter] public bool ShowMonthDropdown { get; set; }

    /// <summary>
    /// Whether to render a year dropdown in the header.
    /// </summary>
    [Parameter] public bool ShowYearDropdown { get; set; }

    /// <summary>
    /// Function to determine if a date should be disabled.
    /// </summary>
    [Parameter] public Func<DateTime, bool>? IsDateDisabled { get; set; }

    /// <summary>
    /// Function to determine if a date is highlighted (e.g., today).
    /// </summary>
    [Parameter] public Func<DateTime, bool>? IsDateHighlighted { get; set; }

    /// <summary>
    /// Currently displayed month.
    /// </summary>
    [Parameter]
    public DateTime CurrentMonth { get; set; } = DateTime.Today;

    /// <summary>
    /// Navigate to the previous month.
    /// </summary>
    internal void PreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        StateHasChanged();
    }

    /// <summary>
    /// Navigate to the next month.
    /// </summary>
    internal void NextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        StateHasChanged();
    }

    /// <summary>
    /// Navigate to the previous year.
    /// </summary>
    internal void PreviousYear()
    {
        CurrentMonth = CurrentMonth.AddYears(-1);
        StateHasChanged();
    }

    /// <summary>
    /// Navigate to the next year.
    /// </summary>
    internal void NextYear()
    {
        CurrentMonth = CurrentMonth.AddYears(1);
        StateHasChanged();
    }

    /// <summary>
    /// Navigate to a specific month.
    /// </summary>
    internal void GoToMonth(DateTime month)
    {
        CurrentMonth = new DateTime(month.Year, month.Month, 1);
        StateHasChanged();
    }

    internal void GoToMonthNumber(int month)
    {
        CurrentMonth = new DateTime(CurrentMonth.Year, month, 1);
        StateHasChanged();
    }

    internal void GoToYear(int year)
    {
        CurrentMonth = new DateTime(year, CurrentMonth.Month, 1);
        StateHasChanged();
    }

    /// <summary>
    /// Navigate to today.
    /// </summary>
    internal void GoToToday()
    {
        CurrentMonth = DateTime.Today;
        StateHasChanged();
    }

    /// <summary>
    /// Handle preset selection and update the calendar.
    /// </summary>
    public async Task HandlePresetSelected(CalendarPresetItem preset)
    {
        // Navigate to the preset date's month
        CurrentMonth = new DateTime(preset.Date.Year, preset.Date.Month, 1);
        
        // Select the date
        await SelectDateAsync(preset.Date);
        
        StateHasChanged();
    }

    /// <summary>
    /// Select a date based on the current mode.
    /// </summary>
    internal async Task SelectDateAsync(DateTime date)
    {
        if (IsDateDisabled?.Invoke(date) == true)
            return;

        switch (Mode)
        {
            case CalendarMode.Single:
                await SelectSingleDateAsync(date);
                break;
            case CalendarMode.Range:
                await SelectRangeDateAsync(date);
                break;
            case CalendarMode.Multiple:
                await SelectMultipleDateAsync(date);
                break;
        }

        await DateChanged.InvokeAsync(new DateChangedEventArgs
        {
            Date = date,
            Mode = Mode,
            SelectedDate = SelectedDate,
            SelectedRange = SelectedRange,
            SelectedDates = SelectedDates
        });
    }

    private async Task SelectSingleDateAsync(DateTime date)
    {
        SelectedDate = date;
        await SelectedDateChanged.InvokeAsync(SelectedDate);
    }

    private async Task SelectRangeDateAsync(DateTime date)
    {
        if (_rangeStart == null || (_selectedRange != null && _rangeStart != null))
        {
            // Start new range
            _rangeStart = date;
            SelectedRange = new DateRange { Start = date, End = date };
        }
        else
        {
            // Complete range
            if (date < _rangeStart.Value)
            {
                SelectedRange = new DateRange { Start = date, End = _rangeStart.Value };
            }
            else
            {
                SelectedRange = new DateRange { Start = _rangeStart.Value, End = date };
            }
            _rangeStart = null;
        }
        await SelectedRangeChanged.InvokeAsync(SelectedRange);
    }

    private async Task SelectMultipleDateAsync(DateTime date)
    {
        var existing = SelectedDates.FirstOrDefault(d => d.Date == date.Date);
        if (existing != default)
        {
            SelectedDates.Remove(existing);
        }
        else
        {
            SelectedDates.Add(date);
        }
        await SelectedDatesChanged.InvokeAsync(SelectedDates);
    }

    /// <summary>
    /// Check if a date is selected based on the current mode.
    /// </summary>
    internal bool IsSelected(DateTime date)
    {
        return Mode switch
        {
            CalendarMode.Single => SelectedDate.HasValue && SelectedDate.Value.Date == date.Date,
            CalendarMode.Range => SelectedRange?.IsInRange(date) == true,
            CalendarMode.Multiple => SelectedDates.Any(d => d.Date == date.Date),
            _ => false
        };
    }

    /// <summary>
    /// Check if a date is the start of a range.
    /// </summary>
    internal bool IsRangeStart(DateTime date)
    {
        return Mode == CalendarMode.Range && SelectedRange?.IsStart(date) == true;
    }

    /// <summary>
    /// Check if a date is the end of a range.
    /// </summary>
    internal bool IsRangeEnd(DateTime date)
    {
        return Mode == CalendarMode.Range && SelectedRange?.IsEnd(date) == true;
    }

    /// <summary>
    /// Check if today is highlighted for a date.
    /// </summary>
    internal bool IsToday(DateTime date)
    {
        if (IsDateHighlighted != null)
            return IsDateHighlighted(date);
        return date.Date == DateTime.Today;
    }

    /// <summary>
    /// Check if a date is outside the current month.
    /// </summary>
    internal bool IsOutsideCurrentMonth(DateTime date)
    {
        return date.Month != CurrentMonth.Month || date.Year != CurrentMonth.Year;
    }

    /// <summary>
    /// Check if a date is disabled.
    /// </summary>
    internal bool IsDisabled(DateTime date)
    {
        return IsDateDisabled?.Invoke(date) == true;
    }

    internal IReadOnlyList<string> GetMonthNames() => MonthNames;

    internal IEnumerable<int> GetSelectableYears()
    {
        var startYear = CurrentMonth.Year - 50;
        var endYear = CurrentMonth.Year + 50;
        return Enumerable.Range(startYear, endYear - startYear + 1);
    }

    /// <summary>
    /// Get all weeks for the current month view.
    /// </summary>
    internal List<List<DateTime>> GetWeeks()
    {
        var weeks = new List<List<DateTime>>();
        var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        var daysInMonth = DateTime.DaysInMonth(CurrentMonth.Year, CurrentMonth.Month);
        var startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        var currentDay = firstDayOfMonth.AddDays(-startDayOfWeek);

        int weekCount = FixedWeeks ? 6 : GetWeekCount(firstDayOfMonth, daysInMonth, startDayOfWeek);

        for (int week = 0; week < weekCount; week++)
        {
            var weekDays = new List<DateTime>();
            for (int day = 0; day < 7; day++)
            {
                weekDays.Add(currentDay);
                currentDay = currentDay.AddDays(1);
            }
            weeks.Add(weekDays);
        }

        return weeks;
    }

    private int GetWeekCount(DateTime firstDayOfMonth, int daysInMonth, int startDayOfWeek)
    {
        int totalCells = startDayOfWeek + daysInMonth;
        return (int)Math.Ceiling(totalCells / 7.0);
    }

    private string GetClass()
    {
        var baseClass = "cn-calendar group/calendar bg-background";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }
}

/// <summary>
/// Event arguments for date change events.
/// </summary>
public class DateChangedEventArgs
{
    public DateTime Date { get; set; }
    public CalendarMode Mode { get; set; }
    public DateTime? SelectedDate { get; set; }
    public DateRange? SelectedRange { get; set; }
    public List<DateTime> SelectedDates { get; set; } = new();
}
