namespace XPlot.D3

open System.IO
open System
open System.Text
open Newtonsoft.Json

type Label = 
| TextLabel of string
| HtmlLabel of string

type Color = {
    Red:byte
    Green:byte
    Blue:byte
}

type Node = {
        Name:string
    }

type Edge = {
    From:Node
    To:Node
}

type Link = {
    source:int
    target:int
}

[<AutoOpen>]
module Configuration = 

    type NodeOptions = {
        Fill: Color
        Stroke: Color
        StrokeWidth:float
        RadiusScale:float
        Label: Option<Label>
    }

    type NodeStyle = {
            FillHex :string
            StrokeHex:string
            StrokeWidth:string
            RadiusScale:float
        }

    type EdgeOptions = {
        Stroke: Color
        StrokeWidth:float
        Distance:float
    }

    type EdgeStyle = {
        StrokeHex: string
        StrokeWidth: string
        Distance:float
    }

    type Options = {
        EdgeOptions:Edge -> EdgeOptions
        NodeOptions:Node -> NodeOptions
        Gravity:float
        Charge:float
    }

    let private grey = {Red = 200uy; Green = 200uy; Blue = 200uy}
    let private darkGrey = {Red = 100uy; Green = 100uy; Blue = 100uy}
    

    let defaultNodeOptions:NodeOptions = 
        {
            RadiusScale = 1.0
            Fill = grey
            Stroke = darkGrey
            StrokeWidth = 2.0
            Label = None
        }
    let defaultEdgeOptions:EdgeOptions = 
        {
            StrokeWidth = 2.0
            Stroke = darkGrey
            Distance = 150.0
        }

    let defaultOptions = 
        {
            Charge = -3000.0
            Gravity = 1.0
            NodeOptions = (fun n -> defaultNodeOptions)
            EdgeOptions = (fun e -> defaultEdgeOptions)
        }

    
