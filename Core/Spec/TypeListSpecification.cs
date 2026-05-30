using Core.Entities;
using Core.spec;

namespace Core;

public class TypeListSpecification : BaseSpecification<Product,string>
{
    public TypeListSpecification()
    {
        AddSelect(x=> x.Type);
        ApplyDistinct();
    }
}