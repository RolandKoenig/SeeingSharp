#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
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
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// Some namespace mappings
#if DESKTOP
using System.Windows.Media.Imaging;
using System.Windows;
using WinForms = System.Windows.Forms;
using GDI = System.Drawing;
#endif

#if WINRT || UNIVERSAL
using Windows.System.Threading;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.Storage;
#endif

namespace SeeingSharp.Util
{
    /// <summary>
    /// Some common extension methods used in most coding of SeeingSharp.
    /// </summary>
    public static partial class CommonExtensions
    {
        private static Dictionary<System.Threading.Timer, object> s_timerDict;
        private static object s_timerDictLock;

        /// <summary>
        /// Initializes the <see cref="CommonExtensions" /> class.
        /// </summary>
        static CommonExtensions()
        {
            s_timerDict = new Dictionary<System.Threading.Timer, object>();
            s_timerDictLock = new object();
        }

#if DESKTOP
        /// <summary>
        /// Writes the given bitmap to the desktop.
        /// </summary>
        /// <param name="bitmap">The bitmap to write to the desktop.</param>
        /// <param name="fileName">The target file name.</param>
        public static void DumpToDesktop(this GDI.Bitmap bitmap, string fileName)
        {
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            bitmap.Save(Path.Combine(desktopDir, fileName));
        }
#endif

        public static void CreateDummyObjectUntilCapacityReached<T>(this List<T> list, int targetCapacity)
        {
            if (list.Capacity < targetCapacity) { list.Capacity = targetCapacity; }
            while(list.Count < targetCapacity)
            {
                list.Add(default(T));
            }
        }

        /// <summary>
        /// Gets most exact value of elapsed milliseconds from the given stopwatch.
        /// </summary>
        /// <param name="stopwatch">The stopwatch to get elapsed milliseconds from.</param>
        public static double GetTrueElapsedMilliseconds(this Stopwatch stopwatch)
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Gets most exact value of elapsed milliseconds from the given stopwatch.
        /// </summary>
        /// <param name="stopwatch">The stopwatch to get elapsed milliseconds from.</param>
        public static long GetTrueElapsedMillisecondsRounded(this Stopwatch stopwatch)
        {
            return (long)Math.Round(stopwatch.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Sets all values of the given array to the given value.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="newValue">The new value.</param>
        public static void SetAllValuesTo<T>(this T[] array, T newValue)
        {
            for(int loop=0 ; loop<array.Length; loop++)
            {
                array[loop] = newValue;
            }
        }

        /// <summary>
        /// Determines whether the specified collection contains the given string.
        /// </summary>
        /// <param name="collection">The collection to be searched for the gien string.</param>
        /// <param name="compareString">The string used for comparison.</param>
        /// <param name="comparison">The comparison mode.</param>
        public static bool ContainsString(this IEnumerable<string> collection, string compareString, StringComparison comparison = StringComparison.CurrentCulture)
        {
            foreach(string actString in collection)
            {
                if (string.Equals(actString, compareString, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts the given collection to a comma separated string (e. g. object1, object2, object3, ...).
        /// The ToString method is used to get the strings for each individual object.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="collection">The collection to format.</param>
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> collection)
        {
            return collection.ToCommaSeparatedString(", ");
        }

        /// <summary>
        /// Converts the given collection to a comma separated string (e. g. object1, object2, object3, ...).
        /// The ToString method is used to get the strings for each individual object.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="collection">The collection to format.</param>
        /// <param name="separator">A custom separator string.</param>
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> collection, string separator)
        {
            StringBuilder readMessageBuilder = new StringBuilder();
            foreach (T actObject in collection)
            {
                if (readMessageBuilder.Length > 0) { readMessageBuilder.Append(separator); }
                readMessageBuilder.Append("" + actObject);
            }
            return readMessageBuilder.ToString();
        }

        /// <summary>
        /// Converts the given collection to a comma separated string (e. g. object1, object2, object3, ...).
        /// The given toStringFunc method is used to get the strings for each individual object.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="collection">The collection to format.</param>
        /// <param name="toStringFunc">To string function.</param>
        /// <returns></returns>
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> collection, Func<T, string> toStringFunc)
        {
            return collection.ToCommaSeparatedString(toStringFunc, ", ");
        }

        /// <summary>
        /// Converts the given collection to a comma separated string (e. g. object1, object2, object3, ...).
        /// The given toStringFunc method is used to get the strings for each individual object.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="collection">The collection to format.</param>
        /// <param name="toStringFunc">To string function.</param>
        /// <param name="separator">A custom separator string.</param>
        /// <returns></returns>
        public static string ToCommaSeparatedString<T>(this IEnumerable<T> collection, Func<T, string> toStringFunc, string separator)
        {
            StringBuilder readMessageBuilder = new StringBuilder();
            foreach (T actObject in collection)
            {
                if (readMessageBuilder.Length > 0) { readMessageBuilder.Append(separator); }
                readMessageBuilder.Append("" + toStringFunc(actObject));
            }
            return readMessageBuilder.ToString();
        }

        /// <summary>
        /// Gets the backing array of the given list.
        /// </summary>
        /// <param name="lst">The list from which to get the backing array for faster loop access.</param>
        public static T[] GetBackingArray<T>(this List<T> lst)
        {
            return CommonTools.GetBackingArray(lst);
        }

        /// <summary>
        /// Gets the backing array of the given queue.
        /// </summary>
        /// <param name="queue">The queue from which to get the backing array for faster loop access.</param>
        public static T[] GetBackingArray<T>(this Queue<T> queue)
        {
            return CommonTools.GetBackingArray(queue);
        }

        public static T[] Subset<T>(this T[] givenArray, int startIndex, int count)
        {
            T[] result = new T[count];
            for (int loop = 0; loop < count; loop++)
            {
                result[loop] = givenArray[startIndex + loop];
            }
            return result;
        }

        /// <summary>
        /// Raises the given event.
        /// </summary>
        /// <param name="eventHandler">The event to be raised.</param>
        /// <param name="sender">The sender parameter.</param>
        /// <param name="eventArgs">The event args parameter.</param>
        public static void Raise(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            if (eventHandler != null) { eventHandler(sender, eventArgs); }
        }

        /// <summary>
        /// Raises the given event.
        /// </summary>
        /// <param name="eventHandler">The event to be raised.</param>
        /// <param name="sender">The sender parameter.</param>
        /// <param name="eventArgs">The event args parameter.</param>
        public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T eventArgs)
        {
            if (eventHandler != null) { eventHandler(sender, eventArgs); }
        }

#if UNIVERSAL
        /// <summary>
        /// Gets the file with the given name or returns null if the file does not exist.
        /// </summary>
        /// <param name="storageFolder">The folder from which to get the file.</param>
        /// <param name="fileName">The name of the file.</param>
        public static async Task<StorageFile> GetOrReturnNull(this StorageFolder storageFolder, string fileName)
        {
            try
            {
                return await storageFolder.GetFileAsync(fileName);
            }
            catch (FileNotFoundException) { }

            return null;
        }
#endif

#if DESKTOP
        /// <summary>
        /// Gets the pixel size of the given UIElement.
        /// </summary>
        /// <param name="uiElement">The UIElement to get the pixel size from.</param>
        /// <param name="minSize">The minimum size to be returned.</param>
        public static Size GetPixelSize(this UIElement uiElement, Size minSize)
        {
            PresentationSource source = PresentationSource.FromVisual(uiElement);
            double dpiScaleFactorX = 1.0;
            double dpiScaleFactorY = 1.0;
            if (source != null)
            {
                dpiScaleFactorX = source.CompositionTarget.TransformToDevice.M11;
                dpiScaleFactorY = source.CompositionTarget.TransformToDevice.M22;
            }

            return new Size(
                Math.Max(uiElement.RenderSize.Width * dpiScaleFactorX, 100),
                Math.Max(uiElement.RenderSize.Height * dpiScaleFactorY, 100));
        }

        /// <summary>
        /// Transporms the given wpf point to pure pixel coordinates.
        /// </summary>
        /// <param name="uiElement">The ui element on which to transform the coordinate.</param>
        /// <param name="wpfPoint">The wpf point to be transformed.</param>
        public static Point GetPixelLocation(this UIElement uiElement, System.Windows.Point wpfPoint)
        {
            PresentationSource source = PresentationSource.FromVisual(uiElement);
            double dpiScaleFactorX = 1.0;
            double dpiScaleFactorY = 1.0;
            if (source != null)
            {
                dpiScaleFactorX = source.CompositionTarget.TransformToDevice.M11;
                dpiScaleFactorY = source.CompositionTarget.TransformToDevice.M22;
            }

            return new Point(
                (int)Math.Round(wpfPoint.X * dpiScaleFactorX),
                (int)Math.Round(wpfPoint.Y * dpiScaleFactorY));
        }

        /// <summary>
        /// Creates create and dipsose calls for the live scope of the given control.
        /// If the control is loaded, then the object is created. If the control is unloaded, then dispose will be called.
        /// </summary>
        /// <param name="control">The control on which to create/attach the object</param>
        /// <param name="createMethod">The create method.</param>
        public static void HandleCreateDisposeOnControl(this WinForms.Control control, Func<IDisposable> createMethod)
        {
            IDisposable createdObject = null;

            // Attach on handle created
            control.HandleCreated += (sender, eArgs) =>
            {
                if (createdObject == null)
                {
                    createdObject = createMethod();
                }
            };

            // Attach on handle destroyed
            control.HandleDestroyed += (sender, eArgs) =>
            {
                if (createdObject != null)
                {
                    createdObject.Dispose();
                    createdObject = null;
                }
            };

            // Create object initially if control is already initialized
            if (control.IsHandleCreated)
            {
                createdObject = createMethod();
            }
        }
#endif

        /// <summary>
        /// Reads all bytes from the given stream.
        /// </summary>
        /// <param name="inStream">The stream to read all the data from.</param>
        public static byte[] ReadAllBytes(this Stream inStream)
        {
            if (inStream.Length > Int32.MaxValue) { throw new NotSupportedException("Given stream is to big!"); }

            byte[] result = new byte[inStream.Length];
            inStream.Read(result, 0, (int)inStream.Length);
            return result;
        }

        /// <summary>
        /// Clears and disposes all items of the given collection.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="listOfDisposables">A collection containing all items to be disposed and removed.</param>
        public static void ClearAndDisposeAll<T>(this ICollection<T> listOfDisposables)
            where T : class, IDisposable
        {
            foreach (var actDisposeable in listOfDisposables)
            {
                CommonTools.DisposeObject(actDisposeable);
            }
            listOfDisposables.Clear();
        }

        /// <summary>
        /// Performs the given Action for each element within the enumeration.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="enumeration">Enumeration to loop through.</param>
        /// <param name="actionPerElement">Action to perform for each element.</param>
        public static void ForEachInEnumeration<T>(this IEnumerable<T> enumeration, Action<T> actionPerElement)
        {
            foreach (T actElement in enumeration)
            {
                actionPerElement(actElement);
            }
        }

        /// <summary>
        /// Converts all items from the given source enumeration to the given target enumeration.
        /// </summary>
        public static IEnumerable<TTarget> Convert<TSource, TTarget>(this IEnumerable<TSource> enumeration, Func<TSource, TTarget> converter)
        {
            foreach (TSource actSourceItem in enumeration)
            {
                yield return converter(actSourceItem);
            }
        }

        public static void PostAlsoIfNull(this SynchronizationContext syncContext, Action postAction)
        {
            if (syncContext == null)
            {
#if DESKTOP
                ThreadPool.QueueUserWorkItem(obj => postAction());
#else
                ThreadPool.RunAsync((arg) => { postAction(); })
                    .FireAndForget();
#endif

            }
            else
            {
                syncContext.Post(obj => postAction(), null);
            }
        }

#if WINRT || UNIVERSAL
        /// <summary>
        /// "Forgets" the given task, but still tries to dispatch exception somewhere the user / developer
        /// can see them.
        /// </summary>
        /// <param name="asyncAction">The action to be fired.</param>
        public static async void FireAndForget(this IAsyncAction asyncAction)
        {
            await asyncAction;
        }

        /// <summary>
        /// "Forgets" the given task, but still tries to dispatch exception somewhere the user / developer
        /// can see them.
        /// </summary>
        /// <param name="asyncAction">The action to be fired.</param>
        public static async void FireAndForget(this Task asyncAction)
        {
            await asyncAction;
        }
#endif

#if DESKTOP
        /// <summary>
        /// "Forgets" the given task, but still tries to dispatch exception somewhere the user / developer
        /// can see them.
        /// </summary>
        /// <param name="asyncAction">The action to be fired.</param>
        public static async void FireAndForget(this Task asyncAction)
        {
            await asyncAction;
        }

#endif

        /// <summary>
        /// Posts the given action to the given synchronization context also if it is null.
        /// If it is null, then a new task will be started.
        /// </summary>
        /// <param name="syncContext">The context to send the action to.</param>
        /// <param name="actionToSend">The action to send.</param>
        /// <param name="actionIfNull">What should we do if weg get no SyncContext?</param>
        public static void PostAlsoIfNull(this SynchronizationContext syncContext, Action actionToSend, ActionIfSyncContextIsNull actionIfNull)
        {
            if (syncContext != null) { syncContext.Post((arg) => actionToSend(), null); }
            else
            {
                switch (actionIfNull)
                {
                    case ActionIfSyncContextIsNull.InvokeSynchronous:
                        actionToSend();
                        break;

                    case ActionIfSyncContextIsNull.InvokeUsingNewTask:
                        Task.Factory.StartNew(actionToSend);
                        break;

                    case ActionIfSyncContextIsNull.DontInvoke:
                        break;

                    default:
                        throw new ArgumentException("actionIfNull", "Action " + actionIfNull + " unknown!");
                }
            }
        }

        /// <summary>
        /// Post the given action in an async manner to the given SynchronizationContext.
        /// </summary>
        /// <param name="syncContext">The target SynchronizationContext.</param>
        /// <param name="postAction">The action to be posted.</param>
        /// <param name="actionIfNull">What should we do if we get no SyncContext?</param>
        public static Task PostAlsoIfNullAsync(this SynchronizationContext syncContext, Action postAction, ActionIfSyncContextIsNull actionIfNull)
        {
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            syncContext.PostAlsoIfNull(() =>
                {
                    try
                    {
                        postAction();
                        completionSource.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                }, 
                actionIfNull);
            return completionSource.Task;
        }

        /// <summary>
        /// Post the given action in an async manner to the given SynchronizationContext.
        /// </summary>
        /// <param name="syncContext">The target SynchronizationContext.</param>
        /// <param name="postAction">The action to be posted.</param>
        public static Task PostAsync(this SynchronizationContext syncContext, Action postAction)
        {
            TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
            syncContext.Post((arg) =>
            {
                try
                {
                    postAction();
                    completionSource.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    completionSource.TrySetException(ex);
                }
            }, null);
            return completionSource.Task;
        }

        /// <summary>
        /// Executes a delayd post to the given synchronization context.
        /// </summary>
        /// <param name="syncContext">The synchronization context to post to.</param>
        /// <param name="callBack">The delegate to be called.</param>
        /// <param name="state">The parameter of the delegate.</param>
        /// <param name="delayTime">The total time to wait.</param>
        public static void PostDelayed(this SynchronizationContext syncContext, SendOrPostCallback callBack, object state, TimeSpan delayTime)
        {
            if (syncContext == null) { throw new ArgumentNullException("syncContext"); }
            if (callBack == null) { throw new ArgumentNullException("callBack"); }
            if (delayTime <= TimeSpan.Zero) { throw new ArgumentException("Delay time musst be greater than zero!", "delayTime"); }

            //Start and register timer in local timer store (ensures that no dispose gets called..)
            lock (s_timerDictLock)
            {
                System.Threading.Timer newTimer = null;
                newTimer = new System.Threading.Timer(
                    (arg) =>
                    {
                        lock (s_timerDictLock)
                        {
                            s_timerDict.Remove(newTimer);
                        }
                        syncContext.Post(callBack, state);
                    },
                    null,
                    (int)delayTime.TotalMilliseconds,
                    Timeout.Infinite);
                s_timerDict.Add(newTimer, null);
            }
        }

#if DESKTOP
        public static GDI.Bitmap ToWinFormsBitmap(this BitmapSource bitmapsource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(stream);

                using (var tempBitmap = new GDI.Bitmap(stream))
                {
                    // According to MSDN, one "must keep the stream open for the lifetime of the Bitmap."
                    // So we return a copy of the new bitmap, allowing us to dispose both the bitmap and the stream.
                    return new GDI.Bitmap(tempBitmap);
                }
            }
        }

        public static BitmapSource ToWpfBitmap(this GDI.Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, GDI.Imaging.ImageFormat.Bmp);

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
#endif
    }
}
