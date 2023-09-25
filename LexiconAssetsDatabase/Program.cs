using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public enum AssetType
{
    Phone,
    Laptop
}

public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Office { get; set; }
    public decimal Price { get; set; }
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

    // C
    public async Task CreateAsset(Asset newAsset)
    {
        Assets.Add(newAsset);
        await SaveChangesAsync();
    }

    // R
    public async Task<List<Asset>> GetAllAssets()
    {
        return await Assets.ToListAsync();
    }

    public async Task<Asset?> GetAssetById(int assetId)
    {
        return await Assets.FindAsync(assetId);
    }
    
    // U
    public async Task UpdateAsset(Asset updatedAsset)
    {
        if(updatedAsset == null) 
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
    static async Task Main()
    {
        await using var dbContext = new AssetDbContext();
        Console.WriteLine("Welcome to the asset manager");

        // Check if assets exist in the database
        if (!await dbContext.Assets.AnyAsync())
        {
            // Add initial testing assets if there are none
            dbContext.Assets.AddRange(
                new Computer { Name = "Asus", Office = "Miami", Price = new decimal(999.9), PurchaseDate = new DateOnly(2020, 01, 01) },
                new Computer { Name = "Lenovo", Office = "Miami", Price = new decimal(999.99), PurchaseDate = new DateOnly(2021, 07, 15) },
                new Computer { Name = "MacBook", Office = "Miami", Price = new decimal(999.99), PurchaseDate = new DateOnly(2022, 09, 01) },
                new Phone { Name = "iPhone", Office = "Madrid", Price = new decimal(499.99), PurchaseDate = new DateOnly(2019, 05, 11) },
                new Phone { Name = "Samsung", Office = "Madrid", Price = new decimal(499.99), PurchaseDate = new DateOnly(2023, 04, 18) },
                new Phone { Name = "Nokia", Office = "Madrid", Price = new decimal(499.99), PurchaseDate = new DateOnly(2020, 11, 23) }
            );

            await dbContext.SaveChangesAsync();
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Asset Tracking Menu:");
            Console.WriteLine("(1) List All Assets");
            Console.WriteLine("(2) Create New Asset");
            Console.WriteLine("(3) Edit Asset");
            Console.WriteLine("(4) Delete Asset");
            Console.WriteLine("(5) Save and Quit");
            var allAssets = await dbContext.GetAllAssets();
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    PrintAllAssets(allAssets);
                    break;

                case "2":
                    var newAsset = CreateAsset();
                    if (newAsset != null)
                    {
                        await dbContext.CreateAsset(newAsset);
                        Console.WriteLine("Asset created successfully.");
                    }
                    break;

                case "3":
                    PrintAllAssets(allAssets);
                    Console.Write("Enter the asset ID to edit: ");
                    if (int.TryParse(Console.ReadLine(), out var editId))
                    {
                        var assetToEdit = await dbContext.GetAssetById(editId);
                        if (assetToEdit != null)
                        {
                            EditAsset(assetToEdit);
                            await dbContext.UpdateAsset(assetToEdit);
                            Console.WriteLine("Asset updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Asset not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid asset ID.");
                    }
                    break;

                case "4":
                    PrintAllAssets(allAssets);
                    Console.Write("Enter the asset ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out var deleteId))
                    {
                        await dbContext.DeleteAsset(deleteId);
                        Console.WriteLine("Asset deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid asset ID.");
                    }
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }

    // Create a new asset using console input
    static Asset CreateAsset()
    {
        //Validate type
        Console.Write("Enter asset type (Phone or Laptop): ");
        AssetType assetType;
        do
        {
            var input = Console.ReadLine()?.Trim();
            if (Enum.TryParse(input, true, out assetType))
            {
                break;
            }
            Console.WriteLine("Invalid input. Please enter a valid asset type (Phone or Laptop): ");
        } while (true);

        //Validate name
        string name;
        do
        {
            Console.Write("Enter asset name: ");
            name = Console.ReadLine().Trim();
            if (!string.IsNullOrEmpty(name))
            {
                break;
            }
            Console.WriteLine("Please enter an asset name");
        } while (true);

        // Validate office
        string office;
        do
        {
            var validOffices = new List<string> {"madrid", "malmö", "miami"};
            Console.Write("Enter asset office (Madrid, Malmö or Miami): ");
            office = Console.ReadLine().Trim();

            if (validOffices.Contains(office.ToLower()))
            {
                break;
            }
            Console.WriteLine("Please enter a valid office (Madrid, Malmö or Miami)");
        } while (true);

        // Validate price
        decimal price;
        do
        {
            Console.Write("Enter asset price: ");
            var input = Console.ReadLine()?.Trim();
            if (decimal.TryParse(input, out price) && price >= 0)
            {
                break;
            }
            Console.WriteLine("Invalid price. Please enter a valid non-negative number: ");
        } while (true);

        //Validate purchase date
        DateOnly purchaseDate;
        do
        {
            Console.Write("Enter purchase date (YYYY-MM-DD): ");
            var input = Console.ReadLine()?.Trim();
            if (DateOnly.TryParse(input, out purchaseDate))
            {
                break;
            }
            Console.WriteLine("Invalid date format. Please enter a valid date in YYYY-MM-DD format: ");
        } while (true);

        if (purchaseDate == default)
        {
            Console.WriteLine("Using today's date.");
            purchaseDate = DateOnly.FromDateTime(DateTime.Now);
        }

        switch (assetType)
        {
            case AssetType.Phone:
                return new Phone
                {
                    Name = name,
                    Office = office,
                    Price = price,
                    PurchaseDate = purchaseDate
                };
            case AssetType.Laptop:
                return new Computer
                {
                    Name = name,
                    Office = office,
                    Price = price,
                    PurchaseDate = purchaseDate
                };
            default:
                return null;
        }
    }

    //Edit a chosen asset
    static void EditAsset(Asset asset)
    {
        Console.Write($"Editing Asset (ID: {asset.Id})\n");
        Console.Write($"Name ({asset.Name}): ");
        var name = Console.ReadLine();
        if (!string.IsNullOrEmpty(name))
        {
            asset.Name = name;
        }

        Console.Write($"Office ({asset.Office}): ");
        var office = Console.ReadLine();
        if (!string.IsNullOrEmpty(office))
        {
            asset.Office = office;
        }

        Console.Write($"Price ({asset.Price}): ");
        if (decimal.TryParse(Console.ReadLine(), out var price) && price >= 0)
        {
            asset.Price = price;
        }

        Console.Write($"Purchase Date ({asset.PurchaseDate}): ");
        if (DateOnly.TryParse(Console.ReadLine(), out var purchaseDate))
        {
            asset.PurchaseDate = purchaseDate;
        }
    }

    // Print all assets in database, sorted and colored
    static void PrintAllAssets(IEnumerable<Asset> assets)
    {
        Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10} {4,-15} {5,-15} {6,-10}",
            "ID", "Type", "Name", "Office", "Price", "Purchase Date", "Expiry Date");

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        foreach (var asset in assets.OrderBy(a => a.Office).ThenBy(a => a.PurchaseDate))
        {
            var expiryDate = asset.PurchaseDate.AddYears(3);
            var monthsToDeprecation = (expiryDate.Year - currentDate.Year) * 12 + expiryDate.Month - currentDate.Month;

            Console.ForegroundColor = monthsToDeprecation switch
            {
                <= 3 => ConsoleColor.Red,
                <= 6 => ConsoleColor.Yellow,
                _ => Console.ForegroundColor
            };

            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-10} {4,-15} {5,-15:yyyy-MM-dd} {6,-10}",
                asset.Id, asset.GetType().Name, asset.Name, asset.Office, ConvertFromUsd(asset), asset.PurchaseDate, expiryDate);

            Console.ResetColor();
        }
    }

    // Mockup method to convert currencies. Using string matching is probably a bad idea though...
    static decimal ConvertFromUsd(Asset asset)
    {
        var conversionRate = asset.Office.ToLower() switch
        {
            "miami" => new decimal(1.0),
            "madrid" => new decimal(0.87),
            "malmö" => new decimal(11.5),
            _ => throw new ArgumentOutOfRangeException()
        };
        return asset.Price * conversionRate;
    }
}
