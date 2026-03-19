# Ember Energy RSS Feed

Unofficial RSS feed for [Ember Energy – Latest Insights](https://ember-energy.org/latest-insights/).

## Subscribe

Feed URL:

```
https://raw.githubusercontent.com/Roald87/ember-energy-rss/main/rss.xml
```

Read it in your browser with [RSS Reader](https://rssrdr.com/?rss=https://raw.githubusercontent.com/Roald87/ember-energy-rss/main/rss.xml).

## For developers

### Run unit tests (uses local HTML fixture, no network)

```bash
dotnet run --project tests/EmberEnergyRss.Tests/EmberEnergyRss.Tests.fsproj
```

### Generate feed from a local HTML file

```bash
dotnet run --project src/EmberEnergyRss/EmberEnergyRss.fsproj -- /tmp/test.rss "Latest Insights _ Ember.html"
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
