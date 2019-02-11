open System
open System.Drawing
open Spoke_Analyzer
open Spoke_Analyzer

[<Literal>]
let FULL_CIRCLE = 360.
[<Literal>]
let ROTATION_OFFSET = -90. // -FULL_CIRCLE / 4.
[<Literal>]
let IMAGE_DIR = "temp/images"

[<EntryPoint>]
let main argv =
    let getShape pointSize penWidth color i =
        let spokes = i |> sprintf "Enter the number of spokes for shape %i (whole number > 0): " |> Input.getInputInt
        { Spokes = spokes; Angle = FULL_CIRCLE / (spokes |> float); PointSize = pointSize; PenWidth = penWidth; Color = color }
        
    let imageWidth = Input.getInputIntOption "Enter the image dimension (whole number > 0) [160]: " |> Option.defaultValue 160
    let shape1 = getShape 8 2.5f (Color.FromArgb(255, 224, 32, 32)) 1
    let shape2 = getShape 6 1.5f (Color.FromArgb(255, 32, 224, 32)) 2
    let offset = Input.getInputDoubleOption "Enter the radial offset in percentage (0.0 - 1.0) [0]: " |> Option.defaultValue 0.
    let ffmpeg = Input.getInputFileOption "ffmpeg" "Enter the location of ffmpeg (if available) []: "
    let fps = ffmpeg |> Option.bind (fun s -> Input.getInputIntOption "Enter the fps of the output (whole number > 0) [24]: ") |> Option.defaultValue 24    
    printfn ""
    
    let angleDifference = Spoke_Analyzer.Math.getRotation FULL_CIRCLE shape1.Angle shape2.Angle
    let totalRotations = FULL_CIRCLE / angleDifference
        
    IMAGE_DIR |> Spoke_Analyzer.IO.createDirIfNotExists |> ignore
    IMAGE_DIR |> Spoke_Analyzer.IO.clearDir |> ignore
    Graphics.saveImages imageWidth IMAGE_DIR shape1 shape2 ROTATION_OFFSET offset angleDifference totalRotations
    printfn "Images saved."    
    Graphics.makeGif fps IMAGE_DIR ffmpeg
    
    printfn "Shape 1 Angle (%i spokes): %f° / %f rads" shape1.Spokes shape1.Angle (shape1.Angle |> Spoke_Analyzer.Math.degToRad)
    printfn "Shape 2 Angle (%i spokes): %f° / %f rads" shape2.Spokes shape2.Angle (shape2.Angle |> Spoke_Analyzer.Math.degToRad)
    printfn "Overlap Angle Difference: %f° / %f rads (%f rotations)" angleDifference (angleDifference |> Spoke_Analyzer.Math.degToRad) (angleDifference / FULL_CIRCLE)
    printfn "Overlaps per Rotation: %f" totalRotations
    
    0
