namespace HCollections

open TypeEquality

/// SumOfProducts is a heterogeneous union type, similar to the HUnion
/// type, but where each of the choices are HLists.
///
/// We represent the types of each of the cases in the union in the single generic
/// type parameter of SumOfProducts.
///
/// For example, if we want to represent the choice type
///     (string -> unit) HList or (bool -> unit) HList,
/// we can represent this as the type:
///     ((string -> unit) -> (bool -> unit) -> unit) SumOfProducts
[<NoComparison>]
[<NoEquality>]
type 'tss SumOfProducts

[<RequireQualifiedAccess>]
module SumOfProducts =

    /// Congruence proof for SumOfProducts - given a proof of equality between two types 'tss1 and 'tss2,
    /// returns a proof that 'tss1 SumOfProducts and 'tss2 SumOfProducts are the same type.
    val cong : Teq<'tss1, 'tss2> -> Teq<'tss1 SumOfProducts, 'tss2 SumOfProducts>

    /// Given a TypeListList and an HList, creates a SumOfProducts whose cases are exactly the
    /// cases in the TypeListList, plus one case for the HList supplied.
    /// Notice that when a SumOfProducts is created using make, the HList that it holds is always
    /// the first of the choices. Use SumOfProducts.Extend to prepend further choices.
    val make<'ts, 'tss> : 'tss TypeListList -> 'ts HList -> ('ts -> 'tss) SumOfProducts

    /// Given a SumOfProducts, extends the choices by prepending a single additional choice to
    /// the front. Note that we do not have to supply a value as the SumOfProducts must, by
    /// definition, already be holding a single value.
    /// Note that the type that we prepend must be a valid type for an HList - we enforce this
    /// by requiring the user to supply a 'ts TypeList.
    val extend<'ts, 'tss> : 'ts TypeList -> 'tss SumOfProducts -> ('ts -> 'tss) SumOfProducts

    /// Given a ('ts -> 'tss) SumOfProducts, returns a choice of either a 'ts HList (in the case where
    /// the value of the SumOfProducts corresponded to the first case of the choice) or a
    /// 'tss SumOfProducts in the case where the value of the SumOfProducts corresponds to one of the
    /// choices denoted by 'tss.
    val split<'ts, 'tss> : ('ts -> 'tss) SumOfProducts -> Choice<'ts HList, 'tss SumOfProducts>

    /// Given a SumOfProducts that contains only a single case, returns the HList of that case.
    val getSingleton : ('ts -> unit) SumOfProducts -> 'ts HList
