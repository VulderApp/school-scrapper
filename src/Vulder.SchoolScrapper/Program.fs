module Program

open Argu
open Microsoft.FSharp.Core
open Vulder.SchoolScrapper.Commands.Version
open Vulder.SchoolScrapper.Logger
open Vulder.SchoolScrapper.Arguments
open Vulder.SchoolScrapper.Commands.SchoolTimetableScrapper

[<EntryPoint>]
let main argv =
    setupLogging

    let parser =
        ArgumentParser.Create<CliArguments>(programName = "Vulder.SchoolScrapper")

    match parser.ParseCommandLine argv with
    | p when p.Contains(Csv_File) && p.Contains(Output_Path) ->
        schoolTimetableScrapper (p.GetResult(Csv_File), p.GetResult(Output_Path))
    | p when p.Contains(Version) -> version()
    | _ -> printfn "%s" (parser.PrintUsage())
    0
