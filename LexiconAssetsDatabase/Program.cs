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
        if(updatedAsset is null) 
            return;

        Assets.Update(updatedAsset);

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
    static async Task Main(string[] args)
    {
        await using var dbContext = new AssetDbContext();
        Console.WriteLine("Welcome to the asset manager");

        // Check if assets exist in the database
        if (!await dbContext.Assets.AnyAsync())
        {
            // Add initial testing assets if there are none
            dbContext.Assets.AddRange(
                new Asset { Name = "Laptop", Office = "Miami", Price = 999.99, PurchaseDate = DateTime.Now },
                new Asset { Name = "Mobile Phone", Office = "Madrid", Price = 499.99, PurchaseDate = DateTime.Now }
            );

            await dbContext.SaveChangesAsync();
        }
    
        var allAssets = await dbContext.GetAllAssets();
        PrintAllAssets(allAssets);
    }
    
    static void PrintAllAssets(List<Asset> assets)
    {
        if (assets.Count == 0)
        {
            Console.WriteLine("No assets to print");
            return;
        }

        Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-20}",
            "OFFICE",
            "NAME",
            "PRICE",
            "PURCHASE DATE",
            "EXPIRY DATE"
        );

        foreach (var asset in assets)
        {
            Console.WriteLine("{0,-20} {1,-20} {2,-20:C} {3,-20:d} {4,-20:d}",
                asset.Office,
                asset.Name,
                asset.Price,
                asset.PurchaseDate,
                asset.PurchaseDate.AddYears(3)
            );
        }
    }
}