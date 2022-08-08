module Vulder.SchoolScrapper.Tests.Parsers.CsvParserTests

open Xunit
open Vulder.SchoolScrapper.Parsers.CsvReader

[<Literal>]
let CSV_PATH = "Files/test.csv"

[<Fact>]
let ``Check if elements of parsed csv not empty`` () =
    let schools =
        parseSchoolList CSV_PATH |> List.ofSeq

    Assert.NotEmpty schools.Head.Name
    Assert.NotEmpty schools.Head.WWW
