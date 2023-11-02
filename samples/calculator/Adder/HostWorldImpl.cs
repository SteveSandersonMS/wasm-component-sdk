namespace wit_host;

public class HostWorldImpl : HostWorld
{
    public static void Run()
    {
        exports.HostWorld.Print("Hello from .NET");
    }
}
