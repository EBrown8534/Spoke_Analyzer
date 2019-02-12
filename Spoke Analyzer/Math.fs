module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let rec getRotation fullCircle small large =
    if small > large then
        getRotation fullCircle large small
    else
        if small = large then small
        elif large = fullCircle then small
        else
            let v1 = large / small
            let divisions1 = v1 |> int |> float
            let divisions2 = divisions1 + 1.
            let smallOfDivisions = min (large - small * divisions1) (small * divisions2 - large)
            let v2 = min (large - small) smallOfDivisions
            min (fullCircle / v1) v2
