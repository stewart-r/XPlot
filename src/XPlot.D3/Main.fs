namespace XPlot.D3

open System.IO
open System
open System.Drawing
open System.Text
open Newtonsoft.Json

type Node = {
        Name:string
    }


[<AutoOpen>]
module Customisation = 

    type NodeOptions = {
        Fill: Color
        Stroke: Color
        StrokeWidth:int
    }

    type NodeStyle = {
            FillHex :string
            StrokeHex:string
            StrokeWidth:string
        }

    type Options = {
        NodeOptions:Node -> NodeOptions
    }

    let grey = Color.FromArgb(200,200,200)
    let darkGrey = Color.FromArgb(150,150,150)

    let defaultOptions = 
        {
            NodeOptions = (fun _ -> 
            {
                Fill = Color.FromName("red")
                Stroke = darkGrey
                StrokeWidth = 2
            })
        }

module Html =

    let jsTemplate =
        """
        // set a width and height for our SVG
        var width = 640,
            height = 480;

        // Define the nodes to be drawn
        var nodes = {NODES};

        // Add a SVG to the body for our graph
        var svg = d3.select('#{GUID}').append('svg')
            .attr('width', width)
            .attr('height', height);

        var nodeStyles = {NODESTYLES}

        // Now it's the nodes turn. Each node is drawn as a circle.
        var node = svg.selectAll('.node')
            .data(nodes)
            .enter().append('circle')
            .attr('class', 'node')
            .style('stroke',function(d,i) { return nodeStyles[i]['StrokeHex']; })
            .style('stroke-width',function(d,i) { return nodeStyles[i]['StrokeWidth']; })
            .style('fill',function(d,i) { return nodeStyles[i]['FillHex']; })
            .attr('cx', function(d,i) { return (i+1)*(width/4); }) //relative position
            .attr('cy', function(d,i) { return height/2; }) //relative position
            .attr('r', width * 0.05); //radius = size of circle


        // create the force layout graph
        var force = d3.layout.force()
            .size([width, height])
            .nodes(nodes)
            .start();
        """

    let inlineTemplate =
        """
            <script type="text/javascript">
                {JS}
            </script>

            <div id="{GUID}" style="width: {WIDTH}px; height: {HEIGHT}px;"></div>
        """

    let pageTemplate =
        """<!DOCTYPE html>
            <html>
                <head>
                    <meta charset="UTF-8">
                    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
                    <title>D3 Chart</title>
                </head>
                <body>
                    <div id="{GUID}" style="width: {WIDTH}px; height: {HEIGHT}px;"></div>
                    <script src="http://d3js.org/d3.v3.min.js"></script>
                    <script type="text/javascript">
                        
                        {JS}
                    </script>
                </body>
            </html>"""

type ChartGallery =
    | ForceLayout


type ForceLayoutChart() = 

    [<DefaultValue>]
    val mutable private nodes : seq<Node> 
    
    // [<DefaultValue>]
    // val mutable private options : Options
    [<DefaultValue>]
    val mutable private ``type`` : ChartGallery

    member val Options = defaultOptions with get, set
    member __.GetHtml() =
        
        let packages =
            match __.``type`` with
            | ForceLayout -> "forcelayoutchart"

        Html.pageTemplate
            .Replace("{PACKAGES}", packages)
            .Replace("{JS}", __.GetInlineJS())
            .Replace("{GUID}", __.Id)
            .Replace("{WIDTH}", string(__.Width))
            .Replace("{HEIGHT}", string(__.Height))
    /// Displays a chart in the default browser.
    member __.Show() =
        let html = __.GetHtml()
        let tempPath = Path.GetTempPath()
        let file = sprintf "%s.html" __.Id
        let path = Path.Combine(tempPath, file)
        File.WriteAllText(path, html)
        System.Diagnostics.Process.Start(path) |> ignore

    member __.GetInlineHtml() =
        Html.inlineTemplate
            .Replace("{JS}", __.GetInlineJS())
            .Replace("{GUID}", __.Id)
            .Replace("{WIDTH}", string(__.Width))
            .Replace("{HEIGHT}", string(__.Height))
    
    let toHex (color:Color) = 
        String.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B)

    let toNodeStyle (nodeOptions:NodeOptions) =
            {
                FillHex = toHex nodeOptions.Fill
                StrokeHex = toHex nodeOptions.Stroke
                StrokeWidth = sprintf "%dpx" nodeOptions.StrokeWidth
            }

    /// The chart's inline JavaScript code.
    member __.GetInlineJS() =
        let nodesJson = __.nodes |> JsonConvert.SerializeObject   
        let nodeStylesJson = 
            __.nodes 
            |> Seq.map (__.Options.NodeOptions >> toNodeStyle) 
            |> Array.ofSeq 
            |> JsonConvert.SerializeObject

        Html.jsTemplate.Replace("{NODES}", nodesJson)
            .Replace("{NODESTYLES}", nodeStylesJson )
            // .Replace("{TYPE}", __.``type``.ToString())
            // .Replace("{GUID}", __.Id)

    member val Height = 500 with get, set


    member val Width = 900 with get, set

    

    /// Sets the chart's height.
    member __.WithHeight height = __.Height <- height

    /// Sets the chart's container div id.
    member __.WithId newId = __.Id <- newId

    /// The chart's container div id.
    member val Id =
        Guid.NewGuid().ToString().Replace("-","")
        |> sprintf "d3%s"
        with get, set
    
    member __.WithNodeOptions nodeOptions = 
        __.Options <- {__.Options with NodeOptions = nodeOptions} 

    static member Create nodes = 
        let ret = ForceLayoutChart()
        ret.nodes <- nodes
        ret

type Chart =
    static member Create nodes = 
        ForceLayoutChart.Create nodes

    static member WithNodeOptions nodeOptions (chart:ForceLayoutChart) =
        chart.WithNodeOptions nodeOptions
        chart 


    /// Displays a chart in the default browser.
    static member Show(chart : ForceLayoutChart) = chart.Show()

type Chart with
    static member ForceLayout(nodes:seq<Node>) =
        Chart.Create nodes



    