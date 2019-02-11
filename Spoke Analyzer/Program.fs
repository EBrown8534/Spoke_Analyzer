open System
open System.Drawing
open Spoke_Analyzer

let inline degToRad deg = deg * Math.PI / 180.
let inline radToDeg rad = rad * 180. / Math.PI

[<Literal>]
let FULL_CIRCLE = 360.
[<Literal>]
let ROTATION_OFFSET = -90. // -FULL_CIRCLE / 4.
[<Literal>]
let IMAGE_DIR = "temp/images"

[<EntryPoint>]
let main argv =
    let imageWidth = Input.getInputIntOption "Enter the image dimension (whole number > 0) [160]: " |> Option.defaultValue 160
    let spoke1 = Input.getInputInt "Enter the number of spokes for shape 1 (whole number > 0): "
    let spoke2 = Input.getInputInt "Enter the number of spokes for shape 2 (whole number > 0): "
    let offset = Input.getInputDoubleOption "Enter the radial offset in percentage (0.0 - 1.0) [0]: " |> Option.defaultValue 0.
    let ffmpeg = Input.getInputFileOption "ffmpeg" "Enter the location of ffmpeg (if available) []: "
    let fps = ffmpeg |> Option.bind (fun s -> Input.getInputIntOption "Enter the fps of the output (whole number > 0) [24]: ") |> Option.defaultValue 24
    let angleDegrees1 = FULL_CIRCLE / (spoke1 |> float)
    let angleDegrees2 = FULL_CIRCLE / (spoke2 |> float)
    printfn ""
    
    let rec getRotation small large =
        if small > large then
            getRotation large small
        else        
            let v1 = large / small
            let divisions = v1 |> int |> float
            let v2 = min (large - small) (large - small * divisions)            
            min (FULL_CIRCLE / v1) v2
        
    let rotation = getRotation angleDegrees1 angleDegrees2
    let rotations = FULL_CIRCLE / rotation

    let rec drawSaveImage i =
        use bmp = new Bitmap(imageWidth, imageWidth)
        do
            use g = bmp |> Graphics.FromImage
            g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
            use fillBrush = new SolidBrush(Color.FromArgb(255, 32, 32, 32))
            g.FillRectangle(fillBrush, Rectangle(0, 0, bmp.Width, bmp.Height))
            let origin = Point(bmp.Width / 2, bmp.Height / 2)
            use pen1 = new Pen(Color.FromArgb(255, 224, 32, 32), 2.5f)
            use brush1 = new SolidBrush(Color.FromArgb(255, 224, 32, 32))
            let drawLine1 = Spoke_Analyzer.Graphics.drawLine origin g pen1
            use pen2 = new Pen(Color.FromArgb(255, 32, 224, 32), 1.5f)      
            use brush2 = new SolidBrush(Color.FromArgb(255, 32, 224, 32))  
            let drawLine2 = Spoke_Analyzer.Graphics.drawLine origin g pen2    
            let drawPoint = Spoke_Analyzer.Graphics.drawPoint origin g      
            
            let distance = (imageWidth / 2) |> float
            
            let rec drawSpoke drawLine distance offset angle max num =
                if num = max then ()
                else
                    drawLine
                        (Point(0, 0),
                         Point((distance * ((angle * (num |> float) + offset) |> degToRad |> cos)) |> int,
                               (distance * ((angle * (num |> float) + offset) |> degToRad |> sin)) |> int))
                    drawSpoke drawLine distance offset angle max (num + 1)
                
            drawSpoke drawLine1 distance ROTATION_OFFSET angleDegrees1 spoke1 0
            drawSpoke drawLine2 distance (angleDegrees2 * offset + ROTATION_OFFSET + rotation * (i |> float)) angleDegrees2 spoke2 0
            drawPoint brush1 (Point(0, -distance |> int)) 8
            drawPoint brush2 (Point((distance * ((rotation * (i |> float) + ROTATION_OFFSET) |> degToRad |> cos)) |> int,
                                    (distance * ((rotation * (i |> float) + ROTATION_OFFSET) |> degToRad |> sin)) |> int)) 6
            ()
        bmp.Save(sprintf "%s/rot_%i.png" IMAGE_DIR i, Imaging.ImageFormat.Png)
        if Double.IsInfinity(rotations) |> not && (i |> float) + 1. < rotations then drawSaveImage (i + 1)
        ()
        
    if IMAGE_DIR |> System.IO.Directory.Exists |> not then IMAGE_DIR|> System.IO.Directory.CreateDirectory |> ignore
    IMAGE_DIR |> System.IO.Directory.GetFiles |> Array.iter System.IO.File.Delete
    drawSaveImage 0
    printfn "Images saved."
    
    match ffmpeg with
    | Some ffmpeg ->
        let ffmpeg =
            if ffmpeg.EndsWith("ffmpeg") = false && ffmpeg.EndsWith("ffmpeg.exe") = false then
                System.IO.Path.Combine(ffmpeg, "ffmpeg")
            else ffmpeg
            
        printfn "Running ffmpeg..."
        System
            .Diagnostics
            .Process
            .Start(ffmpeg, sprintf "-framerate %i -f image2 -i %s/rot_%%d.png -c:v libx264 -crf 0 -r %i -preset ultrafast -tune stillimage %s/temp.avi" fps IMAGE_DIR fps IMAGE_DIR)
            .WaitForExit()
        System
            .Diagnostics
            .Process
            .Start(ffmpeg, sprintf "-i %s/temp.avi -pix_fmt rgb24 %s/_final.gif" IMAGE_DIR IMAGE_DIR)
            .WaitForExit()
        printfn "Images converted to gif."
        printfn ""
    | _ -> ()
    
    printfn "Shape 1 Angle (%i spokes): %f° / %f rads" spoke1 angleDegrees1 (angleDegrees1 |> degToRad)
    printfn "Shape 2 Angle (%i spokes): %f° / %f rads" spoke2 angleDegrees2 (angleDegrees2 |> degToRad)
    printfn "Phase Difference: %f° / %f rads (%f rotations)" rotation (rotation |> degToRad) (rotation / FULL_CIRCLE)
    printfn "Overlaps per Rotation: %f" (rotations)
    
    0
