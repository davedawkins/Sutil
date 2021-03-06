#load "node_modules/fable-publish-utils/PublishUtils.fs"
open PublishUtils


// ATTENTION: Packages must appear in dependency order
let packages =[
    "Sutil"
]

match argsLower with
| IgnoreCase "publish"::_rest ->
    for pkg in packages do
        pushNuget ("src" </> pkg </> pkg + ".fsproj") [] doNothing
| _ -> ()