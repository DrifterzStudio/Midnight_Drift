using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InviteButtonScript : MonoBehaviour
{

    public GameObject Parent;
    public void OnClick()
    {
        Parent.gameObject.GetComponent<PlayerInfoItem>().OnInvite();
    }
}
