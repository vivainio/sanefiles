open System.IO
open System
open Argu
open QuiteFake.FileSystemHelper

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.


type Options = {
    mutable Full: bool
    mutable AllFiles: bool
}

let options = {
    Full = false;
    AllFiles = false
}

type CLIArguments =
    | [<AltCommandLine("-f")>] Full
    | [<AltCommandLine("-a")>] All

with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Full _ -> "Show all conflicting lines"
            | All _ -> "Check all files regardless of extension"
    


let rec walk_path dir = 
    if isFile dir then seq [dir] else 
    seq {
        yield! Directory.GetFiles(dir, "*")
        for subdir in Directory.GetDirectories(dir) do yield! walk_path subdir
    }


let find_bytes (haystack: byte[]) (needle: byte) = 
    let mutable i = 0
    seq {
        while i < haystack.Length do
            if haystack.[i] = needle then yield i
            i <- i+1
    } |> Seq.toArray

let line_at (cont: byte[]) (pos: int) =
    let mutable cur = pos
    // seek to start
    while cur > 0 && cont.[cur-1] <> '\n'B do
        cur <- cur-1
    let startpos = cur
    // seek to end
    cur <- pos
    while cur < cont.Length-1 && cont.[cur] <> '\n'B do
        cur <- cur+1
    (startpos, cur)


let get_string (cont: byte[]) (start, e) = 
    cont.[start..e] |>
    Text.Encoding.UTF8.GetString
   

type Analysis = 
    FullLinefeeds | SomeLinefeeds of (byte[] * int[])
    | Clean | Unchecked

let print_line (line: string) = 
    line.Replace("\t", "<TAB>")
        .Replace("\r", "<CR>")
        .Replace("\n", "<LF>") |>
        printfn "%s" 

    

let visualize path analysis =
    match analysis with
    | Analysis.Clean | Analysis.Unchecked -> ()
    | Analysis.FullLinefeeds -> printfn "N %s" path
    | Analysis.SomeLinefeeds(cont, offsets) -> 
        printfn "N- (%d) %s" offsets.Length path
        let maxlen = if options.Full then 1000 else 10
        if offsets.Length < maxlen then 
            offsets |> Seq.iter (fun p -> print_line (get_string cont (line_at cont p)))
        //for p in offsets do
        //    printf "  %s" (get_string cont (line_at cont p))

let lf_line_endings (cont: byte[]) =
    let endings = find_bytes cont '\n'B
    let bad_endings = 
        endings |> 
            Seq.filter (fun eidx -> eidx > 1 && cont.[eidx-1] <> '\r'B) |> 
            Seq.toArray
    match bad_endings.Length with 
        | 0 -> Clean
        | i when i = endings.Length -> FullLinefeeds
        | i -> SomeLinefeeds(cont, bad_endings) 



type FileType = 
    | Source
    | Text
    | Unknown

let get_file_type path = 
    if options.AllFiles then Source else
        let ext = Path.GetExtension path
        match ext.ToLower() with
        | ".cs" | ".py" | ".fs" | ".ts" -> Source
        | _ -> Unknown

// return number of failures
let check_file path = 
    match get_file_type path with 
    | Source ->
        let cont = File.ReadAllBytes path 
        lf_line_endings cont
    | _ -> Unchecked

let test_line_at() = 
    let cont = "0\n2\n\n5"B
    let line = line_at cont 0
    
    let assert_line_is offset wanted = 
        let line = line_at cont offset
        let s = get_string cont line
        assert (s = wanted)

    assert_line_is 0 "0\n"
    assert_line_is 1 "0\n"
    assert_line_is 2 "2\n"
    assert_line_is 3 "2\n"
    assert_line_is 4 "\n"
    assert_line_is 5 "5"
    
funit.add_test "line_at" test_line_at

let analyze_tree p = 
    let paths = walk_path p
    for p in paths do 
        let analysis = check_file p
        visualize p analysis
    
[<EntryPoint>]
let main argv =
    //funit.run_tests()
    let parser = ArgumentParser.Create<CLIArguments>()
    let parsed = parser.ParseCommandLine(ignoreUnrecognized = true)

    for p in parsed.GetAllResults() do 
        match p with 
        | Full -> options.Full <- true
        | All -> options.AllFiles <- true
   
    match parsed.UnrecognizedCliParams with 
    | [] -> printfn "%s" (parser.PrintUsage())
    | [pth] -> analyze_tree pth
    | _ -> printfn "%s" (parser.PrintUsage())

        
    0 // return an integer exit code
