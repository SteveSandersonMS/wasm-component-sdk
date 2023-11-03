using wit_hostapp.Wit.imports.example.calculator.Operations;

Console.WriteLine("Hello, World!");

var left = 123;
var right = 456;
var result = OperationsInterop.Add(left, right);
Console.WriteLine($"{left} + {right} = {result}");
