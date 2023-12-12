using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CharacterAnalyser;

public class AnalyserService : IAnalyserService
{
    private readonly ILogger<AnalyserService> _logger;
    private readonly IAnalyser _analyser;

    public AnalyserService(ILogger<AnalyserService> logger, IAnalyser analyser)
    {
        _logger = logger;
        _analyser = analyser;
    }

    public async Task Run()
    {
        while (true)
        {
            var worldIds = await _analyser.GetDistinctWorldIdsFromRemainingScans();
            if (worldIds.Count == 0)
            {
                return;
            }

            try
            {
                foreach (var worldId in worldIds)
                {
                    var stopwatch = Stopwatch.StartNew();

                    var worldScans = await _analyser.GetWorldScansToAnalyseAsync(worldId);
                    await _analyser.Seed(worldScans);

                    stopwatch.Stop();
                    _logger.LogInformation("{methodName} execution time - WorldScan({worldScanId}) - World({worldId}): {time} ms.",
                        nameof(AnalyserService), worldScans[0].WorldScanId, worldId, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Execution of {methodName} failed", nameof(AnalyserService));
            }
        }
    }
}