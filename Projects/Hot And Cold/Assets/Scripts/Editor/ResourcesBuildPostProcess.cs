using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Apache.Core.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Apache.Core.Editor {

	/*
	 * Copies the Resources folder across to the Build folder (for Windows builds only), ignoring certain files and folders as defined by the .buildignore file.
	 * This file uses a proprietary command format, loosely based on GIT's ignore file. See the comment header in the .buildignore file for details.
	 */
	public static class ResourcesBuildPostProcess {

		//-----------------------------------------------------------------------------------------
		// Constants:
		//-----------------------------------------------------------------------------------------

		public const int EXECUTION_ORDER = 5; // give a little space for other post-processing to come in before and after us.

		private const string EXE_EXTENSION = ".exe";

		private const string BUILD_RESOURCES_PATH_FORMAT = "{0}_Data/../Resources"; // relative to 'pathToBuiltProject'.
		private const string PROJECT_RESOURCES_PATH_FORMAT = "{0}/../Resources";    // relative to Application.dataPath.

		private const string BUILD_IGNORE_FILE = ".buildignore";

		private const string BUILD_IGNORE_COMMENT_PREFIX = "//";

		private const string BUILD_IGNORE_FILETYPE_SEPARATOR = ".";
		private const string BUILD_IGNORE_DIRECTORY_SEPARATOR = "\\";

		// N.B. we expect build ignores in terms of files, so if we get a directory, we can convert it into file-specific by adding "\*".
		private const string BUILD_IGNORE_DIRECTORY_TO_FILE_SUFFIX = BUILD_IGNORE_DIRECTORY_SEPARATOR + BUILD_IGNORE_WILDCARD;

		private const string BUILD_IGNORE_WILDCARD = "*";
		private static readonly Regex BUILD_IGNORE_WILDCARD_REGEX = new Regex(@"[\w/\\ .-]*");

		private const string BUILD_IGNORE_REGEX_PREFIX = "^";
		private const string BUILD_IGNORE_REGEX_SUFFIX = "$";

		private const string BUILD_IGNORE_PARTIAL_IDENTIFIER = "~";

		//-----------------------------------------------------------------------------------------
		// Type Definitions:
		//-----------------------------------------------------------------------------------------

		public enum IgnoreTypes {
			None,    // the file is not ignored.
			Partial, // the file is only ignored if it already exists in the build location. If it doesn't exist, it is copied.
			Full     // the file is fully ignored, regardless of whether it already exists or not.
		}

		//-----------------------------------------------------------------------------------------
		// Public Methods:
		//-----------------------------------------------------------------------------------------

		[PostProcessBuild(EXECUTION_ORDER)]
		public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {

			// back out if we're building for Android or iOS.
			// N.B. not using platform-dependent compilation to avoid numerous ReSharper warnings about unused code.
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) return;

			// figure out the build plugins path by removing .exe and using format.
			// use get full path in order to resolve relative path constructs (e.g. "../").
			string buildResourcesPath = Path.GetFullPath(string.Format(BUILD_RESOURCES_PATH_FORMAT, pathToBuiltProject.Replace(EXE_EXTENSION, string.Empty)));

			// now figure out the path for project resources.
			string projectResourcesPath = Path.GetFullPath(string.Format(PROJECT_RESOURCES_PATH_FORMAT, Application.dataPath));

			// find the build ignore file.
			string ignorePath = Path.Combine(projectResourcesPath, BUILD_IGNORE_FILE);
			string[] ignoreEntries = new string[0];

			// if the ignore file exists, pull all non comment text lines to a filter array.
			if (File.Exists(ignorePath)) {
				ignoreEntries = File.ReadAllLines(ignorePath)
					.Where(l => !l.StartsWith(BUILD_IGNORE_COMMENT_PREFIX) && !l.IsNullOrWhitespace())
					.ToArray();
			}

			// with a goal to sanitise ignore entries...
			for (int i = 0; i < ignoreEntries.Length; i++) {

				// we might want to write windows locations with forward slashes instead of backslashes, so replace.
				// N.B. it would be kind of silly to separate these out into class-level constants.
				ignoreEntries[i] = ignoreEntries[i].Replace("/", BUILD_IGNORE_DIRECTORY_SEPARATOR);

				// work out if the entry represents a directory by first checking if there is no filetype.
				bool containsFile = (ignoreEntries[i].Contains(BUILD_IGNORE_FILETYPE_SEPARATOR));
				bool isDirectory  = (!containsFile);
				if (containsFile) {

					// N.B. the below assumes that the directory doesn't have a filetype separator in it, in which case this check would fail.
					// It is possible to name a directory something like "Directory.txt", but supporting this in all instances bloats complexity.
					// This limitation only affects entries which rely on automatic directory inferrence (e.g. an entry of "Directory.txt" when
					// attempting to match the above-mentioned directory). We can still support this with an entry of "Directory.txt/*".

					// now check if it's a directory by seeing if a directory separator comes after the filetype separator (or not at all).
					isDirectory = (ignoreEntries[i].LastIndexOf(BUILD_IGNORE_DIRECTORY_SEPARATOR, StringComparison.Ordinal) >
					               ignoreEntries[i].LastIndexOf(BUILD_IGNORE_FILETYPE_SEPARATOR,  StringComparison.Ordinal));
				}

				// if it's a directory, and it doesn't already have a file-specific suffix, append suffix.
				if (isDirectory && !ignoreEntries[i].EndsWith(BUILD_IGNORE_DIRECTORY_TO_FILE_SUFFIX)) {
					ignoreEntries[i] += BUILD_IGNORE_DIRECTORY_TO_FILE_SUFFIX;
				}
			}

			// find all the files in the project resources folder.
			DirectoryInfo projectResourcesDirectoryInfo = new DirectoryInfo(projectResourcesPath);
			FileInfo[] files = projectResourcesDirectoryInfo.GetFiles("*", SearchOption.AllDirectories);

			foreach (FileInfo file in files) {
				if (file.Name == BUILD_IGNORE_FILE || file.Directory == null) continue;

				// find the path relative to resources.
				string relativePath = file.FullName.Remove(0, projectResourcesPath.Length + 1);

				// find the full paths for the build and project versions of each file.
				string buildFilePath   = Path.Combine(buildResourcesPath,  relativePath);
				string projectFilePath = Path.Combine(file.Directory.Name, file.FullName);

				// work out the type of ignore this file has based on path.
				IgnoreTypes ignoreType = IsIgnored(relativePath, ignoreEntries);

				// if it's a full ignore, continue right away.
				// ReSharper disable once ConvertIfStatementToSwitchStatement
				if (ignoreType == IgnoreTypes.Full) continue;

				// if it's a partial ignore, only continue if the file does not already exist.
				// N.B. partially ignored simply means that, if the file exists already in the build location, ignore it. Otherwise copy it.
				if (ignoreType == IgnoreTypes.Partial && File.Exists(buildFilePath)) continue;

				// create the necessary directory in the build location, if it doesn't already exist.
				DirectoryInfo directoryInfo = new FileInfo(buildFilePath).Directory;
				if (directoryInfo != null) {
					Directory.CreateDirectory(directoryInfo.FullName);
				}

				// copy the file.
				File.Copy(projectFilePath, buildFilePath, true);
			}
		}

		// Determines whether the given path matches as an ignore on the given entries.
		public static IgnoreTypes IsIgnored(string relativePath, string[] ignoreEntries) {
			
			// loop through our entries...
			for (int i = 0; i < ignoreEntries.Length; i++) {

				// grab entry and work out whether it's a partial match, and if it is, make sure to remove the identifier, as it might screw with matching and regex.
				string ignoreEntry = ignoreEntries[i];
				bool isPartial = ignoreEntry.StartsWith(BUILD_IGNORE_PARTIAL_IDENTIFIER);
				if (isPartial) {
					ignoreEntry = ignoreEntry.Substring(1);
				}

				// if we have an exact match, return whether it's a partial or full ignore.
				if (relativePath == ignoreEntry) {
					return (isPartial) ? IgnoreTypes.Partial : IgnoreTypes.Full;
				}

				// if the entry doesn't have a wildcard character, continue.
				if (!ignoreEntry.Contains(BUILD_IGNORE_WILDCARD)) continue;

				// convert entry into a regex by building a string.
				StringBuilder regexStringBuilder = new StringBuilder();
				string[] ignoreEntryCharacters = ignoreEntry.ToCharArray().Select(c => c.ToString()).ToArray();
				foreach (string ignoreEntryCharacter in ignoreEntryCharacters) {

					// add an appropriate regex character for each entry character.
					switch (ignoreEntryCharacter) {
						case BUILD_IGNORE_WILDCARD:
							regexStringBuilder.Append(BUILD_IGNORE_WILDCARD_REGEX);
							break;
						default:
							regexStringBuilder.Append(Regex.Escape(ignoreEntryCharacter));
							break;
					}
				}

				// construct regex and return ignore type if we find a match.
				Regex regex = new Regex(BUILD_IGNORE_REGEX_PREFIX + regexStringBuilder + BUILD_IGNORE_REGEX_SUFFIX);
				if (regex.IsMatch(relativePath)) {
					return (isPartial) ? IgnoreTypes.Partial : IgnoreTypes.Full;
				}
			}

			// no match, so return none.
			return IgnoreTypes.None;
		}
	}
}