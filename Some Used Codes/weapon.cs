using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weapon : MonoBehaviour {

	private Animator anim;
	//public Animator ControllerAnim;

	private AudioSource _AudioSource;

	public float range = 100f;
	public int bulletPerMag = 30;
	public int bulletsLeft = 200;
	public int currentBullets;
	public ParticleSystem muzzleFlash;
	public AudioClip shootSound;
	public AudioClip reloadSound;
	//public AudioClip bulletImpactSound;
	public bool isReloading;
	private bool shootInput;
	private Vector3 originalPosition;
	private Vector3 originalRotation;
	public Vector3 aimPosition;
	public Vector3 recoilRotation;

	private bool isAiming;


	public float aimDownSightSpeed = 8f;

	public float recoilSpeed = 3f;

	public Transform shootPoint;

	public GameObject[] ParticlesHit;
	public PhysicMaterial[] PhysicMaterial;
	

	public enum ShootMode { Auto, Semi } 
	public ShootMode shootingMode;

	public float fireRate = 0.1f;
	public float damage = 20f;

	public float punchPower = 5f;

	float fireTimer;

	public Text uitext;

public GameObject Enemy;




	// Use this for initialization
	void Start () {

		anim = GetComponent<Animator> ();

		_AudioSource = GetComponent<AudioSource>();

		currentBullets = bulletPerMag;

		originalPosition = transform.localPosition;
		

	}
	
	// Update is called once per frame
	void Update () {
	
		switch (shootingMode) {

		case ShootMode.Auto:
			shootInput = Input.GetButton ("Fire1");
			break;

		case ShootMode.Semi:
			shootInput = Input.GetButtonDown ("Fire1");
			break;


			uitext.text = "ammo" + bulletsLeft.ToString ();

			if (Input.GetKeyDown ("left shift"))
			{

				anim.SetBool("Run",true);
			}
			if (Input.GetKeyUp ("left shift"))
			{
				anim.SetBool("Run",false);
			}


		}

		if (shootInput)
		{
			if (currentBullets > 0){
				Fire ();
				
			}
			else if(bulletsLeft > 0){
				DoReload ();
			
			}
		}

		if (Input.GetKeyDown (KeyCode.R)) 
		{
			if(currentBullets < bulletPerMag && bulletsLeft >0)
				DoReload();

		}

			
		if (fireTimer < fireRate)
		{
			fireTimer += Time.deltaTime;
		}

		AimDownSight ();
		Recoil();

	}


	private void AimDownSight(){
	
	
		if (Input.GetButton ("Fire2") && !isReloading) {
		
			transform.localPosition = Vector3.Lerp (transform.localPosition, aimPosition, Time.deltaTime * aimDownSightSpeed);
			isAiming = true;

		}
		else {
		
			transform.localPosition = Vector3.Lerp (transform.localPosition, originalPosition, Time.deltaTime * aimDownSightSpeed);
			isAiming = false;

		}

	}


		private void Recoil(){
	
	
		if (Input.GetButton ("Fire1") && !isReloading) {
		
			
			

		}
		else {
		/*	if(isAiming)
			transform.localPosition = Vector3.Lerp (transform.localPosition, aimPosition, Time.deltaTime * recoilSpeed);

			transform.localPosition = Vector3.Lerp (transform.localPosition, originalPosition, Time.deltaTime * recoilSpeed);
			*/

		}

	}




	void FixedUpdate()
	{

		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo (0);

		isReloading = info.IsName ("Reload");

		if (info.IsName ("Fire"))
		anim.SetBool ("Fire", false);
		anim.SetBool ("Aim", isAiming);

		
	}

	private void Fire()
	{
	

		anim.SetBool("Fire",true);

	    var shootVariation = UnityEngine.Random.insideUnitSphere;
            shootVariation *= 0.02f;
		if (fireTimer < fireRate || currentBullets <= 0 || isReloading)
			return;
		

		RaycastHit hit;

		//string materialName = hit.collider.sharedMaterial.name;

		if (Physics.Raycast (shootPoint.position, shootPoint.transform.forward + shootVariation, out hit, range)) 
		{
			Debug.Log (hit.transform.name + "Found!");


			Rigidbody enemyrigid = hit.collider.GetComponent <Rigidbody> ();

			if (enemyrigid != null) {
			
				enemyrigid.velocity = (hit.point - shootPoint.transform.position).normalized * punchPower;
			
			}
			if (hit.collider.CompareTag ("Enemy")) {
GameObject bulletHole = Instantiate(ParticlesHit[4], hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
					bulletHole.transform.SetParent(hit.transform);
					hit.transform.GetComponent<Soldier>().Die(damage);

			}

			if (hit.collider.sharedMaterial != null)
			{ for(int i=0;i<ParticlesHit.Length;i++){
				if(hit.collider.sharedMaterial.name == PhysicMaterial[i].name){
					GameObject bulletHole = Instantiate(ParticlesHit[i], hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
					bulletHole.transform.SetParent(hit.transform);
					Destroy(bulletHole, 10f);
				}
			}
			
			}
			
			if (hit.transform.GetComponent<healthCont> ()) {
			
				hit.transform.GetComponent<healthCont> ().ApplyDamage (damage);

				//hit.transform.GetComponent<healthCont> ().PlayBulletImpact();
			}
			if (hit.transform.GetComponent<enemyHealth> ()) {

				hit.transform.GetComponent<enemyHealth> ().ApplyDamage (damage);

				//hit.transform.GetComponent<healthCont> ().PlayBulletImpact();
			}
                        if (hit.transform.GetComponent<enemyHealth> ()) {

				hit.transform.GetComponent<objectHealth> ().ApplyDamage (damage);

				//hit.transform.GetComponent<healthCont> ().PlayBulletImpact();
			}



		}
	

		anim.CrossFadeInFixedTime ("Fire", 0.01f);
		//anim.SetBool ("Fire", true);
		muzzleFlash.Play();
		PlayShootSound ();

		currentBullets--;

		fireTimer = 0.0f; //reset
	
	}

/*
	public void ApplyHit(RaycastHit hit)
        {
            //Play ricochet sfx
           // RicochetSFX();
            //Set tag and transform of hit to HitParticlesFXManager
            HitParticlesFXManager(hit);

            //Decrease health of object by calculatedDamage
            if (hit.collider.GetComponent<ObjectHealth>())
                hit.collider.GetComponent<ObjectHealth>().health -= calculatedDamage;

            if (!hit.rigidbody)
            {
                //Set hit position to decal manager
                DecalManager(hit, false);
            }

            if (hit.rigidbody)
            {
                //Add force to the rigidbody for bullet impact effect
                hit.rigidbody.AddForceAtPosition(weaponSetting.rigidBodyHitForce * mainCamera.transform.forward, hit.point);
            }

            if (hit.collider.tag == "Target")
            {
                hit.rigidbody.isKinematic = false;
                hit.rigidbody.AddForceAtPosition(rigidbodyHitForce * mainCamera.transform.forward, hit.point);
            }


            if (hit.collider.GetComponent<PlayerHitTarget>())
                hit.collider.GetComponent<PlayerHitTarget>().npc.GetHit(calculatedDamage, transform);

            if (hit.collider.GetComponent<ZombieNPC>())
                hit.collider.GetComponent<ZombieNPC>().ApplyHit(Random.Range(damageMin, damageMax));
        }



		public void HitParticlesFXManager(RaycastHit hit)
        {
            if(hitFXManager == null)
            {
                hitFXManager = FindObjectOfType<HitFXManager>();
            }
            
            if (hit.collider.tag == "Wood")
            {
                hitFXManager.objWoodHitFX.Stop();
                hitFXManager.objWoodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objWoodHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objWoodHitFX.Play(true);
            }
            else if (hit.collider.tag == "Concrete")
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }
            else if (hit.collider.tag == "Dirt")
            {
                hitFXManager.objDirtHitFX.Stop();
                hitFXManager.objDirtHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objDirtHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objDirtHitFX.Play(true);
            }
            else if (hit.collider.tag == "Metal")
            {
                hitFXManager.objMetalHitFX.Stop();
                hitFXManager.objMetalHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objMetalHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objMetalHitFX.Play(true);
            }
            else if (hit.collider.tag == "Flesh")
            {
                hitFXManager.objBloodHitFX.Stop();
                hitFXManager.objBloodHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objBloodHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objBloodHitFX.Play(true);
            }
            else
            {
                hitFXManager.objConcreteHitFX.Stop();
                hitFXManager.objConcreteHitFX.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                hitFXManager.objConcreteHitFX.transform.LookAt(mainCamera.transform.position);
                hitFXManager.objConcreteHitFX.Play(true);
            }

        }

*/


	public void Reload()
	{
		_AudioSource.PlayOneShot (reloadSound);
		if (bulletsLeft <= 0)
			return;
		int bulletToLoad = bulletPerMag - currentBullets;
		//                    IF                         THEN    1st     ELSE     2nd
		int bulletsToDeduct = (bulletsLeft >= bulletToLoad) ? bulletToLoad : bulletsLeft;

		bulletsLeft -= bulletsToDeduct;
		currentBullets += bulletsToDeduct;

	}

	public void DoReload()
	{
		AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo (0);

		if (isReloading)
			return;

		anim.CrossFadeInFixedTime ("Reload", 0.01f);
		

	}


	private void PlayShootSound()
	{
		_AudioSource.PlayOneShot (shootSound);
		//_AudioSource.clip = shootSound;
		//_AudioSource.Play();

	}

	//private void PlayBulletImpact()
	//{
	//	_AudioSource.PlayOneShot (bulletImpactSound);
		
	//}



	void OnGUI()
	{
		GUI.Label(new Rect(1700, 800, 150, 60), "Ammo: "+currentBullets+" /  "+bulletsLeft);
	} 




}

