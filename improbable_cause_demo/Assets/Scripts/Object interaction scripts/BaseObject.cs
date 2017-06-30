﻿using UnityEngine;

public class BaseObject : MonoBehaviour
{
    /* This script should go onto a usable object. It allows the player to pick up
     * and move the object by changing the objects layer from the default to the ignore
     * raycast layer. It also manages which anchor point is "holding" the object */
    public string description = "";
    public KeyCode xPositive = KeyCode.W;
    public KeyCode xNegative = KeyCode.S;
    public KeyCode yPositive = KeyCode.A;
    public KeyCode yNegative = KeyCode.D;
    public bool isPickupable = true;
    public bool isKinematic = false;

    [HideInInspector]
    public bool isHeld = false;

    private float ROTATION = 90.0f;

    protected AnchorPoint anchorPoint;
    protected bool isPaused = false;
    protected int IGNORE_RAYCAST_LAYER = 2;
    protected int DEFAULT_LAYER = 0;
    protected Vector3 startingPosition;
    protected Quaternion startingRotation;
    private bool isRaycasting = false;

    [Tooltip("Object type label.")]
    public ObjectType objectType;

    public string GetDescription()
    {
        // Debug.Log(description);
        return description;
    }

    public enum ObjectType
    {
        Launcher,
        Stacker,
        Throwable,
        Domino
    }

    public string getObjectType()
    {
        return objectType.ToString();
    }

    public virtual void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Update()
    {
        if (isHeld)
        {
            if (Input.GetKeyDown(xPositive))
            {
                transform.Rotate(transform.up, ROTATION, Space.World);
            }
            if (Input.GetKeyDown(xNegative))
            {
                transform.Rotate(transform.up, -ROTATION, Space.World);
            }
            if (Input.GetKeyDown(yPositive))
            {
                transform.Rotate(transform.forward, ROTATION, Space.World);
            }
            if (Input.GetKeyDown(yNegative))
            {
                transform.Rotate(transform.forward, -ROTATION, Space.World);
            }
        }
    }

    public virtual void Play()
    {
        if (!isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Rigidbody>())
                {
                    child.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }

    public virtual void Pause()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public virtual void Restart()
    {
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        GetComponent<Rigidbody>().isKinematic = true;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Rigidbody>())
            {
                child.GetComponent<Rigidbody>().isKinematic = true;
            }
            if (child.GetComponent<StabberComponent>())
            {
                child.GetComponent<StabberComponent>().Restart();
            }
        }
    }

    public virtual void pickUp()
    {
        gameObject.layer = IGNORE_RAYCAST_LAYER;
        if (anchorPoint)
        {
            anchorPoint.IsOccupied = false;
            anchorPoint = null;
        }

        try
        {
            gameObject.GetComponent<HitSound>().PlaySoundPickUp(gameObject);
        }
        catch
        {
            Debug.LogError("You have not attached the HitSound Script to this object");
        }
    }

    public void ResetRotation()
    {
        transform.rotation = startingRotation;
    }

    public void startRaycast()
    {
        isRaycasting = true;
    }

    public void stopRaycast()
    {
        isRaycasting = false;
    }

    public virtual void place(GameObject dropLocation, AnchorPoint anchorPoint)
    {
        this.anchorPoint = anchorPoint;
        gameObject.layer = DEFAULT_LAYER;
        gameObject.transform.position = dropLocation.GetComponent<AnchorPoint>().GetPosition(GetComponent<Renderer>().bounds.size.y);
        try
        {
            gameObject.GetComponent<HitSound>().PlaySound(gameObject);
        }
        catch
        {
            Debug.LogError("You have not attached the HitSound Script to this object");
        }
    }
}