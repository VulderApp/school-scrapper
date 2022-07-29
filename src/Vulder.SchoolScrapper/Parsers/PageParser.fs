module Vulder.SchoolScrapper.Parsers.PageParser

open System.Text.RegularExpressions
open FSharp.Data
open Serilog
open Vulder.SchoolScrapper.Models.School
open Vulder.SchoolScrapper.Models.Timetable

let logNotFoundSchool (school: string) =
    Log.Information("{0} timetable not found", school)

let logFoundTimetableWithDifferentSchema (school: string) =
    Log.Warning("{0} Found timetable with different schema", school)

let isVulcanTimetableUrl (page: HtmlDocument) =
    Regex.IsMatch(page.Body.ToString(), "(Optivum|VULCAN)")

let downloadHtmlDocument (url: string) = HtmlDocument.Load url

let filterHrefElements (schoolPage: HtmlDocument) =
    schoolPage.Descendants [ "a" ]
    |> Seq.choose (fun x ->
        x.TryGetAttribute("href")
        |> Option.map (fun a -> x.InnerText(), a.Value()))
    |> Seq.filter (fun (text, _) -> text = "Plan Lekcji")
    |> Seq.map snd
    |> Seq.exactlyOne

let vulcanTimetableSchools (schools: School List) : seq<Timetable> =
    seq {
        for school in schools do
            let schoolPage =
                downloadHtmlDocument school.WWW

            let timetableUrl =
                filterHrefElements schoolPage

            if timetableUrl = "" then
                logNotFoundSchool school.Name
            else
                ()

            let timetablePage =
                downloadHtmlDocument timetableUrl

            let isVulcanTimetable =
                isVulcanTimetableUrl timetablePage

            if not isVulcanTimetable then
                logFoundTimetableWithDifferentSchema school.Name
                ()

            yield
                { School = school.Name
                  Url = timetableUrl }
    }
