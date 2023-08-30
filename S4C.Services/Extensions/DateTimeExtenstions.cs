using System.Globalization;

namespace С4S.API.Extensions
{
    public static class DateTimeExtenstions
    {
        public static bool TryParseYandexDate(string source, out DateTime? result)
        {
            var months = new []
            {
                "янв.", "февр.", "мар.", "апр.", "мая", "июн.", "июл.", "авг.", "сент.", "окт.", "нояб.", "дек."
            };

            var dateTimeFormatInfo = new DateTimeFormatInfo
            {
                MonthNames = months,
                AbbreviatedMonthGenitiveNames = months,
                AbbreviatedMonthNames = months
            };

            var boolResult = true;
            DateTime? dateTime = null;

            try
            {
                dateTime = DateTime.ParseExact(source, "dd MMM yyyy 'г.'", dateTimeFormatInfo);
            }
            catch (Exception)
            {
                boolResult = false;
            }
            finally
            {
                result = dateTime;
            }

            return boolResult;

        }
    }
}
