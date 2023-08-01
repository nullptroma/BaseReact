using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseReact.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class DataController : ControllerBase
{
    [HttpGet]
    public IResult Get()
    {
        return Results.Json(new {message=$"Hello, {ControllerContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value}!"});
    }
}