module Vulder.SchoolScrapper.Arguments

open Argu

type CliArguments =
    | Csv_File of file: string
    | Output_Path of output: string
    | Version

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Csv_File _ -> ".csv file from goverment schoool and institution registry https://rspo.gov.pl/"
            | Output_Path _ -> "Specify output file path"
            | Version _ -> "Display Vulder.SchoolScrapper version in use"
