namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class GetCurriculumVitaeResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}