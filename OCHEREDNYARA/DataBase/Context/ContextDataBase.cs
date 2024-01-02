using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class ContextDataBase : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Queue> Queues { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Window> Windows { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Reserved> Reserveds { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<WeekDay> WeekDays { get; set; }


    public ContextDataBase()
    {
        //ResetDB();
    }

    void ResetDB()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
        AutoComlitedDayOfWeek();
    }

    async void AutoComlitedDayOfWeek()
    {
        await AddWeekDay("Понедельник");
        await AddWeekDay("Вторник");
        await AddWeekDay("Среда");
        await AddWeekDay("Четверг");
        await AddWeekDay("Пятница");
        await AddWeekDay("Суббота");
        await AddWeekDay("Воскресенье");
    }

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

        modelBuilder.Entity<Reserved>().
            HasKey(c => new { c.ClientId, c.WindowId });

        modelBuilder.Entity<Favorite>().
            HasKey(c => new { c.ClientId, c.RoomId });

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
        //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Database1;Integrated Security=True");
    }

    async Task<bool> AddWeekDay(string name)
    {
        List<WeekDay> weekDays = await WeekDays.Where(p => p.Name == name).ToListAsync();
        if (weekDays.Count != 0)
        {
            WeekDay weekDay = new() { Name = name };
            await WeekDays.AddAsync(weekDay);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<List<Window>> GetWeekWindows(long idQueue)
    {
        return await Windows.Where(p => p.QueueId == idQueue).ToListAsync();
    }

    public async Task<WeekDay?> GetWeek(long id)
    {
        return await WeekDays.Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> AddWindow(long queueId, long weekDayId, DateTime? start, DateTime? end)
    {
        List<Window> windows = await Windows.Where(p => p.QueueId == queueId && p.WeekDayId == weekDayId && p.Start == start && p.End == end).ToListAsync();
        if (windows.Count != 0)
        {
            Window window = new() { QueueId = queueId, WeekDayId = weekDayId, Start = start, End = end };
            await Windows.AddAsync(window);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<Window?> GetWindow(long idWindow)
    {
        return await Windows.Where(p => p.Id == idWindow).FirstOrDefaultAsync();
    }

    public async Task<List<Window>> GetWindows(long idQueue, long idWeekDay)
    {
        return await Windows.Where(p => p.QueueId == idQueue && p.WeekDayId == idWeekDay).ToListAsync();
    }

    public async Task<bool> DeleteWindow(long id)
    {
        Window? window = await Windows.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (window is not null)
        {
            Windows.Remove(window);
            SaveChanges();
            return true;
        }
        return false;
    }

    public async Task<List<Reserved>> GetReserved(long idClient)
    {
        return await Reserveds.Where(p => p.ClientId == idClient).ToListAsync();
    }

    public async Task<List<Reserved>> GetReserveds(long idWindow)
    {
        return await Reserveds.Where(p => p.WindowId == idWindow).ToListAsync();
    }

    public async Task<bool> AddReserved(long idClient, long idWindow, DateTime dateTime)
    {
        List<Reserved> favorites = await Reserveds.Where(p => p.WindowId == idWindow && p.Date == dateTime).ToListAsync();
        if (favorites.Count == 0)
        {
            await Reserveds.AddAsync(new Reserved()
            {
                ClientId = idClient,
                WindowId = idWindow,
                Date = dateTime
            });
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteReserved(long idClient, long idWindow)
    {
        Reserved? reserved = await Reserveds.Where(p => p.ClientId == idClient && p.WindowId == idWindow).FirstOrDefaultAsync();
        if (reserved is not null)
        {
            Reserveds.Remove(reserved);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> AddFavorite(long idClient, long idRoom)
    {
        List<Favorite> favorites = await Favorites.Where(p => p.ClientId == idClient && p.RoomId == idRoom).ToListAsync();
        if (favorites.Count == 0)
        {
            await Favorites.AddAsync(new Favorite()
            {
                ClientId = idClient,
                RoomId = idRoom
            });
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteFavorite(long idClient, long idRoom)
    {
        Favorite? favorite = await Favorites.Where(p => p.ClientId == idClient && p.RoomId == idRoom).FirstOrDefaultAsync();
        if (favorite is not null)
        {
            Favorites.Remove(favorite);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> IsValidTime(long idQueue, long idWeekDay, DateTime? start, DateTime? end)
    {
        List<Window> windows = await GetWindows(idQueue, idWeekDay);
        if (start < end)
        {
            foreach (Window window in windows)
            {
                if (!(start < window.Start && end <= window.Start && start >= window.End && end > window.Start))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public async Task<bool> IsValidTime(long idWindow, DateTime? start, DateTime? end)
    {
        Window? oldWindow = await GetWindow(idWindow);
        if (oldWindow is not null)
        {
            List<Window> windows = await GetWindows(oldWindow.QueueId, oldWindow.WeekDayId);
            if (start < end)
            {
                foreach (Window window in windows)
                {
                    if (oldWindow.Id != window.Id)
                    {
                        if (!(start < window.Start && end <= window.Start && start >= window.End && end > window.Start))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
        return false;
    }

    public async Task<List<Room>> GetRooms(long id)
    {
        return await Rooms.Where(p => p.OwnerId == id).ToListAsync();
    }

    public async Task<Room?> GetRoom(long id)
    {
        return await Rooms.Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteRoom(long id)
    {
        Room? room = await Rooms.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (room is not null)
        {
            Rooms.Remove(room);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> AddRooms(long id, string name)
    {
        if (await IsOrganizationActiv(id))
        {
            Room room = new() { OwnerId = id, Name = name };
            await Rooms.AddAsync(room);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }

    }


    public async Task<bool> AddQueues(long id, long idRoom)
    {
        if (await IsOrganizationActiv(id))
        {
            Queue queue = new() { RoomId = idRoom };
            await Queues.AddAsync(queue);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<List<Queue>> GetQueues(long id)
    {
        return await Queues.Where(p => p.RoomId == id).ToListAsync();
    }

    public async Task<bool> DeleteQueue(long id)
    {
        Queue? queue = await Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            Queues.Remove(queue);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> ChangeThemeClient(long id, int number)
    {
        Client? client = await Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null)
        {
            client.Theme = number;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<List<Room>> SearchNameOrg(string name)
    {
        Organization? organization = await Organizations.Where(p => p.Name == name).FirstOrDefaultAsync();
        await Organizations.Include(u => u.Rooms).ToListAsync();
        List<Organization> organizations = await Organizations.ToListAsync();
        if (organization != null && organization.Rooms != null)
        {
            return organization.Rooms;
        }
        else
        {
            return new();
        }
    }

    public async Task<List<Room>> GetFavoritsRoom(long id)
    {
        List<Favorite> favorites = await Favorites.Where(p => p.ClientId == id).ToListAsync();
        List<Room> rooms = new();
        foreach (Favorite favorite in favorites)
        {
            Room? room = await Rooms.Where(p => p.Id == favorite.RoomId).FirstOrDefaultAsync();
            if (room is not null)
            {
                rooms.Add(room);
            }
        }
        return rooms;
    }

    public async Task<List<Room>> SearchIdOrg(long id)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        await Organizations.Include(u => u.Rooms).ToListAsync();

        if (organization is not null && organization.Rooms is not null)
        {
            await Rooms.Include(u => u.Favorites).ToListAsync();
            return organization.Rooms;
        }
        else
        {
            return new();
        }
    }

    public async Task<int> GetThemeClient(long id)
    {
        Client? client = await Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null)
        {
            return client.Theme;
        }
        return -1;
    }

    public async Task<int> GetThemeOrganization(long id)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            return organization.Theme;
        }
        return -1;
    }

    public async Task<string> GetThemeDataOrganization(long id)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null && organization.ThemeData is not null)
        {
            return organization.ThemeData;
        }
        return string.Empty;
    }


    public async Task<bool> ChangeThemeOrganization(long id, int number)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.Theme = number;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> ChangeThemeDataOrganization(long id, string data)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.ThemeData = data;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> AddPartThemeDataOrganization(long id, string data)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.ThemeData += "!" + data;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<Organization?> GetOrganization(long id)
    {
        return await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<bool> IsClientActiv(long id)
    {
        return await Clients.AnyAsync(o => o.Id == id);
    }

    public async Task<bool> AddClient(long id)
    {
        if (!await IsClientActiv(id))
        {
            Client client = new() { Id = id };
            await Clients.AddAsync(client);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> IsOrganizationActiv(long id)
    {
        return await Organizations.AnyAsync(o => o.Id == id);
    }

    public async Task<bool> AddOrganization(long id, string name)
    {
        if (!await IsOrganizationActiv(id))
        {
            Organization organization = new() { Id = id, Name = name };
            await Organizations.AddAsync(organization);
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> EditLevel(long id)
    {
        Client? client = await Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null && client.Level == 0)
        {
            client.Level = 1;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> EditRoomName(long id, string newName)
    {
        Room? room = await Rooms.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (room is not null)
        {
            room.Name = newName;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> EditOrganizationName(long id, string newName)
    {
        Organization? organization = await Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.Name = newName;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> FreezUnfreez(long id)
    {
        Queue? queue = await Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            queue.Freezing = !queue.Freezing;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> Freez(long id)
    {
        Queue? queue = await Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            queue.Freezing = true;
            SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }
}
