module EmberEnergyRss.Tests.ParserTests

open System
open System.IO
open EmberEnergyRss.Parser
open EmberEnergyRss.RssGenerator

let private fixturePath =
    Path.Combine(__SOURCE_DIRECTORY__, "fixture.json")

let private loadFixture () =
    File.ReadAllText(fixturePath)

let private check label ok =
    if ok then
        printfn "  PASS  %s" label
    else
        failwithf "FAIL  %s" label

let runUnitTests () =
    let json = loadFixture ()
    let articles = parseArticles json

    check "article count is 20" (articles.Length = 20)

    let first = articles.[0]
    check "first article title" (first.Title = "Solar growth in South Asia has cut fuel imports for power but deeper reductions need electrification and regional grids")
    check "first article link" (first.Link = "https://ember-energy.org/latest-insights/solar-growth-in-south-asia-has-cut-fuel-imports-for-power-but-deeper-reductions-need-electrification-and-regional-grids")
    check "first article date" (first.Date = DateTime(2026, 3, 19, 0, 1, 0))

    // Article with <br> tag in title
    let brArticle = articles |> List.find (fun a -> a.Link.Contains("european-resource-adequacy-assessment-2"))
    check "<br> stripped from title" (not (brArticle.Title.Contains("<br>")) && brArticle.Title.EndsWith("European Resource Adequacy Assessment"))

    // Sort order: descending by date
    let dates = articles |> List.map (fun a -> a.Date)
    let isSorted = dates |> List.pairwise |> List.forall (fun (a, b) -> a >= b)
    check "articles sorted descending by date" isSorted

    // RSS XML generation
    let doc = generate articles
    let xml = doc.ToString()
    check "RSS contains first article title" (xml.Contains(first.Title))
    check "RSS contains first article link" (xml.Contains(first.Link))
    check "RSS version attribute" (xml.Contains("version=\"2.0\""))

let runLiveTest () =
    printfn ""
    printfn "=== Live integration test ==="
    use client = new System.Net.Http.HttpClient()
    let url = "https://ember-energy.org/wp-json/wp/v2/insight_page?per_page=100&_fields=title,link,date&orderby=date&order=desc"
    let json = client.GetStringAsync(url).Result
    let articles = parseArticles json
    if articles.IsEmpty then
        failwith "Live test: no articles parsed from ember-energy.org"
    else
        printfn "  PASS  live fetch returned %d articles" articles.Length
        printfn "  First: %s (%s)" articles.[0].Title (articles.[0].Date.ToString("yyyy-MM-dd"))
