namespace FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
internal static class DateTimeExtensions
{
    public static System.DateTime TrimMilliSeconds(
        this System.DateTime datetime
        )
    {
        return new System.DateTime(
            year: datetime.Year,
            month: datetime.Month,
            day: datetime.Day,
            hour: datetime.Hour,
            minute: datetime.Minute,
            second: datetime.Second,
            millisecond: 0,
            kind: datetime.Kind
        );
    }
}
