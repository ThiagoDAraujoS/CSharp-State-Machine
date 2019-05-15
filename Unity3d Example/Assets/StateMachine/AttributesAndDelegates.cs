using System;

namespace StateMachine
{
	/// <summary>Custon link delegate.</summary>
	public delegate bool Link();

	/// <summary> Signs the INITIAL state. </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class InitialAttribute : Attribute{}

	/// <summary> REVERSEs the link, instead of pointing to a state, it implements a state at the target pointing to the caster </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class ReverseAttribute : Attribute{}

	/// <summary> OVERRIDEs a previously declared state </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class OverrideAttribute : Attribute{}	

	/// <summary> Demarcates the method as a LINK. </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class LinkToAttribute : Attribute
	{
		public string name; 
		public LinkToAttribute(string name)	{	this.name = name;}
		public LinkToAttribute(Type name)	{	this.name = name.Name;}
	}
	
	/// <summary> Demarcates the omnilink to avoid some states. </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class MaskAttribute : Attribute
	{
		public Type[] mask;
		public MaskAttribute(params Type[] mask)	{	this.mask = mask;}
	}

	/// <summary> Demarcates the class as a state and hold the information about what state machine is its owner. </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class StateFromAttribute : Attribute
	{
		public string name;
		public StateFromAttribute(string name)	{	this.name = name;}
	}

	/// <summary>
	/// Link does not belong to current state machine exception.
	/// </summary>
	public class LinkNotBelongToStateMachineException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:LinkNotBelongToStateMachineException"/> class
		/// </summary>
		public LinkNotBelongToStateMachineException (string message) : base(message){}
	}

	/// <summary>
	/// Link with all and reverse attributes exception.
	/// </summary>
	public class LinkWithAllAndReverseAttributesException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:LinkWithAllAndReverseAttributesException"/> class
		/// </summary>
		public LinkWithAllAndReverseAttributesException (string message) : base(message){}
	}

	/// <summary>
	/// Missing initial state exception.
	/// </summary>
	public class MissingInitialStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingInitialStateException"/> class
		/// </summary>
		public MissingInitialStateException (string message) : base(message){}
	}

	/// <summary>
	/// Missing [state from] attribute exception.
	/// </summary>
	public class MissingStateFromAttributeException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingStateFromAttributeException"/> class
		/// </summary>
		public MissingStateFromAttributeException (string message) : base(message){}
	}

	/// <summary>
	/// Null state machine exception.
	/// </summary>
	public class NullStateMachineException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NullStateMachineException"/> class
		/// </summary>
		public NullStateMachineException (string message) : base(message){}
	}

	/// <summary>
	/// Missing state exception.
	/// </summary>
	public class MissingStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingStateException"/> class
		/// </summary>
		public MissingStateException (string message) : base(message){}
	}

	/// <summary>
	/// Duplicated state name exception.
	/// </summary>
	public class DuplicatedStateNameException : Exception
	{		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DuplicatedStateNameException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public DuplicatedStateNameException (string message) : base (message){}
	}
}