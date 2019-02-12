module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let getRotation small large smallCount largeCount =
    ([|0..smallCount|], [|0..largeCount|])
    ||> Array.allPairs
    |> Array.map (fun (smalli, largei) -> abs ((smalli |> float) * small - (largei |> float) * large))
    |> Array.filter ((<) 0.)
    |> Array.min
    