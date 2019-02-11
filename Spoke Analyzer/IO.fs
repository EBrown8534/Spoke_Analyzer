module Spoke_Analyzer.IO

let createDirIfNotExists dir =
    if dir |> System.IO.Directory.Exists then dir |> System.IO.DirectoryInfo
    else dir |> System.IO.Directory.CreateDirectory
    
let clearDir = System.IO.Directory.GetFiles >> Array.iter System.IO.File.Delete
