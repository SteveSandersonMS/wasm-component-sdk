# WasmComponent.Sdk

**An experimental package to simplify building WASI preview 2 components using .NET, including early support for WIT files.**

The build output is fully AOT compiled and is known to work in recent versions of wasmtime and WAMR.

## Purpose

This is to simplify experimentation and prototyping.

Without this package, if you wanted to build a WASI preview 2 component with .NET, including using WIT imports/exports, there are about 5 different tools you'd need to discover, download, configure, and manually chain together. Just figuring out which versions of each are compatible with the others is a big challenge. Working out how to get started would be very painful.

With this package, you can add one NuGet reference and then get on with your experiments.

## Support

**No support!** This is for experimentation. All the underlying technologies are under heavy development and are missing key features. When you encounter an issue, which you absolutely will, please try to file it on the relevant underlying tool (see below). Only file issues on this repo if you're sure the problem is here in this repo.

## Getting started

**Limitation**: Although the resulting `.wasm` files can run on any OS, [the compiler itself is currently limited to Windows](https://github.com/dotnet/runtimelab/issues/1890#issuecomment-1221602595). Hopefully that limitation will be resolved soon, since everything else in the toolchain is cross-platform.

### 1. Set up SDKs

If you don't already have it, install [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Also install an up-to-date Python 3.x. For example on Windows, [install Python from the Microsoft Store](https://www.microsoft.com/store/productId/9NCVDN91XZQP), and make sure it's available on your `PATH` (for example: check `python --version` prints a version). This is only required temporarily (a bug in Clang for WASI SDK means we require Emscripten, which in turn requires Python).

### 2. Create a project and add WaskComponent.Sdk package

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

If you have [wasmtime](https://wasmtime.dev/) on your path, you can now run

    wasmtime bin\Debug\net8.0\wasi-wasm\native\YourApp.wasm

(replace `YourApp.wasm` with the actual name of your project)

Note that the compilation will also have generated `YourApp.component.wasm`, which is a WASI preview 2 component. You can also run that if you want.



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

The goal is that you can install just one NuGet package and build your project, saving you downloading and configuring many different tools.
