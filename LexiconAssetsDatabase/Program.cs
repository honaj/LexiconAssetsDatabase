using Microsoft.EntityFrameworkCore;

public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Office { get; set; }
    public double Price { get; set; }
    public DateTime PurchaseDate { get; set; }
}

public class AssetDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Assets.db");
    }

    public async Task<List<Asset>> GetAllAssets()
    {
        return await Assets.ToListAsync();
    }

    public async Task CreateAsset(Asset newAsset)
    {
        Assets.Add(newAsset);
        await SaveChangesAsync();
    }
    
    
}

class Program
{
    static void Main(string[] args)
    {
        using (var dbContext = new AssetDbContext())
        {
            Console.WriteLine("Welcome to the asset manager");

            // Add initial testing assets
            dbContext.Assets.AddRange(
                new Asset { Name = "Laptop", Office = "Miami", Price = 999.99, PurchaseDate = DateTime.Now },
                new Asset { Name = "Mobile Phone", Office = "Madrid", Price = 499.99, PurchaseDate = DateTime.Now }
            );

            dbContext.SaveChanges();

            // Continue with your application logic here
        }
    }
}