using System; 
using bridgefluence.Providers;
using bridgefluence.Tools;
using bridgefluence_api; 
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace bridgefluence.Controllers;

[ApiController]
[EnableCors("Allau")]
[Route("[controller]")]
public class BaseController : ControllerBase
{
    protected readonly DataContext db;

    protected readonly IDateTimeProvider dateTimeProvider;

    public BaseController(DataContext context, IDateTimeProvider dtProvider)
    {
        dateTimeProvider = dtProvider;
        db = context;
    }

    protected int GetUserId()
    {
        var authHeader = HttpContext.Request.Headers.Authorization;

        var result = UtilityMethods.ExtractUserId(authHeader);

        if (result == null)
        {
            throw new UnauthorizedAccessException("Couldn't extract user id from the following header " + authHeader);
        }

        return (int) result;
    } 
}