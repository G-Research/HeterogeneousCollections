namespace HCollections.Test

open HCollections
open TypeEquality
open System
open Xunit

module TestTypeList =

    [<Fact>]
    let ``TypeList.toTypes returns the correct types in the correct order`` () =

        let ts : (int -> string -> bool -> unit) TypeList =
            TypeList.empty
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons

        let expected = [ typeof<int> ; typeof<string> ; typeof<bool> ]

        Assert.Equal<Type list>(expected, ts |> TypeList.toTypes)

    [<Fact>]
    let ``TypeList.length is correct on the empty list`` () =
        Assert.Equal<int> (0, TypeList.length TypeList.empty)

    [<Fact>]
    let ``TypeList.length is correct on a list of length 1`` () =
        let ts : (int -> unit) TypeList =
            TypeList.empty
            |> TypeList.cons
        Assert.Equal<int> (1, TypeList.length ts)

    [<Fact>]
    let ``TypeList.length is correct on a list of length 3 which was made by splitting a list of length 4`` () =
        let ts : (float -> int -> string -> bool -> unit) TypeList =
            TypeList.empty
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
        let chopped : (int -> string -> bool -> unit) TypeList =
            match TypeList.split ts with
            | Choice1Of2 _ -> failwith "The split should definitely have worked"
            | Choice2Of2 teq -> 
                teq.Apply { new TypeListConsEvaluator<_,_> with
                    member __.Eval<'u, 'us> (us : 'us TypeList) (t : Teq<float -> int -> string -> bool -> unit, 'u -> 'us>) =
                        us
                        |> Teq.castFrom (TypeList.cong (Teq.Cong.rangeOf t))
                }

        Assert.Equal<int> (3, TypeList.length chopped)
