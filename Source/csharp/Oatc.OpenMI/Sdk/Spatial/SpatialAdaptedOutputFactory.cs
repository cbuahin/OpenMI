#region Copyright
/*
* Copyright (c) 2005-2010, OpenMI Association
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenMI Association nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "OpenMI Association" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "OpenMI Association" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
﻿using System.Collections.Generic;
﻿using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
    public class SpatialAdaptedOutputFactory : IAdaptedOutputFactory
    {
        private string _caption = String.Empty;
        private string _description = String.Empty;
        private readonly string _id;

        public SpatialAdaptedOutputFactory(string id)
        {
            _id = id;
        }

        public IIdentifiable[] GetAvailableAdaptedOutputIds(IBaseOutput adaptee, IBaseInput target)
        {
            return GetAvailableMethods(adaptee, target);
        }

        public IBaseAdaptedOutput CreateAdaptedOutput(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
        {
          return CreateAdaptedOutputMethod(adaptedOutputId, adaptee, target);
        }

        public static IIdentifiable[] GetAvailableMethods(IBaseOutput adaptee, IBaseInput target)
        {
            ITimeSpaceOutput tsadaptee = adaptee as ITimeSpaceOutput;

            // Only works with timespace output as adaptee, and element set as spatial definition
            if (tsadaptee == null || !(tsadaptee.SpatialDefinition is IElementSet))
                return new IIdentifiable[0];
            
            IList<IIdentifiable> methods = new List<IIdentifiable>();
            
            GetAvailableOperationMethods(ref methods, tsadaptee.ElementSet().ElementType);

            // Check if the target is there and is a timespace input
            ITimeSpaceInput tstarget = target as ITimeSpaceInput;
            if (target == null || tstarget == null || !(tstarget.SpatialDefinition is IElementSet))
                return ((List<IIdentifiable>)methods).ToArray();

            GetAvailableMappingMethods(ref methods, tsadaptee.ElementSet().ElementType, tstarget.ElementSet().ElementType);

            return ((List<IIdentifiable>)methods).ToArray();
        }

        public static ITimeSpaceAdaptedOutput CreateAdaptedOutputMethod(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IBaseInput target)
        {
            IElementSet elmtSet = null;
            ITimeSpaceInput tsTarget = target as ITimeSpaceInput;
            if (tsTarget != null && tsTarget.SpatialDefinition is IElementSet)
                elmtSet = tsTarget.ElementSet();

            return CreateAdaptedOutputMethod(adaptedOutputId, adaptee, elmtSet);
        }

        public static ITimeSpaceAdaptedOutput CreateAdaptedOutputMethod(IIdentifiable adaptedOutputId, IBaseOutput adaptee, IElementSet targetElmtSet)
        {
            SpatialMethod method = FindSMethod(adaptedOutputId);

            ITimeSpaceAdaptedOutput adaptedOutput = null;

            if (method.ElementMapperMethod != ElementMapperMethod.None)
            {
                if (targetElmtSet == null)
                    throw new ArgumentException("Target not defined or spatial definition is not an element set. Can not create adaptor");
                adaptedOutput = new ElementMapperAdaptedOutput(adaptedOutputId, (ITimeSpaceOutput)adaptee, targetElmtSet);
            }
            else
            {
                if (string.Equals(_ElementOperationPrefix + "200", adaptedOutputId.Id, StringComparison.OrdinalIgnoreCase))
                    adaptedOutput = new ElementLineLengthOperationAdaptor(adaptedOutputId.Id, (ITimeSpaceOutput) adaptee);
                else if (string.Equals(_ElementOperationPrefix + "300", adaptedOutputId.Id, StringComparison.OrdinalIgnoreCase))
                    adaptedOutput = new ElementAreaOperationAdaptor(adaptedOutputId.Id, (ITimeSpaceOutput)adaptee);
            }

            if (adaptedOutput == null)
                throw new ArgumentException("Adapted output id could not be found","adaptedOutputId");

            // Connect adaptor and adaptee
            if (!adaptee.AdaptedOutputs.Contains(adaptedOutput))
            {
                adaptee.AddAdaptedOutput(adaptedOutput);
            }

            return adaptedOutput;
        }
        public string Id
        {
            get { return _id; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        internal class AdaptedOutputIdentifier : IIdentifiable
        {
            private readonly string _adaptedOutputId;

            public AdaptedOutputIdentifier(string adaptedOutputId)
            {
                _adaptedOutputId = adaptedOutputId;
            }

            public string Id
            {
                get
                {
                    return _adaptedOutputId;
                }
            }

            public string Caption { get; set; }
            public string Description { get; set; }
        }


        #region Listing available methods

        static SpatialAdaptedOutputFactory()
        {
            SpatialMethod method;
            _AvailableMethods = new List<SpatialMethod>();

            // Operation values:
            // Id        :   0- 99
            // Point     : 100-199
            // Polyline  : 200-299
            // Polygon   : 100-199
            // Polyhedron: 100-199
            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.PolyLine,
                ToElementsShapeType = ElementType.PolyLine,
                ElementMapperMethod = ElementMapperMethod.None,
                Description = "Polyline operation, multiply by line length",
                Id = _ElementOperationPrefix + "200",
            };
            _AvailableMethods.Add(method);

            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.None,
                Description = "Polygon operation, multiply by area",
                Id = _ElementOperationPrefix + "300",
            };
            _AvailableMethods.Add(method);

            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.Point,
                ElementMapperMethod = ElementMapperMethod.Nearest,
                Description = "Point-to-point Nearest",
                Id = _ElementMapperPrefix + "100",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.Point,
                ElementMapperMethod = ElementMapperMethod.Inverse,
                Description = "Point-to-point Inverse",
                Id = _ElementMapperPrefix + "101",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.PolyLine,
                ElementMapperMethod = ElementMapperMethod.Nearest,
                Description = "Point-to-polyline Nearest",
                Id = _ElementMapperPrefix + "200",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.PolyLine,
                ElementMapperMethod = ElementMapperMethod.Inverse,
                Description = "Point-to-polyline Inverse",
                Id = _ElementMapperPrefix + "201",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.Mean,
                Description = "Point-to-polygon Mean",
                Id = _ElementMapperPrefix + "300",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Point,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.Sum,
                Description = "Point-to-polygon Sum",
                Id = _ElementMapperPrefix + "301",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.PolyLine,
                ToElementsShapeType = ElementType.Point,
                ElementMapperMethod = ElementMapperMethod.Nearest,
                Description = "Polyline-to-point Nearest",
                Id = _ElementMapperPrefix + "400",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.PolyLine,
                ToElementsShapeType = ElementType.Point,
                ElementMapperMethod = ElementMapperMethod.Inverse,
                Description = "Polyline-to-point Inverse",
                Id = _ElementMapperPrefix + "401",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.PolyLine,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.WeightedMean,
                Description = "Polyline-to-polygon Weighted Mean",
                Id = _ElementMapperPrefix + "500",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.PolyLine,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.WeightedSum,
                Description = "Polyline-to-polygon Weighted Sum",
                Id = _ElementMapperPrefix + "501",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.Point,
                ElementMapperMethod = ElementMapperMethod.Value,
                Description = "Polygon-to-point Value",
                Id = _ElementMapperPrefix + "600",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.PolyLine,
                ElementMapperMethod = ElementMapperMethod.WeightedMean,
                Description = "Polygon-to-polyline Weighted Mean",
                Id = _ElementMapperPrefix + "700",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.PolyLine,
                ElementMapperMethod = ElementMapperMethod.WeightedSum,
                Description = "Polygon-to-polyline Weighted Sum",
                Id = _ElementMapperPrefix + "701",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.WeightedMean,
                Description = "Polygon-to-polygon Weighted Mean",
                Id = _ElementMapperPrefix + "800",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.WeightedSum,
                Description = "Polygon-to-polygon Weighted Sum",
                Id = _ElementMapperPrefix + "801",
            };
            _AvailableMethods.Add(method);


            method = new SpatialMethod()
            {
                FromElementsShapeType = ElementType.Polygon,
                ToElementsShapeType = ElementType.Polygon,
                ElementMapperMethod = ElementMapperMethod.Distribute,
                Description = "Polygon-to-polygon Distribute",
                Id = _ElementMapperPrefix + "802",
            };
            _AvailableMethods.Add(method);

        }

        private const string _ElementMapperPrefix = "ElementMapper";
        private const string _ElementOperationPrefix = "ElementOperation";
        private static readonly List<SpatialMethod> _AvailableMethods;


        #region Nested type: SpatialMethod

        private class SpatialMethod
        {
            public string Description;
            public ElementMapperMethod ElementMapperMethod;
            public ElementType FromElementsShapeType;
            public string Id;
            public ElementType ToElementsShapeType;
        }

        #endregion

        #endregion

        /// <summary>
        /// Gives a list of id's + descriptions (strings) for available methods 
        /// given the combination of fromElementType and toElementType
        /// </summary>
        /// 
        /// <param name="sourceElementType">Element type of the elements in
        /// the fromElementset</param>
        /// <param name="targetElementType">Element type of the elements in
        /// the toElementset</param>
        /// 
        /// <returns>
        ///	<p>ArrayList of method descriptions</p>
        public static IIdentifiable[] GetAvailableMethods(ElementType sourceElementType, ElementType targetElementType)
        {
            IList<IIdentifiable> methods = new List<IIdentifiable>();
            GetAvailableOperationMethods(ref methods, sourceElementType);
            GetAvailableMappingMethods(ref methods, sourceElementType, targetElementType);
            return ((List < IIdentifiable>)methods).ToArray();
        }

        /// <summary>
        /// Gives a list of descriptions (strings) for available mapping methods 
        /// given the combination of fromElementType and toElementType
        /// </summary>
        /// 
        /// <param name="sourceElementType">Element type of the elements in
        /// the fromElementset</param>
        /// <param name="targetElementType">Element type of the elements in
        /// the toElementset</param>
        /// 
        /// <returns>
        ///	<p>ArrayList of method descriptions</p>
        public static void GetAvailableMappingMethods(ref IList<IIdentifiable> methods, ElementType sourceElementType, ElementType targetElementType)
        {
            for (int i = 0; i < _AvailableMethods.Count; i++)
            {
                SpatialMethod availableMethod = _AvailableMethods[i];

                // Check if operation or mapping - only add mappings here
                if (availableMethod.ElementMapperMethod != ElementMapperMethod.None)
                {
                    // In case of a mapping, the target is important
                    if (sourceElementType == _AvailableMethods[i].FromElementsShapeType)
                    {
                        if (targetElementType == _AvailableMethods[i].ToElementsShapeType)
                        {
                            methods.Add(new Identifier(_AvailableMethods[i].Id) { Description = _AvailableMethods[i].Description });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gives a list of descriptions (strings) for available operation methods 
        /// given the source ElementType
        /// </summary>
        /// <param name="sourceElementType">Element type of the elements in the source Elementset</param>
        /// <returns>
        public static void GetAvailableOperationMethods(ref IList<IIdentifiable> methods, ElementType sourceElementType)
        {
            for (int i = 0; i < _AvailableMethods.Count; i++)
            {
                SpatialMethod availableMethod = _AvailableMethods[i];

                // Check if operation or mapping, only add operations here
                if (availableMethod.ElementMapperMethod == ElementMapperMethod.None)
                {
                    // In case of an operation, the target is not important
                    if (sourceElementType == _AvailableMethods[i].FromElementsShapeType)
                    {
                        methods.Add(new Identifier(_AvailableMethods[i].Id) { Description = _AvailableMethods[i].Description });
                    }
                }
            }
        }

        public static IDescribable GetAdaptedOutputDescription(IIdentifiable identifiable)
        {
            SpatialMethod spatialMethod = FindSMethod(identifiable);
            return new Describable(spatialMethod.Id, spatialMethod.Description);
        }

        /// <summary>
        /// Check if the provided id was created by/can be used by this factory
        /// </summary>
        /// <param name="identifiable">Id to check</param>
        /// <returns>True of identifier can be used with this factory.</returns>
        public static bool HasId(IIdentifiable identifiable)
        {
            foreach (SpatialMethod availableMethod in _AvailableMethods)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(availableMethod.Id, identifiable.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public static ElementMapperMethod GetMethod(IIdentifiable identifiable)
        {
            return FindSMethod(identifiable).ElementMapperMethod;
        }

        public static ElementType GetToElementType(IIdentifiable identifiable)
        {
            return FindSMethod(identifiable).ToElementsShapeType;
        }

        private static SpatialMethod FindSMethod(IIdentifiable identifiable)
        {
            foreach (SpatialMethod availableMethod in _AvailableMethods)
            {
                if (String.Compare(availableMethod.Id, identifiable.Id) == 0)
                {
                    return availableMethod;
                }
            }
            throw new ArgumentException("Invalid indentifier, identifier does not belong to this factory" + identifiable.Id);
        }

        /// <summary>
        /// This method will return an ArrayList of <see cref="ITimeSpaceAdaptedOutput"/> that the ElementMapper 
        /// provides when mapping from the ElementType specified in the method argument. 
        /// </summary>
        /// <remarks>
        ///  Each <see cref="ITimeSpaceAdaptedOutput"/> object will contain:
        ///  <p> [Key]              [Value]                      [ReadOnly]    [Description]----------------- </p>
        ///  <p> ["Type"]           ["SpatialMapping"]           [true]        ["Using the ElementMapper"] </p>
        ///  <p> ["Caption"]        [The Operation Caption]      [true]        ["Internal ElementMapper adaptedOutput Caption"] </p>
        ///  <p> ["Description"]    [The Operation Description]  [true]        ["Using the ElementMapper"] </p>
        ///  <p> ["ToElementType"]  [ElementType]                [true]        ["Valid To-Element Types"]  </p>
        /// </remarks>
        /// <returns>
        ///  ArrayList which contains the available <see cref="ITimeSpaceAdaptedOutput"/>.
        /// </returns>
        public static List<IArgument> GetAdaptedOutputArguments(IIdentifiable methodIdentifier)
        {
            if (!(methodIdentifier.Id.StartsWith(_ElementMapperPrefix) || methodIdentifier.Id.StartsWith(_ElementOperationPrefix)))
            {
                throw new Exception("Unknown method identifier: " + methodIdentifier);
            }

            for (int i = 0; i < _AvailableMethods.Count; i++)
            {
                SpatialMethod method = _AvailableMethods[i];
                if (String.Compare(method.Id, methodIdentifier.Id) == 0)
                {
                    if (method.ElementMapperMethod == ElementMapperMethod.None)
                    {
                        var arguments = new List<IArgument>();
                        arguments.Add(new ArgumentString("Caption", method.Id,
                                                         "Internal ElementOperation AdaptedOutput Caption"));
                        arguments.Add(new ArgumentString("Description", method.Description,
                                                         "Operation description"));
                        arguments.Add(new ArgumentString("Type", "SpatialOperation", "Using an Element Operator"));
                        arguments.Add(new ArgumentString("FromElementType",
                                                         method.FromElementsShapeType.ToString(),
                                                         "Valid From-Element Types"));
                        return arguments;

                    }
                    else
                    {
                        var arguments = new List<IArgument>();
                        arguments.Add(new ArgumentString("Caption", method.Id,
                                                         "Internal ElementMapper AdaptedOutput Caption"));
                        arguments.Add(new ArgumentString("Description", method.Description,
                                                         "Mapping description"));
                        arguments.Add(new ArgumentString("Type", "SpatialMapping", "Using the ElementMapper"));
                        arguments.Add(new ArgumentString("FromElementType",
                                                         method.FromElementsShapeType.ToString(),
                                                         "Valid From-Element Types"));
                        arguments.Add(new ArgumentString("ToElementType",
                                                         method.ToElementsShapeType.ToString(),
                                                         "Valid To-Element Types"));
                        return arguments;

                    }
                }
            }
            throw new Exception("Unknown methodID: " + methodIdentifier.Id);
        }
    }
}
