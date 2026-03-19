module EmberEnergyRss.Parser

open System
open System.Text.Json
open System.Text.RegularExpressions
open EmberEnergyRss.Types

let private stripTags (s: string) =
    Regex.Replace(s, "<[^>]+>", "").Trim()

let parseArticles (json: string) : Article list =
    use doc = JsonDocument.Parse(json)
    [ for el in doc.RootElement.EnumerateArray() do
        let link = el.GetProperty("link").GetString()
        let rawTitle = el.GetProperty("title").GetProperty("rendered").GetString()
        let title = stripTags rawTitle
        let dateStr = el.GetProperty("date").GetString()

        let mutable date = DateTime.MinValue
        if DateTime.TryParse(dateStr, &date) then
            yield { Title = title; Link = link; Date = date } ]
    |> List.sortByDescending (fun a -> a.Date)
