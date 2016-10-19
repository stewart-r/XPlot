(*** hide ***)
#I "../../../bin/"
#r "XPlot.D3.dll"
open XPlot.D3

(**
##D3 Network Example
*)
(*** define-output:netchart ***)
let edges = 
    [   "A", "B"
        "B", "C"
        "C", "A"
        "A", "D"
        "B", "E"]
edges
|> Chart.ForceLayout
|> Chart.WithHeight 300
|> Chart.WithWidth 400
|> Chart.WithGravity 0.5
|> Chart.WithCharge -2000.0
|> Chart.WithEdgeOptions (fun e ->
    if e.From.Name = "A" then
        if e.To.Name = "B" then
            {defaultEdgeOptions with Distance = 200.0}
        else 
            {defaultEdgeOptions with StrokeWidth = 4.5}
    else 
        {defaultEdgeOptions with Distance = 100.0}
        )
|> Chart.WithNodeOptions(fun n ->
    match n.Name with
    | "A" -> {defaultNodeOptions with Fill = {Red = 150uy; Green = 150uy; Blue=195uy}} 
    | "B" -> {defaultNodeOptions with RadiusScale=1.5; Fill = {Red = 150uy; Green = 195uy; Blue=150uy}}
    | _ -> defaultNodeOptions)
|> Chart.Show
(*** include-it:netchart ***)
