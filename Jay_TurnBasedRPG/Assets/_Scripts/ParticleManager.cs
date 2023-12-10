using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    [SerializeField] private List<GameObject> particles = new List<GameObject>();

    public IEnumerator InstantiateParticle(string particleName, Transform pos, float timeToDestroy, bool isParent, float size, bool hasDelay)
    {
        foreach (GameObject g in particles)
        {
            if (g.name == particleName)
            {
                if (hasDelay)
                    yield return new WaitForSeconds(0.35f);
                else
                    yield return new WaitForSeconds(0);

                var go = Instantiate(g, pos.position, Quaternion.identity);

                go.transform.localScale = new Vector3(size, size, size);

                if (isParent)
                {
                    go.transform.SetParent(pos);
                }

                Destroy(go, timeToDestroy);
            }
        }
    }
}
