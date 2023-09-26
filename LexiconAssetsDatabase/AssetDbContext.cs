using Microsoft.EntityFrameworkCore;

// Class for interfacing between code and database
public class AssetDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Assets.db");
    }

    // Create
    public async Task CreateAsset(Asset newAsset)
    {
        Assets.Add(newAsset);
        await SaveChangesAsync();
    }

    // Read
    public async Task<List<Asset>> GetAllAssets()
    {
        return await Assets.ToListAsync();
    }

    public async Task<Asset?> GetAssetById(int assetId)
    {
        return await Assets.FindAsync(assetId);
    }
    
    // Update
    public async Task UpdateAsset(Asset updatedAsset)
    {
        if(updatedAsset == null) 
            return;

        Assets.Update(updatedAsset);

        await SaveChangesAsync();
    }
    
    // Delete
    public async Task DeleteAsset(int assetId)
    {
        var assetToDelete = await GetAssetById(assetId);
        
        if(assetToDelete is null)
            return;

        Assets.Remove(assetToDelete);
        await SaveChangesAsync();
    }
}
