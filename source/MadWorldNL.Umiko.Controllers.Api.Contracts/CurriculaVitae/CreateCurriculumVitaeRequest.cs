using System.ComponentModel.DataAnnotations;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class CreateCurriculumVitaeRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
}