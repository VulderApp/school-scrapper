module Vulder.SchoolScrapper.Parsers.PageParser

open System.Text.RegularExpressions
open FSharp.Data
open Serilog
open Vulder.SchoolScrapper.Models.School
open Vulder.SchoolScrapper.Models.Timetable

let timetableKeywords = ["plan lekcji"; "plan zajęć"; "podział godzin"; "plan"; "podział"]

let private logNotFoundSchool (school: string) =
    Log.Information("{0} timetable not found", school)

let private logFoundTimetableWithDifferentSchema (school: string) =
    Log.Warning("{0} Found timetable with different schema", school)

let private isVulcanTimetableUrl (page: HtmlDocument) =
    Regex.IsMatch(page.Body.ToString(), "(Optivum|VULCAN)")

let private downloadHtmlDocument (url: string) = HtmlDocument.Load url

let private filterHrefElements (schoolPage: HtmlDocument) =
    schoolPage.Descendants [ "a" ]
    |> Seq.choose (fun x ->
        x.TryGetAttribute("href")
        |> Option.map (fun a -> x.InnerText(), a.Value()))
    |> Seq.filter (fun (text, _) -> List.contains (text.ToLower()) timetableKeywords)
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
            else
                yield
                    { School = school.Name
                      Url = timetableUrl }
    }
