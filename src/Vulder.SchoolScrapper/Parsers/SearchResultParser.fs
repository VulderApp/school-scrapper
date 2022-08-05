module Vulder.SchoolScrapper.Parsers.SearchResultParser

open System.Web
open FSharp.Data

let private encodeGoogleQuery (query: string) = $"https://www.google.com/search?q={HttpUtility.UrlEncode(query)}"

// Thanks to: https://fsprojects.github.io/FSharp.Data/library/HtmlParser.html
let private filterSearchResult (page: HtmlDocument, previousTimetableUrl: string) =
    page.Descendants ["a"]
    |> Seq.choose (fun x ->
           x.TryGetAttribute("href")
           |> Option.map (fun a -> x.InnerText(), a.Value())
    )
    |> Seq.filter (fun (name, url) ->
                    name <> "Cached" && name <> "Similar" && url.StartsWith("/url?"))
    |> Seq.map (fun (name, url) -> name, url.Substring(0, url.IndexOf("&sa=")).Replace("/url?q=", ""))
    |> Seq.exactlyOne


let searchResultParser (school: string, timetableUrl: string) =
    let encodedUrl = encodeGoogleQuery(school + " Plan lekcji")
    let googleSearchPage = HtmlDocument.AsyncLoad encodedUrl |> Async.RunSynchronously
    filterSearchResult(googleSearchPage, timetableUrl)
    