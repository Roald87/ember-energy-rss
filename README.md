# Ember Energy RSS Feed

Unofficial RSS feed for [Ember Energy – Latest Insights](https://ember-energy.org/latest-insights/).

## Subscribe

Feed URL:

```
https://raw.githubusercontent.com/Roald87/ember-energy-rss/main/rss.xml
```

Read it in your browser with [RSS Reader](https://rssrdr.com/?rss=https://raw.githubusercontent.com/Roald87/ember-energy-rss/main/rss.xml).

## For developers

### Run unit tests (uses local JSON fixture, no network)

```bash
dotnet run --project tests/EmberEnergyRss.Tests/EmberEnergyRss.Tests.fsproj
```

### Generate feed from a local JSON file

The main program accepts an optional second argument: a path to a local JSON file in the same format as the [WordPress REST API](https://ember-energy.org/wp-json/wp/v2/insight_page?per_page=100&_fields=title,link,date&orderby=date&order=desc).

```bash
dotnet run --project src/EmberEnergyRss/EmberEnergyRss.fsproj -- /tmp/test.rss tests/EmberEnergyRss.Tests/fixture.json
```

### Generate feed from the live site

```bash
dotnet run --project src/EmberEnergyRss/EmberEnergyRss.fsproj -- /tmp/test.rss
xmllint --noout /tmp/test.rss && echo "Valid"
```

### Live integration test (fetches ember-energy.org)

```bash
EMBER_TEST_LIVE=1 dotnet run --project tests/EmberEnergyRss.Tests/EmberEnergyRss.Tests.fsproj
```

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- No external NuGet packages. Only the .NET base class library.
