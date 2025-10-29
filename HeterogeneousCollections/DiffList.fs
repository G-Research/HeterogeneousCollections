namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type DiffList<'elts, 'tailSlot> =
    private
    | Nil of Teq<'elts, 'tailSlot>
    | Cons of DiffListConsCrate<'elts, 'tailSlot>

and DiffListConsCrate<'elts, 'tailSlot> =
    abstract member Apply<'ret> : DiffListConsEvaluator<'elts, 'tailSlot, 'ret> -> 'ret

and DiffListConsEvaluator<'elts, 'tailSlot, 'ret> =
    abstract member Eval<'elt, 'rest> : 'elt * DiffList<'rest, 'tailSlot> * Teq<'elts, 'elt -> 'rest> -> 'ret

type 'state DiffListFolder =
    abstract Folder<'elt> : 'state -> 'elt -> 'state

[<RequireQualifiedAccess>]
module DiffList =

    let empty<'tailSlot> : DiffList<'tailSlot, 'tailSlot> = DiffList.Nil Teq.refl

    let cons (x : 'elt) (xs : DiffList<'elts, 'tailSlot>) : DiffList<'elt -> 'elts, 'tailSlot> =
        let crate =
            { new DiffListConsCrate<'elt -> 'elts, 'tailSlot> with
                member __.Apply (e) = e.Eval (x, xs, Teq.refl)
            }

        DiffList.Cons crate

    let private congElts (teq : Teq<'elts1, 'elts2>) : Teq<DiffList<'elts1, 'tailSlot>, DiffList<'elts2, 'tailSlot>> =
        Teq.Cong.believeMe teq

    let private congTailSlot
        (teq : Teq<'tailSlot1, 'tailSlot2>)
        : Teq<DiffList<'elts, 'tailSlot1>, DiffList<'elts, 'tailSlot2>>
        =
        Teq.Cong.believeMe teq

    let cong
        (eltsPrf : Teq<'elts1, 'elts2>)
        (tailPrf : Teq<'tailSlot1, 'tailSlot2>)
        : Teq<DiffList<'elts1, 'tailSlot1>, DiffList<'elts2, 'tailSlot2>>
        =
        Teq.transitivity (congElts eltsPrf) (congTailSlot tailPrf)

    let rec append<'elts1, 'elts2, 'tail>
        (first : DiffList<'elts1, 'elts2>)
        (second : DiffList<'elts2, 'tail>)
        : DiffList<'elts1, 'tail>
        =
        match first with
        | Nil teq -> Teq.castFrom (congElts teq) second
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, DiffList<_, _>> with
                    member this.Eval
                        (x : 'elt, xs : DiffList<'rest, 'elts2>, teq : Teq<'elts1, ('elt -> 'rest)>)
                        : DiffList<'elts1, 'tail>
                        =
                        DiffList.Cons
                            { new DiffListConsCrate<'elts1, 'tail> with
                                member this.Apply (e) = e.Eval (x, append xs second, teq)
                            }
                }

    let rec length<'elts, 'tailSlot> (xs : DiffList<'elts, 'tailSlot>) : int =
        match xs with
        | Nil _ -> 0
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member this.Eval (_, xs : DiffList<_, _>, _) = 1 + length xs
                }

    let head (xs : DiffList<'elt -> 'elts, 'tailSlot>) : 'elt =
        match xs with
        | Nil _ -> raise Unreachable
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, _, teq) =
                        let teq = teq |> Teq.Cong.domainOf
                        x |> Teq.castFrom teq
                }

    let tail (xs : DiffList<'elt -> 'elts, 'tailSlot>) : DiffList<'elts, 'tailSlot> =
        match xs with
        | Nil _ -> raise Unreachable
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, xs, teq) =
                        let teq = teq |> Teq.Cong.rangeOf |> congElts
                        xs |> Teq.castFrom teq
                }

    let rec fold<'state, 'elts, 'tailSlot>
        (folder : 'state DiffListFolder)
        (seed : 'state)
        (xs : DiffList<'elts, 'tailSlot>)
        : 'state
        =
        match xs with
        | Nil _ -> seed
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, xs, _) = fold folder (folder.Folder seed x) xs
                }

    let rec toHList<'elts> (xs : DiffList<'elts, unit>) : 'elts HList =
        match xs with
        | Nil teq -> Teq.castFrom (HList.cong teq) HList.empty
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<'elts, unit, 'elts HList> with
                    member this.Eval
                        (x : 'elt, xs : DiffList<'rest, unit>, teq : Teq<'elts, ('elt -> 'rest)>)
                        : 'elts HList
                        =
                        HList.cons x (toHList xs) |> Teq.castFrom (HList.cong teq)
                }
