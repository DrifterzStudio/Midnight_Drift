using UnityEngine;

public class Scene_Manager : MonoBehaviour
{
   
    void Start()
    {
        Scene_Controller.Instance.NewTransition()
            .Load("Session", "Session")
            .EnableOverlay(true)
            .Execute();

    }

    
}
