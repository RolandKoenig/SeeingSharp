#region License information (SeeingSharp and all based games/applications)
/*
    SeeingSharp and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2015 Roland König (RolandK)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using SeeingSharp.Infrastructure;

#if UNIVERSAL
using Windows.Storage;
#endif

namespace SeeingSharp.Util
{
    public static partial class CommonTools
    {
#if DESKTOP
        public static T ReadPrivateMember<T, U>(U sourceObject, string memberName)
        {
            FieldInfo fInfo = typeof(U).GetTypeInfo().GetField(memberName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)fInfo.GetValue(sourceObject);
        }
#endif

        /// <summary>
        /// Gets the backing array of the given list.
        /// </summary>
        /// <param name="lst">The list from which to get the backing array for faster loop access.</param>
        public static T[] GetBackingArray<T>(List<T> lst)
        {
#if !WINRT && !UNIVERSAL
            FieldInfo fInfo = lst.GetType().GetTypeInfo().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            return fInfo.GetValue(lst) as T[];
#else
            return lst.ToArray();
#endif
        }

        /// <summary>
        /// Gets the backing array of the given queue.
        /// </summary>
        /// <param name="queue">The queue from which to get the backing array for faster loop access.</param>
        public static T[] GetBackingArray<T>(Queue<T> queue)
        {
#if !WINRT && !UNIVERSAL
            FieldInfo fInfo = queue.GetType().GetTypeInfo().GetField("_array", BindingFlags.NonPublic | BindingFlags.Instance);
            return fInfo.GetValue(queue) as T[];
#else
            return queue.ToArray();
#endif

        }

        /// <summary>
        /// Calls the given function asynchronous.
        /// </summary>
        /// <param name="funcToExecute">The function to execute.</param>
        public static Task<T> CallAsync<T>(Func<T> funcToExecute)
        {
            return Task.Factory.StartNew(funcToExecute);
        }

        /// <summary>
        /// Formats the given timespan to a compact string.
        /// </summary>
        /// <param name="timespan">The Tiemspan value to be formated.</param>
        public static string FormatTimespanCompact(TimeSpan timespan)
        {
            return
                Math.Floor(timespan.TotalHours).ToString("F0") + ":" +
                timespan.Minutes.ToString().PadLeft(2, '0') + ":" +
                timespan.Seconds.ToString().PadLeft(2, '0') + ":" +
                timespan.TotalMilliseconds.ToString().PadLeft(3, '0');
        }

        /// <summary>
        /// Performs a real delay (sleeps a shorter time and does a dummy loop after then).
        /// </summary>
        /// <param name="delayMilliseconds">Total delay time.</param>
        public static async Task MaximumDelayAsync(double delayMilliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Ensure initial short delay
            if (delayMilliseconds > 5.0) { await Task.Delay(2); }

            // Do short delay pahses until we reach a point where we are "near" the target value
            while(stopwatch.GetTrueElapsedMilliseconds() < delayMilliseconds - 10.0)
            {
                await Task.Delay(2);
            }
        }

        /// <summary>
        /// Performs a real delay (uses a manual reset event for waiting..).
        /// </summary>
        /// <param name="delayMilliseconds">Total time for delay.</param>
        public static void MaximumDelay(double delayMilliseconds)
        {
            using(var waitHandle = new System.Threading.ManualResetEvent(false))
            {
                waitHandle.WaitOne((int)delayMilliseconds);
            }
        }

        /// <summary>
        /// Gets the language key from the given file name;
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        public static SeeingSharpLanguageKey GetLanguageKeyFromFileName(string fileName)
        {
            string[] splittedFileName = fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedFileName.Length < 3) { return SeeingSharpLanguageKey.Default; }

            // normaly the format is <filename>.<language>.<extension>
            string langKey = splittedFileName[splittedFileName.Length - 2]
                .ToLower();
            if (!langKey.StartsWith("lang-")) { return SeeingSharpLanguageKey.Default; }

            // Try to parse the language key
            SeeingSharpLanguageKey result = SeeingSharpLanguageKey.Default;
            Enum.TryParse<SeeingSharpLanguageKey>(langKey.Substring(5).ToUpper(), out result);
            return result;
        }

        /// <summary>
        /// Gets the language file category by the given file name.
        /// This file format is expected: [category].[lang-[Key]].langXml
        /// </summary>
        /// <param name="fileName">The file name from which to get the category.</param>
        public static string GetLanguageFileCategory(string fileName)
        {
            // Check for correct file extension
            if(!fileName.EndsWith(SeeingSharpConstants.FILE_EXTENSION_LANG_WORDS))
            {
                throw new SeeingSharpException("Invalid format of language file (filename: " + fileName + ")!");
            }

            // Split the file name
            string[] splittedFileName = fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string result = string.Empty;
            if (splittedFileName.Length < 2) { throw new SeeingSharpException("Invalid format of language file (filename: " + fileName + ")!"); }
            else if(splittedFileName.Length == 2)
            {
                // Assuming first is file name and second is extension
                result = splittedFileName[0];
            }
            else
            {
                if (splittedFileName[splittedFileName.Length - 2].StartsWith("lang-"))
                {
                    result = splittedFileName[splittedFileName.Length - 3];
                }
                else
                {
                    result = splittedFileName[splittedFileName.Length - 2];
                }
            }

            // Check the result string
            if(string.IsNullOrEmpty(result))
            {
                throw new SeeingSharpException("No category defined by language file (filename=" + fileName + ")!");
            }

            return result.ToUpper();
        }

#if UNIVERSAL
        /// <summary>
        /// Deserializes an object of the given type from the given storage file.
        /// </summary>
        /// <param name="storagefile">The file to deserialize from.</param>
        public static async Task<T> DeserializeFromXmlFile<T>(StorageFile storagefile)
            where T : class
        {
            try
            {
                //Open file for reading
                using (Stream inStream = await storagefile.OpenStreamForReadAsync())
                {
                    //Create the serializer
                    XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                    //Deserialize the object and return the result
                    return await Task.Factory.StartNew(() => serializer.Deserialize(inStream) as T);
                }
            }
            catch (FileNotFoundException) { }

            //Return default value if something went wrong
            return default(T);
        }

        /// <summary>
        /// Serializes the given object to the given storage file.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="storageFile">The target file.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        public static async Task SerializeToXmlFile<T>(StorageFile storageFile, T objectToSerialize)
        {
            using (Stream outStream = await storageFile.OpenStreamForWriteAsync())
            {
                //Create the serializer
                XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                await Task.Factory.StartNew(() => serializer.Serialize(outStream, objectToSerialize));

                return;
            }
        }
#endif

#if DESKTOP
        /// <summary>
        /// Deserializes an object of the given type from the given storage file.
        /// </summary>
        /// <param name="storagefile">The file to deserialize from.</param>
        public static async Task<T> DeserializeFromXmlFile<T>(string storagefile)
            where T : class
        {
            try
            {
                //Open file for reading
                using (Stream inStream = File.OpenRead(storagefile))
                {
                    //Create the serializer
                    XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                    //Deserialize the object and return the result
                    return await Task.Factory.StartNew(() => serializer.Deserialize(inStream) as T);
                }
            }
            catch (FileNotFoundException) { }

            //Return default value if something went wrong
            return default(T);
        }

        /// <summary>
        /// Serializes the given object to the given storage file.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="storageFile">The target file.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        public static async Task SerializeToXmlFile<T>(string storageFile, T objectToSerialize)
        {
            using (Stream outStream = new FileStream(storageFile, FileMode.Create, FileAccess.Write))
            {
                // Create the serializer
                XmlSerializer serializer = await Task.Factory.StartNew(() => new XmlSerializer(typeof(T)));

                await Task.Factory.StartNew(() => serializer.Serialize(outStream, objectToSerialize));

                return;
            }
        }
#endif

        /// <summary>
        /// Inserts the given object using a binary search algorithm.
        /// </summary>
        /// <param name="targetList">The list to be modified.</param>
        /// <param name="newItem">The new item to be inserted.</param>
        public static void BinaryInsert<T>(List<T> targetList, T newItem)
        {
            int targetIndex = targetList.BinarySearch(newItem);
            if(targetIndex <= 0)
            {
                targetList.Insert(~targetIndex, newItem);
            }
        }

        /// <summary>
        /// Removes the given object using binary search algorithm.
        /// </summary>
        /// <param name="targetList">The target list to be modified.</param>
        /// <param name="toRemove">The object to be removed.</param>
        public static void BinaryRemove<T>(List<T> targetList, T toRemove)
        {
            int targetIndex = targetList.BinarySearch(toRemove);
            if (targetIndex >= 0)
            {
                targetList.RemoveAt(targetIndex);
            }
        }

        /// <summary>
        /// Disposes the given object.
        /// </summary>
        public static void SafeDispose<T>(ref T toDispose)
            where T : class, IDisposable
        {


            toDispose = DisposeObject(toDispose);
        }

        /// <summary>
        /// Disposes the given object.
        /// </summary>
        public static void SafeDisposeLazy<T>(ref Lazy<T> toDispose)
            where T : class, IDisposable
        {
            toDispose = DisposeObjectLazy(toDispose);
        }

        /// <summary>
        /// Disposes the given object and returns null.
        /// </summary>
        public static T DisposeObject<T>(T objectToDispose)
            where T : class, IDisposable
        {
            if (objectToDispose == null) { return null; }

            try { objectToDispose.Dispose(); }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Disposes the given lazy object (if created already).
        /// </summary>
        /// <param name="objectToDispose">The object to be disposed.</param>
        public static Lazy<T> DisposeObjectLazy<T>(Lazy<T> objectToDispose)
            where T : class, IDisposable
        {
            if (objectToDispose == null) { return null; }
            if (!objectToDispose.IsValueCreated) { return null; }

            DisposeObject(objectToDispose.Value);
            return null;
        }

        /// <summary>
        /// Disposes all objects within the given enumeration.
        /// </summary>
        /// <param name="enumeration">Enumeration containing all disposable objects.</param>
        public static void DisposeObjects<T>(IEnumerable<T> enumeration)
            where T : class, IDisposable
        {
            if (enumeration == null) { throw new ArgumentNullException("enumeration"); }

            foreach (T actItem in enumeration)
            {
                DisposeObject(actItem);
            }
        }

        /// <summary>
        /// Raises an unhandled exception.
        /// </summary>
        /// <param name="sourceType">The source class where the exception was catched.</param>
        /// <param name="sourceObject">The source object where the exception was catched.</param>
        /// <param name="ex">The exception object itself</param>
        /// <param name="methodDescription">A short description what the method does exactly.</param>
        /// <param name="category">The category of the exception.</param>
        public static void RaiseUnhandledException(
            Type sourceType,
            object sourceObject, 
            Exception ex, 
            string methodDescription = "",
            ExceptionCategory category = ExceptionCategory.NonCritical)
        {

        }
    }
}
