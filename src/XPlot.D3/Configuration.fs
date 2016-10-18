namespace XPlot.D3

open System.IO
open System
open System.Drawing
open System.Text
open Newtonsoft.Json

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
    }

    let grey = Color.FromArgb(200,200,200)
    let darkGrey = Color.FromArgb(150,150,150)

    let defaultNodeOptions:NodeOptions = 
        {
            RadiusScale = 1.0
            Fill = grey
            Stroke = darkGrey
            StrokeWidth = 2.0
        }
    let defaultEdgeOptions:EdgeOptions = 
        {
            StrokeWidth = 2.0
            Stroke = Color.Black
            Distance = 150.0
        }

    let defaultOptions = 
        {
            Gravity = 1.0
            NodeOptions = (fun n -> defaultNodeOptions)
            EdgeOptions = (fun e -> defaultEdgeOptions)
        }

    
