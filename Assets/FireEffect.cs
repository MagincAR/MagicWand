using UnityEngine;

public class FireEffect : MonoBehaviour
{
    private ParticleSystem fireParticleSystem;

    void Start()
    {
        // Fireball이 생성될 때 ParticleSystem을 찾습니다.
        fireParticleSystem = GetComponent<ParticleSystem>();

        if (fireParticleSystem == null)
        {
            Debug.LogError("FireEffect requires a ParticleSystem on the same GameObject.");
        }
    }

    // 불타는 효과를 시작하는 메서드
    public void StartFire()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Play(); // ParticleSystem 시작
        }
    }

    // 불타는 효과를 멈추는 메서드 (필요 시 추가)
    public void StopFire()
    {
        if (fireParticleSystem != null)
        {
            fireParticleSystem.Stop(); // ParticleSystem 중지
        }
    }
}