using System.Diagnostics;
using System.Reflection;
using Xunit;

namespace PackageTest;

public class CompositionTest
{
    [Fact]
    public void CanComposeImportWithExportE2E()
    {
        var composed = FindModulePath("../testapps/E2EConsumer", "composed.wasm");
        var stdout = ExecuteCommandComponent(composed);
        Assert.StartsWith("Hello, world on Wasm", stdout);
        Assert.Contains("123 + 456 = 579", stdout);
    }

    private static string WasmtimeExePath { get; } = GetAssemblyMetadataValue("WasmtimeExe")!;

    private static string? GetAssemblyMetadataValue(string key) =>
        typeof(CompositionTest).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .SingleOrDefault(x => x.Key == key)?.Value;

    private static string ExecuteCommandComponent(string componentFilePath)
    {
        var startInfo = new ProcessStartInfo(WasmtimeExePath, $"-W component-model {componentFilePath}") { RedirectStandardOutput = true };
        var stdout = Process.Start(startInfo)!.StandardOutput.ReadToEnd();
        return stdout;
    }

    private static string FindModulePath(string searchDir, string filename)
    {
        var resolvedSearchDir = Path.Combine(
            Path.GetDirectoryName(typeof(CompositionTest).Assembly.Location)!,
            "../../..",
            searchDir);

        if (!Directory.Exists(resolvedSearchDir))
        {
            throw new InvalidOperationException($"No such directory: {Path.GetFullPath(resolvedSearchDir)}");
        }

        var matches = Directory.GetFiles(resolvedSearchDir, filename, SearchOption.AllDirectories);
        return Path.GetFullPath(matches.Single());
    }
}
