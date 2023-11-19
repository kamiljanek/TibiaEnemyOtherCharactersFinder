using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace TibiaEnemyOtherCharactersFinder.Api.Controllers.v1;

[ApiController]
[EnableRateLimiting(ConfigurationConstants.GlobalRateLimiting)]
[Route("api/tibia-eocf/v{version:apiVersion}/[controller]")]
public class TibiaBaseController : ControllerBase
{
}