using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

[CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/Jokes", order = 1)]
public class JokeSO : DialogueSO
{
    public float punchlineBufferTime = 1;
    public Vector2 punchlineRange = Vector2.zero;
}
