module Vulder.SchoolScrapper.Parsers.SearchResultParser

open System.Web
open FSharp.Data

let internal encodeGoogleQuery (query: string) =
    $"http://www.google.com/search?q={HttpUtility.UrlEncode(query)}+Plan+lekcji"

// Thanks to: https://fsprojects.github.io/FSharp.Data/library/HtmlParser.html
let internal filterSearchResult (page: HtmlDocument, previousTimetableUrl: string) =
    page.Descendants [ "a" ]
    |> Seq.choose (fun x ->
        x.TryGetAttribute("href")
        |> Option.map (fun a -> x.InnerText(), a.Value()))
    |> Seq.filter (fun (name, url) ->
        name <> "Cached"
        && name <> "Similar"
        && name.Contains("Plan")
        || name.Contains("lekcji") && url.StartsWith("/url?"))
    |> Seq.map (fun (name, url) ->
        name,
        url
            .Substring(0, url.IndexOf("&sa="))
            .Replace("/url?q=", ""))
    |> Seq.filter (fun (_, url) -> not (url.Contains(previousTimetableUrl)))
    |> Seq.head


let searchResultParser (school: string, timetableUrl: string) =
    let encodedUrl =
        encodeGoogleQuery school

    let googleSearchPage =
        HtmlDocument.AsyncLoad encodedUrl
        |> Async.RunSynchronously

    filterSearchResult (googleSearchPage, timetableUrl)
