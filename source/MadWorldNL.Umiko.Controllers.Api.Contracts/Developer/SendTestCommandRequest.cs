using System.ComponentModel.DataAnnotations;

namespace MadWorldNL.Umiko.Developer;

public sealed class SendTestCommandRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}