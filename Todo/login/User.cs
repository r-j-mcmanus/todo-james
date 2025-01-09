
public class User{
     public Guid Id { get; set; }
     required public string UserName { get; set; }
     required public string HashedPassword { get; set; }
     public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
};
