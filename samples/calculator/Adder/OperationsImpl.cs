namespace wit_computer.Wit.exports.example.calculator.Operations;

public class OperationsImpl : Operations
{
    public static int Add(int left, int right)
    {
        return left + right;
    }

    public static string ToUpper(string input)
    {
        return input.ToUpperInvariant();
    }
}
