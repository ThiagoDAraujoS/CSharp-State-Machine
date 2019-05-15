using UnityEngine;
using System.Collections;
using StateMachine;

public class HappyCube : CommonCube		//<---- CHILD OF COMMONCUBE!
{
	#region notImportant
	[SerializeField]
	Material[] colors = null;
	ParticleSystemController pSController = null;

	[SerializeField]
	ParticleSystem 	fireParticle = null, chargeParticle = null;

	public float charge = 0.0f;
	public float chargeMax = 0.0f;

	protected override void OverrideStart ()
	{
		base.OverrideStart ();
		pSController = GetComponent<ParticleSystemController>();
	}
	#endregion

	#region important
	[Initial][Override][StateFrom("x")]
	public class Patrol : State 				//OVERRIDE THE INITIAL STATE PATROL FROM BASE CLASS
	{
		HappyCube s;							//GET THE COMMONCUBE REFERENCE
		public override void Awake ()			//STATE's ON AWAKE 
		{
			s = (HappyCube)m;					//CAST DOWN THE m VARIABLE INTO A COMMONCUBE VARIABLE
		}

		public override void Update ()			//THE STATE's ON UPDATE METHOD
		{								
			s.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * Time.deltaTime*5.0f);	//READ THE CONTROL TO ENABLE MOVEMENT
			s.transform.Rotate(0.0f,Input.GetAxis("Horizontal")*Time.deltaTime*200f,0.0f);
		}
	
		[LinkTo(typeof(Charging))]
		public bool StartCharging () 			//LINK TO CHARGING STATE
		{
			return Input.GetAxis("Fire1") == 1.0f;		//IF FIRE1 WAS PRESSED
		}

		[LinkTo(typeof(Hover))]					//LINK TO HOVER STATE
		public bool Jump () 
		{
			return Input.GetAxis("Jump") == 1.0f;		//IF JUMP WAS PRESSED
		}
	}

	[StateFrom("x")]
	public class Hover : State
	{
		HappyCube s;							//GET A COMMONCUBE REFERENCE
		public override void Awake ()			//STATE's ON AWAKE 
		{
			s = (HappyCube)m;					//CAST DOWN THE m VARIABLE INTO A COMMONCUBE VARIABLE
		}

		public override void Start ()			//THE STATE's START METHOD
		{
			s.rend.material = s.colors[2];			//SWAP MATERIAL TO THE BLUE ONE
			s.physics.AddForce(Vector3.up*100.0f);//ADD UPWARD FORCE
			s.pSController.StartParticles();	//START THE PARTICLE GENERATOR
		}

		public override void Update ()			//THE STATE's ON UPDATE METHOD
		{
			s.physics.AddForce(Vector3.up*Time.deltaTime*488.0f);									//ADD A STABLE FORCE TO HOLD IT IN THE AIR
			s.transform.Translate(Vector3.forward*Input.GetAxis("Vertical")*Time.deltaTime*10.0f);	//READ THE CONTROL "vertical" AXIS TO ENABLE THE BOX TO BE CONTROLED
			s.transform.Rotate(0.0f,Input.GetAxis("Horizontal")*Time.deltaTime*400f,0.0f);			//READ THE CONTROL "horizontal" AXIS TOO
		}

		public override void End ()				//THE STATE's ON END METHOD
		{
			s.rend.material = s.colors[0];			//RETURN THE WHITE MATERIAL 
			s.pSController.StopParticles();		//STOP PARTICLE BURST
		}

		[LinkTo(typeof(Patrol))]
		public bool Landing () 					//LINK TO PATROL STATE
		{
			return Input.GetAxis("Jump") == 0.0f;	//IF STOP PRESSING THE JUMP BUTTON
		}
	}

	[StateFrom("x")]
	public class Charging : State
	{
		HappyCube s;							//GET A COMMONCUBE REFERENCE
		public override void Awake ()			//STATE's ON AWAKE 
		{
			s = (HappyCube)m;					//CAST DOWN THE m VARIABLE INTO A COMMONCUBE VARIABLE
		}

		public override void Start ()			//THE STATE's ON END METHOD
		{	
			s.rend.material = s.colors[1];			//CHANGE COLOR TO RED MATERIAL
			s.chargeParticle.Play();			//PLACE PARTICLE SYSTEM
		}

		public override void Update ()			//THE STATE1s ON UPDATE METHOD
		{	
			s.charge = Mathf.Lerp(s.charge, 1.5f,Time.deltaTime);		//CHARGE DASH
			s.transform.Rotate(0.0f,Input.GetAxis("Horizontal")*Time.deltaTime*200f,0.0f);	//READ PLAYER CONTROL TO SPIN THE CUBE	
		}

		public override void End ()				//THE STATE's END STATE
		{
			s.rend.material = s.colors[0];				//SWAP MATERIAL
			s.physics.AddForce(s.transform.forward * s.charge*(s.chargeMax/2));		//ADD DASH FOWARD
			s.physics.AddForce(Vector3.up * 100.0f);			//ADD FORCE UPWARDS
			s.chargeParticle.Stop();				//STOP CHARGING PARTICLE EFFECTS
			s.fireParticle.Play();					//PLAY FIRE PARTICLE SYSTEM
			s.charge = 0.01f;						//RESET CHARGE VARIABLE
		}

		[LinkTo(typeof(Patrol))]
		public bool UnleashDash () 				//LINK TO PATROL
		{
			return (Input.GetAxis("Fire1") == 0.0f);	//IF STOP PRESSING FIRE1 BUTTON
		}
	}
	#endregion
}
