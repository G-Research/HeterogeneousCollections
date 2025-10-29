namespace HCollections

open TypeEquality

/// DiffList is a heterogeneous list similar to HList, except that it has a "unification" variable for the base.
/// This allows for an append function between two DiffLists.
[<NoComparison>]
[<NoEquality>]
type DiffList<'elts, 'tailSlot>

/// DiffListFolder allows you to perform a fold over an DiffList.
/// The single type parameter, 'state, denotes the type of the value
/// that you want the fold to return.
type 'state DiffListFolder =

    /// Folder takes the current state, the next element in the DiffList and returns a new state.
    /// Because elements in the DiffList may have arbitrary type, F must be generic on
    /// the element type, i.e. can be called for any element type.
    abstract Folder<'elt> : 'state -> 'elt -> 'state

[<RequireQualifiedAccess>]
module DiffList =

    /// Congruence proof for DiffList - given two proofs of equality between elements types 'elts1 and 'elts2
    /// and tail slot types 'tailSlot1 and 'tailSlot2, returns a proof that
    /// DiffList<'elts1, 'tailSlot1> and DiffList<'elts2, 'tailSlot2> are the same type.
    val cong<'elts1, 'elts2, 'tailSlot1, 'tailSlot2> :
        Teq<'elts1, 'elts2> ->
        Teq<'tailSlot1, 'tailSlot2> ->
            Teq<DiffList<'elts1, 'tailSlot1>, DiffList<'elts2, 'tailSlot2>>

    /// The empty DiffList.
    val empty<'elts> : DiffList<'elts, 'elts>

    /// Given an element and an DiffList, returns a new DiffList with the element prepended to it.
    val cons<'elt, 'elts, 'tailSlot> : 'elt -> DiffList<'elts, 'tailSlot> -> DiffList<'elt -> 'elts, 'tailSlot>

    /// Append two DiffLists together.
    /// A DiffList is essentially a heterogeneous list which also has an "unspecified tail" slot at its end;
    /// the type `'elts2` ensures that the first list's "unspecified tail" is of the right shape to be filled by the elements of the second list.
    val append<'elts1, 'elts2, 'tailSlot> :
        first : DiffList<'elts1, 'elts2> -> second : DiffList<'elts2, 'tailSlot> -> DiffList<'elts1, 'tailSlot>

    /// Returns the length of the given DiffList.
    /// This operation takes time constant in the length of the DiffList.
    val length<'elts, 'tailSlot> : DiffList<'elts, 'tailSlot> -> int

    /// Given a non-empty DiffList, returns the first element.
    val head<'elt, 'elts, 'tailSlot> : DiffList<'elt -> 'elts, 'tailSlot> -> 'elt

    /// Given a non-empty DiffList, returns a new DiffList containing all of the elements
    /// except the head.
    val tail<'elt, 'elts, 'tailSlot> : DiffList<'elt -> 'elts, 'tailSlot> -> DiffList<'elts, 'tailSlot>

    /// Given an DiffListFolder, an initial state and an DiffList, returns the result
    /// of folding the DiffListFolder over the elements of the DiffList.
    val fold<'state, 'elts, 'tailSlot> : 'state DiffListFolder -> seed : 'state -> DiffList<'elts, 'tailSlot> -> 'state

    /// Given a DiffList which has no "unspecified tail", returns an HList whose elements correspond to the
    /// elements of the DiffList.
    /// This operation takes time constant in the length of the DiffList.
    val toHList<'elts> : DiffList<'elts, unit> -> 'elts HList
