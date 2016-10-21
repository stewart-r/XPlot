#I "bin/"
#r "bin/Newtonsoft.Json.dll"

#load "src/XPlot.D3/Configuration.fs"
#load "src/XPlot.D3/Main.fs"

open XPlot.D3


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
|> Chart.WithCharge -2500.0
|> Chart.WithEdgeOptions (fun e ->
    let pr = e.From.Name, e.To.Name
    match pr with 
    | "A","B" -> { defaultEdgeOptions with Distance = 140.0 }
    | "A","D" -> { defaultEdgeOptions with StrokeWidth = 4.5 } 
    | _ -> {defaultEdgeOptions with Distance = 100.0})
|> Chart.WithNodeOptions(fun n ->
    match n.Name with
    | "A" -> {defaultNodeOptions with Fill = {Red = 150uy; Green = 150uy; Blue=195uy}} 
    | "B" -> {defaultNodeOptions with RadiusScale=1.5; Fill = {Red = 150uy; Green = 195uy; Blue=150uy}}
    | _ -> defaultNodeOptions)
|> Chart.Show