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

open PublishUtils
open System
open System.IO
open Fake.Core

// Boilerplate for adapter
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

// ---------------------------------------------------
// -- Your targets and regular FAKE code goes below --

Target.create "publish" (fun _ ->
    let pkg = "Sutil"
    pushNuget ("src" </> pkg </> pkg + ".fsproj") [] doNothing
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
                File.AppendAllText( targetFs, "\nview() |> Program.mountElement \"sutil-app\"\n")
        )
)

Target.runOrDefault "usage"
