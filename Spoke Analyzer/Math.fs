module Spoke_Analyzer.Math

let inline degToRad deg = deg * System.Math.PI / 180.
let inline radToDeg rad = rad * 180. / System.Math.PI
let inline angleMath distance angle fn = (distance * (angle |> degToRad |> fn)) |> int 

let inline getXY distance angle = cos |> angleMath distance angle, sin |> angleMath distance angle

let inline gcd a b =
    let mutable a, b = a, b

    while b > LanguagePrimitives.GenericZero do
        let temp = a
        a <- b
        b <- temp % b
    a

let inline getOverlaps count1 count2 =
    let inline calculation smallCount largeCount =
        let totalOverlaps = (smallCount |> float) * (largeCount |> float)
        // Divide by the GCD of the two as that will be how many active overlaps there are.
        totalOverlaps / (gcd smallCount largeCount |> float)

    if count1 > count2 then calculation count2 count1
    else calculation count1 count2 
