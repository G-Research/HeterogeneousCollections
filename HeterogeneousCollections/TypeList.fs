namespace HCollections

open System
open TypeEquality

[<NoComparison>]
[<NoEquality>]
type 'ts TypeList =
    private
    | Empty of Teq<'ts, unit>
    | Cons of 'ts TypeListConsCrate * length : int

and TypeListConsEvaluator<'ts, 'ret> =
    abstract Eval<'t, 'ts2> : 'ts2 TypeList -> Teq<'ts, 't -> 'ts2> -> 'ret

and 'ts TypeListConsCrate =
    abstract Apply<'ret> : TypeListConsEvaluator<'ts, 'ret> -> 'ret

[<RequireQualifiedAccess>]
module TypeList =

    let cong (teq : Teq<'ts1, 'ts2>) : Teq<'ts1 TypeList, 'ts2 TypeList> =
        Teq.Cong.believeMe teq

    let empty = Empty Teq.refl

    let length<'ts> (ts : 'ts TypeList) : int =
        match ts with
        | Empty _ -> 0
        | Cons (_crate, length) ->
            length

    let cons<'t, 'ts> (types : 'ts TypeList) =
        let crate =
            { new TypeListConsCrate<_> with
                member __.Apply e = e.Eval types Teq.refl<'t -> 'ts>
            }
        let length = 1 + length types

        Cons (crate, length)

    let tail<'t, 'ts> (ts : ('t -> 'ts) TypeList) : 'ts TypeList =
        match ts with
        | Empty _ -> raise Unreachable
        | Cons (crate, _length) ->
            crate.Apply
                { new TypeListConsEvaluator<_,_> with
                    member __.Eval ts teq =
                        ts |> Teq.castFrom (teq |> Teq.Cong.rangeOf |> cong)
                }

    let split (ts : 'ts TypeList) =
        match ts with
        | Empty teq -> Choice1Of2 teq
        | Cons (crate, _length) -> Choice2Of2 crate

    let rec toTypes<'ts> (ts : 'ts TypeList) : Type list =
        match ts with
        | Empty _ -> []
        | Cons (crate, _length) ->
            crate.Apply
                { new TypeListConsEvaluator<_,_> with
                    member __.Eval ts (_ : Teq<_,'a -> _>) =
                        typeof<'a> :: toTypes ts
                }

