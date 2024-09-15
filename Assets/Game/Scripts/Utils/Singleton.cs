using UnityEngine;


namespace CanDurmazLibrary
{
    /// Global singleton with multiple scenes.
    /// <summary>
    /// A generic singleton class that ensures only one instance of the specified type exists within a scene.
    /// The instance is automatically created if it doesn't exist and destroyed if a duplicate is found.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class inheriting from this base class.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    /*
        Example Usage:

        // The GameManager class inherits from Singleton<GameManager>
        public class GameManager : Singleton<GameManager>
        {
            public int playerScore = 0;

            public void AddScore(int score)
            {
                playerScore += score;
                Debug.Log($"Current Score: {playerScore}");
            }
        }

        // In another script, you can access the singleton like this:
        // GameManager.Instance.AddScore(10);
    */
}