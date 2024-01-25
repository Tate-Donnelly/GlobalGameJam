using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JokeData", menuName = "ScriptableObjects/Jokes", order = 1)]
public class JokeSO : ScriptableObject
{
    public string jokeString;
    public string punchlineString;
    public float jokeDuration;
    public float punchlineBufferTime = 1;
    public Vector2 punchlineRange = Vector2.zero;
    public AudioClip jokeClip;
}
