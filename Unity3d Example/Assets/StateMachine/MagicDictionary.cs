using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using UnityEngine;
using Blueprint = System.Collections.Generic.List<StateMachine.StateID>;
namespace StateMachine
{
	/// <summary>
	/// The magic dictionary is mixture between a data structure and a scaner. 
	/// When it is called: 
	///		If it does not have the information required it'll scan the caller's class in search for information related to state machines. 
	///		Then it will save the information.
	/// Each entry on the dictionary holds a statemachine name and a caster name, 
	/// each of those entries have types and methodinfos lists containing information on how to assembly that state machine. 
	/// The engine object will use this page to build the state machine and put it running.
	/// </summary>
	public static class MagicDictionary
	{
		///Dictionary id, it got a state machine name and a who built it
		private class MagicDictionaryKeyID
		{
			/// <summary>
			/// The name of the state machine.
			/// </summary>
			public string name;
			
			/// <summary>
			/// The class who built this state machine.
			/// </summary>
			public Type baseCasterType;
			
			/// <summary>verify if this object is equal to another.</summary>
			/// <returns><c>true</c>, true if it is equal, <c>false</c> otherwise.</returns>
			public bool isEqual (MagicDictionaryKeyID obj)
			{	
				return (name == obj.name && baseCasterType == obj.baseCasterType);
			}
			
			/// <summary>
			/// Initializes a new instance of the magic dictionary id <see cref="StateMachine.Engine+MagicDictionaryKeyID"/> class.
			/// </summary>
			public MagicDictionaryKeyID(string name, Type casterType)
			{
				this.name = name;
				this.baseCasterType = casterType;
			}
		}
		
		/// <summary>
		/// The magic dictionary. with all state machies architectures blueprints
		/// </summary>
	//	private static Dictionary<MagicDictionaryKeyID, List<StateID>> magicDictionary;
		
		private static Dictionary<MagicDictionaryKeyID, Blueprint> blueprintSet;

        /// <summary>Fills the magic dictionary link lists.</summary>
        /// <param name="id">Identifier in the magic dictionary to fill its states.</param>
        private static void FillMagicDictionaryLinkLists(MagicDictionaryKeyID pageId)
		{
			//For each state at the magic dictionary page
			foreach (StateID mainState in blueprintSet[pageId]) 										
			{
				//For each method that the state has
				foreach (MethodInfo link in mainState.stateType.GetMethods())																
				{	
					//Save the link LintToAttribute of that method for further analysis
					LinkToAttribute linkTo = (LinkToAttribute)Attribute.GetCustomAttribute(link,typeof(LinkToAttribute));
					

					//If that method uses a LinkToAttribute
					if (linkTo != null)									
					{
						//If the attribute uses the codename "All"
						if (linkTo.name == "All")													
						{
							//If the link also uses the REVERSE attribute throw an error!
							if (link.GetCustomAttributes(typeof(ReverseAttribute),false).Length >= 1)				
								throw new LinkWithAllAndReverseAttributesException("LINKs with All tag can't have the [Reverse] tag");					

							//Initiate a mask array
							MaskAttribute[] masks = (MaskAttribute[])link.GetCustomAttributes(typeof(MaskAttribute),false);						
							Type[] mask = (masks.Length>=1)? masks[0].mask : new Type[0];
							
							//Foreach unmasked state at the page Implements a REVERSE link pointing at the current state	
							foreach (StateID targetState in blueprintSet[pageId])   							
								if  (!mask.Contains(targetState.stateType))	
									targetState.LinkList.Add(new LinkID(link, mainState, mainState));
								//	omniReceiverState.LinkList.Add(new LinkID(link, mainState, mainState));						
						}
						else {
							StateID targetState = SearchStateWithName(pageId, linkTo.name);

							//Else if the link uses the REVERSE tag Implements a REVERSE link pointing to the current state
							if (link.GetCustomAttributes(typeof(ReverseAttribute),false).Length >= 1)			
								targetState.LinkList.Add(new LinkID(link, mainState, mainState ));		
								//targetState.LinkList.Add (new LinkID(link, mainState, mainState ));								
						
							//Else the link does not use any special tag Implements a common link
							else 																					
								mainState.LinkList.Add(new LinkID(link, targetState, mainState));
						}								
					}
				}
			}
		}
		
		/// <summary>Searchs the state with the given name </summary>
		/// <returns>The state with name.</returns>
		/// <param name="id">Identifier in the magic dictionary.</param><param name="name">Name to be search.</param>
		private static StateID SearchStateWithName (MagicDictionaryKeyID pageId, string name)
		{
			foreach (StateID state in blueprintSet[pageId])
				if (state.stateType.Name == name)
					return state;
			
			throw new MissingStateException("State with missing name... " + name);
		}
		
		/// <summary>Gets a blueprint in the magic dictionary.</summary>
		/// <returns>The blueprint in magic dictionary.</returns>
		/// <param name="stateMachineName">The state machine's name</param>
		/// <param name="stateMachineOwner">The state machine's owner</param>
		public static Blueprint GetBlueprint(string stateMachineName, Object stateMachineOwner)
		{
			//Creates an indentifier obj
			MagicDictionaryKeyID blueprintId = new MagicDictionaryKeyID(stateMachineName,stateMachineOwner.GetType());

			//the blueprint variable that will be returned at the end of the method
			Blueprint blueprint;		

			//initialize the set if it doesnt exist already
			if (blueprintSet == null)								
					blueprintSet  =  new Dictionary<MagicDictionaryKeyID, Blueprint>();

			//If there's no blueprint related in it
			if (!MagicDictionaryContains(blueprintId))					
			{
				//Get a state list and save it
				blueprint = BuildBlueprintContent(blueprintId);			
				
				//annex that blueprint into the dictionary
				AddBlueprint(blueprintId,blueprint);					
			}
			//If the blueprint does exists in the dictionary
			else 	
				//return that blueprint											
				blueprint = blueprintSet[blueprintId];		

			//return the return variable
			return blueprint;											
		}
		
		/// <summary>Adds A page in the magic dictionary.</summary>
		/// <returns><c>true</c>, if A page in magic dictionary was added, <c>false</c> otherwise.</returns>
		/// <param name="id">Page identifier.</param> param name="stateList">Page content.</param>
		private static void AddBlueprint (MagicDictionaryKeyID blueprintId, Blueprint blueprint)
		{
			//If there are states at the state list
			if (blueprint.Count > 0)
			{
				//Add it to the dictionary
				blueprintSet.Add (blueprintId, blueprint);			
				
				//Fill the states's LINK lists
				FillMagicDictionaryLinkLists(blueprintId);			
			}
			else 										
				//if the state list doesnt cointain any nodes
				throw new NullStateMachineException("State machine with the name " + blueprintId.name + " doesn't contain any state... have you mistaken its name?"); 
		}
		
		///<summary> If the dictionary contains. </summary>
		/// <returns><c>true</c>, if dictionary contains the page with the identifier, <c>false</c> otherwise.</returns>
		private static bool MagicDictionaryContains(MagicDictionaryKeyID soughtPage)
		{
			bool result = false;
			
			for (int i = 0; i < blueprintSet.Count && !result; i++)
				result = blueprintSet.ElementAt(i).Key.isEqual(soughtPage);

			return result;
		}
		
		/// <summary>Builds a page content getting a reflection from caster's type and build an avaliable state list </summary>
		/// <returns>The state list.</returns>
		/// <param name="name">State machine name.</param> <param name="caster">User's caster.</param> <param name="initialState">Initial state.</param> <typeparam name="T">User's type.</typeparam>
		private static Blueprint BuildBlueprintContent(MagicDictionaryKeyID pageId)
		{
			List<StateID> stateList = new List<StateID>();				//The returned list
			List<StateID> overrideStateList = new List<StateID>();		//Override State list
			bool initialStateNotFound = true;						//Control if the [INITIAL] tag was used properly 
			bool wasNotOverriden = true;							//Control if the state was or not overriden
			
			//For each class in the hierarchy 
			for (Type baseType = pageId.baseCasterType; baseType != null; baseType = baseType.BaseType)			
			{
				//for each nested class
				foreach (Type state in baseType.GetNestedTypes(BindingFlags.Public))								
				{
					//verify if it is a state class
					if (state.IsSubclassOf(typeof(State)))
					{	
						//save the "state from attribute" for further validation
						StateFromAttribute attribute = (StateFromAttribute)Attribute.GetCustomAttribute(state,typeof(StateFromAttribute),false);
						
						//verify if there's no attribute here and throw an error
						if (attribute == null) 	
							throw new MissingStateFromAttributeException("State " + state.FullName + " without verification tag [StateFrom(string name)]!!!");	

						//verify if the actual state has the same state machine name as the dictionary page
						else if (attribute.name == pageId.name)																	
						{


							//Set wasNotOverriden as true
							wasNotOverriden = true;																				

							//for each state at the state's list that was not overriden
							foreach (StateID overridenState in overrideStateList)

								//if the state has its name on overideStateList
								if (overridenState.stateType.Name == state.Name)

									//set this as overriden and jump further steps
									wasNotOverriden = false;



							//if it was not overriden
							if (wasNotOverriden)																					
							{
								//if there is the [override] tag go to the overrideStateList and keep going
								if (state.GetCustomAttributes(typeof(OverrideAttribute),false).Length >= 1)								
									overrideStateList.Add (new StateID(state, baseType));	

								//If the [INITIAL] tag was not found yet and this one have the [INITIAL] tag, place the current node as the first of the list place the current node as the first of the list
								if (initialStateNotFound && state.GetCustomAttributes(typeof(InitialAttribute),false).Length >= 1)
								{
									stateList.Insert(0,new StateID(state, baseType));
									initialStateNotFound = false;			
								}
								//If there is no special tags go to stateList
								else 																								
									stateList.Add (new StateID(state, baseType));																

							}
						}
					}
				}
			}
			//If the initial state was not found at the end throw and error
			if (initialStateNotFound)																				
				throw new MissingInitialStateException("State machine with the name \""+pageId.name +"\" does not have a [Initial] tag");	

			return stateList;
		}
	}
}
