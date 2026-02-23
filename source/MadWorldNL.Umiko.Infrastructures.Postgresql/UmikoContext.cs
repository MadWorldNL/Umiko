using Microsoft.EntityFrameworkCore;

namespace MadWorldNL.Umiko;

public sealed class UmikoContext : DbContext
{
    public UmikoContext(DbContextOptions<UmikoContext> options) : base(options)
    {
    }
}