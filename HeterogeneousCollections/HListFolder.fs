namespace HCollections

[<RequireQualifiedAccess>]
module HListFolder =

    let makeGappedElementFolder (f : 'state -> 'a option -> 'state) : 'state HListFolder =
        { new HListFolder<_> with
            member __.Folder s (x : 'b) =
                let v =
                    if typeof<'b> = typeof<'a> then
                        x |> unbox |> Some
                    else
                        None
                f s v
        }

    let makeElementFolder (f : 'state -> 'a -> 'state) : 'state HListFolder =
        let g s o = match o with Some a -> f s a | None -> s
        makeGappedElementFolder g
