namespace Todo.test;

public class UnitTest1
{
    [Fact]
    public void TestValidPassword()
    {
        string real_password = "password";
        string hashed_password = PasswordHasher.HashPassword(real_password);

        bool is_correct_password = PasswordHasher.VerifyPassword(real_password, hashed_password);

        Assert.True(is_correct_password);
    }

    [Fact]
    public void TestInvalidPassword()
    {
        string real_password = "password";
        string hashed_password = PasswordHasher.HashPassword(real_password);
        string fake_password ="abc123";

        bool is_correct_password = PasswordHasher.VerifyPassword(fake_password, hashed_password);

        Assert.False(is_correct_password);
    }
}