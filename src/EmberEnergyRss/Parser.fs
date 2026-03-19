module EmberEnergyRss.Parser

open System
open System.Globalization
open System.Text.RegularExpressions
open EmberEnergyRss.Types

let private cardPattern =
    Regex(
        """<a\s+href="(https://ember-energy\.org/latest-insights/[^"]+)"\s+class="card w-inline-block\s+insight_page">([\s\S]*?)</a>""",
        RegexOptions.Compiled
    )

let private titlePattern =
    Regex("""<div\s+class="cardtitle">([\s\S]*?)</div>""", RegexOptions.Compiled)

let private datePattern =
    Regex("""<div\s+class="carddate">\s*(\d{1,2}\s+\w+\s+\d{4})\s*</div>""", RegexOptions.Compiled)

let private stripTags (s: string) =
    Regex.Replace(s, "<[^>]+>", "").Trim()

let parseArticles (html: string) : Article list =
    [ for cardMatch in cardPattern.Matches(html) do
        let link = cardMatch.Groups.[1].Value
        let inner = cardMatch.Groups.[2].Value

        let titleMatch = titlePattern.Match(inner)
        let dateMatch = datePattern.Match(inner)

        if titleMatch.Success && dateMatch.Success then
            let rawTitle = titleMatch.Groups.[1].Value
            let title = stripTags rawTitle
            let dateStr = dateMatch.Groups.[1].Value

            let mutable date = DateTime.MinValue
            if DateTime.TryParseExact(dateStr, "d MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, &date) then
                yield { Title = title; Link = link; Date = date } ]
    |> List.sortByDescending (fun a -> a.Date)
