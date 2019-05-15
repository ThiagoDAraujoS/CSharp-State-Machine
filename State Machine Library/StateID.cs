using System;
using System.Collections.Generic;

namespace StateMachine
{
	//Dictionary value, it got a statetype and who has it nested
	public class StateID
	{
		/// <summary>
		/// The type of the state.
		/// </summary>
		public Type stateType;
		
		/// <summary>
		/// Who has the stateType nested in its architecture
		/// </summary>
		public Type nestedClassHolder;
		
		/// <summary>
		/// The link with other states's list.
		/// </summary>
		public List<LinkID> LinkList;
		
		/// <summary>
		///verify if this object is equal to another.
		///</summary>
		/// <returns><c>true</c>, true if it is equal, <c>false</c> otherwise.</returns>
		public bool isEqual (StateID obj)
		{
			return (stateType == obj.stateType && nestedClassHolder == obj.nestedClassHolder);
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachine.Engine2+StateID"/> class.
		/// </summary>
		public StateID(Type stateType, Type nestedClassHolder)
		{
			this.stateType = stateType;
			this.nestedClassHolder = nestedClassHolder;
			this.LinkList = new List<LinkID>();
		}
	}	
}
