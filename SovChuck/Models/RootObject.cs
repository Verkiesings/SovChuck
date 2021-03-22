using System.Collections.Generic;
using Newtonsoft.Json;

namespace SovChuck.Models
{
	public class RootObject
	{
		public int count { get; set; }
		public string next { get; set; }
		public object previous { get; set; }

		[JsonProperty("results")]
		public List<People> Peoples { get; set; }
	}
}
