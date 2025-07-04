namespace BinaryKits.Zpl.Viewer.CommandAnalyzers;

using BinaryKits.Zpl.Label.Elements;

public class FieldReversePrintZplCommandAnalyzer : ZplCommandAnalyzerBase
{
    public FieldReversePrintZplCommandAnalyzer(VirtualPrinter virtualPrinter) : base("^FR", virtualPrinter) { }

    ///<inheritdoc/>
    public override ZplElementBase Analyze(string zplCommand)
    {
        this.VirtualPrinter.SetNextElementFieldReverse();

        return null;
    }
}
