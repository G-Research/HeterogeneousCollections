namespace HCollections.Test

open HCollections
open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
module TestHUnion =

    let testUnion = HUnion.make TypeList.empty 1234

    [<Test>]
    let ``Splitting an HUnion that hasn't been extended returns the value of the union`` () =

        match testUnion |> HUnion.split with
        | Choice1Of2 j -> j |> shouldEqual 1234
        | Choice2Of2 _ -> failwith "expected Choice1Of2"

    [<Test>]
    let ``Splitting an HUnion that has been extended returns the inner union`` () =

        let union = testUnion |> HUnion.extend<string, _>

        match union |> HUnion.split with
        | Choice1Of2 _ -> failwith "expected Choice2Of2"
        | Choice2Of2 _ -> ()

    [<Test>]
    let ``getSingleton returns the correct value`` () =

        let actual = testUnion |> HUnion.getSingleton
        actual |> shouldEqual 1234

    [<Test>]
    let ``toTypeList is correct on a union of size 1`` () =

        let union = testUnion
        let expected = TypeList.empty |> TypeList.cons<int, _> |> TypeList.toTypes

        HUnion.toTypeList union |> TypeList.toTypes |> shouldEqual expected

    [<Test>]
    let ``toTypeList is correct on a bigger union`` () =

        let union =
            HUnion.make (TypeList.empty |> TypeList.cons<int, _> |> TypeList.cons<float, _>) ()
            |> HUnion.extend<string, _>
            |> HUnion.extend<float, _>

        let expected: TypeList<float -> string -> unit -> float -> int -> unit> =
            TypeList.empty
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons
            |> TypeList.cons

        let expected = expected |> TypeList.toTypes

        HUnion.toTypeList union |> TypeList.toTypes |> shouldEqual expected
