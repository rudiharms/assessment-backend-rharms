namespace Assecor.Api.Infrastructure.Options;

public class SqlOptions
{
    public const string SectionName = "SqlOptions";

    public required bool UseSql { get; set; }
}
