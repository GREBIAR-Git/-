using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class ContextDataBase : DbContext
{
    public ContextDataBase()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    public static string ConnectingString { get; set; } = string.Empty;

    public DbSet<Client> Clients { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Window> Windows { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Reserved> Reserveds { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<WeekDay> WeekDays { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<WeekDay>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Client>()
            .Property(c => c.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Client>().ToTable("Client")
            .HasKey(k => k.Id);

        modelBuilder.Entity<Organization>()
            .Property(c => c.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Organization>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Queue>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Window>()
            .HasKey(k => k.Id);

        modelBuilder.Entity<Reserved>().HasKey(c => new { c.ClientId, c.WindowId });

        modelBuilder.Entity<Favorite>().HasKey(c => new { c.ClientId, c.RoomId });

        modelBuilder.Entity<Room>()
            .HasOne(c => c.Organization)
            .WithMany(p => p.Rooms)
            .HasForeignKey(c => c.OwnerId);

        modelBuilder.Entity<Queue>()
            .HasOne(c => c.Room)
            .WithMany(p => p.Queues)
            .HasForeignKey(c => c.RoomId);

        modelBuilder.Entity<Window>()
            .HasOne(c => c.Queue)
            .WithMany(p => p.Windows)
            .HasForeignKey(c => c.QueueId);

        modelBuilder.Entity<Window>()
            .HasOne(c => c.WeekDay)
            .WithMany(p => p.Windows)
            .HasForeignKey(c => c.WeekDayId);

        modelBuilder.Entity<Reserved>()
            .HasOne(c => c.Window)
            .WithMany(p => p.Reserveds)
            .HasForeignKey(c => c.WindowId);

        modelBuilder.Entity<Reserved>()
            .HasOne(c => c.Client)
            .WithMany(p => p.Reserveds)
            .HasForeignKey(c => c.ClientId);


        modelBuilder.Entity<Favorite>()
            .HasOne(c => c.Client)
            .WithMany(p => p.Favorites)
            .HasForeignKey(c => c.ClientId);

        modelBuilder.Entity<Favorite>()
            .HasOne(c => c.Room)
            .WithMany(p => p.Favorites)
            .HasForeignKey(c => c.RoomId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectingString);
        }

        base.OnConfiguring(optionsBuilder);
    }
}