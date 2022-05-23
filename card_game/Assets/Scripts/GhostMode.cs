using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMode : MonoBehaviour
{
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    public List<Behaviour> disabledBehaviours = new List<Behaviour>();

    public void Enable()
    {
        foreach (var renderer in spriteRenderers)
        {
            var previousColor = renderer.color;
            renderer.color = new Color(previousColor.r, previousColor.g, previousColor.b, 0.5f);
        }
        foreach (var behaviour in disabledBehaviours)
        {
            behaviour.enabled = false;
        }
    }
}
