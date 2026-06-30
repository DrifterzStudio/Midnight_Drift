using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Steamworks;


public class ZoneChangementScene : NetworkBehaviour
{
    // ──────────────────────────────────────────
    // PARAMÈTRES ÉDITEUR
    // ──────────────────────────────────────────
    private float timer = 0;

    [Header("Apparence de la zone")]
    [Tooltip("Couleur de la zone au repos")]
    public Color couleurNormale = new Color(0f, 0.8f, 1f, 0.25f);
    [Tooltip("Couleur quand le joueur entre dans la zone")]
    public Color couleurActivee = new Color(1f, 0.6f, 0f, 0.45f);

    private bool change = false;



    // ──────────────────────────────────────────
    // VARIABLES PRIVÉES
    // ──────────────────────────────────────────

    private Renderer zoneRenderer;
   
    //private bool enTransition = false;

    // ──────────────────────────────────────────
    // INITIALISATION
    // ──────────────────────────────────────────
        [ServerCallback]

    private void Update()
    {


        if ((ActivePlayer_List.Instance.Count == 1 || Keyboard.current.tKey.IsPressed()) && !change && ActivePlayer_List.Instance.Ready)
        {
            timer += Time.deltaTime;

            if (timer >= 3f)
            {
                SetCouleur(couleurActivee);
                
                LancerTransition();
            }
        }
        else
        {
            timer = 0f; 
        }

    }

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        InitVisuel();
    }

    void InitVisuel()
    {
        zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer == null) return;

        Material mat = CreerMaterialTransparent();
        mat.color = couleurNormale;

        zoneRenderer.material = mat;
        zoneRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        zoneRenderer.receiveShadows = false;
    }

    Material CreerMaterialTransparent()
    {
        var rp = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        bool isURP = rp != null && rp.GetType().Name.Contains("Universal");

        Material mat;

        if (isURP)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit")
                         ?? Shader.Find("Universal Render Pipeline/Unlit");

            if (shader == null)
            {
                Debug.LogWarning("[ZoneChangementScene] Shader URP introuvable.");
                return new Material(Shader.Find("Standard"));
            }

            mat = new Material(shader);
            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 3000;
        }
        else
        {
            Shader shader = Shader.Find("Standard");
            mat = new Material(shader);
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        return mat;
    }

    // ──────────────────────────────────────────
    // DÉTECTION DU JOUEUR
    // ──────────────────────────────────────────

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Plane")
            return;


        PlayerInfos instantiate = other.gameObject.GetComponent<PlayerInfos>();

        AddPLayer(other, instantiate);
     
    }

    [ServerCallback]
    void OnTriggerExit(Collider other)
    {
        PlayerInfos instantiate = other.gameObject.GetComponent<PlayerInfos>();

        SetCouleur(couleurNormale);
        RemovePlayer(other, instantiate);
    }

    [Server]
    private void AddPLayer(Collider other, PlayerInfos instantiate)
    {
        Debug.Log($" Add : {SteamUser.GetSteamID().m_SteamID}");
        ActivePlayer_List.Instance.CmdAddId(instantiate.SteamId);
    }
    [Server]

    private void RemovePlayer(Collider other, PlayerInfos instantiate)
    {
        Debug.Log($" Remove : {SteamUser.GetSteamID().m_SteamID}");
        ActivePlayer_List.Instance.CmdRemove(instantiate.SteamId);
    }
    // ──────────────────────────────────────────
    // TRANSITION VIA SCENE_CONTROLLER
    // ──────────────────────────────────────────
    [Server]

    void LancerTransition()
    {
        // whait two player in the area to change scene 
        ActivePlayer_List.Instance.PlayerSteamId.Clear();
        Mirror_Manager.Instance.ChangeScene("Game", "GameScene");
        change = true;
    }

    // ──────────────────────────────────────────
    // UTILITAIRES
    // ──────────────────────────────────────────

    void SetCouleur(Color c)
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = c;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(couleurNormale.r, couleurNormale.g, couleurNormale.b, 0.6f);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (GetComponent<BoxCollider>() is BoxCollider box)
            Gizmos.DrawWireCube(box.center, box.size);
        else if (GetComponent<SphereCollider>() is SphereCollider sphere)
            Gizmos.DrawWireSphere(sphere.center, sphere.radius * 2f);
        else if (GetComponent<CapsuleCollider>() is CapsuleCollider caps)
            Gizmos.DrawWireSphere(caps.center, caps.radius);
    }
}