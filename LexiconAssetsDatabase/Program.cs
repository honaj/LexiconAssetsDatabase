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
    
    // C
    public async Task CreateAsset(Asset newAsset)
    {
        Assets.Add(newAsset);
        await SaveChangesAsync();
    }

    // R
    public async Task<Asset> GetAssetByID(int assetId)
    {
        return await Assets.FindAsync(assetId);
    }
    
    // U
    public async Task UpdateAsset(Asset updatedAsset)
    {
        var currentAsset = await GetAssetByID(updatedAsset.Id);
        
        if(currentAsset is null) 
            return;

        currentAsset.Name = updatedAsset.Name;
        currentAsset.Office = updatedAsset.Office;
        currentAsset.Price = updatedAsset.Price;
        currentAsset.PurchaseDate = updatedAsset.PurchaseDate;

        await SaveChangesAsync();
    }
    
    // D
    public async Task DeleteAsset(int assetId)
    {
        var assetToDelete = await GetAssetByID(assetId);
        
        if(assetToDelete is null)
            return;

        Assets.Remove(assetToDelete);
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