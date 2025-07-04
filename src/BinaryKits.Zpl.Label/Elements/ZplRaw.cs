namespace BinaryKits.Zpl.Label.Elements;

using System.Collections.Generic;

public class ZplRaw : ZplElementBase
{
    public string RawContent { get; private set; }

    public ZplRaw(string rawZplCode)
    {
        RawContent = rawZplCode;
    }

    ///<inheritdoc/>
    public override IEnumerable<string> Render(ZplRenderOptions context)
    {
        return new[] { RawContent };
    }
}
