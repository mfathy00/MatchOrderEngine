using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class OrderBookLoader
{
    public static IEnumerable<Order> LoadFromCsv(string[] lines)
    {
        if (lines == null || lines.Length == 0)
            throw new ArgumentException("CSV content is empty", nameof(lines));

        if (lines.Length == 1)
            throw new ArgumentException("CSV contains only header row", nameof(lines));

        return lines.Skip(1).Select((line, index) =>
        {
            try
            {
                var parts = line.Split(',');
                if (parts.Length != 6)
                    throw new FormatException($"Expected 6 columns but found {parts.Length}");

                return new Order(
                    parts[0]?.Trim() ?? throw new ArgumentException("CompanyId cannot be empty"),
                    parts[1]?.Trim() ?? throw new ArgumentException("OrderId cannot be empty"),
                    Enum.Parse<OrderDirection>(parts[2]?.Trim() ?? throw new ArgumentException("Direction cannot be empty")),
                    int.Parse(parts[3]?.Trim() ?? throw new ArgumentException("Volume cannot be empty")),
                    decimal.Parse(parts[4]?.Trim() ?? throw new ArgumentException("Notional cannot be empty")),
                    DateTime.Parse(parts[5]?.Trim() ?? throw new ArgumentException("OrderDateTime cannot be empty"))
                );
            }
            catch (Exception ex)
            {
                throw new FormatException($"Error parsing line {index + 2}: {line}", ex);
            }
        });
    }

    /// <summary>
    /// Asynchronously reads all lines from the specified CSV file and parses them into <see cref="Order"/> objects.
    /// </summary>
    public static async Task<IEnumerable<Order>> LoadFromCsvAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        if (!File.Exists(path))
            throw new FileNotFoundException($"CSV file not found: {path}");

        var lines = await File.ReadAllLinesAsync(path, cancellationToken).ConfigureAwait(false);
        return LoadFromCsv(lines);
    }
}
