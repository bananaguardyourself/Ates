using Newtonsoft.Json;

namespace Contracts
{
	public class ExceptionResource
	{ 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string? Exception { get; set; }

		public string Target { get; set; }

		public string Message { get; set; }

		public string Code { get; set; }		

		public ExceptionResource(string code, string target, string message)
		{
			Code = code;
			Target = target;
			Message = message;
		}
	}
}
