namespace Mechera.Sec.Data.Models;

public partial class User : IKeyProvider<string>
{
    public string Provide() => Username;    
}
