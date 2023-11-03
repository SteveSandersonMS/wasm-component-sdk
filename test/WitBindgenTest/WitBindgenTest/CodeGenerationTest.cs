using Xunit;

namespace WitBindgenTest;

public class CodeGenerationTest
{
    [Fact]
    public void GeneratesSimpleImport()
    {
        // The fact that this compiles shows that the codegen worked
        var ex = Assert.Throws<DllNotFoundException>(() =>
            LibraryUsingWit.Code.CallSimpleDoSomething());

        // Currently, it generates [DllImport("*", ...)] so validate that
        Assert.StartsWith("Unable to load DLL '*'", ex.Message);
    }
}
