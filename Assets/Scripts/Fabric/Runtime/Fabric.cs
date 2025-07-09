using Fabric.Runtime.Internal;
using System;
using System.Reflection;
using UnityEngine;

namespace Fabric.Runtime
{
	public class Fabric
	{
		private static readonly Impl impl;

		static Fabric()
		{
			impl = Impl.Make();
		}
	}
}
