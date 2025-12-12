using CsvHelper.Configuration;

namespace Assecor.Api.Infrastructure.CSV;

public sealed class CsvPersonMap : ClassMap<CsvPerson>
{
    public CsvPersonMap()
    {
        Map(static m => m.LastName).Index(0).Default(string.Empty);
        Map(static m => m.FirstName).Index(1).Default(string.Empty);
        Map(static m => m.Address).Index(2).Optional();
        Map(static m => m.ColorId).Index(3).Optional();
    }
}
