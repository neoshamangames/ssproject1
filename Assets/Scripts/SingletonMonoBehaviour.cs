using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour
	where T : SingletonMonoBehaviour<T>
{
	public static T Instance
	{
		get {
			if (!instance)
			{
				instance = FindObjectOfType(typeof(T)) as T;
				if (!instance)
				{
					Debug.Log(string.Format("creating {0} instance", typeof(T)));
					GameObject obj = new GameObject();
					instance = obj.AddComponent<T>();
				}
			}
			return instance as T;
		}
	}
	
	public void OnApplicationQuit()
	{
		instance = null;		
	}
	
	private static SingletonMonoBehaviour<T> instance;
}