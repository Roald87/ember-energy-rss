module EmberEnergyRss.Tests.Program

open System

[<EntryPoint>]
let main argv =
    let live =
        Environment.GetEnvironmentVariable("EMBER_TEST_LIVE") = "1"
        || (argv.Length > 0 && argv.[0] = "--live")

    printfn "=== EmberEnergyRss unit tests ==="
    try
        ParserTests.runUnitTests ()
        printfn "All unit tests passed."

        if live then
            ParserTests.runLiveTest ()

        0
    with ex ->
        eprintfn "%s" ex.Message
        1
