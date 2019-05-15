using System.Reflection;

namespace StateMachine
{
	//Link value, it has a methodinfo with the link method and the state he is pointing at it
	public class LinkID
	{
		/// <summary>
		/// The method info with the link execution question.
		/// </summary>
		public MethodInfo link;

		/// <summary>
		/// The state pointed by this link.
		/// </summary>
		public StateID target;

		/// <summary>
		/// state who owns this link
		/// </summary>
		public StateID caster;

		/// <summary>
		/// Initializes a new instance of the LinkID class. 
		/// </summary>
		public LinkID(MethodInfo link, StateID target, StateID caster)
		{
			this.link = link;
			this.target = target;
			this.caster = caster;
		}
	}
}
