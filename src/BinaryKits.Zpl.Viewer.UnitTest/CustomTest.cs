namespace BinaryKits.Zpl.Viewer.UnitTest;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CustomTest
{
    [TestMethod]
    public void Custom()
    {
        string zplString = Common.LoadZPL("custom");
        Common.DefaultPrint(zplString, "custom.png");
    }
}
