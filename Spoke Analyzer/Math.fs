module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let inline getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let inline getOverlaps count1 count2 =
    let inline calculation smallCount largeCount =
        let totalOverlaps = (smallCount |> float) * (largeCount |> float)
        // If the small divides into large evenly, then there are `small` coincident spokes, and we'll divide our total
        // overlap count by that number.
        if largeCount % smallCount <> 0 then totalOverlaps
        else totalOverlaps / (smallCount |> float)

    if count1 > count2 then calculation count2 count1
    else calculation count1 count2 
