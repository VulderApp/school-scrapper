module Vulder.SchoolScrapper.Parsers.PageParser

open System
open System.Text.RegularExpressions
open FSharp.Data
open Microsoft.FSharp.Core
open Serilog
open Vulder.SchoolScrapper.Models.School
open Vulder.SchoolScrapper.Models.Timetable
open Vulder.SchoolScrapper.Parsers.SearchResultParser

let private timetableKeywords =
    [ "plan lekcji"
      "plan zajęć"
      "podział godzin"
      "plan"
      "podział" ]

let private logErrorUrlValidation (school: string) =
    Log.Error("{0} problem with URL validation", school)

let private logNotFoundSchool (school: string) =
    Log.Information("{0} timetable not found", school)

let private logFoundTimetableWithDifferentSchema (school: string) =
    Log.Warning("{0} Found timetable with different schema", school)

let private isVulcanTimetableUrl (page: string) = Regex.IsMatch(page, "(Optivum|VULCAN)")

let private downloadHtmlDocument (url: string) =
    HtmlDocument.AsyncLoad url
    |> Async.RunSynchronously

let private filterHrefElements (schoolPage: HtmlDocument) =
    schoolPage.Descendants [ "a" ]
    |> Seq.choose (fun x ->
        x.TryGetAttribute("href")
        |> Option.map (fun a -> x.InnerText(), a.Value()))
    |> Seq.filter (fun (text, _) -> List.contains (text.ToLower()) timetableKeywords)
    |> Seq.map snd
    |> Seq.exactlyOne

let private validateOptivumTimetable (timetableUrl: string) : bool =
    try
        let timetablePage =
            Http.AsyncRequestString timetableUrl
            |> Async.RunSynchronously

        let isVulcanTimetable =
            isVulcanTimetableUrl timetablePage

        isVulcanTimetable
    with
    | :? UriFormatException ->
        logErrorUrlValidation (timetableUrl)
        false

let vulcanTimetableSchools (schools: School List) : seq<Timetable> =
    seq {
        for school in schools do
            let schoolPage =
                downloadHtmlDocument school.WWW

            let timetableUrl =
                filterHrefElements schoolPage

            if timetableUrl = "" then
                logNotFoundSchool school.Name
                ()

            let baseWWW = Uri(school.WWW)

            let (parsed, timetableUri) =
                Uri.TryCreate(baseWWW, timetableUrl)

            if not parsed then
                logErrorUrlValidation school.Name

            if validateOptivumTimetable (timetableUri.AbsoluteUri) then
                yield
                    { School = school.Name
                      Url = timetableUri.AbsoluteUri }

            let (searchName, searchUrl) =
                searchResultParser (school.Name, timetableUri.AbsoluteUri)

            if searchName |> String.IsNullOrEmpty then
                logFoundTimetableWithDifferentSchema school.Name
                ()

            let validTimetable =
                validateOptivumTimetable searchUrl

            if validTimetable then
                yield
                    { School = school.Name
                      Url = searchUrl }
    }
