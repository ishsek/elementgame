using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float Damage;
    [SerializeField] private float m_PullStrength;
    private float mVoidTimer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mVoidTimer += Time.deltaTime;
    }
}
