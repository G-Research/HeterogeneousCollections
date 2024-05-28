namespace HCollections.Test

open System
open HCollections
open Xunit

module TestHUnion =

    let testUnion = HUnion.make TypeList.empty 1234

    [<Fact>]
    let ``Splitting an HUnion that hasn't been extended returns the value of the union`` () =

        match testUnion |> HUnion.split with
        | Choice1Of2 j -> Assert.Equal(1234, j)
        | Choice2Of2 _ -> Assert.True false

    [<Fact>]
    let ``Splitting an HUnion that has been extended returns the inner union`` () =

        let union =
            testUnion
            |> HUnion.extend<string, _>

        match union |> HUnion.split with
        | Choice1Of2 _ -> Assert.True false
        | Choice2Of2 _ -> Assert.True true

    [<Fact>]
    let ``getSingleton returns the correct value`` () =

        let actual = testUnion |> HUnion.getSingleton
        Assert.Equal(1234, actual)

    [<Fact>]
    let ``toTypeList is correct on a union of size 1`` () =
    
        let union = testUnion
        let expected =
            TypeList.empty
            |> TypeList.cons<int, _>
            |> TypeList.toTypes

        Assert.Equal<Type list>(expected, HUnion.toTypeList union |> TypeList.toTypes)

    [<Fact>]
    let ``toTypeList is correct on a bigger union`` () =
    
        let union =
            HUnion.make (TypeList.empty |> TypeList.cons<int, _> |> TypeList.cons<float, _>) ()
            |> HUnion.extend<string, _>
            |> HUnion.extend<float, _>

        let expected : TypeList<float -> string -> unit -> float -> int -> unit> =
            TypeList.empty
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
        let expected = expected |> TypeList.toTypes

        Assert.Equal<Type list>(expected, HUnion.toTypeList union |> TypeList.toTypes)

