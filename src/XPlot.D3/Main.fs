namespace XPlot.D3

open System.IO
open System
open Newtonsoft.Json

module Html =

    let jsTemplate =
        """google.setOnLoadCallback(drawChart);
            function drawChart() {
                var data = new google.visualization.DataTable({DATA});

                var options = {OPTIONS} 

                var chart = new google.visualization.{TYPE}(document.getElementById('{GUID}'));
                chart.draw(data, options);
            }"""

    let inlineTemplate =
        """<script type="text/javascript">
    {JS}
</script>
<div id="{GUID}" style="width: {WIDTH}px; height: {HEIGHT}px;"></div>"""

    let pageTemplate =
        """<!DOCTYPE html>
<html>
    <head>
        <meta charset="UTF-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title>Google Chart</title>
        <script type="text/javascript" src="https://www.google.com/jsapi"></script>
        <script type="text/javascript">
            google.load("visualization", "{VERSION}", {packages:["{PACKAGES}"]})
            {JS}
        </script>
    </head>
    <body>
        <div id="{GUID}" style="width: {WIDTH}px; height: {HEIGHT}px;"></div>
    </body>
</html>"""

type ChartGallery =
    | ForceLayout


type D3Chart() = 
    [<DefaultValue>]
    val mutable private ``type`` : ChartGallery
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

    /// The chart's inline JavaScript code.
    member __.GetInlineJS() =
        let dataJson = __.dataTable.ToGoogleDataTable().GetJson()         
        let optionsJson = JsonConvert.SerializeObject(__.options)
        Html.jsTemplate.Replace("{DATA}", dataJson)
            .Replace("{OPTIONS}", optionsJson)
            .Replace("{TYPE}", __.``type``.ToString())
            .Replace("{GUID}", __.Id)

    member val Height = 500 with get, set


    member val Width = 900 with get, set

    /// Sets the chart's height.
    member __.WithHeight height = __.Height <- height

    /// Sets the chart's container div id.
    member __.WithId newId = __.Id <- newId

    /// The chart's container div id.
    member val Id =
        Guid.NewGuid().ToString()
        with get, set

type Chart =

    /// Displays a chart in the default browser.
    static member Show(chart : D3Chart) = chart.Show()

// type Chart with
//     static member Combo(data:seq<#seq<'K * 'V>> when 'K :> key and 'V :> value, ?Labels, ?Options) =
//     Chart.Create data Labels Options Combo Datum.New

[<AutoOpen>]
module ForceLayout = 
    type Node = {
        Name:string
    }

    