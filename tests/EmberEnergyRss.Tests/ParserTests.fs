module EmberEnergyRss.Tests.ParserTests

open System
open System.IO
open EmberEnergyRss.Parser
open EmberEnergyRss.RssGenerator

// Path to the saved HTML fixture, relative to this source file
let private htmlFixturePath =
    Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "Latest Insights _ Ember.html")

let private loadFixture () =
    File.ReadAllText(htmlFixturePath)

let private check label ok =
    if ok then
        printfn "  PASS  %s" label
    else
        failwithf "FAIL  %s" label

let runUnitTests () =
    let html = loadFixture ()
    let articles = parseArticles html

    check "article count is 16" (articles.Length = 16)

    let first = articles.[0]
    check "first article title" (first.Title = "The energy security fallout: from fossil fuel fragility to electric independence")
    check "first article link" (first.Link = "https://ember-energy.org/latest-insights/the-energy-security-fall-out-from-fossil-fuel-fragility-to-electric-independence")
    check "first article date" (first.Date = DateTime(2026, 3, 18))

    // Article with <br> tag in title (originally "...Assessment<br>")
    let brArticle = articles |> List.find (fun a -> a.Link.Contains("european-resource-adequacy-assessment-2"))
    check "<br> stripped from title" (not (brArticle.Title.Contains("<br>")) && brArticle.Title.EndsWith("European Resource Adequacy Assessment"))

    // Date with leading zero ("03 March 2026")
    let leadingZeroArticle = articles |> List.find (fun a -> a.Date = DateTime(2026, 3, 3))
    check "leading-zero day parsed correctly" (leadingZeroArticle.Date.Day = 3)

    // Date with leading zero ("09 February 2026")
    let singleDigitArticle = articles |> List.find (fun a -> a.Date = DateTime(2026, 2, 9))
    check "single-digit day (09) parsed correctly" (singleDigitArticle.Date.Day = 9)

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
    client.DefaultRequestHeaders.UserAgent.ParseAdd("EmberEnergyRss/1.0")
    let html = client.GetStringAsync("https://ember-energy.org/latest-insights/").Result
    let articles = parseArticles html
    if articles.IsEmpty then
        failwith "Live test: no articles parsed from ember-energy.org"
    else
        printfn "  PASS  live fetch returned %d articles" articles.Length
        printfn "  First: %s (%s)" articles.[0].Title (articles.[0].Date.ToString("yyyy-MM-dd"))
