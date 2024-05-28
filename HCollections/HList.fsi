namespace HCollections

open TypeEquality

/// HList is a heterogeneous list, that is a list where the types of the elements
/// may all be different.
///
/// In order to be able to act on an HList in a type-safe way, we represent the types
/// of each of the elements of the HList in its generic type parameter.
///
/// For example, the empty list has type: unit HList
/// Whereas the HList containing an int and a string has type: (int -> string -> unit) HList
[<NoComparison>]
[<NoEquality>]
type 'ts HList

/// HListFolder allows you to perform a fold over an HList.
/// The single type parameter, 'state, denotes the type of the value
/// that you want the fold to return.
type 'state HListFolder =

    /// F takes the current state, the next element in the HList and returns a new state.
    /// Because elements in the HList may have arbitrary type, F must be generic on
    /// the element type, i.e. can be called for any element type.
    abstract Folder<'a> : 'state -> 'a -> 'state

module HList =

    /// Congruence proof for HLists - given a proof of equality between two types 'ts1 and 'ts2,
    /// returns a proof that 'ts1 HList and 'ts2 HList are the same type.
    val cong : Teq<'ts1, 'ts2> -> Teq<'ts1 HList, 'ts2 HList>

    /// The unique empty HList
    val empty : unit HList

    /// Given an element and an HList, returns a new HList with the element prepended to it.
    val cons<'t, 'ts> : 't -> 'ts HList -> ('t -> 'ts) HList

    /// Returns the length of the given HList.
    /// This operation takes time constant in the length of the HList.
    val length<'ts> : 'ts HList -> int

    /// Given a non-empty HList, returns the first element.
    val head<'t, 'ts> : ('t -> 'ts) HList -> 't

    /// Given a non-empty HList, returns a new HList containing all of the elements
    /// except the head.
    val tail<'t, 'ts> : ('t -> 'ts) HList -> 'ts HList

    /// Given an HListFolder, an initial state and an HList, returns the result
    /// of folding the HListFolder over the elements of the HList.
    val fold<'state, 'ts> : 'state HListFolder -> seed:'state -> 'ts HList -> 'state

    /// Given an HList, returns a TypeList whose types correspond to the values
    /// of the elements of the HList.
    /// This operation takes time constant in the length of the HList.
    val toTypeList<'ts> : 'ts HList -> 'ts TypeList
