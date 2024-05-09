namespace ThetHtarThuZArDotNetCore.MvcApp.Models.ResponseModel;

public class UpdateUserResponseModel
{
    public long UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
}