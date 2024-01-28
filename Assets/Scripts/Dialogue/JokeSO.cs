using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;

[CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/Jokes", order = 1)]
public class JokeSO : DialogueSO
{
    public float punchlineBufferTime = 1;

    public void Init(string jokeGroup, int index)
    {
        name = $"{jokeGroup}_Punchline0{index}";
        yarnNodeName = name;
    }
}
