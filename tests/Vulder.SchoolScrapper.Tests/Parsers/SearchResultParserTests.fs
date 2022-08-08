module Vulder.SchoolScrapper.Tests.Parsers.SearchResultParserTests

open System.IO
open FSharp.Data
open Vulder.SchoolScrapper.Parsers.SearchResultParser
open WireMock.RequestBuilders
open WireMock.ResponseBuilders
open WireMock.Server
open Xunit

[<Literal>]
let EXPECTED_ENCODED_URL =
    "http://www.google.com/search?q=zsp+w+vulderowie+Plan+lekcji"

[<Literal>]
let EXPECTED_URL = "http://example.com/"

let wiremock = WireMockServer.Start()

let setupEndpoints =
    wiremock
        .Given(Request.Create().WithPath("/").UsingGet())
        .RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithBody(File.ReadAllText("Files/search.html"))
        )

[<Fact>]
let ``Try get optivum timetable search result`` () =
    let document =
        HtmlDocument.Load wiremock.Url

    let _, url =
        filterSearchResult (document, "http://aaa.com/")

    Assert.Equal(EXPECTED_URL, url)

[<Fact>]
let ``Validate URL encoding`` () =
    let result =
        encodeGoogleQuery "zsp w vulderowie"

    Assert.Equal(EXPECTED_ENCODED_URL, result)
