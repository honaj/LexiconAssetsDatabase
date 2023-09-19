using Microsoft.EntityFrameworkCore;

public enum AssetType
{
    Phone,
    Laptop
}

public class Asset
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Office { get; set; }
    public double Price { get; set; }
    public DateOnly PurchaseDate { get; set; }
}

public class Phone : Asset
{
    
}

public class Computer : Asset
{
    
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
    public async Task<Asset?> GetAssetById(int assetId)
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
        var assetToDelete = await GetAssetById(assetId);
        
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
                new Computer { Name = "Asus", Office = "Miami", Price = 999.99, PurchaseDate = new DateOnly(2020, 01, 01) },
                new Computer { Name = "Lenovo", Office = "Miami", Price = 999.99, PurchaseDate = new DateOnly(2021, 07, 15) },
                new Computer { Name = "MacBook", Office = "Miami", Price = 999.99, PurchaseDate = new DateOnly(2022, 09, 01) },
                new Phone { Name = "iPhone", Office = "Madrid", Price = 499.99, PurchaseDate = new DateOnly(2019, 05, 11) },
                new Phone { Name = "Samsung", Office = "Madrid", Price = 499.99, PurchaseDate = new DateOnly(2023, 04, 18) },
                new Phone { Name = "Nokia", Office = "Madrid", Price = 499.99, PurchaseDate = new DateOnly(2020, 11, 23) }
            );

            await dbContext.SaveChangesAsync();
        }
    
        var allAssets = await dbContext.GetAllAssets();
        PrintAllAssets(allAssets);
    }
    
    static void PrintAllAssets(IList<Asset?> assets)
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
    
        // Order the assets first by office then by purchase date
        var orderedAssets = assets.OrderBy(a => a.Office).ThenBy(a => a.PurchaseDate);

        foreach (var asset in orderedAssets)
        {
            // Check if the asset expiry date is less than 3 months or 6 months away from three years
            var expiryDate = asset.PurchaseDate.AddYears(3);
            var monthsToDeprecation = (expiryDate.Year - DateTime.Now.Year) * 12 + expiryDate.Month - DateTime.Now.Month;

            Console.ForegroundColor = monthsToDeprecation switch
            {
                // Change the color of output text based on asset expiry date
                <= 3 => ConsoleColor.Red,
                <= 6 => ConsoleColor.Yellow,
                _ => Console.ForegroundColor
            };

            // Convert price into local currency
            var localCurrency = ConvertToLocalCurrency(asset.Price, asset.Office);
        
            Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20:d} {4,-20:d}",
                asset.Office,
                asset.Name,
                localCurrency,
                asset.PurchaseDate,
                expiryDate
            );
        
            // Reset the console color
            Console.ResetColor();
        }
    }

    static double ConvertToLocalCurrency(double price, string office)
    {
        // This method is a dummy method. You have to implement the logic to convert the currency based on the office location.
        // The conversion rate will be different for each office depending on the country in which it is located.
        // For now, the method returns the price as it is.
        return price;
    }
}