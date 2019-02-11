module Spoke_Analyzer.Input
open System

let rec getInput convert validate prompt =
    printf "%s" prompt
    let input = () |> Console.ReadLine
    if input |> validate then
        input |> convert
    else
        printfn "Invalid, please try again."
        getInput convert validate prompt
let getInputInt =
    getInput
        Int32.Parse
        (Int32.TryParse >> function | true, f when f > 0 -> true | _ -> false)
let getInputIntOption =
    getInput
        (function | "" -> None | s -> s |> Int32.Parse |> Some)
        (function | "" -> true | s -> s |> Int32.TryParse |> function | true, f when f > 0 -> true | _ -> false)
let getInputDoubleOption =
    getInput
        (function | "" -> None | s -> s |> Double.Parse |> Some)
        (function | "" -> true | s -> s |> Double.TryParse |> function | true, f when f >= 0. && f <= 1. -> true | _ -> false)
let getInputDouble =
    getInput
        Double.Parse
        (Double.TryParse >> function | true, f when f >= 0. && f <= 1. -> true | _ -> false)
let getInputFileOption (file : string) =
    getInput
        (function | "" -> None | s -> Some s)
        (function
         | "" -> true
         | s ->
             if Uri.IsWellFormedUriString(sprintf "file:///%s" (s.Replace('\\', '/')), UriKind.RelativeOrAbsolute) then
                let test = 
                    if s.EndsWith(file) = false && s.EndsWith(sprintf "%s.exe" file) = false then
                        let t = System.IO.Path.Combine(s, file)
                        if System.IO.File.Exists(t) then t
                        else System.IO.Path.Combine(s, sprintf "%s.exe" file)
                    else s
                if System.IO.File.Exists(test) then
                    true
                else false
             else false)
        