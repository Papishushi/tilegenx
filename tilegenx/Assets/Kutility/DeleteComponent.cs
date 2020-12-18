using UnityEngine;

namespace Kutility
{
    /// <summary>
    /// Destroys inmediatelly any component.
    /// </summary>
    public class DeleteComponent : Object
    {
        /// <summary>
        /// Destroys inmediatelly a given component.
        /// </summary>
        /// <param name="componentReference">The component to be destroyed.</param>
        public DeleteComponent(Component componentReference)
        {
            DestroyImmediate(componentReference);
            DestroyImmediate(this);
        }
    }
}
