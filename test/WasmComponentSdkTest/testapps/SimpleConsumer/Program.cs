using System.Runtime.InteropServices;
using wit_consumer.Wit.imports.test.producerConsumer.Operations;

Console.WriteLine($"Hello, world on {RuntimeInformation.OSArchitecture}");

var result = OperationsInterop.Add(123, 456);
Console.WriteLine($"123 + 456 = {result}");
