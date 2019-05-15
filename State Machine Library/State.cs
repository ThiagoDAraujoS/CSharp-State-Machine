using System.Collections.Generic;
//using UnityEngine;
using System;

namespace StateMachine
{
	/// <summary>
	/// The state abstract class, is an observer template, it serves to indicate that the class is an engine's observer
	/// and this class will work as a "vessel" do deliver a intrincate sequence of messasges.
	/// </summary>
	public abstract class State
	{
		/// <summary>
		/// state's link contains a delegate with the link information and the state that link point at
		/// </summary>
		public Dictionary<Link,State> links; 

		/// <summary>
		/// Reference to the caster of the state machine
		/// </summary>
		public Object m;

		/// <summary>
		/// Message: Happens when the object is built
		/// </summary>
		public virtual void Awake(){}

		/// <summary>
		/// Message: Starting the state
		/// </summary>
		public virtual void Start(){}

		/// <summary>
		/// Mensage: Updating the state
		/// </summary>
		public virtual void Update(){}

		/// <summary>
		/// Message: Ending the state
		/// </summary>
		public virtual void End(){}
	}
}
