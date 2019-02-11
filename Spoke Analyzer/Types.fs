[<AutoOpen>]
module Spoke_Analyzer.Types

[<Struct>]
type Shape =
    { Spokes : int
      Angle : float
      PointSize : int
      PenWidth : float32
      Color : System.Drawing.Color }
