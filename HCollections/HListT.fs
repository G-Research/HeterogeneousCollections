namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type HListT<'ts, 'elem> =
    private
    | Empty of 'ts HList * Teq<'ts, unit>
    | Cons of 'ts HList * 'elem list * HListTConsCrate<'ts, 'elem>

and private HListTConsEvaluator<'ts, 'elem, 'ret> =
    abstract member Eval<'t, 'ts2> : HListT<'ts2, 'elem> -> Teq<'ts, 't -> 'ts2> -> 'ret

and private HListTConsCrate<'ts, 'elem> =
    abstract member Apply<'ret> : HListTConsEvaluator<'ts, 'elem, 'ret> -> 'ret

type HListTFolder<'state, 'elem> =
    abstract member Folder<'a> : 'state -> 'a -> 'elem -> 'state

module HListT =

    let cong (teq1 : Teq<'ts1, 'ts2>) (_ : Teq<'elem1, 'elem2>) : Teq<HListT<'ts1, 'elem1>, HListT<'ts2, 'elem2>> =
        Teq.Cong.believeMe teq1

    let toTypeList<'ts, 'elem> (list : HListT<'ts, 'elem>) : 'ts TypeList =
        match list with
        | Empty (h, _)
        | Cons (h, _, _) -> HList.toTypeList h

    let toHList<'ts, 'elem> (input : HListT<'ts, 'elem>) : 'ts HList =
        match input with
        | Empty (hlist, _)
        | Cons (hlist, _, _) -> hlist

    let toList<'ts, 'elem> (input : HListT<'ts, 'elem>) : 'elem list =
        match input with
        | Empty _ -> []
        | Cons (_, elems, _) -> elems

    let empty<'elem> : HListT<unit, 'elem> = HListT.Empty (HList.empty, Teq.refl)

    let length<'ts, 'elem> (xs : HListT<'ts, 'elem>) : int =
        match xs with
        | Empty _ -> 0
        | Cons (_, elems, _) -> List.length elems

    let cons<'t, 'ts, 'elem> (x : 't) (elem : 'elem) (xs : HListT<'ts, 'elem>) =
        let cons =
            { new HListTConsCrate<_, 'elem> with
                member __.Apply e =
                    e.Eval xs Teq.refl
            }
        HListT.Cons (HList.cons x (toHList xs), elem::(toList xs), cons)

    let head (xs : HListT<'t -> 'ts, 'elem>) : 't * 'elem =
        match xs with
        | Empty _ -> raise Unreachable
        | Cons (hlist, elems, _cons) ->
            HList.head hlist, List.head elems

    let tail (xs : HListT<'t -> 'ts, 'elem>) : HListT<'ts, 'elem> =
        match xs with
        | Empty _ -> raise Unreachable
        | Cons (_, _, cons) ->
            cons.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval xs teq =
                        let teq = cong (teq |> Teq.Cong.rangeOf) Teq.refl
                        xs |> Teq.castFrom teq
                }

    let rec fold<'state, 'ts, 'elem> (folder : HListTFolder<'state, 'elem>) (seed : 'state) (xs : HListT<'ts, 'elem>) : 'state =
        match xs with
        | Empty _ -> seed
        | Cons (hlist, elems, cons) ->
            cons.Apply
                { new HListTConsEvaluator<_,_,_> with
                    member __.Eval xs teq =
                        let x = HList.head (hlist |> Teq.castTo (HList.cong teq))
                        let y = List.head elems
                        fold folder (folder.Folder seed x y) xs
                }
