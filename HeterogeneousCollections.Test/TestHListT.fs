namespace HCollections.Test

open HCollections
open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
module TestHListT =

    [<Test>]
    let ``HListT to type list is correct for an empty HListT`` () =
        let testHlist = HListT.empty<int>

        HListT.toTypeList testHlist |> TypeList.toTypes |> shouldEqual []

    [<Test>]
    let ``HListT to type list is correct for an HListT of length 2`` () =
        let testHlist = HListT.empty<int> |> HListT.cons "hi" 4 |> HListT.cons 4.5 10

        let expected = TypeList.empty |> TypeList.cons<string, _> |> TypeList.cons<float, _>

        HListT.toTypeList testHlist
        |> TypeList.toTypes
        |> shouldEqual (TypeList.toTypes expected)

    [<Test>]
    let ``HListT length is right for length 2`` () =
        let testHlist = HListT.empty<int> |> HListT.cons "hi" 4 |> HListT.cons 4.5 10

        HListT.length testHlist |> shouldEqual 2

    [<Test>]
    let ``HlistT.tolist returns the correct list`` () =
        let testHlist =
            HListT.empty<int> |> HListT.cons "hi" 4 |> HListT.cons 4.5 10 |> HListT.toList

        testHlist |> shouldEqual [ 10; 4 ]

    [<Test>]
    let ``HListT.toList on an empty HListT returns an empty list`` () =
        let testList = HListT.toList HListT.empty<int>

        testList |> shouldEqual []

    [<Test>]
    let ``HListT.toHList returns the correct HList type`` () =
        let testHlist =
            HListT.empty<int> |> HListT.cons "hi" 4 |> HListT.cons 4.5 10 |> HListT.toHList

        let expected = TypeList.empty |> TypeList.cons<string, _> |> TypeList.cons<float, _>

        HList.toTypeList testHlist
        |> TypeList.toTypes
        |> shouldEqual (TypeList.toTypes expected)

    [<Test>]
    let ``HListT.toHList on an empty HListT then getting the type list returns an empty list`` () =
        let testHlist = HListT.empty<int> |> HListT.toHList

        HList.toTypeList testHlist |> TypeList.toTypes |> shouldEqual []
