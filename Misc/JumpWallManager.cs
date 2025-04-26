using System;
using System.Collections;
using UnityEngine;

public class JumpWallManager : MonoBehaviour
{
    public GameObject wallCam;
    public Transform pivot;

    private bool isOn;
    private Coroutine releaseCoroutine;

    private void Start()
    {
        wallCam.SetActive(false);
        Level.Instance.player.playerEvents.OnWallDrag.AddListener(WallDrag);
        Level.Instance.player.playerEvents.OnWallDragEnded.AddListener(WallDragEnded);
    }

    private void Update()
    {
        if (isOn)
        {
            pivot.position = new Vector3(pivot.position.x, Level.Instance.player.transform.position.y, pivot.position.z);
        }
    }

    private void WallDrag()
    {
        isOn = true;

        // 如果正在进行延迟取消，打断它
        if (releaseCoroutine != null)
        {
            StopCoroutine(releaseCoroutine);
            releaseCoroutine = null;
        }

        wallCam.SetActive(true);
    }

    private void WallDragEnded()
    {
        isOn = false;

        // 开启延迟协程
        releaseCoroutine = StartCoroutine(DelayedRelease(0.5f));
    }

    private IEnumerator DelayedRelease(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isOn)
        {
            wallCam.SetActive(false);
        }

        releaseCoroutine = null;
    }
}