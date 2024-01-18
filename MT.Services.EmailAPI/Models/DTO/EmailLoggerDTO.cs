namespace MT.Services.EmailAPI.Models.DTO;

public class EmailLoggerDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }
    public DateTime? EmailSent { get; set; }
}