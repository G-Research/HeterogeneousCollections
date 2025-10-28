namespace HCollections.Test

open HCollections
open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
module TestDiffList =

    [<Test>]
    let ``DiffList to HList to type list is correct for an empty DiffList`` () =
        let testDiffList = DiffList.empty

        testDiffList
        |> DiffList.toHList
        |> HList.toTypeList
        |> TypeList.toTypes
        |> shouldEqual []

    [<Test>]
    let ``DiffList to HList to type list is correct for an DiffList of size 1`` () =
        let testDiffList = DiffList.empty |> DiffList.cons 300

        let expected = TypeList.empty |> TypeList.cons<int, _> |> TypeList.toTypes

        testDiffList
        |> DiffList.toHList
        |> HList.toTypeList
        |> TypeList.toTypes
        |> shouldEqual expected

    [<Test>]
    let ``DiffList append example`` () =
        let firstDiffList =
            DiffList.empty |> DiffList.cons 300 |> DiffList.cons "abc" |> DiffList.cons true

        let secondDiffList = DiffList.empty |> DiffList.cons 2 |> DiffList.cons 123.456M

        let testDiffList = DiffList.append firstDiffList secondDiffList

        let expected =
            TypeList.empty
            |> TypeList.cons<int, _>
            |> TypeList.cons<decimal, _>
            |> TypeList.cons<int, _>
            |> TypeList.cons<string, _>
            |> TypeList.cons<bool, _>
            |> TypeList.toTypes

        testDiffList
        |> DiffList.toHList
        |> HList.toTypeList
        |> TypeList.toTypes
        |> shouldEqual expected

    [<Test>]
    let ``DiffList head example`` () =
        let testDiffList = DiffList.empty |> DiffList.cons true |> DiffList.cons "abc"

        let head = DiffList.head testDiffList

        head |> shouldEqual "abc"

    [<Test>]
    let ``DiffList tail example`` () =
        let testDiffList =
            DiffList.empty
            |> DiffList.cons "abc"
            |> DiffList.cons true
            |> DiffList.cons 123
            |> DiffList.cons 45m

        let expected =
            TypeList.empty
            |> TypeList.cons<string, _>
            |> TypeList.cons<bool, _>
            |> TypeList.cons<int, _>
            |> TypeList.toTypes

        testDiffList
        |> DiffList.tail
        |> DiffList.toHList
        |> HList.toTypeList
        |> TypeList.toTypes
        |> shouldEqual expected

    [<Test>]
    let ``DiffList folder example`` () =
        let testDiffList =
            DiffList.empty<unit>
            |> DiffList.cons false
            |> DiffList.cons "hi"
            |> DiffList.cons 300
            |> DiffList.cons 4.0

        let folder =
            { new DiffListFolder<_> with
                member _.Folder state elt = state + " " + elt.ToString ()
            }

        DiffList.fold folder "" testDiffList |> shouldEqual " 4 300 hi False"
