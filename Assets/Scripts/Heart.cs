using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(DynamicGateBehaviour))]
public class Heart : MonoBehaviour
{
    [SerializeField]
    private DynamicGateBehaviour _ownGate;

    public void OnDestroy()
    {
        SceneManager.LoadScene(2);
    }
}
