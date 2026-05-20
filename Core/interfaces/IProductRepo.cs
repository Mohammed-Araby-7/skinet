namespace Core.interfaces;
using Core.Entities;

public interface IProductRepo
{

    Task<IReadOnlyList<Product>> GetProductsAsync(string? brand , string? type,string? sort);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypeAsync();
    Task<Product?> GetProductByIdAsync(int id);
    void AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(Product product);
    bool ProductExists(int id);
    Task<bool> SaveChangeAsync();
    
    
}