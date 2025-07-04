namespace Application.UseCase.ZplToPdf;

using BinaryKits.Zpl.Viewer.Models;

public interface IZplAnalyzer
{
    public AnalyzeInfo Analyze(string zplData);
}
