using Microsoft.EntityFrameworkCore;

namespace MadWorldNL.Umiko;

public class UmikoContext : DbContext
{
    public UmikoContext(DbContextOptions<UmikoContext> options) : base(options)
    {
    }
}