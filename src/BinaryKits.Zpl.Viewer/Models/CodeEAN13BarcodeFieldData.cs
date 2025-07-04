namespace BinaryKits.Zpl.Viewer.Models;

using BinaryKits.Zpl.Label;

public class CodeEAN13BarcodeFieldData : FieldDataBase
{
    public FieldOrientation FieldOrientation { get; set; }
    public int Height { get; set; }
    public bool PrintInterpretationLine { get; set; }
    public bool PrintInterpretationLineAboveCode { get; set; }
  
}
