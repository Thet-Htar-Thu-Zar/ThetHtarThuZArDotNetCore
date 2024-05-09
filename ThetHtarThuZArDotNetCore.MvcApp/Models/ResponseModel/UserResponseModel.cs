namespace ThetHtarThuZArDotNetCore.MvcApp.Models.ResponseModel;

public class UserResponseModel
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserRole { get; set; } = null!;
    public bool IsActive { get; set; }
}