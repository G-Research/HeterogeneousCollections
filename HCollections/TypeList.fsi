namespace HCollections

open System
open TypeEquality

/// TypeList is a type-level list of types.
///
/// We represent the list of types using the single type parameter.
/// The empty list is represented with the unit type and
/// types are prepended to the list using the binary operator ->.
/// For example, the list of types [ int ; string ] is represented by:
///     (int -> string -> unit) TypeList
[<NoComparison>]
[<NoEquality>]
type 'ts TypeList

type TypeListConsEvaluator<'ts, 'ret> =
    abstract Eval<'t, 'ts2> : 'ts2 TypeList -> Teq<'ts, 't -> 'ts2> -> 'ret

and 'ts TypeListConsCrate =
    abstract Apply<'ret> : TypeListConsEvaluator<'ts, 'ret> -> 'ret


[<RequireQualifiedAccess>]
module TypeList =

    /// Congruence proof for TypeLists - given a proof of equality between two types 'ts1 and 'ts2,
    /// returns a proof that 'ts1 TypeList and 'ts2 TypeList are the same type.
    val cong : Teq<'ts1, 'ts2> -> Teq<'ts1 TypeList, 'ts2 TypeList>

    /// The unique empty TypeList
    val empty : unit TypeList

    /// Given an TypeList, prepends a new type
    /// to the list of types being represented.
    val cons<'t, 'ts> : 'ts TypeList -> ('t -> 'ts) TypeList

    /// Given a non-empty TypeList, returns a new TypeList containing all of the elements
    /// except the head.
    val tail<'t, 'ts> : ('t -> 'ts) TypeList -> 'ts TypeList

    /// Given a TypeList, returns either a proof that the list is empty, or a crate
    /// containing the tail of the TypeList.
    val split : 'ts TypeList -> Choice<Teq<'ts, unit>, 'ts TypeListConsCrate>

    /// Given a TypeList, returns the corresponding list of runtime types.
    val toTypes : 'ts TypeList -> Type list

    /// Given a TypeList, returns the length of the list.
    /// This is equal to the number of `cons` operations on `empty` that would create the list.
    /// This operation takes time constant in the length of the TypeList.
    val length : 'ts TypeList -> int
