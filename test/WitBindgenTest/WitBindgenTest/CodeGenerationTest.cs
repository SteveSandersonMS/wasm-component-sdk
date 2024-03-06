using MyFuncsWorld.exports;
using ProducerWorld;
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
        Assert.StartsWith("Unable to load DLL 'do-something'", ex.Message);
    }

    [Fact]
    public void GeneratesSimpleExport()
    {
        // The fact that this compiles is what matters. There would be no point calling
        // the function to validate its behavior, as that has nothing to do with WIT codegen
        // and wouldn't even be running as WebAssembly.
        Assert.NotNull((Func<int>)MyFuncsWorldImpl.GetNumber);
    }

    [Fact]
    public void CanSpecifyWorld()
    {
        Assert.NotNull((Func<int>)SomeStuffImpl.GetNumber);
    }
}
