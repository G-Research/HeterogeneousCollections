namespace HCollections.Test

open HCollections
open NUnit.Framework
open FsUnitTyped

[<TestFixture>]
module TestHList =

    [<Test>]
    let ``HList to type list is correct for an empty HList`` () =
        let testHlist = HList.empty

        HList.toTypeList testHlist
        |> TypeList.toTypes
        |> shouldEqual []

    [<Test>]
    let ``HList to type list is correct for an HList of size 1`` () =
        let testHlist = HList.empty |> HList.cons 300

        let expected = TypeList.empty |> TypeList.cons<int, _> |> TypeList.toTypes
        HList.toTypeList testHlist |> TypeList.toTypes
        |> shouldEqual expected

    [<Test>]
    let ``HList to type list is correct for an HList of size 4`` () =
        let hlist : (float -> int -> string -> bool -> unit) HList =
            HList.empty
            |> HList.cons false
            |> HList.cons "hi"
            |> HList.cons 300
            |> HList.cons 4.0

        let expected =
            TypeList.empty
            |> TypeList.cons<bool, _>
            |> TypeList.cons<string, _>
            |> TypeList.cons<int, _>
            |> TypeList.cons<float, _>

        HList.toTypeList hlist |> TypeList.toTypes
        |> shouldEqual (TypeList.toTypes expected)
