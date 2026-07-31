using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResetAllButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        if (RebindManager.instance != null)
            RebindManager.instance.ResetAll();
    }
}
