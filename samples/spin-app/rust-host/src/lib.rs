wit_bindgen::generate!({ world: "hostapp" });

use spin_sdk::http::{IntoResponse, Request};
use spin_sdk::http_component;
use example::calculator::operations;

/// A simple Spin HTTP component.
#[http_component]
fn handle_my_rust_app(req: Request) -> anyhow::Result<impl IntoResponse> {
    example::calculator::operations::add(1, 2);
    println!("Handling request to {:?}", req.header("spin-full-url"));
    Ok(http::Response::builder()
        .status(200)
        .header("content-type", "text/plain")
        .body(operations::to_upper("Hello from Rust"))?)
}
