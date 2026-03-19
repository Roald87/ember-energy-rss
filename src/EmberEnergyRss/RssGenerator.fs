module EmberEnergyRss.RssGenerator

open System
open System.Globalization
open System.Xml.Linq
open EmberEnergyRss.Types

let private rfc822 (dt: DateTime) =
    dt.ToString("ddd, dd MMM yyyy HH:mm:ss +0000", CultureInfo.InvariantCulture)

let generate (articles: Article list) : XDocument =
    let ns = XNamespace.None

    let items =
        articles
        |> List.map (fun a ->
            XElement("item",
                XElement("title", a.Title),
                XElement("link", a.Link),
                XElement("guid", a.Link),
                XElement("pubDate", rfc822 a.Date)))

    let lastBuild =
        match articles with
        | [] -> rfc822 DateTime.UtcNow
        | a :: _ -> rfc822 a.Date

    let channelContent : obj list =
        [ XElement("title", "Ember Energy – Latest Insights")
          XElement("link", "https://ember-energy.org/latest-insights/")
          XElement("description", "Latest insights from Ember Energy")
          XElement("language", "en")
          XElement("lastBuildDate", lastBuild)
          yield! (items |> List.map (fun x -> x :> obj)) ]

    let channel = XElement("channel", channelContent |> Array.ofList)

    XDocument(
        XDeclaration("1.0", "utf-8", "yes"),
        XElement("rss",
            XAttribute("version", "2.0"),
            channel) :> obj)
