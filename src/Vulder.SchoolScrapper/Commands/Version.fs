module Vulder.SchoolScrapper.Commands.Version

open System.Reflection

let version () =
    printfn
        "%s"
        (Assembly.GetExecutingAssembly().GetName().Version
         |> string)
