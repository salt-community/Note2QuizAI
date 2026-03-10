namespace Note2Quiz.API.Interfaces;

public interface IVisionService
{
    Task<string> ExtractTextFromImageAsync(Stream imageStream, CancellationToken ct);
}
