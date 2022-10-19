using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace SchemaRegistry
{
	public static class JsonSchemaRegistry
	{
		public static bool Validate(JObject message, string name, int version)
		{
			var schema = Schemas[name][version];
			return message.IsValid(schema);
		}

		private static readonly Dictionary<string, Dictionary<int, JSchema>> Schemas = new()
		{
			{ "RegisterUser", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(RegisterUserV1Schema),
				[2] = JSchema.Parse(RegisterUserV2Schema),
				}
			},
			{ "TaskUpdated", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(TaskCUDV1Schema),
				[2] = JSchema.Parse(TaskCUDV2Schema),
				}
			},
			{ "TaskCreated", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(TaskCUDV1Schema),
				[2] = JSchema.Parse(TaskCUDV2Schema),
				}
			},
			{ "TaskClosed", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(TaskEventV1Schema),
				[2] = JSchema.Parse(TaskEventV2Schema),
				}
			},
			{ "TaskAdded", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(TaskEventV1Schema),
				[2] = JSchema.Parse(TaskEventV2Schema),
				}
			},
			{ "TaskAssigned", new Dictionary<int, JSchema>() {
				[1] = JSchema.Parse(TaskEventV1Schema),
				[2] = JSchema.Parse(TaskEventV2Schema),
				}
			},
		};

		private const string RegisterUserV1Schema = @"{
			'description': 'A user account',
			'type': 'object',
			  'properties': {
				'publicId': {
				  'type': 'string'
				},
				'userName': {
				  'type': 'string'
				},
				'email': {
				  'type': 'string'
				},
				'role': {
				  'type': 'string'
				}
			  }
		}";

		private const string RegisterUserV2Schema = @"{
			'description': 'A user account',
			'type': 'object',
			'properties': {
				'eventId': {
				  'type': 'string'
				},
				'eventVersion': {
				  'type': 'integer'
				},
				'eventName': {
				  'type': 'string'
				},
				'eventTime': {
				  'type': 'string'
				},
				'producer': {
				  'type': 'string'
				},
				'data': {
				  'type': 'object',
				  'properties': {
					'publicId': {
					  'type': 'string'
					},
					'userName': {
					  'type': 'string'
					},
					'email': {
					  'type': 'string'
					},
					'role': {
					  'type': 'string'
					}
				  },
				}
		  }
		}";

		private const string TaskCUDV1Schema = @"{
			'description': 'A task CUD event',
			'type': 'object',
			  'properties': {
				'eventName': {
				  'type': 'string'
				},
				'publicId': {
				  'type': 'string'
				},
				'userId': {
				  'type': 'string'
				},
				'taskName': {
				  'type': 'string'
				},
				'taskDescription': {
				  'type': 'string'
				},
				'taskStatus': {
					  'type': 'integer'
				}
			  }
			}
		}";

		private const string TaskCUDV2Schema = @"{
			'description': 'A task CUD event',
			'type': 'object',
			'properties': {
				'eventId': {
				  'type': 'string'
				},
				'eventVersion': {
				  'type': 'integer'
				},
				'eventName': {
				  'type': 'string'
				},
				'eventTime': {
				  'type': 'string'
				},
				'producer': {
				  'type': 'string'
				},
				'data': {
				  'type': 'object',
				  'properties': {
					'publicId': {
					  'type': 'string'
					},
					'userId': {
					  'type': 'string'
					},
					'title': {
					  'type': 'string'
					},
					'jiraId': {
					  'type': 'string'
					},
					'taskDescription': {
					  'type': 'string'
					},
					'taskStatus': {
					  'type': 'integer'
					}
				  }
				}
  }
		}";

		private const string TaskEventV1Schema = @"{
			'description': 'A task business event',
			'type': 'object',
			  'properties': {
				'eventName': {
				  'type': 'string'
				},
				'publicId': {
				  'type': 'string'
				},
				'userId': {
				  'type': 'string'
				}
			  }
		}";

		private const string TaskEventV2Schema = @"{
			'description': 'A task business event',
			'type': 'object',
			  'properties': {
				'eventId': {
				  'type': 'string'
				},
				'eventVersion': {
				  'type': 'integer'
				},
				'eventName': {
				  'type': 'string'
				},
				'eventTime': {
				  'type': 'string'
				},
				'producer': {
				  'type': 'string'
				},
				'data': {
				  'type': 'object',
				  'properties': {
					'publicId': {
					  'type': 'string'
					},
					'userId': {
					  'type': 'string'
					}
				  }
				}
			  }
		}";
	}
}