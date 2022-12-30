using System; 
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace bridgefluence_api.Tests;

public class Seed
{ 
    public DateTime MasterDateTime = new DateTime(2021, 1, 1).AsUtc();
  
    public Seed()
    {
        AssertionOptions.AssertEquivalencyUsing(options =>
        {
            options.Using<DateTime>(ctx => ctx.Subject.Should()
                    .BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(999)))
                .WhenTypeIs<DateTime>();
            options.Using<DateTimeOffset>(ctx => ctx.Subject.Should()
                    .BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(999)))
                .WhenTypeIs<DateTimeOffset>();
            return options;
        });
    }

    public DataContext GetTestContext()
    {
        var connStr = "Host=localhost;Database=test;Username=postgres;Password=Password123";

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(connStr!)
            .Options;

        return new DataContext(options);
    }

    // public void Clear(DataContext context)
    // {
    //     foreach (var tableName in new string[]
    //              {
    //                  "brand",
    //                  "publisher", 
    //              })
    //     {
    //         context.Database.ExecuteSqlRaw("TRUNCATE TABLE \"" + tableName + "\" RESTART IDENTITY CASCADE;");
    //     }
    // }
}