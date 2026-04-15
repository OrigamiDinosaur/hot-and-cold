using System;
using UnityEngine;

namespace Apache.Core {

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class Title : HeaderAttribute {
		public Title(string message) : base(message) { }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class Subtitle : SubheaderAttribute {
		public Subtitle(string message) : base(message) { }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class Description : TooltipAttribute {
		public Description(string message) : base(message) { }
	}
}