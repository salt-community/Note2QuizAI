using Azure.AI.Vision.ImageAnalysis;
using Note2Quiz.API.Interfaces;

namespace Note2Quiz.API.Services;

public class VisionService : IVisionService
{
    private readonly ImageAnalysisClient _client;

    public VisionService(ImageAnalysisClient client)
    {
        _client = client;
    }

    public async Task<string> ExtractTextFromImageAsync(Stream imageStream, CancellationToken ct)
    {
        var data = await BinaryData.FromStreamAsync(imageStream, ct);

        var result = await _client.AnalyzeAsync(
            data,
            VisualFeatures.Read,
            cancellationToken: ct
        );

        var lines = result.Value.Read.Blocks
            .SelectMany(b => b.Lines)
            .Select(l => l.Text);

        return string.Join(" ", lines);
    }
}
