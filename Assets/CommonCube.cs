using UnityEngine;
using System.Collections;
using StateMachine;

public class CommonCube : MonoBehaviour
{	
	#region notImportant
	[SerializeField]
	protected ParticleSystem deathParticle = null;
	protected Rigidbody physics = null;
	protected Renderer rend = null;
	protected Collider col = null;
	
	[HideInInspector]
	public bool wasShot = false;
	public Transform[] route = null;
	private int routeIndex = 0;

	void OnMouseOver()
	{
		if (!wasShot && Input.GetMouseButton(0))//VERIFY IF IT WAS CLICKED
			wasShot = true;
	}
	protected virtual void OverrideStart()
	{
		rend = GetComponent<Renderer>();	//GET SOME COMPONENTS
		physics = GetComponent<Rigidbody>();	//GET SOME COMPONENTS
		col = GetComponent<Collider>();
	}
	#endregion

	#region important
	Engine engine;								//REFERENCE TO THE STATE MACHINE ENGINE
	
	void Start  () 
	{	
		engine = new Engine("x", this);			//INITIATE THE ENGINE
		OverrideStart();
	}
	void Update () 
	{	
		engine.Run();							//RUN THE ENGINE
	}

	[Initial][StateFrom("x")]
	public class Patrol : State  				//PATROL STATE
	{
		CommonCube s;							//GET A COMMONCUBE REFERENCE
		public override void Awake ()			//STATE's ON AWAKE 
		{
			s = (CommonCube)m;					//CAST DOWN THE m VARIABLE INTO A COMMONCUBE VARIABLE
		}

		public override void Update ()			//STATE's ON UPDATE METHOD
		{
												//TRACK FOLLOWING METHOD
			s.transform.position = Vector3.Lerp(s.transform.position, s.route[s.routeIndex].position, Time.deltaTime * 2.0f / Vector3.Distance(s.transform.position, s.route[s.routeIndex].position));

												//IF TOO NEAR THE TARGET, SWAP TARGET
			if (Vector3.Distance(s.transform.position, s.route[s.routeIndex].position) <= 0.5)
				s.routeIndex = (Vector3.Distance(s.transform.position, s.route[s.routeIndex].position) <= 0.5 && s.routeIndex == s.route.Length-1)? 0 : s.routeIndex+1;

												//LOOK AT TARGET
			s.transform.LookAt(s.route[s.routeIndex].position);
		}
	}

	[StateFrom("x")]
	public class Death : State 					//DEATH STATE
	{
		CommonCube s;							//GET A COMMONCUBE REFERENCE
		public override void Awake ()			//STATE's ON AWAKE METHOD
		{
			s = (CommonCube)m;					//CAST DOWN THE m VARIABLE INTO A COMMONCUBE VARIABLE
		}

		public override void Start ()			//STATE's ON START METHOD
		{
			s.rend.enabled = false;				//DISABLE RENDERER
			s.physics.Sleep();					//PUT PHYSICS TO SLEEP
			s.deathParticle.Play();				//PLAY DEATH PARTICLES
			s.col.enabled = false;				//DISABLE COLLIDER	
		}

		[LinkTo("All")][Mask(typeof(Death))]
		public bool LinkName () 				//OMNI REVERSE LINK TO DEATH, IT IGNORES' DEATH STATE
		{
			return s.wasShot;					//IF IT WAS CLICKED
		}
	}
	#endregion
}
