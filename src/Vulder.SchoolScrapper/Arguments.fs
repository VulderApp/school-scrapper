module Vulder.SchoolScrapper.Arguments

open Argu

type CliArguments =
    | Csv_File of file: string
    | Output_Path of output: string

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Csv_File _ -> ".csv file from goverment places list "
            | Output_Path _ -> "Specify output file path"
