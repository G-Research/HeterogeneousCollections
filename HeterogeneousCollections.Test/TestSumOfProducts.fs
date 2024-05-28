namespace HCollections.Test

open HCollections
open Xunit

module TestSumOfProducts =

    let testHList = HList.empty |> HList.cons 1234 |> HList.cons "Foo"
    let testSop = SumOfProducts.make TypeListList.empty testHList

    let assertHListsEqual
        (expected : (string -> int -> unit) HList)
        (actual : (string -> int -> unit) HList) =

        let expectedString = expected |> HList.head
        let actualString = actual |> HList.head

        Assert.Equal(expectedString, actualString)

        let expectedInt = expected |> HList.tail |> HList.head
        let actualInt = actual |> HList.tail |> HList.head

        Assert.Equal(expectedInt, actualInt)

    [<Fact>]
    let ``Splitting a SumOfProducts that hasn't been extended returns an HList`` () =

        match testSop |> SumOfProducts.split with
        | Choice1Of2 xs -> assertHListsEqual testHList xs
        | Choice2Of2 _ -> Assert.True false

    [<Fact>]
    let ``Splitting a SumOfProducts that has been extended returns the inner SumOfProducts`` () =

        let union = testSop |> SumOfProducts.extend (TypeList.empty |> TypeList.cons<bool, _>)

        match union |> SumOfProducts.split with
        | Choice1Of2 _ -> Assert.True false
        | Choice2Of2 _ -> Assert.True true

    [<Fact>]
    let ``getSingleton returns the correct value`` () =

        let actual = testSop |> SumOfProducts.getSingleton
        assertHListsEqual testHList actual
