module EasyBuild.Main

open SimpleExec
open Spectre.Console.Cli
open PublishUtils

type EmptySettings() = inherit CommandSettings()

type CommandFn( f : unit -> unit ) =
    inherit Command<EmptySettings>() 
        override _.Execute(context, settings) =
            f()
            0    

type CommandNameArgs( cmdName : string, cmdArgs : string ) =
    inherit Command<EmptySettings>() 
        override _.Execute(context, settings) =
            Command.Run(cmdName, cmdArgs)
            0    

module Utils =
    open System.IO

    let deleteFileIfExists file =
        if (File.Exists(file)) then
            File.Delete(file)

    let copyFileOverwrite source target =
        deleteFileIfExists target
        File.Copy( source, target )

    let setDotNet (v : int) =
        let globalJson = "global.json"
        copyFileOverwrite (sprintf "%s-dotnet%d" globalJson v) globalJson

type CleanCommand() = inherit CommandFn( fun _ -> 
        Command.Run("dotnet", "fable clean --yes")
        Command.Run("dotnet", "clean")
    )

type PublishPackageCommand() = inherit CommandFn( fun _ -> 
        let pkg = "Sutil"
        pushNuget ("src" </> pkg </> pkg + ".fsproj") [] doNothing
    )

type BuildCommand() = inherit CommandNameArgs( "dotnet", "fable src/App --run webpack --mode production" )
type PackCommand() = inherit CommandNameArgs( "dotnet", "pack -c Release src/Sutil/Sutil.fsproj" )

type DeployCommand() = inherit CommandNameArgs( 
    "bash", 
    "deployToLinode.sh ./public sutil '' deploy@213.52.129.104 /home/deploy/apps" )

type SutilXmlCommand() = inherit CommandFn( fun _ ->
        Command.Run("dotnet", 
            "build -c Release src/Sutil/Sutil.fsproj"
        )
        Utils.copyFileOverwrite "src/Sutil/bin/Release/netstandard2.0/Sutil.xml" "public/Sutil.xml"
    )

let dependencies = 
    [
        "deploy:linode", 
            [
            "samples"
            "build:app"
            "fsdocs"
            ]

        "publish:package",  [ "clean" ]
        
        "pack",  [ "clean" ]

    ] |> Map

module FsDocsCommand =
    open System.IO
    open System.ComponentModel
    open Utils

    let fsdocs (local : bool) =
        let fsDocsCache = Path.Combine(".fsdocs", "cache")
        let docsInputFolder = Path.Combine("temp", "docs")
        let root = if local then "http://127.0.0.1:5500/public/apidocs/" else "https://sutil.dev/apidocs/"

        deleteFileIfExists fsDocsCache

        //let fscOptions = " -r:/Users/david/.nuget/packages/fable.core/3.7.1/lib/netstandard2.0/Fable.Core.dll"

        Command.Run("dotnet", "build src/Sutil/Sutil.fsproj")
        //run($"dotnet fsdocs build --output public/docs --fscoptions \"{fscOptions}\" --parameters root ../")

        //setDotNet(7)
        if not (Directory.Exists(docsInputFolder)) then
            Directory.CreateDirectory(docsInputFolder) |> ignore

        Command.Run("dotnet", "tool install fsdocs-tool")
        Command.Run("dotnet", sprintf "fsdocs build --input %s --output public/apidocs --parameters fsdocs-logo-src ../../images/logo-small.png root %s" docsInputFolder root)

        //Command.Run("dotnet", "tool uninstall fsdocs-tool")
        Directory.Delete(docsInputFolder)
        //setDotNet(6)

    type FsDocsSettings() =
        inherit CommandSettings()
        [<CommandOption("-l|--local")>]
        [<Description("Use http://127.0.0.1:5500/public/apidocs as base URL")>]
        member val IsLocal: bool = false with get, set

    type FsDocsCommand() =
        inherit Command<FsDocsSettings>()

        override _.Execute(context, settings) =
            fsdocs(settings.IsLocal)
            0

open System
open System.IO

type SamplesCommand() = inherit CommandFn(fun _ ->
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

[<EntryPoint>]
let main args =

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config
            .AddCommand<SamplesCommand>("build:app")
            .WithDescription("Build app and create bundle for sutil.dev")
        |> ignore

        config
            .AddCommand<CleanCommand>("clean")
            .WithDescription("Clean the project")
        |> ignore

        config
            .AddCommand<DeployCommand>("deploy:linode")
            .WithDescription("Publish the Sutil app with examples and docs to sutil.dev")
        |> ignore

        config
            .AddCommand<FsDocsCommand.FsDocsCommand>("fsdocs")
            .WithDescription("Generate API docs using fsdocs")
        |> ignore

        config
            .AddCommand<PackCommand>("pack")
            .WithDescription("Create nuget package")
        |> ignore

        config
            .AddCommand<PublishPackageCommand>("publish:package")
            .WithDescription("Build package and push to nuget")
        |> ignore

        config
            .AddCommand<SamplesCommand>("samples")
            .WithDescription("Generate samples for sutil.dev")
        |> ignore

        config
            .AddCommand<SutilXmlCommand>("sutilxml")
            .WithDescription("Generate Sutil.xml and copy to public/")
        |> ignore
    )

    let dependenciesOf( target : string ) =
        target |> (dependencies.TryFind) |> Option.defaultValue []

    let rec run (args : string array) =
        match args |> Array.tryHead with
        | Some target ->
            dependenciesOf target |> List.iter (fun dep ->
                Console.WriteLine("Target '" + target + "' depends on '" + dep + "':")

                if run( [| dep |] ) <> 0 then
                    failwith ("Dependency '" + dep + "' returned non-zero exit code")

                Console.WriteLine("Dependency '" + target + "' completed successfully")
                    
            )
        | _ -> ()

        app.Run(args)

    run args