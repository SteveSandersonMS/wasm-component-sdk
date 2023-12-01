cargo_component_bindings::generate!();

use spin_sdk::http::{IntoResponse, Request};
use spin_sdk::http_component;

use crate::bindings::example::calculator::operations;

/// A simple Spin HTTP component.
#[http_component]
fn handle_my_rust_app(req: Request) -> anyhow::Result<impl IntoResponse> {
    let result = operations::add(200, 1);
    println!("Handling request to {:?}", req.header("spin-full-url"));
    Ok(http::Response::builder()
        .status(u16::try_from(result).unwrap())
        .header("content-type", "text/plain")
        .body("Hello, Fermyon")?)
}
