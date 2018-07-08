using DotLiquid;
using JetBrains.Annotations;

namespace Scalider.DotLiquid
{
    
    /// <summary>
    /// Defines the basic functionality of a <see cref="Hash"/> converter.
    /// </summary>
    public interface IHashConverter
    {

        /// <summary>
        /// Converts a model to a <see cref="Hash"/> for rendering.
        /// </summary>
        /// <param name="model">The model to convert.</param>
        /// <returns>
        /// The <see cref="Hash"/> representing the converted model.
        /// </returns>
        Hash ToHash([NotNull] object model);

    }
    
}