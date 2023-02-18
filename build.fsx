//
// FAKE script adapter for FSI
//
// Adapter details from here:
//      https://github.com/fsharp/FAKE/issues/2517
//
// Use this adapter template if you're suffering from this:
//      Error: Package manager key 'paket' was not registered
// Explanation here:
//      https://stackoverflow.com/questions/66665009/fix-for-package-manager-key-paket-was-not-registered-in-build-fsx
//
// Usage:
//   % dotnet fsi build.fsx
//   % dotnet fsi build.fsx --target <target>

#r "nuget: System.Reactive" // Prevent "Could not load file or assembly ..." error when using adapter
#r "nuget: Fake.Core.Target"
#load "node_modules/fable-publish-utils/PublishUtils.fs"

//open PublishUtils
open System
open System.IO
open Fake.Core
open PublishUtils
open Fake.Core.TargetOperators

// Boilerplate for adapter
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

// ---------------------------------------------------
// -- Your targets and regular FAKE code goes below --

let deleteFileIfExists file =
    if (File.Exists(file)) then
        File.Delete(file)

let copyFileOverwrite source target =
    deleteFileIfExists target
    File.Copy( source, target )

let setDotNet (v : int) =
    let globalJson = "global.json"
    copyFileOverwrite (sprintf "%s-dotnet%d" globalJson v) globalJson

let fsdocs (local : bool) =
    let fsDocsCache = Path.Combine(".fsdocs", "cache")
    let docsInputFolder = Path.Combine("temp", "docs")
    let root = if local then "http://127.0.0.1:5500/public/apidocs/" else "https://sutil.dev/apidocs/"

    deleteFileIfExists fsDocsCache

    //let fscOptions = " -r:/Users/david/.nuget/packages/fable.core/3.7.1/lib/netstandard2.0/Fable.Core.dll"

    run("dotnet build src/Sutil/Sutil.fsproj")
    //run($"dotnet fsdocs build --output public/docs --fscoptions \"{fscOptions}\" --parameters root ../")

    setDotNet(7)
    if not (Directory.Exists(docsInputFolder)) then
        Directory.CreateDirectory(docsInputFolder) |> ignore

    sprintf "dotnet tool install fsdocs-tool" |> run
    sprintf "dotnet fsdocs build --input %s --output public/apidocs --parameters fsdocs-logo-src ../../images/logo-small.png root %s" docsInputFolder root
    |> run
    sprintf "dotnet tool uninstall fsdocs-tool" |> run

    Directory.Delete(docsInputFolder)
    setDotNet(6)

Target.create "fsdocs" (fun _ -> fsdocs false)

Target.create "fsdocs:local" (fun _ -> fsdocs true)

Target.create "publish:package" (fun _ ->
    let pkg = "Sutil"
    pushNuget ("src" </> pkg </> pkg + ".fsproj") [] doNothing
)

Target.create "deploy:linode" (fun _ ->
    //   "deploy:linode": "bash deployToLinode.sh ./public sutil '' deploy@213.52.129.104 /home/deploy/apps",
    PublishUtils.run "bash deployToLinode.sh ./public sutil '' deploy@213.52.129.104 /home/deploy/apps"
)

Target.create "sutilxml" (fun _ ->
    PublishUtils.run("dotnet build -c Release src/Sutil/Sutil.fsproj")
    copyFileOverwrite "src/Sutil/bin/Release/netstandard2.0/Sutil.xml" "public/Sutil.xml"
)

Target.create "publish:website" (fun _ ->
    PublishUtils.run("node publish.js")
)

Target.create "usage" (fun _ ->
    Console.WriteLine("Targets: publish")
)

Target.create "samples" (fun _ ->
    let samplesTarget = Path.Combine( "public", "samples" )

    if (not(Directory.Exists(samplesTarget))) then
        Directory.CreateDirectory(samplesTarget) |> ignore

    let re = System.Text.RegularExpressions.Regex(@"view\s*\(\)")

    Directory.EnumerateFiles( "src/App", "*.fs" )
        |> Seq.toList
        |> List.filter (fun f -> Path.GetFileName(f) <> "App.fs")
        |> List.iter (fun f ->
            Console.WriteLine("copy {0}", f)
            let targetFs = Path.Combine(samplesTarget, Path.GetFileName(f))
            if (File.Exists(targetFs)) then
                File.Delete(targetFs)
            File.Copy( f, targetFs )

            let content = File.ReadAllText(targetFs)
            if (re.Match(content).Success) then
                Console.WriteLine("   Found view() function")
                File.AppendAllText( targetFs, "\nview() |> Program.mount\n")
        )
)

Target.create "build:app" (fun _ ->
    run "dotnet fable src/App --run webpack --mode production"
)

"fsdocs"
    ==> "publish:website"
    |> ignore

"fsdocs"
    ==> "build:app"
    ==> "samples"
    ==> "deploy:linode"
    |> ignore


Target.runOrDefault "usage"
