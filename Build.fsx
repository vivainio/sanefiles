#r "packages/FAKE/tools/FakeLib.dll" // include Fake lib
open Fake

let deployDir = "./deploy/"

let version = "0.1"
let buildDir = "./bin/Debug/"
let appName = "sanefiles"


Target "Zip" (fun _ ->
    let zipname = sprintf "%s%s.%s.zip" deployDir appName version
    printfn "zip is %s" zipname
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir zipname
)

Target "Deploy" (fun _ -> 
    printfn "deploying"
)

Run "Zip"