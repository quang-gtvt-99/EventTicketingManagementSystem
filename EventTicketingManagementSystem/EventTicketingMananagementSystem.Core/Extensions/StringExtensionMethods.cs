namespace EventTicketingMananagementSystem.Core.Extensions
{
    public static class StringExtensionMethods
    {
        public static string GetContentType(this string fileName)
        {
            // Simple content type mapping. Extend as needed.
            var types = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                                {
                                    { ".txt", "text/plain" },
                                    { ".pdf", "application/pdf" },
                                    { ".doc", "application/vnd.ms-word" },
                                    { ".docx", "application/vnd.ms-word" },
                                    { ".xls", "application/vnd.ms-excel" },
                                    { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                                    { ".png", "image/png" },
                                    { ".jpg", "image/jpeg" },
                                    { ".jpeg", "image/jpeg" },
                                    { ".gif", "image/gif" },
                                    { ".csv", "text/csv" }
                                    // Add more mappings as needed
                                };

            var extension = Path.GetExtension(fileName);
            return types.ContainsKey(extension) ? types[extension] : "application/octet-stream";
        }
    }
}
