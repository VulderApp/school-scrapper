module Vulder.SchoolScrapper.Tests.Parsers.PageParserTests

open System.IO
open Vulder.SchoolScrapper.Models.School
open Vulder.SchoolScrapper.Parsers.PageParser
open WireMock.RequestBuilders
open WireMock.ResponseBuilders
open WireMock.Server
open Xunit

let wiremock = WireMockServer.Start()

let setupEndpoints =
    wiremock
        .Given(
            Request
                .Create()
                .WithPath("/school.html")
                .UsingGet()
        )
        .RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithBody(
                    File
                        .ReadAllText("Files/school.html")
                        .Replace("timetable.html", $"{wiremock.Url}/timetable.html")
                )
        )

    wiremock
        .Given(
            Request
                .Create()
                .WithPath("/timetable.html")
                .UsingGet()
        )
        .RespondWith(
            Response
                .Create()
                .WithStatusCode(200)
                .WithBody(File.ReadAllText("Files/timetable.html"))
        )

let schools: School list =
    [ { Name = "Szkoła Podstawowa nr 2"
        WWW = $"{wiremock.Url}/school.html" } ]

[<Fact>]
let ``Check if the parser found timetable`` () =
    let parsedTimetables =
        vulcanTimetableSchools (schools, false)
        |> List.ofSeq

    let expectedUrl =
        schools.Head.WWW.Replace("school.html", "timetable.html")

    Assert.Equal(schools.Head.Name, parsedTimetables.Head.School)
    Assert.Equal(expectedUrl, parsedTimetables.Head.Url)
