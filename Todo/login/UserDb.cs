using Microsoft.EntityFrameworkCore;

class UserDb:  DbContext
{
    public UserDb(DbContextOptions<TodoDb> options) : base(options){

    }

    public DbSet<User> Users => Set<User>();
}