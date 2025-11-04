namespace HCollections

open TypeEquality

/// <summary>A heterogeneous list which additionally has a <c>DiffList.append</c> function.</summary>
/// <remarks>
/// DiffList is a heterogeneous list similar to <c>HList</c>, except that it is modelled as a prefix of some larger
/// list type.
/// This allows the type system to express that some suffix of a DiffList has the same type as another DiffList;
/// this is the necessary information to express the type of <c>DiffList.append</c>, which <c>HList</c> cannot have.
///
/// To get the types of the elements in this DiffList, "subtract" the <typeparamref name="suffix"/> from the
/// <typeparamref name="eltsSuper"/>.
/// </remarks>
///
/// <example>
/// If <typeparamref name="eltsSuper"/> were <c>int -> string -> unit</c>, <typeparamref name="suffix"/> could be
/// <c>unit</c> (in which case the DiffList contains an int and a string), or <c>string -> unit</c> (in which case
/// the DiffList contains an int only), or <c>int -> string -> unit</c> (in which case the DiffList is empty).
/// </example>
///
/// <typeparam name="eltsSuper">
/// The types in the <c>DiffList</c>, stored successively as a function type just as <c>HList</c> uses its type
/// parameter, but additionally with "dummy" elements specified by <typeparamref name="suffix"/> appended to the
/// function type.
/// </typeparam>
/// <typeparam name="suffix">
/// A suffix of <typeparamref name="eltsSuper"/>.
/// If you remove <typeparamref name="suffix"/> from the right-hand side of <typeparamref name="eltsSuper"/>, you get
/// the type of the concrete elements stored in the DiffList.
/// </typeparam>
[<NoComparison>]
[<NoEquality>]
type DiffList<'eltsSuper, 'suffix>

/// <summary>An object that expresses how to perform a specific fold over a <c>DiffList</c>.</summary>
/// <remarks>
/// This type is morally just a function, but it must be universally quantified because it will be invoked on
/// every element of the <c>DiffList</c> in turn.
/// F# requires universally quantified functions to be members on an explicit object type; hence the existence of
/// <c>DiffListFolder</c>.
/// </remarks>
/// <typeparam name="state">
/// The type of the accumulator (and the returned value of the fold).
/// </typeparam>
type 'state DiffListFolder =

    /// <summary>Define this member to express the fold.</summary>
    /// <typeparam name="elt">The type of the list element you're about to consume.</typeparam>
    abstract Folder<'elt> : currentState : 'state -> diffListElement : 'elt -> 'state

[<RequireQualifiedAccess>]
module DiffList =

    /// Congruence proof for DiffList - given two proofs of equality between elements types 'elts1 and 'elts2
    /// and tail slot types 'tailSlot1 and 'tailSlot2, returns a proof that
    /// DiffList<'elts1, 'tailSlot1> and DiffList<'elts2, 'tailSlot2> are the same type.
    val cong<'elts1, 'elts2, 'tailSlot1, 'tailSlot2> :
        Teq<'elts1, 'elts2> ->
        Teq<'tailSlot1, 'tailSlot2> ->
            Teq<DiffList<'elts1, 'tailSlot1>, DiffList<'elts2, 'tailSlot2>>

    /// <summary>The empty DiffList.</summary>
    /// <remarks>
    /// The second type parameter of the resulting <c>DiffList</c> is equal to the first, indicating that this
    /// <c>DiffList</c> is empty:
    /// subtracting the "suffix" from the first type parameter always yields "the empty list".
    /// </remarks>
    /// <typeparam name="elts">
    /// A function type, or <c>unit</c> to indicate a DiffList that is empty and will never have anything appended.
    ///
    /// You're not expected to supply this type parameter explicitly. You should let type inference do that; otherwise
    /// you will need to know up front what types you're going to <c>cons</c> onto the list and what lists you're going
    /// to be appending to it.
    /// </typeparam>
    val empty<'elts> : DiffList<'elts, 'elts>

    /// Given an element and a DiffList, returns a new DiffList with the element prepended to it.
    val cons<'head, 'eltsSuper, 'suffix> :
        'head -> DiffList<'eltsSuper, 'suffix> -> DiffList<'head -> 'eltsSuper, 'suffix>

    /// <summary>Append two DiffLists.</summary>
    /// <example>
    /// <code lang="fsharp">
    /// let firstDiffList : DiffList&lt;string -> int -> decimal -> unit, decimal -> unit&gt; =
    ///     DiffList.empty |> DiffList.cons 300 |> DiffList.cons "abc"
    /// let secondDiffList : DiffList&lt;decimal -> unit, unit&gt; =
    ///     DiffList.empty |> DiffList.cons 123.456M
    /// let result : DiffList&lt;string -> int -> decimal -> unit, unit&gt; =
    ///     DiffList.append firstDiffList secondDiffList
    /// </code>
    /// </example>
    val append<'elts1Super, 'elts2Super, 'suffix> :
        first : DiffList<'elts1Super, 'elts2Super> ->
        second : DiffList<'elts2Super, 'suffix> ->
            DiffList<'elts1Super, 'suffix>

    /// <summary>Returns the length of the given DiffList.</summary>
    /// <remarks>
    /// This operation takes time linear in the length of the DiffList: it walks the list to compute its length.
    /// </remarks>
    val length<'eltsSuper, 'suffix> : DiffList<'eltsSuper, 'suffix> -> int

    /// Given a non-empty DiffList, returns the first element.
    val head<'head, 'tailSuper, 'suffix> : DiffList<'head -> 'tailSuper, 'suffix> -> 'head

    /// Given a non-empty DiffList, returns a new DiffList containing all the elements
    /// except the head.
    val tail<'head, 'tailSuper, 'suffix> : DiffList<'head -> 'tailSuper, 'suffix> -> DiffList<'tailSuper, 'suffix>

    /// Given a DiffListFolder, an initial state and a DiffList, returns the result
    /// of folding the DiffListFolder over the elements of the DiffList.
    val fold<'state, 'eltsSuper, 'suffix> :
        'state DiffListFolder -> seed : 'state -> DiffList<'eltsSuper, 'suffix> -> 'state

    /// <summary>
    /// Given a DiffList where the suffix is empty (so the prefix alone expresses the type of the contents), build
    /// an HList which contains the same elements as the input list.
    /// </summary>
    ///
    /// <remarks>
    /// This operation takes time linear in the length of the DiffList: it walks the list to construct the new HList.
    /// </remarks>
    val toHList<'elts> : DiffList<'elts, unit> -> 'elts HList
