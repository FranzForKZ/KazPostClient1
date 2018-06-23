// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"

open Fake

// Default target
Target "Default" (fun _ -> trace "Hello World from FAKE")

let buildBaseDir = __SOURCE_DIRECTORY__ + "/build/"
let buildDir = buildBaseDir + "KazPostARM/"









Target "Clean" (fun _ -> CleanDirs [ buildBaseDir ])
Target "BuildApp" (fun _ -> 
    trace "BuildApp start"
    //let fsprojectsPattern = __SOURCE_DIRECTORY__ + "/**/*.fsproj"
    //let csprojectsPattern = __SOURCE_DIRECTORY__ + "/**/*.csproj"
    let prDir = __SOURCE_DIRECTORY__ + "/PostUserActivity/KazPostARM.csproj"
    //!! "/**/*.fsproj" 
    //!! csprojectsPattern 
    !!prDir
    //|> MSBuildRelease deployDir "Build"    
    |> MSBuildDebug buildDir "Build"
    |> Log "AppBuild-Output: "
    trace "BuildApp stop")

let dateStr = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm")

let file = buildBaseDir + "KazPostARM." + dateStr + ".zip"

Target "Zip" (fun _ ->     
    !!(buildDir + "/**/*.*") -- "*.zip" |> Zip buildDir file
    
)

Target "DeleteLibraries" (fun _ -> 
    let x86 = buildDir + "x86/"
    let cascades = buildDir + "Cascades/"
    let lib = buildDir + "Lib/"
    CleanDirs [ x86; cascades; lib ]
)

Target "CopyToTest" (fun _ ->
    CopyFile "\\\\COMP-TEST30\\Shared_End_8-1_32\\" file
)

"Clean" 
    ==> "BuildApp"
    //==> "DeleteLibraries"
    ==> "Zip"
    ==> "CopyToTest"
    ==> "Default"
// start build
RunTargetOrDefault "Default"
