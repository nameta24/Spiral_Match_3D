using UnityEngine;

public class Cell : MonoBehaviour
{
    public int ID;

    [HideInInspector] public MeshRenderer _meshRenderer;
    [SerializeField] private ParticleSystem _particleSystem;
    public void ShowSelectionParticles()
    {
        _particleSystem.startColor = _meshRenderer.material.color* 1.4f;;
        GameObject spawnedParticles = Instantiate(_particleSystem.gameObject);
        spawnedParticles.transform.position = _particleSystem.transform.position;
       spawnedParticles.SetActive(true);
       Destroy(spawnedParticles,1.5f);
    }
    
}
