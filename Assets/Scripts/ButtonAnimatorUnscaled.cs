using UnityEngine;
public class ButtonAnimatorUnscaled : MonoBehaviour
{
    void Awake()
    {
        var anim = GetComponent<Animator>();
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }
}