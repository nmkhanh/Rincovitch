using RincovitchApp.Models.ModelChilds;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace RincovitchApp.API.Date
{
  public record HolidayResponse(DateTime Date, string LocalName, string Name);
  public static class F_Date
  {
    public static async Task<List<HolidayResponse>> GetHolidaysAsync(int year)
    {
      var result = new List<HolidayResponse>();

      try
      {
        //using var client = new HttpClient();
        //result = await client.GetFromJsonAsync<List<HolidayResponse>>("https://date.nager.at/api/v3/PublicHolidays/2026/VN");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }

      return result;
    }

    public static List<NMK_M_DateSchedule> BuildDateSchedules(DateTime dateStart, DateTime dateEnd)
    {
      var result = new List<NMK_M_DateSchedule>();

      if (dateEnd <= dateStart)
        return result;

      DateTime currentDate = dateStart.Date;

      while (currentDate <= dateEnd.Date)
      {
        // ❌ Bỏ thứ 7 và chủ nhật
        if (currentDate.DayOfWeek == DayOfWeek.Saturday ||
            currentDate.DayOfWeek == DayOfWeek.Sunday)
        {
          currentDate = currentDate.AddDays(1);
          continue;
        }

        DateTime workMorningStart = currentDate.AddHours(8).AddMinutes(30);
        DateTime workMorningEnd = currentDate.AddHours(12).AddMinutes(30);

        DateTime workAfternoonStart = currentDate.AddHours(13).AddMinutes(30);
        DateTime workAfternoonEnd = currentDate.AddHours(17).AddMinutes(30);

        // Giới hạn thời gian thực tế trong ngày
        DateTime actualStart = dateStart > workMorningStart ? dateStart : workMorningStart;
        DateTime actualEnd = dateEnd < workAfternoonEnd ? dateEnd : workAfternoonEnd;

        double totalHours = 0;

        if (actualEnd > actualStart)
        {
          // ---- Ca sáng ----
          DateTime morningStart = actualStart > workMorningStart ? actualStart : workMorningStart;
          DateTime morningEnd = actualEnd < workMorningEnd ? actualEnd : workMorningEnd;

          if (morningEnd > morningStart)
            totalHours += (morningEnd - morningStart).TotalHours;

          // ---- Ca chiều ----
          DateTime afternoonStart = actualStart > workAfternoonStart ? actualStart : workAfternoonStart;
          DateTime afternoonEnd = actualEnd < workAfternoonEnd ? actualEnd : workAfternoonEnd;

          if (afternoonEnd > afternoonStart)
            totalHours += (afternoonEnd - afternoonStart).TotalHours;
        }

        if (totalHours > 0)
        {
          result.Add(new NMK_M_DateSchedule
          {
            Date = currentDate,
            Time = Math.Round(totalHours, 2),
          });
        }

        currentDate = currentDate.AddDays(1);
      }

      return result;
    }


    public static NMK_M_Day EndDayNotWeek(DateTime minDay, int Day)
    {
      if (minDay.DayOfWeek == DayOfWeek.Sunday)
        minDay = minDay.AddDays(1);
      else if (minDay.DayOfWeek == DayOfWeek.Saturday)
        minDay = minDay.AddDays(2);

      if (Day == 0 || Day == 1)
        return new NMK_M_Day { Name = minDay };

      for (int i = 1; i < Day; )
      {
        minDay = minDay.AddDays(1);
        if (minDay.DayOfWeek != DayOfWeek.Saturday && minDay.DayOfWeek != DayOfWeek.Sunday)
          i++;
      }

      return new NMK_M_Day { Name = minDay };
    }

    public static List<NMK_M_Day> CreateDayListWeek(DateTime minDay, DateTime maxDay)
    {
      if (minDay > maxDay)
        throw new ArgumentException("minDay phải nhỏ hơn hoặc bằng maxDay");

      return Enumerable
          .Range(0, (maxDay.Date - minDay.Date).Days + 1)
          .Select(i => minDay.Date.AddDays(i))
          .Where(d => d.DayOfWeek == DayOfWeek.Saturday
                 || d.DayOfWeek == DayOfWeek.Sunday)
          .Select(d => new NMK_M_Day { Name = d })
          .ToList();
    }

    public static List<NMK_M_Day> CreateDayListNotWeek(DateTime minDay, DateTime maxDay)
    {
      if (minDay.DayOfWeek == DayOfWeek.Sunday)
        minDay = minDay.AddDays(1);
      else if (minDay.DayOfWeek == DayOfWeek.Saturday)
        minDay = minDay.AddDays(2);

      if (maxDay.DayOfWeek == DayOfWeek.Saturday)
        maxDay = maxDay.AddDays(-1);
      else if (maxDay.DayOfWeek == DayOfWeek.Sunday)
        maxDay = maxDay.AddDays(-2);

      if (minDay > maxDay)
        throw new ArgumentException("minDay phải nhỏ hơn hoặc bằng maxDay");

      return Enumerable
          .Range(0, (maxDay.Date - minDay.Date).Days + 1)
          .Select(i => minDay.Date.AddDays(i))
          .Where(d => d.DayOfWeek != DayOfWeek.Saturday
                 && d.DayOfWeek != DayOfWeek.Sunday)
          .Select(d => new NMK_M_Day { Name = d })
          .ToList();
    }

    public static List<NMK_M_Day> CreateDayList(DateTime minDay, DateTime maxDay)
    {
      if (minDay > maxDay)
        throw new ArgumentException("minDay phải nhỏ hơn hoặc bằng maxDay");

      return Enumerable
          .Range(0, (maxDay.Date - minDay.Date).Days + 1)
          .Select(i => minDay.Date.AddDays(i))
          .Select(d => new NMK_M_Day { Name = d })
          .ToList();
    }

    public static List<NMK_M_Day> CreateDayListByMonth()
    {

      int year = DateTime.Now.Year;
      int month = DateTime.Now.Month;

      return Enumerable
          .Range(1, DateTime.DaysInMonth(year, month))
          .Select(day => new DateTime(year, month, day))
          .Where(d => d.DayOfWeek != DayOfWeek.Saturday
                 && d.DayOfWeek != DayOfWeek.Sunday)
          .Select(d => new NMK_M_Day { Name = d })
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
}
