using System;
using System.IO;
using FullSerializer;

namespace Apache.Core {
	public class FullSerializerSerialiser {

		//-----------------------------------------------------------------------------------------
		// Private Fields:
		//-----------------------------------------------------------------------------------------

		private static readonly fsSerializer serialiser;

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		static FullSerializerSerialiser() {
			serialiser = new fsSerializer();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		/// <summary>Deserialises the file at the given path into an object of type <c>T</c></summary>
		/// <remarks>If deserialisation fails, such as due to an issue locating a file at the given path, a default instance of type <c>T</c> is returned.</remarks>
		public T Deserialise<T>(string path) {

			// if the file doesnt exist, create a new instance and return that instead.
			if (!File.Exists(path)) return Activator.CreateInstance<T>();

			// attempt to read all data at our path into a string.
			string data;
			try {
				data = File.ReadAllText(path);
			}
			catch {
				
				// if we fail reading, such as on Android because we have no permissions, return a default instance.
				return Activator.CreateInstance<T>();
			}
			
			// if the data is not null or empty, deserialise it, otherwise create a default instance and return that.
			return (!string.IsNullOrEmpty(data)) ? DeserialiseJson<T>(data) : Activator.CreateInstance<T>();
		}

		/// <summary>Deserialises the given string into an object of type <c>T</c>.</summary>
		public T DeserialiseJson<T>(string jsonString) {
			fsData data = fsJsonParser.Parse(jsonString);

			// ReSharper disable once RedundantTypeSpecificationInDefaultExpression
			T deserialised = default(T);
			serialiser.TryDeserialize(data, ref deserialised);
			return deserialised;
		}

		/// <summary>Serialises the given object of type <c>T</c> into a JSON string.</summary>
		public string Serialise<T>(T value) {
			// ReSharper disable once InlineOutVariableDeclaration
			fsData data;
			serialiser.TrySerialize(value, out data);

			return fsJsonPrinter.PrettyJson(data);
		}
	}
}