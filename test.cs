using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

class Program {
    static void Main() {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), ""Mystic-Journey-API""))
            .AddJsonFile(""appsettings.json"")
            .AddJsonFile(""appsettings.Development.json"", optional: true);
        var config = builder.Build();
        var connStr = config.GetConnectionString(""DefaultConnection"");
        Console.WriteLine(""ConnStr: "" + connStr);
    }
}
