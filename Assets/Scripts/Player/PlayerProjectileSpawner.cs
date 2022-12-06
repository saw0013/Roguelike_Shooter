using UnityEngine;
using UnityEngine.UI;

public class PlayerProjectileSpawner : MonoBehaviour {

	[Header("UI")]
	[SerializeField] private Text _textCartridges;

	[Header("Input")]
	public KeyCode spawnKey = KeyCode.Mouse0;
	public KeyCode reloadKey = KeyCode.Mouse1;

	[Header("Spawner Settings")]
	[SerializeField] private Transform _bullet;
	[SerializeField] private Transform _spawnPoint;

	public float SpawnRate;
	public float ReloadTime;
	public int MaxCartridges;

	private float timer;
	private float timerReload;

	private int сartridges;

	private bool reloading;

	[Header("Particles")]
	public ParticleSystem spawnParticles;

	[Header("Audio")]
	[SerializeField] private AudioSource _shootAudio;
	[SerializeField] private AudioSource _reloadAudio;
	[SerializeField] private AudioSource _nullShootAudio;

	[SerializeField] private PlayerData playerData;

    private void Start()
    {
		сartridges = MaxCartridges;
		_textCartridges.text = $"Cartridges: {сartridges}";
	}

    void Update()
	{
		timer += Time.deltaTime;

        if (playerData.GetInputActive())
        {
			if (Input.GetKey(reloadKey) && !reloading && сartridges != MaxCartridges)
			{
				reloading = true;

				if (_reloadAudio) _reloadAudio.Play();
			}

			if (Input.GetKey(spawnKey) && timer >= SpawnRate && !reloading)
			{
				if (сartridges > 0)
				{
					SpawnProjectile();
				}
				else
				{
					_nullShootAudio.Play();
				}
			}

			if (reloading)
			{
				timerReload += Time.deltaTime;
				if (timerReload >= ReloadTime)
				{
					сartridges = MaxCartridges;
					timerReload = 0;
					reloading = false;
					ReloadText();
				}
			}
		}
	}

	public void ReloadText() => _textCartridges.text = $"Cartridges: {сartridges}";

	void SpawnProjectile()
	{
		timer = 0f;
		сartridges--;
		ReloadText();
		//EZ_PoolManager.Spawn(_bullet, _spawnPoint.position, _spawnPoint.rotation /*Quaternion.Euler(_spawnPoint.eulerAngles.x, _spawnPoint.eulerAngles.y, 90)*/);;

		if(spawnParticles)
			spawnParticles.Play();
		
		if(_shootAudio)
            _shootAudio.Play();
	}
}
