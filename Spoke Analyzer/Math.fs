module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let rec getOverlaps smallCount largeCount =
    if smallCount > largeCount then getOverlaps largeCount smallCount
    else
        let totalOverlaps = (smallCount |> float) * (largeCount |> float)
        // If the small divides into large evenly, then there are `small` coincident spokes, and we'll divide our total
        // overlap count by that number.
        if largeCount % smallCount <> 0 then totalOverlaps
        else totalOverlaps / (smallCount |> float)
        