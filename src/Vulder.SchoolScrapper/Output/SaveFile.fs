module Vulder.SchoolScrapper.Output.SaveFile

open System.IO
open System.Text
open FSharp.Json
open Vulder.SchoolScrapper.Models.Timetable

let saveSchoolsToFile (path: string, timetables: Timetable list) =
    let dump = Json.serialize timetables
    let fileStream = File.Create(path)
    fileStream.Write(Encoding.UTF8.GetBytes(dump))
    fileStream.Close()
