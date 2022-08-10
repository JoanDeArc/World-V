using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrainingGame : MonoBehaviour
{
    protected bool isPlaying;
    protected int difficultyLevel; //0 = easy, 1 = medium, 2 = hard
    public abstract string InfoString { get; }

    public Transform PromptCanvas;
    public Transform GameCanvas;
    public CinemachineVirtualCamera GameCam;

    public abstract IEnumerator StartGame(int difficultyLevel);

    public abstract IEnumerator EndGame();
}
