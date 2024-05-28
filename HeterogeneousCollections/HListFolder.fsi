namespace HCollections

/// Module containing utility functions for creating common types of HListFolders
[<RequireQualifiedAccess>]
module HListFolder =

    /// Creates an HListFolder that folds over an HList using the given function.
    /// When an element of the HList has type 'a, the folding function is invoked
    /// with the element wrapped in Some as the second argument.
    /// When an element of the HList does not have type 'a, the folding function is
    /// invoked with None as the second argument.
    val makeGappedElementFolder : ('state -> 'a option -> 'state) -> 'state HListFolder

    /// Creates an HListFolder that folds over an HList using the given function.
    /// When an element of the HList has type 'a, the folding function is invoked
    /// with the element as the second argument to the function.
    /// When an element of the HList does not have type 'a, the folding function is
    /// not invoked.
    val makeElementFolder : ('state -> 'a -> 'state) -> 'state HListFolder
