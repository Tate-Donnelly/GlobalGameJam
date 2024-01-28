using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/JokeGroup", order = 1)]
public class JokeGroup:ScriptableObject
{
    public List<JokeSO> jokeList=new List<JokeSO>();
    private Queue<JokeSO> jokeQue=new Queue<JokeSO>();

    void Awake()
    {
        jokeQue = new Queue<JokeSO>(jokeList);
        foreach (JokeSO joke in jokeList)
        {
            jokeQue.Enqueue(joke);
        }
    }

    void LoadQue()
    {
        jokeQue = new Queue<JokeSO>(jokeList);
        foreach (JokeSO JokeSO in jokeList)
        {
            jokeQue.Enqueue(JokeSO);
        }
    }

    public bool HasMoreJokes()
    {
        return jokeQue.Count != 0;
    }

    public JokeSO GetNextJoke()
    {
        if (jokeQue.Count == 0) LoadQue();
        return jokeQue.Dequeue();
    }
}
