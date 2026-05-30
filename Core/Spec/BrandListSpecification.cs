using Core.Entities;

namespace Core.spec;


public class BrandListSpecification :BaseSpecification<Product,string>
{
    public BrandListSpecification()
    {
        AddSelect(x => x.Brand);
        ApplyDistinct();
    }
}