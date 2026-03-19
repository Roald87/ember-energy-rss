module EmberEnergyRss.Program

open System
open System.IO
open System.Net.Http
open EmberEnergyRss.Parser
open EmberEnergyRss.RssGenerator

[<EntryPoint>]
let main argv =
    let outputPath =
        if argv.Length > 0 then argv.[0]
        else "rss.xml"

    let inputPath =
        if argv.Length > 1 then Some argv.[1]
        else None

    let source, html =
        match inputPath with
        | Some path ->
            path, File.ReadAllText(path)
        | None ->
            let url = "https://ember-energy.org/latest-insights/"
            use client = new HttpClient()
            client.DefaultRequestHeaders.UserAgent.ParseAdd("EmberEnergyRss/1.0")
            url, client.GetStringAsync(url).Result

    let articles = parseArticles html

    if articles.IsEmpty then
        eprintfn "ERROR: No articles parsed from %s — aborting to preserve existing feed." source
        1
    else
        let doc = generate articles
        doc.Save(outputPath)
        printfn "Wrote %d articles to %s" articles.Length outputPath
        0
