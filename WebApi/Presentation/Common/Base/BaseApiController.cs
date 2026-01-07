using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace coo.Presentation.Common.Base;

[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ISender _sender;
    public BaseApiController(ISender sender)
    {
        _sender = sender;
    }
} 