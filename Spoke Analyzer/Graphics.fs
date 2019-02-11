module Spoke_Analyzer.Graphics
open System
open System.Drawing
open Spoke_Analyzer

let drawLine (origin : Point) (g : Graphics) (pen : Pen) (start : Point, stop : Point) =
    g.DrawLine(pen, Point(start.X + origin.X, start.Y + origin.Y), Point(stop.X + origin.X, stop.Y + origin.Y))
    
let drawPoint (origin : Point) (g : Graphics) (brush : Brush) width (start : Point) =
    g.FillEllipse(brush, Rectangle(start.X + origin.X - (width / 2), start.Y + origin.Y - (width / 2), width, width))
    
let drawSpokes (origin : Point) (g : Graphics) distance (pen : Pen) (brush : Brush) shape offset =
    let drawLine = drawLine origin g pen
    let drawPoint = drawPoint origin g brush
    let drawSpoke num =
        (Point(0, 0),
         (shape.Angle * (num |> float) + offset) |> Spoke_Analyzer.Math.getXY distance |> Point)
        |> drawLine
    [|0..shape.Spokes|] |> Array.iter (drawSpoke)
    offset |> Spoke_Analyzer.Math.getXY distance |> Point |> drawPoint shape.PointSize

let saveImages imageWidth imageDir shape1 shape2 rotationOffset offset angleDifference totalRotations = 
    let drawImage (bmp : Bitmap) (clearColor : Color) i =
        use pen1 = new Pen(shape1.Color, shape1.PenWidth)
        use brush1 = new SolidBrush(shape1.Color)
        use pen2 = new Pen(shape2.Color, shape2.PenWidth)
        use brush2 = new SolidBrush(shape2.Color)            
        
        use g = bmp |> Graphics.FromImage
        g.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
        g.Clear(clearColor)
        let drawSpokes = drawSpokes (Point(bmp.Width / 2, bmp.Height / 2)) g (((imageWidth - ((max shape1.PointSize shape2.PointSize) / 2 + 2)) / 2) |> float)
        drawSpokes pen1 brush1 shape1 rotationOffset
        drawSpokes pen2 brush2 shape2 (shape2.Angle * offset + rotationOffset + angleDifference * (i |> float))
        ()
        
    let totalRotations = if totalRotations |> Double.IsInfinity then 1 else totalRotations |> int
    use bmp = new Bitmap(imageWidth, imageWidth)
    [|0..totalRotations - 1|]
    |> Array.iter (fun i ->
        drawImage bmp (Color.FromArgb(255, 32, 32, 32)) i
        bmp.Save(sprintf "%s/rot_%i.png" imageDir i, Imaging.ImageFormat.Png))
    
let makeGif fps imageDir : string option -> unit =    
    function
    | Some ffmpeg ->
        let ffmpeg =
            if ffmpeg.EndsWith("ffmpeg") = false && ffmpeg.EndsWith("ffmpeg.exe") = false then
                System.IO.Path.Combine(ffmpeg, "ffmpeg")
            else ffmpeg
            
        printfn "Running ffmpeg..."
        System
            .Diagnostics
            .Process
            .Start(ffmpeg, sprintf "-framerate %i -f image2 -i %s/rot_%%d.png -c:v libx264 -crf 0 -r %i -preset ultrafast -tune stillimage %s/temp.avi" fps imageDir fps imageDir)
            .WaitForExit()
        System
            .Diagnostics
            .Process
            .Start(ffmpeg, sprintf "-i %s/temp.avi -pix_fmt rgb24 %s/_final.gif" imageDir imageDir)
            .WaitForExit()
        printfn "Images converted to gif."
        printfn ""
    | _ -> ()
