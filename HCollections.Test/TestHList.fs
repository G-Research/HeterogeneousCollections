namespace HCollections.Test

open HCollections
open TypeEquality
open System
open Xunit

module TestHList =

    [<Fact>]
    let ``HList to type list is correct for an empty HList`` () =
        let testHlist = HList.empty

        Assert.Equal (TypeList.empty, HList.toTypeList testHlist)

    [<Fact>]
    let ``HList to type list is correct for an HList of size 1`` () =
        let testHlist = HList.empty |> HList.cons 300

        Assert.Equal<Type list> (TypeList.empty |> TypeList.cons<int, _> |> TypeList.toTypes, HList.toTypeList testHlist |> TypeList.toTypes)

    [<Fact>]
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

        Assert.Equal<Type list> (TypeList.toTypes expected, HList.toTypeList hlist |> TypeList.toTypes)
