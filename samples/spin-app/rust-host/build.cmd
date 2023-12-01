cargo component build
cargo build --target wasm32-wasi --release
..\..\..\src\WasmComponent.Sdk\tools\win-x64\wasm-tools component new target/wasm32-wasi/release/my_rust_app.wasm --adapt ..\..\..\src\WasmComponent.Sdk\tools\wasi-wasm\wasi_snapshot_preview1.reactor.wasm -o target/wasm32-wasi/release/my_rust_app.component.wasm

set MIDDLEWARE_COMPONENT=..\c-middleware\dist\main.component.wasm
set MIDDLEWARE_COMPONENT=..\csharp-middleware\bin\Debug\net8.0\wasi-wasm\native\csharp-middleware.component.wasm

..\..\..\src\WasmComponent.Sdk\tools\win-x64\wasm-tools compose -o target\wasm32-wasi\release\my_rust_app.composed.wasm target\wasm32-wasi\release\my_rust_app.component.wasm -d %MIDDLEWARE_COMPONENT%
