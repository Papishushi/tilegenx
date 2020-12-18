using UnityEngine;

namespace Kutility.Serialization
{
    /// <summary>  
    /// This structure prevents users from creating more than one instance of this component on the scene.
    ///  <para>
    ///  To use this class just use it as base class for any intended MonoBehaviour class, then pass the derived class as a parameter, last but not least override the derived class methods: OnValidate(), Update(), OnDestroy(), and Reset() with the base functionallity.
    /// </para>
    /// </summary>
    /// <typeparam name="ClassToMakeSingleton">You must pass the type from the derived class as a parameter.</typeparam>
    public abstract class Singleton<ClassToMakeSingleton> : MonoBehaviour
    {
        public static Component OldInstance = null;

        //This code will only be compiled on the editor.
#if UNITY_EDITOR
        /// <summary>
        /// Check if <see cref="OldInstance"/> is null in that case asign this child instance to the reference.
        /// </summary>
        public virtual void OnValidate()
        {
            if (OldInstance == null)
            {
                OldInstance = this;
            }
        }

        /// <summary>
        ///  Check if <see cref="OldInstance"/> isn't null and isn't this child instance. 
        ///  <para>In that case creates a pop up window for the editor user to determine wheter to create a new instance or not.</para>
        ///  <para> If the editor user determines to replace it, creates an instance from <see cref="DeleteComponent"/> passing as parameter <see cref="OldInstance"/> then set it to null.</para>
        ///  <para> Otherwise, creates an instance from <see cref="DeleteComponent"/> passing as parameter this child instance.</para>
        /// </summary>
        public virtual void Reset()
        {
            if (OldInstance != null && OldInstance != this)
            {
                if (UnityEditor.EditorUtility.DisplayDialog(OldInstance.GetType().Name + " already exists on scene", 
                    "This component can only be instantiated once per scene. Do you want to replace it?",
                    "Ok, replace it", "No, thanks!"))
                {
                    new DeleteComponent(OldInstance);
                    OldInstance = null;
                }
                else
                {
                    new DeleteComponent(this);
                }
            }
        }
#endif
        //This code will only be compiled on a standalone version.
#if UNITY_STANDALONE
        /// <summary>
        ///  Check if <see cref="OldInstance"/> isn't null and isn't this child instance. 
        ///  <para> In that case creates an instance from <see cref="DeleteComponent"/> passing as parameter this child instance.</para>
        ///  <para> Otherwise, set <see cref="OldInstance"/> to this child instance.</para>
        /// </summary>
        public virtual void Update()
        {
            if (OldInstance != null && OldInstance != this)
            {
               new DeleteComponent(this);
            }
            else
            {
                OldInstance = this;
            }
        }

        /// <summary>
        /// Check if <see cref="OldInstance"/> isn't null and is this child instance. 
        /// <para> In that case set <see cref="OldInstance"/> to null.</para>
        /// </summary>
        public virtual void OnDestroy()
        {
            if (OldInstance != null && OldInstance == this)
            {
                OldInstance = null;
            }
        }
#endif
    }
}
