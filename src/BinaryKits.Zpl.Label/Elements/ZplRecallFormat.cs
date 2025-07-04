namespace BinaryKits.Zpl.Label.Elements;

using System;
using System.Collections.Generic;

public class ZplRecallFormat : ZplElementBase
{
    public string FormatName { get; private set; }

    public ZplRecallFormat(string formatName)
    {
        FormatName = formatName;
    }

    public override IEnumerable<string> Render(ZplRenderOptions context)
    {
        throw new InvalidOperationException();
    }
}
