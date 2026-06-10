using UnityEngine;

public class dont_delete_obj : MonoBehaviour
{
  protected virtual void Awake()
  {
    DontDestroyOnLoad(gameObject);
  }
}
