module Vulder.SchoolScrapper.Parser.CsvReader

open System.Text.RegularExpressions
open FSharp.Data
open Vulder.SchoolScrapper.Models.School

[<Literal>]
let URL_REGEX =
    @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)"

let matchUrl (url: string) = Regex.IsMatch(url, URL_REGEX)

let parsedSchoolsCsv (file: string) =
    CsvFile.Load(file, separators = ";").Cache()

let parsedSchoolList (file: string) : seq<School> =
    seq {
        for row in parsedSchoolsCsv(file).Rows do
            let www = row.GetColumn "Strona www"

            if matchUrl www then
                yield
                    { Name = (row.GetColumn "Nazwa")
                      WWW = www }

            ()
    }
