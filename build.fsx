#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"
#r "netstandard"

//open Fake.IO.Shell
open Fake.Core
open Fake.DotNet
open Fake.IO

Target.initEnvironment ()

let srcPath = Path.getFullName "./src/Sutil"
let deployDir = Path.getFullName "./deploy"
let testsPath = Path.getFullName "./tests/Shared"

let npm args workingDir =
    let npmPath =
        match ProcessUtils.tryFindFileOnPath "npm" with
        | Some path -> path
        | None ->
            "npm was not found in path. Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
            |> failwith

    let arguments = args |> String.split ' ' |> Arguments.OfArgs

    Command.RawCommand (npmPath, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let dotnet cmd workingDir =
    let result = DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

Target.create "Clean" (fun _ -> Shell.cleanDir deployDir)

Target.create "InstallClient" (fun _ -> npm "install" ".")

Target.create "Bundle" (fun _ ->
    dotnet (sprintf "publish -c Release -o \"%s\"" deployDir) srcPath
    npm "run build" "."
)

Target.create "BundleForGHPages" (fun _ ->
    npm "run build-gh-pages" "."
)

Target.create "Run" (fun _ ->
    dotnet "build" srcPath
    [ async { dotnet "watch run" srcPath }
      async { npm "run start" "." } ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

Target.create "RunTests" (fun _ ->
    dotnet "build" testsPath
    [ async { dotnet "watch run" testsPath }
      async { npm "run test:live" "." } ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

Target.create "PublishApp" <| fun _ ->
    npm "run publish" "."

let (</>) a b = Path.combine a b

let publish (projectPath : string) = fun _ ->
    [ projectPath </> "bin"
      projectPath </> "obj" ] |> Shell.cleanDirs
    dotnet "restore --no-cache" projectPath
    dotnet "pack -c Release" projectPath
    let nugetKey =
        match Environment.environVarOrNone "NUGET_KEY" with
        | Some nugetKey -> nugetKey
        | None -> failwith "The Nuget API key must be set in a NUGET_KEY environmental variable"
    let nupkg =
        System.IO.Directory.GetFiles(projectPath </> "bin" </> "Release")
        |> Seq.head
        |> System.IO.Path.GetFullPath

    let pushCmd = sprintf "nuget push %s -s nuget.org -k %s" nupkg nugetKey
    System.Console.WriteLine(pushCmd)
    dotnet pushCmd projectPath

Target.create "PublishPkg" (publish ("src" </> "Sutil"))

open Fake.Core.TargetOperators

"Clean"
    ==> "InstallClient"
    ==> "Run"

"Clean"
    ==> "InstallClient"
    ==> "RunTests"

"Clean"
    ==> "InstallClient"
    ==> "BundleForGHPages"
    ==> "PublishApp"

Target.runOrDefaultWithArguments "Bundle"
