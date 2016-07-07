module funit

open FSharp.Quotations

// trivial unit testing framework

let mutable all_tests = [];

let add_test (name:string) (f: unit -> unit) =
     all_tests <- (name, f) :: all_tests
    
    
let run_tests() = 
    all_tests |> Seq.iter (fun (name,f) ->
        printfn "Test %s" name 
        f())
    