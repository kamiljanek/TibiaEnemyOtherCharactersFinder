using Microsoft.AspNetCore.Mvc;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.v1;

[ApiController]
[Route("api/tibia-eocf/v{version:apiVersion}/[controller]")]
public class TibiaBaseController : ControllerBase
{
}