using Microsoft.Data.SqlClient;

namespace EmployeeManagement.API.Common
{
    public static class SqlDataReaderExtensions
    {
        public static int GetInt32(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            var value = reader.GetValue(ordinal);

            // Handle if database stores as string
            if (value is string strValue)
            {
                return int.Parse(strValue);
            }

            return Convert.ToInt32(value);
        }

        public static int? GetNullableInt32(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            // Handle if database stores as string
            if (value is string strValue)
            {
                return string.IsNullOrEmpty(strValue) ? null : int.Parse(strValue);
            }

            return Convert.ToInt32(value);
        }

        public static decimal GetDecimal(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            var value = reader.GetValue(ordinal);

            // Handle if database stores as string
            if (value is string strValue)
            {
                return decimal.Parse(strValue);
            }

            return Convert.ToDecimal(value);
        }

        public static decimal? GetNullableDecimal(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is string strValue)
            {
                return string.IsNullOrEmpty(strValue) ? null : decimal.Parse(strValue);
            }

            return Convert.ToDecimal(value);
        }

        public static bool GetBoolean(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            var value = reader.GetValue(ordinal);

            // Handle various formats
            if (value is string strValue)
            {
                return strValue == "1" || strValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            if (value is int intValue)
            {
                return intValue == 1;
            }

            return Convert.ToBoolean(value);
        }

        public static DateTime GetDateTime(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            var value = reader.GetValue(ordinal);

            if (value is string strValue)
            {
                return DateTime.Parse(strValue);
            }

            return Convert.ToDateTime(value);
        }

        public static DateTime? GetNullableDateTime(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            var value = reader.GetValue(ordinal);

            if (value is string strValue)
            {
                return string.IsNullOrEmpty(strValue) ? null : DateTime.Parse(strValue);
            }

            return Convert.ToDateTime(value);
        }

        public static string? GetNullableString(this SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            return reader.GetValue(ordinal)?.ToString();
        }
    }
}
