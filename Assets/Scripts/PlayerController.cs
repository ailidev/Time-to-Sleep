using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
#if ENABLE_INPUT_SYSTEM
using Aili.Inputs;
using UnityEngine.InputSystem;
#endif

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [Space]
    [Header("REFERENCES")]
    public Camera m_Camera;
    public Animator m_Animator;
    public Transform m_Indicator;

    [Space]
    [Header("DEBUG")]
    public bool m_Walking = false;
    public Transform currentCube;
    public Transform clickedCube;
    public List<Transform> finalPath;

    float blend;
    bool m_PointClicked;
    Vector2 m_ClickedPosition;
    InputControls m_Input;

    public PlayerState m_State = PlayerState.Idle;
    public enum PlayerState
    {
        Idle,
        Walking
    }

#region Inputs
    void OnEnable()
    {
        m_Input.Enable();
    }

    void OnDisable()
    {
        m_Input.Disable();
    }

    public void Click(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            m_PointClicked = true;
            m_ClickedPosition = m_Input.Player.MousePosition.ReadValue<Vector2>();
        }
        else
        {
            m_PointClicked = false;
        }
    }
#endregion

#region Editor
    void OnDrawGizmos()
    {
        // Draw Ray below player
        Gizmos.color = Color.blue;
        Ray ray = new Ray(transform.GetChild(0).position, -transform.up);
        Gizmos.DrawRay(ray);
    }
#endregion

    void Awake()
    {
        m_Input = new InputControls();
    }

    void Update()
    {
        if (m_Animator != null)
        {
            m_Animator.SetBool("walking", m_Walking);
        }

        
        RayCastDown(); //GET CURRENT CUBE (UNDER PLAYER)
        Movement();

        // parent player to moving ground
        if (currentCube.GetComponent<Walkable>().movingGround)
        {
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }
    }

    void Movement()
    {
        // CLICK ON CUBE
        if (m_PointClicked)
        {
            Ray mouseRay = m_Camera.ScreenPointToRay(m_ClickedPosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                // If there's a Walkable component when clicking object...
                if (mouseHit.transform.GetComponent<Walkable>() != null)
                {
                    clickedCube = mouseHit.transform; // Assign clickedCube with clicked object
                    DOTween.Kill(gameObject.transform); // Kill this object with DOTween?

                    finalPath.Clear(); // Clear calculated path
                    FindPath(); // Find new path

                    blend = transform.position.y - clickedCube.position.y > 0 ? -1 : 1; // Used for stair function, go to RayCastDown()

#region Click Effect
                    m_Indicator.position = mouseHit.transform.GetComponent<Walkable>().GetWalkPoint();
                    Sequence s = DOTween.Sequence();
                    s.AppendCallback(
                        () => m_Indicator.GetComponentInChildren<ParticleSystem>().Play()
                    );
                    s.Append(m_Indicator.GetComponent<Renderer>().material.DOColor(Color.white, .1f));
                    s.Append(
                        m_Indicator
                            .GetComponent<Renderer>()
                            .material.DOColor(Color.black, .3f)
                            .SetDelay(.2f)
                    );
                    s.Append(m_Indicator.GetComponent<Renderer>().material.DOColor(Color.clear, .3f));
#endregion
                }
            }
        }
    }

#region Path Functionality
    void FindPath()
    {
        List<Transform> nextCubes = new List<Transform>();
        List<Transform> pastCubes = new List<Transform>();

        foreach (WalkPath path in currentCube.GetComponent<Walkable>().possiblePaths)
        {
            if (path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = currentCube;
            }
        }

        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        if (current == clickedCube)
        {
            return;
        }

        foreach (WalkPath path in current.GetComponent<Walkable>().possiblePaths)
        {
            if (!visitedCubes.Contains(path.target) && path.active)
            {
                nextCubes.Add(path.target);
                path.target.GetComponent<Walkable>().previousBlock = current;
            }
        }

        visitedCubes.Add(current);

        if (nextCubes.Any())
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    void BuildPath()
    {
        Transform cube = clickedCube;
        while (cube != currentCube)
        {
            finalPath.Add(cube);
            if (cube.GetComponent<Walkable>().previousBlock != null)
                cube = cube.GetComponent<Walkable>().previousBlock;
            else
                return;
        }

        finalPath.Insert(0, clickedCube);

        FollowPath();
    }

    void FollowPath()
    {
        Sequence s = DOTween.Sequence();

        m_Walking = true;

        for (int i = finalPath.Count - 1; i > 0; i--)
        {
            float time = finalPath[i].GetComponent<Walkable>().isStair ? 1.5f : 1;

            s.Append(
                transform
                    .DOMove(finalPath[i].GetComponent<Walkable>().GetWalkPoint(), .2f * time)
                    .SetEase(Ease.Linear)
            );

            if (!finalPath[i].GetComponent<Walkable>().dontRotate)
                s.Join(
                    transform.DOLookAt(finalPath[i].position, .1f, AxisConstraint.Y, Vector3.up)
                );
        }

        if (clickedCube.GetComponent<Walkable>().isButton)
        {
            s.AppendCallback(() => GameManager.instance.RotateRightPivot());
        }

        s.AppendCallback(() => Clear());
    }

    void Clear()
    {
        foreach (Transform t in finalPath)
        {
            t.GetComponent<Walkable>().previousBlock = null;
        }
        finalPath.Clear();
        m_Walking = false;
    }
#endregion

#region Ground Raycast
    public void RayCastDown()
    {
        // Do Raycast below player
        Ray playerRay = new Ray(transform.GetChild(0).position, -transform.up);
        RaycastHit playerHit;

        if (Physics.Raycast(playerRay, out playerHit))
        {
            if (playerHit.transform.GetComponent<Walkable>() != null)
            {
                currentCube = playerHit.transform;

                // Blend Tree (Animator) functionality when stepping stair
                if (playerHit.transform.GetComponent<Walkable>().isStair)
                {
                    DOVirtual.Float(GetBlend(), blend, .1f, SetBlend);
                }
                else
                {
                    DOVirtual.Float(GetBlend(), 0, .1f, SetBlend);
                }
            }
        }
    }
#endregion

#region Animations
    float GetBlend()
    {
        // Get value from Blend Tree
        return GetComponentInChildren<Animator>().GetFloat("Blend");
    }

    void SetBlend(float x)
    {
        GetComponentInChildren<Animator>().SetFloat("Blend", x);
    }
}
#endregion
