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
        var result = await _client.AnalyzeAsync(
            BinaryData.FromStream(imageStream),
            VisualFeatures.Read,
            new ImageAnalysisOptions { Language = "en" },
            ct
        );
        return string.Join(
            "\n",
            result.Value.Read.Blocks.SelectMany(b => b.Lines).Select(l => l.Text)
        );
    }
}
