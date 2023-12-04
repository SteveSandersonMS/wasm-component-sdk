# WasmComponent.Sdk

**An experimental package to simplify building WASI preview 2 components using .NET, including early support for WIT files.**

The build output is fully AOT compiled and is known to work in recent versions of wasmtime and WAMR.

## Purpose

This is to simplify experimentation and prototyping.

Without this package, if you wanted to build a WASI preview 2 component with .NET, including using WIT imports/exports, there are about 5 different tools you'd need to discover, download, configure, and manually chain together. Just figuring out which versions of each are compatible with the others is a big challenge. Working out how to get started would be very painful.

With this package, you can add one NuGet reference and then get on with your experiments.

## Support

**No support!** This is for experimentation. All the underlying technologies are under heavy development and are missing key features. When you encounter an issue, which you absolutely will, please try to file it on the relevant underlying tool (see credits below). Only file issues on this repo if you're sure the problem is here in this repo.

## Getting started

**Limitation**: Although the resulting `.wasm` files can run on any OS, [the compiler itself is currently limited to Windows](https://github.com/dotnet/runtimelab/issues/1890#issuecomment-1221602595). Hopefully that limitation will be resolved soon, since everything else in the toolchain is cross-platform.

### 1. Set up SDKs

If you don't already have it, install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Also install an up-to-date Python 3.x. For example on Windows, [install Python from the Microsoft Store](https://www.microsoft.com/store/productId/9NCVDN91XZQP), and make sure it's available on your `PATH` (for example: check `python --version` prints a version). This is only required temporarily (a bug in Clang for WASI SDK means we require Emscripten, which in turn requires Python).

### 2. Create a project and add WasmComponent.Sdk package

* `dotnet new console -o MyApp`
* `cd MyApp`
* `dotnet new nugetconfig`
* `dotnet nuget add source --name dotnet8-experimental https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-experimental/nuget/v3/index.json` 
* `dotnet add package WasmComponent.Sdk --prerelease`

### 3. Configure the compilation output

Edit the `.csproj` file, adding the following inside the `<PropertyGroup>`:

```xml
    <RuntimeIdentifier>wasi-wasm</RuntimeIdentifier>
    <UseAppHost>false</UseAppHost>
    <PublishTrimmed>true</PublishTrimmed>
    <InvariantGlobalization>true</InvariantGlobalization>
```

Now you can `dotnet build` to produce a `.wasm` file using NativeAOT compilation.

**Troubleshooting:** If you get the error *'python' is not recognized as an internal or external command*, go back and install Python as mentioned above. Also delete the `.emsdk` directory inside your user profile directory, then try again.

### 4. Run the WebAssembly binary

If you have [wasmtime 14.0.4](https://github.com/bytecodealliance/wasmtime/releases/tag/v14.0.4) on your path, you can now run

    wasmtime bin\Debug\net8.0\wasi-wasm\native\MyApp.wasm

(if needed, replace `MyApp.wasm` with the actual name of your project)

## Creating a WASI Preview 2 component, including WIT support

This is much more advanced and is likely to break frequently, since the underlying tool ecosystem is continually changing.

The compilation above will also have generated `MyApp.component.wasm`, which is a WASI preview 2 component. You can also run that if you want, using `wasmtime --wasm component-model bin\Debug\net8.0\wasi-wasm\native\MyApp.component.wasm`.

**Troubleshooting:** If you get an error like *import 'wasi:...' has the wrong type*, you need a different version of Wasmtime. Currently this package targets [Wasmtime 14.0.4](https://github.com/bytecodealliance/wasmtime/releases/tag/v14.0.4). Unfortunately WASI preview 2 is not yet stable, and every time the standard APIs change shape, compatibility is lost. The spec should become stable in the coming months. I will update this periodically.

### Referencing a WIT file

The whole point of the WASI preview 2 component model is to be able to interoperate across components. This is achieved using [WebAssembly Interface Type (WIT)](https://github.com/WebAssembly/component-model/blob/main/design/mvp/WIT.md) files that specify data structures and functions to be imported or exported across components.

This package wraps `wit-bindgen` so that any `.wit` files in your project will automatically generate corresponding C# sources, allowing you to import or export functionality. **Caution:** wit-bindgen's support for C# is *extremely early* and many definitions do not yet work.

For example, add a file called `calculator.wit` into your project, containing:

```
package example:calculator;

interface operations {
  add: func(left: s32, right: s32) -> s32;
}

world computer {
  export operations;
}

world hostapp {
  import operations;
}
```

Before you can build, you'll now need to specify which *world* you're generating code for, i.e., choose whether you're importing or exporting functions.

#### Importing an implementation

One way to pick a world is by editing your `.csproj`, adding the following:

```xml
  <ItemGroup>
    <Wit Update="calculator.wit" World="hostapp" />
  </ItemGroup>
```

Another option, if you use Visual Studio, is to select the WIT file in *Solution Explorer*, and then look at the *Properties* pane:

![image](https://github.com/dotnet/runtimelab/assets/1101362/86a204d1-985d-4d36-8bbd-5581375d989e)

You can simply type the world name `hostapp` into the properties pane.

Now you can call the imported `Add` function by putting the following in `Program.cs`:

```cs
using wit_hostapp.Wit.imports.example.calculator.Operations;

var result = OperationsInterop.Add(123, 456);
Console.WriteLine($"The result is {result}");
```

Since your component is no longer a self-contained application, you can no longer run it without also composing it with another WASI preview 2 component that implements the `add` function. To do that, either:

 * Create another .NET project and this time follow the steps for "exporting an implementation" below
 * Or, read docs for other platforms such as Rust or TinyGo, to implement a WASI component containing the implementation.

Once you have a component containing the implementation, you can use [wasm-tools](https://github.com/bytecodealliance/wasm-tools) to compose a runnable application:

```
wasm-tools compose -o composed.wasm MyApp.component.wasm -d AddImplementation.component.wasm
```

... then run it:

```
wasmtime --wasm component-model composed.wasm
```

#### Exporting an implementation

If you're **exporting** functionality, you'll be building a class library, not an executable. So be sure to go to your `.csproj` and change `<OutputType>` from `exe` to `library` and delete any `Program.cs`.

Once you've done that, change your WIT file to use the `calculator` world using one of the two techniques described above (i.e., either edit the `.csproj` or use the VS Properties pane).

Now when you build, you'll get an error like `The name 'OperationsImpl' does not exist in the current context`. This is because you've said you'll provide an implementation, but haven't yet done so. To fix this, add the following class to your project:

```cs
namespace wit_computer.Wit.exports.example.calculator.Operations;

public class OperationsImpl : Operations
{
    public static int Add(int left, int right)
    {
        return left + right;
    }
}
```

Make sure to get the namespace exactly correct! Although this is quite difficult to figure out at the moment, hopefully a future version of the C# support in wit-bindgen will make it easier.

Now when you build, you'll get a real WASI preview 2 component that exports an implementation for this WIT definition. You can confirm it using [wasm-tools](https://github.com/bytecodealliance/wasm-tools) by running:

```
wasm-tools component wit bin\Debug\net8.0\wasi-wasm\native\MyApp.component.wasm
```

Outputs:

```
package root:component;

world root {
  import ... various things ...

  export example:calculator/operations;
}
```

This component can be used anywhere that WASI preview 2 components can be used. For example, use `wasm-tools compose` as illustrated above.

## Credits

This is a wrapper around various other bits of tooling:

 * [NativeAOT-LLVM](https://github.com/dotnet/runtimelab/tree/feature/NativeAOT-LLVM) for compilation.
   * This produces fully AOT-compiled WebAssembly binaries, very quickly.
   * The vast majority of the work for this was done by [@yowl](https://github.com/yowl) [@SingleAccretion](https://github.com/SingleAccretion), with guidance from [@jkotas](https://github.com/jkotas).
   * In the future, I may add support for Mono AOT compilation
 * [wit-bindgen](https://github.com/bytecodealliance/wit-bindgen) for supporting WIT imports and exports
   * When using `wasm-component-sdk`, you don't have to invoke wit-bindgen manually - it's integrated with .NET's build system and even shows up in the VS UI.
   * wit-bindgen support for C# is thanks to various community members, particularly [@yowl](https://github.com/yowl) and [@jsturtevant](https://github.com/jsturtevant).
* [wasm-tools](https://github.com/bytecodealliance/wasm-tools) for converting WebAssembly core modules into WASI preview 2 components.
 * [WASI SDK](https://github.com/WebAssembly/wasi-sdk) and [Emscripten](https://emscripten.org/), both of which are used by NativeAOT-LLVM.
   * Compatible versions of these will be downloaded and cached on your machine the first time you run a build, so the first build will take a few minutes. After that it will only take seconds.
