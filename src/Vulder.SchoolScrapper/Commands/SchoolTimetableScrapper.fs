module Vulder.SchoolScrapper.Commands.SchoolTimetableScrapper

open Serilog
open Vulder.SchoolScrapper.Parser.CsvReader
open Vulder.SchoolScrapper.Parser.PageParser
open Vulder.SchoolScrapper.Output.SaveFile


let schoolTimetableScrapper (csvPath: string, outputPath: string) =
    Log.Information("Pasring CSV file from {0}", csvPath)

    let parsedSchoolList =
        parsedSchoolList csvPath |> List.ofSeq

    Log.Information("Imported {0} schools from {1} with valid URLs", parsedSchoolList.Length, csvPath)

    let timetables =
        vulcanTimetableSchools parsedSchoolList
        |> List.ofSeq

    Log.Information("Collected {0} schools with valid Vulcan Optivum timetable", timetables.Length)
    Log.Information("Saving schools to {0}", outputPath)
    saveSchoolsToFile (outputPath, timetables)
    Log.Information("Saved")
