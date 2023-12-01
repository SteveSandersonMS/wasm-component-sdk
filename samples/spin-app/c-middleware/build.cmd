..\..\..\src\WitBindgen\tools\win-x64\wit-bindgen c --out-dir generated --world computer ..\middleware.wit
mkdir dist
%WASI_SDK_PATH%\bin\clang -mexec-model=reactor main.c generated\*.o generated\*.c -o dist\main.wasm
..\..\..\src\WasmComponent.Sdk\tools\win-x64\wasm-tools component new dist/main.wasm --adapt ..\..\..\src\WasmComponent.Sdk\tools\wasi-wasm\wasi_snapshot_preview1.reactor.wasm -o dist/main.component.wasm
