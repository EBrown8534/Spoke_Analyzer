module Spoke_Analyzer.Graphics
open System.Drawing

let drawLine (origin : Point) (g : Graphics) (pen : Pen) (start : Point, stop : Point) =
    g.DrawLine(pen, Point(start.X + origin.X, start.Y + origin.Y), Point(stop.X + origin.X, stop.Y + origin.Y))
    
let drawPoint (origin : Point) (g : Graphics) (brush : Brush) (start : Point) width =
    g.FillEllipse(brush, Rectangle(start.X + origin.X - (width / 2), start.Y + origin.Y - (width / 2), width, width))
    