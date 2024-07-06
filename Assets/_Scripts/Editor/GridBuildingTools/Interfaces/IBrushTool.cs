public interface IBrushTool
{
    void IncreaseBrushSize();
    void DecreaseBrushSize();
    bool isPainting { get; }
    void StopPainting();
}