namespace AuthorizationDemo.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required String Password { get; set; }
        public int Age { get; set; }
    }
}
