module Vulder.SchoolScrapper.Logger

open System
open System.IO
open Serilog

[<Literal>]
let dateFormat = "yyyy-MM-ddTHHmmss"

let filePath = $"./Logs/{DateTime.Now.ToString(dateFormat)}.txt"

let setupLogging =
    Log.Logger <-
        LoggerConfiguration()
            .Destructure.FSharpTypes()
            .WriteTo.Console()
            .WriteTo.File(filePath)
            .CreateLogger()
