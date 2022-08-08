module Vulder.SchoolScrapper.Commands.SchoolTimetableScrapper

open Serilog
open Vulder.SchoolScrapper.Parsers.CsvReader
open Vulder.SchoolScrapper.Parsers.PageParser
open Vulder.SchoolScrapper.Output.SaveFile

let schoolTimetableScrapper (csvPath: string, outputPath: string, enableGoogleSearch: bool) =
    Log.Information("Pasring CSV file from {0}", csvPath)

    let parsedSchoolList =
        parseSchoolList csvPath |> List.ofSeq

    Log.Information("Imported {0} schools from {1} with valid URLs", parsedSchoolList.Length, csvPath)

    let timetables =
        vulcanTimetableSchools (parsedSchoolList, enableGoogleSearch)
        |> List.ofSeq

    if timetables.IsEmpty then
        Log.Information("No schools collected")
    else
        Log.Information("Collected {0} schools with valid Vulcan Optivum timetable", timetables.Length)
        Log.Information("Saving schools to {0}", outputPath)
        saveSchoolsToFile (outputPath, timetables)
        Log.Information("Saved")
