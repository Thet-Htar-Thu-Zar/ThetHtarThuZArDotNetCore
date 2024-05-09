namespace ThetHtarThuZArDotNetCore.MvcApp.Models.Entities
{
    public class UserModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string IsActive { get; set; }
    }
}
