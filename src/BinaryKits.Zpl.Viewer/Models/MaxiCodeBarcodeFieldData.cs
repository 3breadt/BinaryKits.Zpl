namespace BinaryKits.Zpl.Viewer.Models;

using BinaryKits.Zpl.Label;

public class MaxiCodeBarcodeFieldData : FieldDataBase
{
    public int Mode { get; set; }
    public int Position { get; set; }
    public int Total { get; set; }
    public bool UseHexadecimalIndicator { get; set; }
}
