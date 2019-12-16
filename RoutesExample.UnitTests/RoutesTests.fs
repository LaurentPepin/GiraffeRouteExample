module RoutesExample.UnitTests.RoutesTests

open Giraffe
open System
open Xunit
open NSubstitute
open System.Collections.Generic
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open System.Threading.Tasks
open System.IO
open System.Text
open RoutesExample.Services
open RoutesExample.Program

let getBody (ctx : HttpContext) =
    ctx.Response.Body.Position <- 0L
    use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
    reader.ReadToEnd()

[<Fact>]
let ``/api/v1/books calls IBooksService GetAll()`` () =
    let expectedBody = Guid.NewGuid().ToString()
    let getAllHandler: HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            text expectedBody next ctx
    let booksService =
        Substitute.For<IBooksService>();
    booksService.GetAll.Returns(getAllHandler) |> ignore;

    let ctx = Substitute.For<HttpContext>()
    ctx.RequestServices.GetService(typeof<IBooksService>).Returns(booksService) |> ignore
    ctx.Request.Method <- "GET"
    ctx.Request.Path.Returns(new PathString("/api/v1/books")) |> ignore
    ctx.Items <- new Dictionary<obj, obj>()   // Required for Giraffe subrouting
    ctx.Response.Body <- new MemoryStream()
    let testHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            apiHandler next ctx

    
    task {
        let! result = testHandler (Some >> Task.FromResult) ctx

        match result with
        | Some ctx -> Assert.Equal(expectedBody, getBody ctx)
        | None -> Assert.True(false)
    }
