cargo component build
cargo build --target wasm32-wasi --release
..\..\..\src\WasmComponent.Sdk\tools\win-x64\wasm-tools component new target/wasm32-wasi/release/my_rust_app.wasm --adapt ..\..\..\src\WasmComponent.Sdk\tools\wasi-wasm\wasi_snapshot_preview1.reactor.wasm -o target/wasm32-wasi/release/my_rust_app.component.wasm
..\..\..\src\WasmComponent.Sdk\tools\win-x64\wasm-tools compose -o target\wasm32-wasi\release\my_rust_app.composed.wasm target\wasm32-wasi\release\my_rust_app.component.wasm -d ..\c-middleware\dist\main.component.wasm
