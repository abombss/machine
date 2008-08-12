using System;

namespace Machine.CastleMonoRail.Calendars.Monthly
{
  public class MonthlyColumnParameters
  {
    private readonly DayOfWeek _day;

    public DayOfWeek DayOfWeek
    {
      get { return _day; }
    }

    public MonthlyColumnParameters(DayOfWeek day)
    {
      _day = day;
    }
  }
}