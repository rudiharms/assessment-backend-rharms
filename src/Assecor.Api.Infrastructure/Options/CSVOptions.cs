namespace Assecor.Api.Infrastructure.Options;

public class CsvOptions
{
    public const string SectionName = "CsvOptions";

    public required string FilePath { get; set; }
    public required string Delimiter { get; set; }
}
