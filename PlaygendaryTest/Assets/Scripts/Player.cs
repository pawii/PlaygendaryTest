﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static event Action OnMoveNext;
    public static event Action OnStickFallDown;
    public static event Action OnScoreUp;


    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private float playerFallSpeed;


    public static Vector2 StartPos { get; private set; }

    
    private Vector2 playerPos;
    private Vector2 statrPosition;
    private Vector2 targetPosition;
    private float fraction;
    private float startTime;
    private float movementTime;
    private bool isMove;
    private bool isFall;
    private bool isContinueMove;


    #region Unity lifecycle

    private void Awake()
    {
        StartPos = transform.position;
        playerPos = StartPos;

        Stick.OnStickFallHorizontal += Player_OnStickFallHorizontal;
    }


    private void OnDestroy()
    {
        Stick.OnStickFallHorizontal -= Player_OnStickFallHorizontal;
    }


    private void Update()
    {
        if (isMove)
        {
            fraction = (Time.realtimeSinceStartup - startTime) / movementTime;

            if (fraction > 1f)
            {
                fraction = 1;

                isMove = false;

                if (!isContinueMove)
                {
                    anim.SetBool("isRun", false);
                }
            }

            playerPos = Vector2.Lerp(statrPosition, targetPosition, fraction);
            transform.position = playerPos;

            if (!isMove && isContinueMove)
            {
                if (isFall)
                {
                    OnStickFallDown();

                    targetPosition = playerPos;
                    targetPosition.y -= 10;
                    movementTime = PlatformManager.MOVE_TIME;

                    anim.SetBool("isRun", false);
                }
                else
                {
                    OnScoreUp();
                    OnMoveNext();

                    targetPosition = StartPos;
                    movementTime = PlatformManager.MOVE_TIME;

                }

                statrPosition = playerPos;
                startTime = Time.realtimeSinceStartup;
                fraction = 0;

                isMove = true;
                isContinueMove = false;
            }
        }
    }

    #endregion


    #region Private methods

    private void StartMove(Vector2 targetPosition, float movementTime, bool isFall)
    {
        anim.SetBool("isRun", true);

        this.targetPosition = targetPosition;
        this.movementTime = movementTime;
        this.isFall = isFall;

        statrPosition = playerPos;
        startTime = Time.realtimeSinceStartup;
        fraction = 0;
        isMove = true;
        isContinueMove = true;
    }

    #endregion


    #region Event handlers
        
    private void Player_OnStickFallHorizontal(float stickSize)
    {
        float endOfPlatform = PlatformManager.NewDistance + PlatformManager.BehindPlatformWidth;
        bool isFall = stickSize < PlatformManager.NewDistance || stickSize > endOfPlatform;

        Vector2 targetPosition = StartPos;
        float movementTime = 1f;

        if (isFall)
        {
            targetPosition.x += stickSize;
            movementTime = stickSize / playerSpeed;
        }
        else
        {
            targetPosition.x += PlatformManager.NewDistance + PlatformManager.BehindPlatformWidth;
            movementTime = (PlatformManager.NewDistance + PlatformManager.BehindPlatformWidth) / playerSpeed;

            float startRedSquare = PlatformManager.NewDistance + (PlatformManager.BehindPlatformWidth / 2f) - 0.25f;
            float endRedSquare = startRedSquare + 0.5f;
            if (stickSize > startRedSquare && stickSize < endRedSquare)
            {
                OnScoreUp();
            }
        }

        StartMove(targetPosition, movementTime, isFall);
    }

    #endregion
}
