namespace HCollections

open TypeEquality

/// DiffList is a heterogeneous list similar to HList, except that it has a "unification" variable for the base.
/// This allows for an append function between two DiffLists.
[<NoComparison>]
[<NoEquality>]
type DiffList<'ty, 'v>

/// DiffListFolder allows you to perform a fold over an DiffList.
/// The single type parameter, 'state, denotes the type of the value
/// that you want the fold to return.
type 'state DiffListFolder =

    /// Folder takes the current state, the next element in the DiffList and returns a new state.
    /// Because elements in the DiffList may have arbitrary type, F must be generic on
    /// the element type, i.e. can be called for any element type.
    abstract Folder<'a> : 'state -> 'a -> 'state

module DiffList =

    /// Congruence proof for DiffList - given a proof of equality between two types 'ty1 and 'ty2,
    /// returns a proof that DiffList<'ty1, 'v> and DiffList<'ty2, 'v> are the same type.
    val cong : Teq<'ty1, 'ty2> -> Teq<DiffList<'ty1, 'v>, DiffList<'ty2, 'v>>

    /// Congruence proof for DiffList unification variable - given a proof of equality between two types 'u and 'v,
    /// returns a proof that DiffList<'ty, 'u> and DiffList<'ty, 'v> are the same type.
    val congUnification : Teq<'u, 'v> -> Teq<DiffList<'ty, 'u>, DiffList<'ty, 'v>>

    /// The empty DiffList.
    val empty<'v> : DiffList<'v, 'v>

    /// Given an element and an DiffList, returns a new DiffList with the element prepended to it.
    val cons<'a, 'ty, 'v> : 'a -> DiffList<'ty, 'v> -> DiffList<'a -> 'ty, 'v>

    /// Append two DiffLists together, given a common unification variable between them.
    val append<'ty, 'u, 'v> : DiffList<'ty, 'u> -> DiffList<'u, 'v> -> DiffList<'ty, 'v>

    /// Returns the length of the given DiffList.
    /// This operation takes time constant in the length of the DiffList.
    val length<'ty, 'v> : DiffList<'ty, 'v> -> int

    /// Given a non-empty DiffList, returns the first element.
    val head<'a, 'ty, 'v> : DiffList<'a -> 'ty, 'v> -> 'a

    /// Given a non-empty DiffList, returns a new DiffList containing all of the elements
    /// except the head.
    val tail<'a, 'ty, 'v> : DiffList<'a -> 'ty, 'v> -> DiffList<'ty, 'v>

    /// Given an DiffListFolder, an initial state and an DiffList, returns the result
    /// of folding the DiffListFolder over the elements of the DiffList.
    val fold<'state, 'ty, 'v> : 'state DiffListFolder -> seed : 'state -> DiffList<'ty, 'v> -> 'state

    /// Given a DiffList, returns an HList whose elements correspond to the
    /// elements of the DiffList.
    /// This operation takes time constant in the length of the DiffList.
    val toHList<'ty> : DiffList<'ty, unit> -> 'ty HList
