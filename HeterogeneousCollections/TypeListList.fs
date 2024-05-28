namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type 'tss TypeListList =
    private
    | Empty of Teq<'tss, unit>
    | Cons of 'tss TypeListListConsCrate

and private TypeListListConsEvaluator<'tss, 'ret> =
    abstract member Eval<'ts, 'tss2> : 'ts TypeList -> 'tss2 TypeListList -> Teq<'tss, 'ts -> 'tss2> -> 'ret

and private 'tss TypeListListConsCrate =
    abstract member Apply<'ret> : TypeListListConsEvaluator<'tss, 'ret> -> 'ret

[<RequireQualifiedAccess>]
module TypeListList =

    let empty = Empty Teq.refl

    let cons<'ts, 'tss> (ts : 'ts TypeList) (tss : 'tss TypeListList) =
        { new TypeListListConsCrate<_> with
            member __.Apply e = e.Eval ts tss Teq.refl<'ts -> 'tss>
        }
        |> Cons
