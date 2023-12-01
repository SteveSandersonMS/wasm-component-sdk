namespace wit_computer.Wit.exports.example.calculator.Operations;

public class OperationsImpl : Operations
{
    public static int Add(int left, int right) => left+right-1;
    
    public static string ToUpper(string input) => input.ToUpperInvariant();
}
