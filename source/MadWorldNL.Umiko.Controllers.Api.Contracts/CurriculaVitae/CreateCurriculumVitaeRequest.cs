using System.ComponentModel.DataAnnotations;

namespace MadWorldNL.Umiko.CurriculaVitae;

public sealed class CreateCurriculumVitaeRequest
{
    [Required]
    [MinLength(1)]
    [RegularExpression(@".*\S.*", ErrorMessage = "The field cannot be empty or consist only of whitespace.")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    [RegularExpression(@".*\S.*", ErrorMessage = "The field cannot be empty or consist only of whitespace.")]
    public string LastName { get; set; } = string.Empty;
}