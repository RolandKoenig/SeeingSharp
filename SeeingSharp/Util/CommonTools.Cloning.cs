#region License information (SeeingSharp and all based games/applications)
/*
    Seeing# and all games/applications distributed together with it. 
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp (sourcecode)
     - http://www.rolandk.de/wp (the autors homepage, german)
    Copyright (C) 2016 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if DESKTOP
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace SeeingSharp.Util
{
    public static partial class CommonTools
    {
        private static List<Type> s_notBinarySerializable = new List<Type>();
        private static object s_notBinarySerializableLock = new object();

#if DESKTOP
        /// <summary>
        /// Clones an object by doing a full deep copy of every field and property.
        /// </summary>
        /// <param name="source">Object to clone</param>
        /// <returns>Cloned copy</returns>
        public static T CloneObject<T>(T source)
        {
            return (T)CloneObjectInternal(source, source, new Dictionary<object, object>());
        }

        /// <summary>
        /// Internal method to clone an object
        /// </summary>
        private static object CloneObjectInternal(object entity, object initiator, Dictionary<object, object> refValues)
        {
            //Null? No work
            if (entity == null)
            {
                return null;
            }

            Type entityType = entity.GetType();

            //Special case value-types; they are passed by value so we have our
            //own copy right here.
            if (entityType.IsValueType)
            {
                return entity;
            }

            //Clone strings (special case)
            if (entityType == typeof(string))
            {
                return ((string)entity).Clone();
            }

            //See if we've seen this object already.  If so, return the clone.
            if (refValues.ContainsKey(entity))
            {
                return refValues[entity];
            }

            //Clone weak references
            WeakReference weakReference = entity as WeakReference;
            if (weakReference != null)
            {
                if (weakReference.IsAlive)
                {
                    object clone = new WeakReference(weakReference.Target);
                    refValues[entity] = clone;
                    return clone;
                }
                else
                {
                    object clone = new WeakReference(new object());
                    refValues[entity] = clone;
                    return clone;
                }
            }

            //Use the ICloneable implemnetation of the object if possible
            if (entity != initiator)
            {
                ICloneable clonableInterface = entity as ICloneable;
                if (clonableInterface != null)
                {
                    object clone = clonableInterface.Clone();
                    refValues[entity] = clone;
                    return clone;
                }
            }

            // If the type is [Serializable], then try that approach first; if serializable
            // fails (a child object is *not* serializable) then we will continue on.
            if (entityType.GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0)
            {
                bool isSerializable = true;
                lock (s_notBinarySerializableLock)
                {
                    isSerializable = !s_notBinarySerializable.Contains(entityType);
                }

                if (isSerializable)
                {
                    ReusableMemoryStream memoryStream = ReusableMemoryStream.TakeMemoryStream();
                    try
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(memoryStream, entity);
                        memoryStream.Position = 0;
                        object clone = binaryFormatter.Deserialize(memoryStream);
                        refValues[entity] = clone;
                        return clone;
                    }
                    catch (Exception)
                    {
                        lock (s_notBinarySerializableLock)
                        {
                            s_notBinarySerializable.Add(entityType);
                        }
                    }
                    finally
                    {
                        ReusableMemoryStream.ReregisterMemoryStream(memoryStream);
                    }
                }
            }

            //If the element is an array, then copy it.
            if (entityType.IsArray)
            {
                Array copy = (Array)((Array)entity).Clone();
                if (copy.Rank > 1)
                {
                    for (int rank = 0; rank < copy.Rank; rank++)
                    {
                        for (int i = copy.GetLowerBound(rank); i <= copy.GetUpperBound(rank); i++)
                            copy.SetValue(CloneObjectInternal(copy.GetValue(rank, i), initiator, refValues), rank, i);
                    }
                }
                else
                {
                    for (int i = copy.GetLowerBound(0); i <= copy.GetUpperBound(0); i++)
                    {
                        object value = copy.GetValue(i);
                        copy.SetValue(CloneObjectInternal(value, initiator, refValues), i);
                    }
                }
                refValues[entity] = copy;
                return copy;
            }

            //Dictionary type
            if (entity is IDictionary)
            {
                IDictionary dictionary = (IDictionary)entity;
                IDictionary clone = (IDictionary)Activator.CreateInstance(entityType);
                foreach (var key in dictionary.Keys)
                {
                    object keyCopy = CloneObjectInternal(key, initiator, refValues);
                    object valCopy = CloneObjectInternal(dictionary[key], initiator, refValues);
                    clone.Add(keyCopy, valCopy);
                }
                refValues[entity] = clone;
                return clone;
            }

            //IList type
            if (entity is IList)
            {
                IList list = (IList)entity;
                IList clone = (IList)Activator.CreateInstance(entityType);
                foreach (var value in list)
                {
                    object valCopy = CloneObjectInternal(value, initiator, refValues);
                    clone.Add(valCopy);
                }
                refValues[entity] = clone;
                return clone;
            }

            //No obvious way to copy the object - do a field-by-field copy
            object result = Activator.CreateInstance(entityType);

            //Save off the reference
            refValues[entity] = result;

            //Walk through all the fields - this will capture auto-properties as well.
            Type actType = entityType;
            Dictionary<FieldInfo, object> alreadyScanned = new Dictionary<FieldInfo, object>();
            while (actType != null)
            {
                foreach (FieldInfo actField in actType.GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    //Check if this field is to be ignored during clone process
                    IgnoreGenericCloneAttribute ignoreCloneAttribute = Attribute.GetCustomAttribute(actField, typeof(IgnoreGenericCloneAttribute))
                        as IgnoreGenericCloneAttribute;
                    if (ignoreCloneAttribute != null) { continue; }

                    //Handling logic for fileds which should get a reference to the original object
                    AssignOriginalObjectAfterCloneAttribute assignOriginalAttribute = Attribute.GetCustomAttribute(actField, typeof(AssignOriginalObjectAfterCloneAttribute))
                        as AssignOriginalObjectAfterCloneAttribute;
                    if (assignOriginalAttribute != null)
                    {
                        try
                        {
                            actField.SetValue(result, entity);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Unable to set original object to field " + actField.Name + ": " + ex.Message, ex);
                        }
                        continue;
                    }

                    //Handle current field
                    if (!alreadyScanned.ContainsKey(actField))
                    {
                        actField.SetValue(result, CloneObjectInternal(actField.GetValue(entity), initiator, refValues));
                        alreadyScanned.Add(actField, null);
                    }
                }
                actType = actType.BaseType;
            }

            return result;
        }
#endif
    }
}
