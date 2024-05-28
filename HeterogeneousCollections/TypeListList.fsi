namespace HCollections

/// TypeListList is a type-level list of list of types.
///
/// We represent the list of types using the single type parameter.
/// The empty list is represented with the unit type and
/// lists of types are prepended by providing a list of types in the form
/// of a TypeList. These lists of types are represented at the type level
/// by chaining with the -> operator.
/// For example, the list of types [[bool] ; [int ; string]] is represented by:
///     ((bool -> unit) -> (int -> string -> unit) -> unit) TypeListList
[<NoComparison>]
[<NoEquality>]
type 'tss TypeListList

[<RequireQualifiedAccess>]
module TypeListList =

    /// The unique empty TypeListList
    val empty : unit TypeListList

    /// Given an TypeListList and a list of types, prepends
    /// the list of types to those represented by the TypeListList
    val cons<'ts, 'tss> : 'ts TypeList -> 'tss TypeListList -> ('ts -> 'tss) TypeListList
