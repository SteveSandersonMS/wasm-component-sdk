// I don't think this namespace should be so different to the one in MyFuncsWorldImpl,
// but currently that's what the codegen requires
using wit_producer.Wit.exports.test.multipleWorlds.SomeStuff;

namespace wit_producer;

public class SomeStuffImpl : SomeStuff
{
    public static int GetNumber()
    {
        return 456;
    }
}
