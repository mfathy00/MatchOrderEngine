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

        return lines.Skip(1).Select(line =>
        {
            var parts = line.Split(',');
            return new Order(
                parts[0],
                parts[1],
                Enum.Parse<OrderDirection>(parts[2]),
                int.Parse(parts[3]),
                decimal.Parse(parts[4]),
                DateTime.Parse(parts[5])
            );
        });
    }

    /// <summary>
    /// Asynchronously reads all lines from the specified CSV file and parses them into <see cref="Order"/> objects.
    /// </summary>
    public static async Task<IEnumerable<Order>> LoadFromCsvAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        var lines = await File.ReadAllLinesAsync(path, cancellationToken).ConfigureAwait(false);
        return LoadFromCsv(lines);
    }
}
