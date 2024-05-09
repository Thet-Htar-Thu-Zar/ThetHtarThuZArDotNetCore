namespace ThetHtarThuZArDotNetCore.MvcApp.Models.Entities;

public class UserModel
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserRole { get; set; } = null!;
    public string IsActive { get; set; } = null!;
}