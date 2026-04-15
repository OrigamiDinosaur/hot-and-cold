using System;
using System.IO;
using UnityEngine;

namespace Apache.Core {
	public abstract class SerialisedElement {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		protected const string DEFAULT_FILENAME = "Abstract Settings Element";
		protected const string FILE_EXTENSION = ".txt";

		protected const string DEFAULT_DIRECTORY_NAME = "Serialised Data";

		protected const string DEFAULT_RELATIVE_DIRECTORY_PATH = "../Resources/" + DEFAULT_DIRECTORY_NAME;
		protected const string PERSISTENT_RELATIVE_DIRECTORY_PATH = "Resources/" + DEFAULT_DIRECTORY_NAME;

		//-----------------------------------------------------------------------------------------
		// Protected Fields:
		//-----------------------------------------------------------------------------------------

		protected static FullSerializerSerialiser Serialiser;

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		public abstract object BaseData { get; }

		public virtual string Filename => DEFAULT_FILENAME;

		public virtual string RelativeDirectoryPath => (IsPersistent) ? PERSISTENT_RELATIVE_DIRECTORY_PATH : DEFAULT_RELATIVE_DIRECTORY_PATH;

		protected string DirectoryPath => Path.Combine((IsPersistent) ? Application.persistentDataPath : Application.dataPath, RelativeDirectoryPath);

		protected string FullPath => Path.Combine(DirectoryPath, Filename + FILE_EXTENSION);

		/// <summary>Persistence is the property that the underlying file is maintained in user AppData across application installs and uninstalls.</summary>
		protected bool IsPersistent { get; set; }

		protected bool DirectoryExists => Directory.Exists(DirectoryPath);

		protected bool FileExists => File.Exists(FullPath);

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		protected SerialisedElement() {
			if (Serialiser != null) return;
			Serialiser = new FullSerializerSerialiser();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public abstract void Save();

		public abstract void Reload();

		//-----------------------------------------------------------------------------------------
		// Private Methods:
		//-----------------------------------------------------------------------------------------

		protected bool ValidateDirectory() {
			if (DirectoryExists) return true;
			try {
				Directory.CreateDirectory(DirectoryPath);
				return true;
			}
			catch {
				return false;
			}
		}
	}

	public class SerialisedElement<T> : SerialisedElement {

		//-----------------------------------------------------------------------------------------
		// Public Properties:
		//-----------------------------------------------------------------------------------------

		public override object BaseData => Data;
		public T Data { get; private set; }

		//-----------------------------------------------------------------------------------------
		// Protected Properties:
		//-----------------------------------------------------------------------------------------

		public override string Filename { get; }

		public override string RelativeDirectoryPath { get; }

		//-----------------------------------------------------------------------------------------
		// Constructors:
		//-----------------------------------------------------------------------------------------

		public SerialisedElement() {
			Filename = typeof(T).Name;
			Load();
		}

		/// <summary>Creates a new serialised element and loads it.</summary>
		/// <param name="filename">The filename excluding extension.</param>
		/// <param name="defaultRelativeDirectoryPath">The directory path relative to the default data path.</param>
		/// <param name="isPersistent">Whether to use a persistent data path.</param>
		/// <param name="persistentRelativeDirectoryPath">The directory path relative to the persistent data path.</param>
		/// <remarks>
		/// On Android, the persistent data path is the only safe place to save files, so we will always use
		/// <c>persistentRelativeDirectoryPath</c> on that platform.
		/// </remarks>
		public SerialisedElement(string filename, string defaultRelativeDirectoryPath = DEFAULT_RELATIVE_DIRECTORY_PATH,
			bool isPersistent = false, string persistentRelativeDirectoryPath = PERSISTENT_RELATIVE_DIRECTORY_PATH) {

			Filename = filename;

			// if we're persistent and not in the editor, or on Android or Lumin and not in the editor, use persistent path for directory path.
			// N.B. not supporting persistence in the editor is mostly so that the team can share default user settings via source control.
			// N.B. the persistent data path is the only safe place we can save files on Android and Lumin, but note it is not truely persistent;
			// files are deleted after uninstall.
			IsPersistent = ((isPersistent || Application.platform == RuntimePlatform.Android)
			                && !Application.isEditor);
			RelativeDirectoryPath = (IsPersistent) ? persistentRelativeDirectoryPath : defaultRelativeDirectoryPath;

			// if we're on Android or Lumin and not in the editor.
			if ((Application.platform == RuntimePlatform.Android) && !Application.isEditor) {

				// N.B. on Android/Lumin, we want the serialised element to always be represented on disk so if it doesn't exist...
				if (!File.Exists(FullPath)) {

					// create default data and save it, so that it's there for editing later.
					// N.B. this has the same affect as loading when the file can't be found.
					Data = Activator.CreateInstance<T>();
					Save();
					return;
				}

				// file exists, so load and return.
				Load();
				return;
			}

			// if we're persistent, see if the persisted file exists, and if it doesn't...
			if (IsPersistent && !FileExists) {

				// validate and create the persistent directory if necessary.
				ValidateDirectory();

				// grab full persistent path.
				string persistentPath = FullPath;

				// act not persistent so we can get the non-persistent full path.
				IsPersistent = false;
				RelativeDirectoryPath = defaultRelativeDirectoryPath;
				File.Copy(FullPath, persistentPath);

				// reset persistence and relative directory path so it points to persistent path.
				IsPersistent = true;
				RelativeDirectoryPath = persistentRelativeDirectoryPath;
			}

			Load();
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		public sealed override void Save() {

			// serialise data into a string, and back out if it ends up empty.
			string dataString = Serialiser.Serialise(Data);
			if (string.IsNullOrEmpty(dataString)) return;

			// if we fail to validate (or create) the directory, just return.
			// N.B. this can happen due to an Android permissions issue, for example, and if it does, it's fine, because when we
			// come to load, we'll just return another default instance.
			if (!ValidateDirectory()) return;

			// attempt to write the data string to the full path.
			try {
				File.WriteAllText(FullPath, dataString);
			}
			catch {

				// N.B. we ignore a failure to write (which may occur due to an Android permissions error, for example) because
				// when we come to load again, we'll just get another default instance, which is fine.
			}
		}

		public override void Reload() {
			Load();
		}

		//-----------------------------------------------------------------------------------------
		// Protected Methods:
		//-----------------------------------------------------------------------------------------

		protected void Load() {
			Data = Serialiser.Deserialise<T>(FullPath);
		}
	}
}