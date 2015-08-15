//#region License information (SeeingSharp and all based games/applications)
///*
//    Seeing# and all games/applications distributed together with it. 
//    More info at 
//     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
//     - http://www.rolandk.de/wp (the autors homepage, german)
//    Copyright (C) 2015 Roland König (RolandK)

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/.
//*/
//#endregion
//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;

//namespace SeeingSharp.Multimedia.Core
//{
//    public class AsyncResourceLoader
//    {
//        private static AsyncResourceLoader s_current;

//        #region Task members
//        private ConcurrentQueue<object> m_openTasks;
//        private int m_runningTaskCount;
//        private ConcurrentQueue<TaskCompletionSource<object>> m_openWaiters;
//        #endregion

//        /// <summary>
//        /// Prevents a default instance of the <see cref="AsyncResourceLoader"/> class from being created.
//        /// </summary>
//        private AsyncResourceLoader()
//        {
//            m_openTasks = new ConcurrentQueue<object>();
//            m_openWaiters = new ConcurrentQueue<TaskCompletionSource<object>>();
//        }

//        /// <summary>
//        /// Waits for all running tasks to be finished.
//        /// </summary>
//        public Task WaitForAllFinishedAsync()
//        {
//            TaskCompletionSource<object> taskSource = new TaskCompletionSource<object>();
//            m_openWaiters.Enqueue(taskSource);

//            this.TriggerTaskQueue();

//            return taskSource.Task;
//        }

//        internal Task<T> EnqueueAsyncResourceLoadingTaskAsync<T>(Func<Task<T>> resourceLoadAction)
//        {
//            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

//            // Register the given task
//            m_openTasks.Enqueue((Func<Task<object>>)(() =>
//            {
//                Task<T> task = resourceLoadAction();
//                return task.ContinueWith((Task<T> givenTask) =>
//                {
//                    if (givenTask.IsCanceled) { taskSource.SetCanceled(); }
//                    else if (givenTask.IsFaulted) { taskSource.SetException(givenTask.Exception); }
//                    else { taskSource.SetResult(givenTask.Result); }

//                    return (object)givenTask.Result;
//                });
//            }));

//            // Trigger task queue processing
//            this.TriggerTaskQueue();

//            return taskSource.Task;
//        }

//        /// <summary>
//        /// Enqueues the given action for resource loading.
//        /// </summary>
//        /// <param name="resourceLoadAction">The resource load action.</param>
//        internal Task<T> EnqueueResourceLoadingTaskAsync<T>(Func<T> resourceLoadAction)
//        {
//            TaskCompletionSource<T> taskSource = new TaskCompletionSource<T>();

//            // Register the given task
//            m_openTasks.Enqueue((Func<object>)(() =>
//            {
//                try
//                {
//                    T result = resourceLoadAction();
//                    taskSource.SetResult(result);
//                    return result;
//                }
//                catch (Exception ex)
//                {
//                    taskSource.SetException(ex);
//                    return default(T);
//                }
//            }));

//            // Trigger task queue processing
//            this.TriggerTaskQueue();

//            return taskSource.Task;
//        }

//        /// <summary>
//        /// Triggers the task queue.
//        /// </summary>
//        private void TriggerTaskQueue()
//        {
//            while ((m_runningTaskCount < Constants.ASYNC_LOADER_MAX_PRALLEL_TASK_COUNT) &&
//                   (m_openTasks.Count > 0))
//            {
//                object actFuncWrapper = null;
//                if(m_openTasks.TryDequeue(out actFuncWrapper))
//                {
//                    Interlocked.Increment(ref m_runningTaskCount);

//                    Func<object> syncFunc = actFuncWrapper as Func<object>;
//                    if (syncFunc != null)
//                    {
//                        // Start this task actually
//                        Task.Run(() =>
//                        {
//                            // Run the registered function
//                            syncFunc();

//                            // Decrement the task counter
//                            Interlocked.Decrement(ref m_runningTaskCount);

//                            // Finally, trigger other tasks if needed
//                            TriggerTaskQueue();
//                        });

//                        continue;
//                    }

//                    Func<Task<object>> asyncFunc = actFuncWrapper as Func<Task<object>>;
//                    if(asyncFunc != null)
//                    {
//                        Task<object> asyncFuncTask = asyncFunc();
//                        asyncFuncTask.ContinueWith((givenTask) =>
//                        {
//                            // Decrement the task counter
//                            Interlocked.Decrement(ref m_runningTaskCount);

//                            // Finally, trigger other tasks if needed
//                            TriggerTaskQueue();
//                        });

//                        continue;
//                    }

//                    // TODO: Handle exception case
//                    Interlocked.Decrement(ref m_runningTaskCount);
//                }
//            }

//            // Notify all awaitors
//            if((m_runningTaskCount == 0) &&
//               (m_openTasks.Count == 0))
//            {
//                TaskCompletionSource<object> actAwaiter = null;
//                while(m_openWaiters.TryDequeue(out actAwaiter))
//                {
//                    actAwaiter.SetResult(null);
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the current instance of this loader.
//        /// </summary>
//        public static AsyncResourceLoader Current
//        {
//            get
//            {
//                if (s_current == null) { s_current = new AsyncResourceLoader(); }
//                return s_current;
//            }
//        }
//    }
//}
