namespace RoutesExample.Services

open Giraffe

type IBooksService =
    abstract member GetAll: HttpHandler

type BooksService() =
    interface IBooksService with
        
        member this.GetAll: HttpHandler = text "This is supposed to be a list of books"

