using AuthBox.Models.Enums;

namespace AuthBox.Models.Dtos;
public class ResponseDto
{
    public EResponseStatus Status { get; set; }
    public object? Object { get; set; }
}
