using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CharacterAnalyser;

public class AnalyserService : IAnalyserService
{
    private readonly ILogger<AnalyserService> _logger;
    private readonly IAnalyser _analyser;

    private static bool _hasDataToAnalyse = true;

    public AnalyserService(ILogger<AnalyserService> logger, IAnalyser analyser)
    {
        _logger = logger;
        _analyser = analyser;
    }

    public async Task Run()
    {
        while (_hasDataToAnalyse)
        {
            await Analyse();
        }
    }

    private async Task Analyse()
    {
        _hasDataToAnalyse = await _analyser.HasDataToAnalyse();
        if (!_hasDataToAnalyse) return;
        try
        {
            foreach (var worldId in _analyser.UniqueWorldIds)
            {
                var stopwatch = Stopwatch.StartNew();

                var worldScans = await _analyser.GetWorldScansToAnalyseAsync(worldId);
                await _analyser.Seed(worldScans);

                stopwatch.Stop();
                _logger.LogInformation("{methodName} execution time - WorldScan({worldScanId}) - World({worldId}): {time} ms.",
                    nameof(Analyse), worldScans[0].WorldScanId, worldId, stopwatch.ElapsedMilliseconds);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Execution of {methodName} failed", nameof(Analyse));
        }
    }
}