using System.Collections;
using System.Collections.Generic;
using EZ_Pooling;
using UnityEngine;

public class PlayerProjectileSpawner : MonoBehaviour {

	[Header("Input")]
	public KeyCode spawnKey = KeyCode.Mouse0;

	[Header("Spawner Settings")]
	[SerializeField] private Transform _bullet;
	[SerializeField] private Transform _spawnPoint;

	public float SpawnRate;
	private float timer;

	[Header("Particles")]
	public ParticleSystem spawnParticles;

	[Header("Audio")]
	public AudioSource spawnAudioSource;

	void Update()
	{
		timer += Time.deltaTime;

		if(Input.GetKey(spawnKey) && timer >= SpawnRate)
		{
			SpawnProjectile();
		}
	}
	

	void SpawnProjectile()
	{
		timer = 0f;
		EZ_PoolManager.Spawn(_bullet, _spawnPoint.position, _spawnPoint.rotation /*Quaternion.Euler(_spawnPoint.eulerAngles.x, _spawnPoint.eulerAngles.y, 90)*/);;

		if(spawnParticles)
			spawnParticles.Play();
		
		if(spawnAudioSource)
			spawnAudioSource.Play();
		
	}

}
