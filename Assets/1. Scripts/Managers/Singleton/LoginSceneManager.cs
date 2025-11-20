using UnityEngine;


public class LoginSceneManager : MonoBehaviour
{
    public static LoginSceneManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;    
    }


}
