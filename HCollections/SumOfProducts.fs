namespace HCollections

open TypeEquality

[<NoComparison>]
[<NoEquality>]
type 'tss SumOfProducts =
    private
    | Value of 'tss SumOfProductsValueCrate
    | Extended of 'tss SumOfProductsExtendedCrate

and private SumOfProductsValueEvaluator<'tss, 'ret> =
    abstract member Eval<'ts, 'tss2> : 'ts HList -> 'tss2 TypeListList -> Teq<'tss, 'ts -> 'tss2> -> 'ret

and private 'tss SumOfProductsValueCrate =
    abstract member Apply<'ret> : SumOfProductsValueEvaluator<'tss, 'ret> -> 'ret

and private SumOfProductsExtendedEvaluator<'tss, 'ret> =
    abstract member Eval<'ts, 'tss2> : 'tss2 SumOfProducts -> Teq<'tss, 'ts -> 'tss2> -> 'ret

and private 'tss SumOfProductsExtendedCrate =
    abstract member Apply : SumOfProductsExtendedEvaluator<'tss, 'ret> -> 'ret

[<RequireQualifiedAccess>]
module SumOfProducts =

    let cong (teq : Teq<'tss1, 'tss2>) : Teq<'tss1 SumOfProducts, 'tss2 SumOfProducts> =
        Teq.Cong.believeMe teq

    let make<'ts, 'tss> (tail : 'tss TypeListList) (value : 'ts HList) =
        { new SumOfProductsValueCrate<_> with
            member __.Apply e = e.Eval value tail Teq.refl
        }
        |> Value

    let extend<'ts, 'tss> (_ : 'ts TypeList) (sop : 'tss SumOfProducts) =
        { new SumOfProductsExtendedCrate<_> with
            member __.Apply e = e.Eval sop Teq.refl<'ts -> 'tss>
        }
        |> Extended

    let split<'ts, 'tss> (sop : ('ts -> 'tss) SumOfProducts) : Choice<'ts HList, 'tss SumOfProducts> =
        match sop with
        | Value c ->
            c.Apply
                { new SumOfProductsValueEvaluator<_,_> with
                    member __.Eval v _ teq =
                        v |> Teq.castFrom (teq |> Teq.Cong.domainOf |> HList.cong) |> Choice1Of2
                }
        | Extended c ->
            c.Apply
                { new SumOfProductsExtendedEvaluator<_,_> with
                    member __.Eval sop teq =
                        sop |> Teq.castFrom (teq |> Teq.Cong.rangeOf |> cong) |> Choice2Of2
                }

    let getSingleton (sop : ('ts -> unit) SumOfProducts) : 'ts HList =
        match sop with
        | Value c ->
            c.Apply
                { new SumOfProductsValueEvaluator<_,_> with
                    member __.Eval v _ teq =
                        v |> Teq.castFrom (teq |> Teq.Cong.domainOf |> HList.cong)
                }
        | Extended _ ->
            raise Unreachable
