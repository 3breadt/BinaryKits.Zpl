namespace BinaryKits.Zpl.Viewer.CommandAnalyzers;

using BinaryKits.Zpl.Label.Elements;

public class RecallFormatCommandAnalyzer : ZplCommandAnalyzerBase
{
    public RecallFormatCommandAnalyzer(VirtualPrinter virtualPrinter) : base("^XF", virtualPrinter) { }

    ///<inheritdoc/>
    public override ZplElementBase Analyze(string zplCommand)
    {
        string formatName = zplCommand.Substring(this.PrinterCommandPrefix.Length);

        return new ZplRecallFormat(formatName);
    }
}
