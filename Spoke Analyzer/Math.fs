module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let rec getOverlaps smallCount largeCount =
    if smallCount > largeCount then getOverlaps largeCount smallCount
    else
        let v = (smallCount |> float) * (largeCount |> float) 
        if largeCount % smallCount <> 0 then v
        else v / (smallCount |> float)
        