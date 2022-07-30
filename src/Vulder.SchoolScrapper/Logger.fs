module Vulder.SchoolScrapper.Logger

open System
open System.IO
open Serilog

[<Literal>]
let private dateFormat = "yyyy-MM-ddTHHmmss"

let private filePath = $"./Logs/{DateTime.Now.ToString(dateFormat)}.txt"

let setupLogging =
    Log.Logger <-
        LoggerConfiguration()
            .Destructure.FSharpTypes()
            .WriteTo.Console()
            .WriteTo.File(filePath)
            .CreateLogger()
