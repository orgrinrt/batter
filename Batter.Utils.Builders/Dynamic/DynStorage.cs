namespace Batter.Utils.Builders.Dynamic;

/// <summary>
///     Storage implementation for EasyBuilder that bridges the gap between
///     the simplified EasyBuilder pattern and the full IBuilder interface.
/// </summary>
public class DynStorage<TContainer> :
    IContainerStorage<DynStorage<TContainer>, TContainer, DynCollection<DynKey, object>>
    where TContainer : class, IContainer<TContainer, DynStorage<TContainer>, DynCollection<DynKey, object>> {

    /// <summary>
    ///     The underlying collection for this storage instance.
    /// </summary>
    public DynCollection<DynKey, object> Collection { get; } = new();

}