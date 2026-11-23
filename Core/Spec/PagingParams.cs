namespace Core.spec;

public class PagingParams
{
    // here we choose the params insted of passing it to the controller as string? brand
    private const int MaxPageSize = 50;
    public int PageIndex {get;set;} = 1;
    private int _pageSize = 6;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}