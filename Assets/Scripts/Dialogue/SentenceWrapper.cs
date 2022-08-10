using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct SentenceWrapper
{
    public string Name;
    public string Sentence;

    public SentenceWrapper((string name, string sentence) sentence)
    {
        Name = sentence.name;
        Sentence = sentence.sentence;
    }

    public static implicit operator SentenceWrapper((string name, string sentence) sentence)
    {
        return new SentenceWrapper(sentence);
    }
}
