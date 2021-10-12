namespace FiasGarImporter.Events
{
    public record ProgressEventArgs(double Percentage, bool IsFinished = false);
}
