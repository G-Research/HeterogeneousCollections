# HeterogeneousCollections

Type-safe heterogeneous collections for F#.

## Concepts

A standard F# list is *homogeneous*: every list element must have the same type.
An `int list` can only contain integers.

The F# standard library does contain heterogeneous collection types, but there are downsides to their use.
For example:

* `System.Collections.IList` can contain different types of object, because it is not generic: every element is of type `obj`. It is therefore highly type-unsafe.
* `Tuple` is heterogeneous: you can construct `(3, "hello")`. However, it was not designed to be a list, so it is not at all ergonomic to use this way: you can't use them while being "generic over the length of the list" in the way that genuine lists naturally are.

HeterogeneousCollections models heterogeneous collections at the type level, so that just as with `Tuple` you retain type safety when manipulating the list.

Concretely, a `HList<'ts>` (for "heterogeneous list") takes a type parameter `'ts` which represents the ordered list of element types.
We use function types to encode pairs at the type level.

For example:

* `HList<unit>` is the type of the empty list.
* `HList<int -> unit>` is the type of lists of one int element.
* `HList<string -> int -> unit>` is the type of lists where the first element is a `string` and the second element is an `int`. (That is, it models the same type as `Tuple<string, int>`.)

The API of `HList` is such that you cannot construct an `HList` where the type parameter does not indicate a heterogeneous list in this encoding.

## The types available

* `HList<_>`, as described above: heterogeneous lists.
* `HListT<'ts, 'elem>`: like an `HList<'ts>`, except every element of the list is a *tuple* whose first entry is heterogeneous and whose second entry is the specific fixed `'elem` (like `int`).
* `HUnion<'ts>`: as a discriminated union is to a tuple, so `HUnion<'ts>` is to `HList<'ts>`. This type represents "a piece of data which is exactly one of the types encoded in `'ts`".
* `SumOfProducts<'ts>`: essentially an `HUnion` of `HList`s.

We also provide:

* `TypeList<'ts>`, which is essentially "the specification of the type of an `HList<'ts>`". It contains no meaningful data.

## Examples

(No type annotations are necessary to use this library; they're only here for the sake of the examples.)

```fsharp
let testHList : HList<string -> int -> unit> =
    HList.empty |> HList.cons 1234 |> HList.cons "Foo"

// No need to `tryHead`: the type asserts the list to be nonempty
HList.head testHList |> shouldEqual "Foo"

let tail : HList<int -> unit> =
    HList.tail testHList

let folder =
    { new HListFolder<string> with
        member _.Folder<'a> (state : string) (elt : 'a) : string =
            state + " " + elt.ToString ()
    }

HList.fold folder "" testHList
|> shouldEqual " Foo 1234"
```

Real-world usages are available in the [ShapeSifter](https://github.com/G-Research/ShapeSifter) library, which uses `HeterogeneousCollections` to decompose and manipulate F# types safely.
