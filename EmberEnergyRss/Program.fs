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

    let apiUrl = "https://ember-energy.org/wp-json/wp/v2/insight_page?per_page=100&_fields=title,link,date&orderby=date&order=desc"

    let source, json =
        match inputPath with
        | Some path ->
            path, File.ReadAllText(path)
        | None ->
            use client = new HttpClient()
            apiUrl, client.GetStringAsync(apiUrl).Result

    let articles = parseArticles json

    if articles.IsEmpty then
        eprintfn "ERROR: No articles parsed from %s — aborting to preserve existing feed." source
        1
    else
        let doc = generate articles
        doc.Save(outputPath)
        printfn "Wrote %d articles to %s" articles.Length outputPath
        0
