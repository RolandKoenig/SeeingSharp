#region License information (FrozenSky and all based games/applications)
/*
    FrozenSky and all games/applications based on it (more info at http://www.rolandk.de/wp)
    Copyright (C) 2014 Roland König (RolandK)

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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSky.Infrastructure
{
    public class TypeQueryHandler
    {
        private Dictionary<Type, List<Type>> m_typesByContract;
        private Dictionary<Assembly, List<Type>> m_typesByAssembly;
        private List<Type> m_types;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeQueryHandler"/> class.
        /// </summary>
        internal TypeQueryHandler()
        {
            m_types = new List<Type>();
            m_typesByAssembly = new Dictionary<Assembly, List<Type>>();
            m_typesByContract = new Dictionary<Type, List<Type>>();
        }

        /// <summary>
        /// Gets and instanciates all types which are implementing the given contract.
        /// </summary>
        /// <typeparam name="ContractType">The implemented contract.</typeparam>
        public List<ContractType> GetAndInstanciateByContract<ContractType>()
            where ContractType : class
        {
            Type contractType = typeof(ContractType);
            List<Type> typesByContract = null;
            m_typesByContract.TryGetValue(contractType, out typesByContract);
            if (typesByContract != null)
            {
                List<ContractType> result = new List<ContractType>(typesByContract.Count);
                foreach (Type actType in typesByContract)
                {
                    ContractType actObject = Activator.CreateInstance(actType) as ContractType;
                    if (actObject != null) { result.Add(actObject); }
                }
                return result;
            }
            else { return new List<ContractType>(); }
        }

        /// <summary>
        /// Gets a collection containing all types which have the given attribute attached.
        /// </summary>
        /// <typeparam name="T1">The type of the attribute.</typeparam>
        public IEnumerable<Tuple<Type, T1>> QueryTypesByAttribute<T1>()
            where T1 : Attribute
        {
            foreach (Type actType in m_types)
            {
                T1 actAttrib = actType.GetTypeInfo().GetCustomAttribute<T1>();
                if (actAttrib != null) { yield return Tuple.Create(actType, actAttrib); }
            }
        }


        /// <summary>
        /// Gets a list containing all types that implement the given contract.
        /// </summary>
        /// <typeparam name="ContractType">The implemented contract.</typeparam>
        public List<Type> GetTypesByContract<ContractType>()
        {
            return GetTypesByContract(typeof(ContractType));
        }

        /// <summary>
        /// Gets a list containing all types that implement the given contract.
        /// </summary>
        /// <param name="contractType">The implemented contract.</param>
        public List<Type> GetTypesByContract(Type contractType)
        {
            List<Type> typesByContract = null;
            m_typesByContract.TryGetValue(contractType, out typesByContract);
            if (typesByContract != null) { return new List<Type>(typesByContract); }
            else { return new List<Type>(); }
        }

        /// <summary>
        /// Queries for types in the given collection of assemblies.
        /// </summary>
        /// <param name="assembliesToQuery">A collection containing all assemblies for the query.</param>
        internal void QueryTypes(IEnumerable<Assembly> assembliesToQuery)
        {
            foreach (Assembly actAssembly in assembliesToQuery)
            {
                List<Type> actByAssemblyList = new List<Type>();
                m_typesByAssembly[actAssembly] = actByAssemblyList;

                foreach (var actAttrib in actAssembly.GetCustomAttributes<AssemblyQueryableTypeAttribute>())
                {
                    // Handle default collections
                    m_types.Add(actAttrib.TargetType);
                    actByAssemblyList.Add(actAttrib.TargetType);

                    // Handle types with contract
                    if (actAttrib.ContractType != null)
                    {
                        List<Type> typesByContract = null;
                        if (!m_typesByContract.TryGetValue(actAttrib.ContractType, out typesByContract))
                        {
                            typesByContract = new List<Type>();
                            m_typesByContract.Add(actAttrib.ContractType, typesByContract);
                        }

                        typesByContract.Add(actAttrib.TargetType);
                    }
                }
            }
        }
    }
}
