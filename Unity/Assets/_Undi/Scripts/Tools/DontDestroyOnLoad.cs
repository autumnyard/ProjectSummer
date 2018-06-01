using UnityEngine;


namespace Alma
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        /// <summary>
        /// Run when new instance of the object is created.
        /// </summary>
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}