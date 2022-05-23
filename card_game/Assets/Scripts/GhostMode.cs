using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMode : MonoBehaviour
{
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    public List<Behaviour> disabledBehaviours = new List<Behaviour>();
    private Color[] colors = new Color[2];
    public enum States : int
    {
        WrongPosition = 0,
        ViablePosition = 1,
    }

    private void Start()
    {
        colors[0] = new Color(255, 0, 0);
        colors[1] = new Color(255, 255, 255);
    }

    public void Enable()
    {
        foreach (var renderer in spriteRenderers)
        {
            renderer.color = new Color(255, 255, 255, 0.5f);
        }
        foreach (var behaviour in disabledBehaviours)
        {
            behaviour.enabled = false;
        }
    }

    public void SetState(States state)
    {
        foreach (var renderer in spriteRenderers)
        {
            renderer.color = new Color(colors[(int)state].r, colors[(int)state].g, colors[(int)state].b, renderer.color.a);
        }
    }
}
