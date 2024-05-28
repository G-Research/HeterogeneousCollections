namespace HCollections.Test

open HCollections
open TypeEquality
open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
module TestTypeList =

    [<Test>]
    let ``TypeList.toTypes returns the correct types in the correct order`` () =

        let ts: (int -> string -> bool -> unit) TypeList =
            TypeList.empty |> TypeList.cons |> TypeList.cons |> TypeList.cons

        let expected = [ typeof<int>; typeof<string>; typeof<bool> ]

        ts |> TypeList.toTypes |> shouldEqual expected

    [<Test>]
    let ``TypeList.length is correct on the empty list`` () =
        TypeList.length TypeList.empty |> shouldEqual 0

    [<Test>]
    let ``TypeList.length is correct on a list of length 1`` () =
        let ts: (int -> unit) TypeList = TypeList.empty |> TypeList.cons
        TypeList.length ts |> shouldEqual 1

    [<Test>]
    let ``TypeList.length is correct on a list of length 3 which was made by splitting a list of length 4`` () =
        let ts: (float -> int -> string -> bool -> unit) TypeList =
            TypeList.empty
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons

        let chopped: (int -> string -> bool -> unit) TypeList =
            match TypeList.split ts with
            | Choice1Of2 _ -> failwith "The split should definitely have worked"
            | Choice2Of2 teq ->
                teq.Apply
                    { new TypeListConsEvaluator<_, _> with
                        member __.Eval<'u, 'us>
                            (us: 'us TypeList)
                            (t: Teq<float -> int -> string -> bool -> unit, 'u -> 'us>)
                            =
                            us |> Teq.castFrom (TypeList.cong (Teq.Cong.rangeOf t)) }

        TypeList.length chopped |> shouldEqual 3
