using API.RequestHelpers;
using Core;
using Core.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BaseApiController :ControllerBase
{
    protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepo<T> repo,
        ISpecification<T> spec , int pageIndex , int pageSize) where T : BaseEntity
    {
        var items = await repo.ListAsync(spec);
        var count = await repo.CountAsync(spec);

        var pagination = new Pagination<T>(pageIndex , pageSize , count , items);

        return Ok(pagination);
    }
}