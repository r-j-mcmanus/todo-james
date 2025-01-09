using Microsoft.EntityFrameworkCore;

public static class UserAPI
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/register/", CreateUser).RequireCors();
        endpoints.MapPost("/login/", Login).RequireCors();
    }

    static async Task<IResult> CreateUser(LoginRequest loginRequest, UserDb db)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);

        if (existingUser != null)
        {
            return TypedResults.Conflict("Already a user with that username");
        }

        string hashedPassword = PasswordHasher.HashPassword(loginRequest.Password);

        var newUser = new User 
        {
            Id = Guid.NewGuid(),
            UserName = loginRequest.UserName,
            HashedPassword = hashedPassword,
        };

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return TypedResults.Created();
    }

    static async Task<IResult> Login(LoginRequest loginRequest, UserDb db)
    {
        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);

        if (existingUser == null)
        {
            return TypedResults.BadRequest();
        }

        string hashedPassword = PasswordHasher.HashPassword(loginRequest.Password);
        string storedHashedPassword = existingUser.HashedPassword;


        bool validPassword = PasswordHasher.VerifyPassword(hashedPassword, storedHashedPassword);

        if (validPassword)
        {
            string JWT = JsonWebToken.makeToken();
            return TypedResults.Ok( new {Token = JWT} );
        }
        else
        {
            return TypedResults.BadRequest();
        }
    }
}

