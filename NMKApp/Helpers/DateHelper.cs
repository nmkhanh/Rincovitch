using NMKApp.Models;

namespace NMKApp.Helpers;

public record HolidayResponse(DateTime Date, string LocalName, string Name);

public static class DateHelper
{
  public static async Task<List<HolidayResponse>> GetHolidaysAsync(int year)
  {
    var result = new List<HolidayResponse>();
    try
    {
      // TODO: Uncomment when API is needed
      // using var client = new HttpClient();
      // result = await client.GetFromJsonAsync<List<HolidayResponse>>($"https://date.nager.at/api/v3/PublicHolidays/{year}/VN");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Trace.WriteLine(ex.Message);
    }
    return result;
  }

  public static List<ScheduleModel> BuildDateSchedules(DateTime dateStart, DateTime dateEnd)
  {
    var result = new List<ScheduleModel>();
    if (dateEnd <= dateStart) return result;

    DateTime currentDate = dateStart.Date;

    while (currentDate <= dateEnd.Date)
    {
      if (currentDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
      {
        currentDate = currentDate.AddDays(1);
        continue;
      }

      DateTime workMorningStart = currentDate.AddHours(8).AddMinutes(30);
      DateTime workMorningEnd = currentDate.AddHours(12).AddMinutes(30);
      DateTime workAfternoonStart = currentDate.AddHours(13).AddMinutes(30);
      DateTime workAfternoonEnd = currentDate.AddHours(17).AddMinutes(30);

      DateTime actualStart = dateStart > workMorningStart ? dateStart : workMorningStart;
      DateTime actualEnd = dateEnd < workAfternoonEnd ? dateEnd : workAfternoonEnd;

      double totalHours = 0;

      if (actualEnd > actualStart)
      {
        DateTime morningStart = actualStart > workMorningStart ? actualStart : workMorningStart;
        DateTime morningEnd = actualEnd < workMorningEnd ? actualEnd : workMorningEnd;
        if (morningEnd > morningStart)
          totalHours += (morningEnd - morningStart).TotalHours;

        DateTime afternoonStart = actualStart > workAfternoonStart ? actualStart : workAfternoonStart;
        DateTime afternoonEnd = actualEnd < workAfternoonEnd ? actualEnd : workAfternoonEnd;
        if (afternoonEnd > afternoonStart)
          totalHours += (afternoonEnd - afternoonStart).TotalHours;
      }

      if (totalHours > 0)
      {
        result.Add(new ScheduleModel
        {
          Date = currentDate,
          Time = Math.Round(totalHours, 2),
        });
      }

      currentDate = currentDate.AddDays(1);
    }

    return result;
  }

  public static DayModel EndDayNotWeek(DateTime minDay, int day)
  {
    if (minDay.DayOfWeek == DayOfWeek.Sunday) minDay = minDay.AddDays(1);
    else if (minDay.DayOfWeek == DayOfWeek.Saturday) minDay = minDay.AddDays(2);

    if (day is 0 or 1)
      return new DayModel { Name = minDay };

    for (int i = 1; i < day;)
    {
      minDay = minDay.AddDays(1);
      if (minDay.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
        i++;
    }

    return new DayModel { Name = minDay };
  }

  public static List<DayModel> CreateDayListWeek(DateTime minDay, DateTime maxDay)
  {
    if (minDay > maxDay) throw new ArgumentException("minDay must be <= maxDay");

    return Enumerable
      .Range(0, (maxDay.Date - minDay.Date).Days + 1)
      .Select(i => minDay.Date.AddDays(i))
      .Where(d => d.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
      .Select(d => new DayModel { Name = d })
      .ToList();
  }

  public static List<DayModel> CreateDayListNotWeek(DateTime minDay, DateTime maxDay)
  {
    if (minDay.DayOfWeek == DayOfWeek.Sunday) minDay = minDay.AddDays(1);
    else if (minDay.DayOfWeek == DayOfWeek.Saturday) minDay = minDay.AddDays(2);

    if (maxDay.DayOfWeek == DayOfWeek.Saturday) maxDay = maxDay.AddDays(-1);
    else if (maxDay.DayOfWeek == DayOfWeek.Sunday) maxDay = maxDay.AddDays(-2);

    if (minDay > maxDay) throw new ArgumentException("minDay must be <= maxDay");

    return Enumerable
      .Range(0, (maxDay.Date - minDay.Date).Days + 1)
      .Select(i => minDay.Date.AddDays(i))
      .Where(d => d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
      .Select(d => new DayModel { Name = d })
      .ToList();
  }

  public static List<DayModel> CreateDayList(DateTime minDay, DateTime maxDay)
  {
    if (minDay > maxDay) throw new ArgumentException("minDay must be <= maxDay");

    return Enumerable
      .Range(0, (maxDay.Date - minDay.Date).Days + 1)
      .Select(i => minDay.Date.AddDays(i))
      .Select(d => new DayModel { Name = d })
      .ToList();
  }

  public static List<DayModel> CreateDayListByMonth()
  {
    int year = DateTime.Now.Year;
    int month = DateTime.Now.Month;

    return Enumerable
      .Range(1, DateTime.DaysInMonth(year, month))
      .Select(day => new DateTime(year, month, day))
      .Where(d => d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
      .Select(d => new DayModel { Name = d })
      .ToList();
  }

  public static (DateTime Start, DateTime End) GetWeekdaysOfWeek(int year, int week)
  {
    DateTime jan1 = new DateTime(year, 1, 1);

    int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
    if (daysOffset > 0) daysOffset -= 7;

    DateTime firstMonday = jan1.AddDays(daysOffset);
    DateTime start = firstMonday.AddDays((week - 1) * 7);
    DateTime end = start.AddDays(4); // Friday

    return (start, end);
  }
}
