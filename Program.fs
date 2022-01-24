﻿module Program

open System.IO
open System.Text.Json
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

module Json =
    let fromObj a =
        task {
            use mem = new MemoryStream()
            do! JsonSerializer.SerializeAsync(mem, a)
            return mem.ToArray()
       }

let getAll : WebPart =
    fun ctx ->
        async {
            let! data = Db.getAll ()
            let! json = Json.fromObj data
            return! ok json ctx
        }

let app =
    choose [
        path "/" >=> GET >=> getAll >=> Writers.setMimeType "application/json; charset=utf-8"
        RequestErrors.NOT_FOUND "Not found"
    ]

startWebServer defaultConfig app