namespace HCollections

open TypeEquality

/// HListT is similar to HList, but rather than just holding a heterogeneous
/// list of elements, instead holds a list of pairs where the first element in
/// each pair is completely heterogeneous, but the second element in each pair
/// is of the same type.
///
/// This allows HListT to have both well-defined heterogeneous and homogeneous
/// components.
///
/// We represent the type of the second element in each pair using the second
/// type parameter on HListT. That is, HListT<'a, int> is an HList of pairs
/// where the type of the second element of each pair is an int.
[<NoComparison>]
[<NoEquality>]
type HListT<'ts, 'elem>

/// HListFolder allows you to perform a fold over an HListT.
/// The first type parameter, 'state, denotes the type of the value
/// that you want the fold to return.
/// The second type parameter, 'elem, denotes the element type of
/// the HListT that you wish to fold over.
type HListTFolder<'state, 'elem> =

    /// F takes the current state, the next pair of elements in the HList and
    /// returns a new state.
    /// Because elements in the HListT may have arbitrary type, F must be generic on
    /// the element type, i.e. can be called for any element type.
    abstract member Folder<'a> : 'state -> 'a -> 'elem -> 'state

module HListT =

    /// Congruence proof for HLists - given a proof of equality between two types 'ts1 and 'ts2
    /// and a proof of equality between two types 'elem1 and 'elem2,
    /// returns a proof that HListT<'ts1, 'elem1> and HList<'ts2, 'elem2> are the same type.
    val cong : Teq<'ts1, 'ts2> -> Teq<'elem1, 'elem2> -> Teq<HListT<'ts1, 'elem1>, HListT<'ts2, 'elem2>>

    /// The unique empty HListT
    val empty<'elem> : HListT<unit, 'elem>

    /// Given an element of any type, an element of type 'elem and an HListT with element type 'elem,
    /// returns a new HListT with the elements prepended to it.
    val cons<'t, 'ts, 'elem> : 't -> 'elem -> HListT<'ts, 'elem> -> HListT<'t -> 'ts, 'elem>

    /// Returns the length of the given HListT.
    /// This operation takes time constant in the length of the HListT.
    val length<'ts, 'elem> : HListT<'ts, 'elem> -> int

    /// Given a non-empty HListT, returns the first pair of elements.
    val head<'t, 'ts, 'elem> : HListT<'t -> 'ts, 'elem> -> 't * 'elem

    /// Given a non-empty HListT, returns a new HListT containing all of the elements
    /// except the first pair.
    val tail<'t, 'ts, 'elem> : HListT<'t -> 'ts, 'elem> -> HListT<'ts, 'elem>

    /// Given an HListTFolder, an initial state and an HListT, returns the result
    /// of folding the HListTFolder over the elements of the HListT.
    val fold<'state, 'ts, 'elem> : HListTFolder<'state, 'elem> -> seed:'state -> HListT<'ts, 'elem> -> 'state

    /// Obtain the corresponding TypeList for the heterogeneous types in an HListT.
    val toTypeList<'ts, 'elem> : HListT<'ts, 'elem> -> TypeList<'ts>

    /// Given an HListT, returns the corresponding HList of heterogeneous types
    val toHList<'ts, 'elem> : HListT<'ts, 'elem> -> 'ts HList

    /// Given an HListT, returns the corresponding list of homogenous types
    val toList<'ts, 'elem> : HListT<'ts, 'elem> -> 'elem list