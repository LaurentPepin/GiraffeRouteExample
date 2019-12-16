module RoutesExample.Program

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http
open RoutesExample.Services

let apiHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let _booksService = ctx.GetService<IBooksService>()
        (
            subRoute "/api"
                (choose [
                    subRoute "/v1"
                        (choose [
                            subRoute "/books" (_booksService.GetAll)
                        ])
                ])
        ) next ctx

let configureApp (app : IApplicationBuilder) =
    app
        .UseGiraffe apiHandler

let configureServices (services : IServiceCollection) =
    services
        .AddScoped<IBooksService, BooksService>()
        .AddGiraffe()
       
    |> ignore

let configureLogging (builder: ILoggingBuilder) =
    builder
        .ClearProviders()
        .AddConsole()
        .AddDebug()

    |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0
