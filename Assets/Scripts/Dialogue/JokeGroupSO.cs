using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "JokeData", menuName = "Dialogue/JokeGroup", order = 1)]
public class JokeGroup:ScriptableObject
{
    public string jokeGroupName;
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

#if UNITY_EDITOR
    [ContextMenu("Add Joke")]
    public void AddJoke()
    {
        JokeSO joke = CreateInstance<JokeSO>();
        jokeList.Add(joke);
        joke.Init(jokeGroupName,jokeList.Count);
        
        EditorUtility.SetDirty(joke);
        EditorUtility.SetDirty(this);
        AssetDatabase.AddObjectToAsset(joke,this);
        AssetDatabase.SaveAssets();
    }
#endif
}
