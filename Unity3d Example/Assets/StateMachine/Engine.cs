using System;
using System.Collections.Generic;
//using UnityEngine;

namespace StateMachine
{
	/// <summary>
	/// The Engine, builds, runs and hold the state machine information, this class works as an observed class to the states
	/// </summary>
	public class Engine
	{
		/// <summary>
		/// The name of this state machine. The system uses this string to identify what node belongs to what state machine
		/// </summary>
		private string name;

		/// <summary>
		/// The current active state. The system always run one state per time, the state saved in this variable will be run once the method Run() is called
		/// </summary>
		public State activeState;

		/// <summary>
		/// The last state before the active state becomes the active one.
		/// </summary>
	//	public State lastState;

		/// <summary>
		/// All states listed by their names.
		/// </summary>
		public Dictionary<string,State> states;

		/// <summary>
		/// Returns a set of States keyed by their infoBlock, 
		/// </summary>
		/// <param name="blueprint">List of stateIds objects depicting whats states should be built</param>
		/// <param name="caster">Who owns the state machine.</param>
		private Dictionary<StateID, State> BuildStates(List<StateID> blueprint,  Object caster){

			//Initialize the list
			Dictionary<StateID, State> stateReferenceTable = new  Dictionary<StateID, State>();	

			//Temporary Auxiliar reference used on the algorithm
			State aux;

			//Initiates all the states at the blueprint, fill its data, and save the object at the state list
			foreach (StateID stateInfo in blueprint) 	
			{
				//Create a instance of the state using the system.activator object
				aux = (State)Activator.CreateInstance(stateInfo.stateType);				

				//Forward the owner's reference
				aux.m = caster;																									
				
				//add the newly created state into the table
				stateReferenceTable.Add(stateInfo, aux);																		

				//add the newly created state into the states dictionary for later use
				states.Add(stateInfo.stateType.Name, aux);																		
			}

			//return the table
			return stateReferenceTable;
		}

		/// <summary>
		/// Link all the states using the information inside their stateReferenceTable , 
		/// </summary>
		/// <param name="blueprint">List of stateIds objects depicting whats states should be built</param>
		/// <param name="caster">Who owns the state machine.</param>
		private void LinkStates(Dictionary<StateID, State> stateReferenceTable){
			
			//For each state on the reference list
			foreach (KeyValuePair<StateID,State> state in stateReferenceTable)	{
				
				//Initiates its link list
				state.Value.links = new Dictionary<Link, State>();				

				//for each linkID in its infoblock
				foreach (LinkID link in state.Key.LinkList) 					
					
					//build a delagate using the method info.
					state.Value.links.Add((Link)Delegate.CreateDelegate(typeof(Link),stateReferenceTable[link.caster],link.link),stateReferenceTable[link.target]); 			
			}
		}
	

		/// <summary>
		/// Builds a new instance of the <see cref="StateMachine.Engine"/> together with the whole state machine structure class.
		/// </summary>
		/// <param name="name">State machine name.</param>
		/// <param name="caster">Who owns the state machine.</param>
		public Engine(string name, Object caster)
		{
			this.name = name;

			states = new Dictionary<string, State>();

			//Request from the scanner a blueprint for construction of the named state machine, the blueprint will be a guide to all states and links for this statemachine
			List<StateID> blueprint = MagicDictionary.GetBlueprint(name, caster);

			//Initiate a state list, To hold all State objects. And build them.
			Dictionary<StateID, State> stateReferenceTable = BuildStates(blueprint, caster);

			//Build the list of links inside the states
			LinkStates(stateReferenceTable);

			//place the initial state as the first on the list
			activeState = stateReferenceTable[blueprint[0]];					
		//	lastState = activeState;

			//Message all states to wake up
			foreach (KeyValuePair<StateID,State> state in stateReferenceTable)	
				state.Value.Awake();
		
			//Message the inital state to start
			activeState.Start();	 											
		}

		/// <summary>
		/// Sets the active state, running the end step from previous state and the start step from the new one.
		/// </summary>
		/// <param name="newState">New state.</param>
		public void SetActiveState(State newState)
		{
			//If its on the debug mode, uses reflection to coloect the name of the state's owner then perform a check if that do not match with this state machine's name 
			#if DEBUG
			string newStatesOwnerName = ((StateFromAttribute)Attribute.GetCustomAttribute(newState.GetType(),typeof(StateFromAttribute),false)).name;

			if (newStatesOwnerName == name)
			{
			#endif
				//Run the END method on the dying state
				activeState.End();
			//	lastState = activeState;
				//swap states
				activeState = newState;

				//Run the Start method on the new state
				activeState.Start();
			#if DEBUG	
			}
			else
				throw new LinkNotBelongToStateMachineException("State " + newStatesOwnerName + " invalid on " + name + "' s state machine");
			#endif
		}

		/// <summary>
		/// Run this active state update and check if its links for a swap
		/// </summary>
		public void Run()
		{
			activeState.Update();
			CheckConditions();
		}

		/// <summary>
		/// Checks the active state conditions.
		/// </summary>
		private void CheckConditions()
		{
			//For each link in the active state list of links
			foreach (var link in activeState.links) 
				//execute them and if one return true
				if (link.Key())
				{	
					//set the link's target as the new state
					SetActiveState(link.Value);

					//break out of the loop
					break;
				}
		}
	}
}