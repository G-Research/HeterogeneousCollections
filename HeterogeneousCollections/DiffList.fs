namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type DiffList<'ty, 'v> =
    private
    | Nil of Teq<'ty, 'v>
    | Cons of DiffListConsCrate<'ty, 'v>

and DiffListConsCrate<'ty, 'v> =
    abstract member Apply<'ret> : DiffListConsEvaluator<'ty, 'v, 'ret> -> 'ret

and DiffListConsEvaluator<'ty, 'v, 'ret> =
    abstract member Eval<'a, 'rest> : 'a * DiffList<'rest, 'v> * Teq<'ty, 'a -> 'rest> -> 'ret

type 'state DiffListFolder =
    abstract Folder<'a> : 'state -> 'a -> 'state

module DiffList =

    let empty<'v> : DiffList<'v, 'v> = DiffList.Nil Teq.refl

    let cons (x : 'a) (xs : DiffList<'ty, 'v>) : DiffList<'a -> 'ty, 'v> =
        let crate =
            { new DiffListConsCrate<'a -> 'ty, 'v> with
                member __.Apply (e) = e.Eval (x, xs, Teq.refl)
            }

        DiffList.Cons crate

    let cong (teq : Teq<'ty1, 'ty2>) : Teq<DiffList<'ty1, 'v>, DiffList<'ty2, 'v>> = Teq.Cong.believeMe teq

    let congUnification (teq : Teq<'u, 'v>) : Teq<DiffList<'ty, 'u>, DiffList<'ty, 'v>> = Teq.Cong.believeMe teq

    let rec append<'ty, 'u, 'v> (xs : DiffList<'ty, 'u>) (ys : DiffList<'u, 'v>) : DiffList<'ty, 'v> =
        match xs with
        | Nil teq -> Teq.castFrom (cong teq) ys
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, DiffList<'ty, 'v>> with
                    member this.Eval
                        (x : 'a, xs : DiffList<'rest, 'u>, teq : Teq<'ty, ('a -> 'rest)>)
                        : DiffList<'ty, 'v>
                        =
                        DiffList.Cons
                            { new DiffListConsCrate<'ty, 'v> with
                                member this.Apply (e) = e.Eval (x, append xs ys, teq)
                            }
                }

    let rec length<'ty, 'v> (xs : DiffList<'ty, 'v>) : int =
        match xs with
        | Nil _ -> 0
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member this.Eval (_, xs : DiffList<_, _>, _) = 1 + length xs
                }

    let head (xs : DiffList<'a -> 'ty, 'v>) : 'a =
        match xs with
        | Nil _ -> raise Unreachable
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, _, teq) =
                        let teq = teq |> Teq.Cong.domainOf
                        x |> Teq.castFrom teq
                }

    let tail (xs : DiffList<'a -> 'ty, 'v>) : DiffList<'ty, 'v> =
        match xs with
        | Nil _ -> raise Unreachable
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, xs, teq) =
                        let teq = teq |> Teq.Cong.rangeOf |> cong
                        xs |> Teq.castFrom teq
                }

    let rec fold<'state, 'ty, 'v> (folder : 'state DiffListFolder) (seed : 'state) (xs : DiffList<'ty, 'v>) : 'state =
        match xs with
        | Nil _ -> seed
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<_, _, _> with
                    member __.Eval (x, xs, _) = fold folder (folder.Folder seed x) xs
                }

    let rec toHList<'ty> (xs : DiffList<'ty, unit>) : 'ty HList =
        match xs with
        | Nil teq -> Teq.castFrom (HList.cong teq) HList.empty
        | Cons crate ->
            crate.Apply
                { new DiffListConsEvaluator<'ty, unit, 'ty HList> with
                    member this.Eval (x : 'a, xs : DiffList<'rest, unit>, teq : Teq<'ty, ('a -> 'rest)>) : 'ty HList =
                        HList.cons x (toHList xs) |> Teq.castFrom (HList.cong teq)
                }
