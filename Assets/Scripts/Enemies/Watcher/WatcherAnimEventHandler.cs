using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherAnimEventHandler : MonoBehaviour
{
    [SerializeField] private Watcher m_Watcher;
    public void Fire()
    {
        m_Watcher.Fire();
    }

    public void EndAttack()
    {
        m_Watcher.EndAttack();
    }
}
