#region Copyright

/*
    Copyright (c) 2005-2010, OpenMI Association
    "http://www.openmi.org/"

    This file is part of OpenMI.Standard2.dll

    OpenMI.Standard2.dll is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    OpenMI.Standard2.dll is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;

namespace OpenMI.Standard2
{
    /// <summary>
    /// <para>The IBaseLinkableComponent is the key interface in the OpenMI standard.</para>
    /// 
    /// <para>OpenMI-compliance definition (The compliancy refers to a set of basic interfaces as well as to 
    /// optional extension interfaces, e.g. for time and space dependent components):</para>
    /// 
    /// <para>§ 1) An OpenMI-compliant component must implement the IBaseLinkableComponent 
    /// interface according to specifications provided as comments in the OpenMI.Standard2 
    /// source code.</para>
    /// 
    /// <para>§ 2) An OpenMI compliant component can also comply to one ore more extensions, 
    /// by implementing the IBaseLinkableComponent interface and the extension interfaces 
    /// which it wishes to comply to, according to the specifications provided as comments 
    /// in the OpenMI.Standard2 source code.</para>
    /// 
    /// <para>§ 3) An OpenMI-compliant component including its extensions must, 
    /// when compiled, reference the OpenMI.Standard2*.dlls/jars, which are compiled and 
    /// and released by the OpenMI Association.</para>
    /// 
    /// <para>§ 4) An OpenMI-compliant component must be associated with an XML file, the so called
    /// OMI file, which complies to (can be validated with) the LinkableComponent.xsd schema.</para>
    ///     
    /// <para>§ 5) An OpenMI-compliant component must be associated with an XML file, the so called
    /// compliancy info file, which complies to (can be validated with) the
    /// OpenMICompliancyInfo.xsd schema. 
    /// This file must be submitted to the OpenMI Association.</para>
    /// 
    /// <para>§ 6) The OpenMI Association provides two additional interfaces that 
    /// OpenMI-compliant components may or may not implement: the <see cref="IManageState"/> 
    /// interface and the <see cref="IByteStateConverter"/> interface. However, if these 
    /// interfaces are implemented, each method and property must be implemented according 
    /// to the comments given in the OpenMI.Standard2 source code.</para>
    /// 
    /// <para>§ 7) The OpenMI Association’s downloadable standard zip file provides the only 
    /// recognized version of source files, XML schemas and assembly files.</para>
    /// </summary>
    public interface IBaseLinkableComponent : IIdentifiable
    {
        /// <summary>
        /// Arguments needed to let the component do its work. An unmodifiable list
        /// of (modifiable) arguments must be returned that is to be used to get
        /// information about the arguments and to set argument values. Validation of
        /// changes can be done either when they occur (e.g. using notifications) or
        /// when the initialize method is called. Initialize will always be called
        /// before any call to the update method of the IBaseLinkableComponent.
        ///
        /// This property must be available as soon is the linkable component instance is created.
        /// Arguments describes the arguments that can be set before the
        /// Initialize() method is called.
        /// </summary>
        IList<IArgument> Arguments { get; }

        ///<summary>
        /// Defines current status of the linkable component. See <see cref="LinkableComponentStatus"/> for the
        /// possible values.
        /// <para>
        /// The first Status that a component sets is <see cref="LinkableComponentStatus.Created"/>, 
        /// as soon after it has been created. In this Status, <see cref="Arguments"/> is the 
        /// only property that may be accessed.
        /// </para>
        ///</summary>
        LinkableComponentStatus Status { get; }

        /// <summary>
        /// The StatusChanged event is fired when Status of the component changes.
        /// See <see cref="LinkableComponentStatus"/> for the possible states, and see
        /// the documentation, e.g. the OpenMI Standard 2 Specification,
        /// for possible state changes.
        /// </summary>
        event EventHandler<LinkableComponentStatusChangeEventArgs> StatusChanged;

        /// <summary>
        /// The list of input items for which a component can recieve values.
        /// </summary>
        /// <remarks>
        /// <para>This property must be accessible after the <see cref="Initialize()"/> method has been
        /// invoked and until the <see cref="Validate"/> method has been invoked. If this property
        /// is accessed before the <see cref="Initialize()"/> method has been invoked or after the
        /// <see cref="Validate"/> method has been invoked and the LinkableComponent cannot handle
        /// this an exception must be thrown.</para>
        /// <para>This method basically returns references to <see cref="IBaseInput"/> items.
        /// There is no guarantee that the list of objects is not altered by other components
        /// after it has been returned. It is the responsibility of the LinkableComponent
        /// to make sure that such possible alterations do not subsequently corrupt
        /// the LinkableComponent.</para>
        /// </remarks>
        IList<IBaseInput> Inputs { get; }
        
        /// <summary>
        /// The list of output items for which a component can produce results.
        /// </summary>
        /// <remarks>
        /// <para>This property must be accessible after the <see cref="Initialize()"/> method has been
        /// invoked and until the <see cref="Validate"/> method has been invoked. If this property
        /// is accessed before the <see cref="Initialize()"/> method has been invoked or after the
        /// <see cref="Validate"/> method has been invoked and the LinkableComponent cannot handle
        /// this an exception must be thrown.</para>
        /// <para>
        /// The list only contains the core IBaseOutput of the component, not
        /// the IBaseAdaptedOutput derived from each IBaseOutput (etc.). To get a complete
        /// list of outputs traverse the chain of IBaseAdaptedOutput that start with the
        /// IOutputs returned in the list.
        /// </para>
        /// <para>This method basically returns references to <see cref="IBaseOutput"/> items.
        /// There is no guarantee that the list of objects is not altered by other components
        /// after it has been returned. It is the responsibility of the LinkableComponent
        /// to make sure that such possible alterations do not subsequently corrupt
        /// the LinkableComponent.</para>
        /// </remarks>
        IList<IBaseOutput> Outputs { get; }

        ///<summary>
        /// Get a list of <see cref="IAdaptedOutputFactory"/>, each allowing to 
        /// create <see cref="IBaseAdaptedOutput"/> item for making outputs fit to inputs
        /// in case they do not already do so.
        /// <para>
        /// </para>
        /// Factories can be added to and removed from the list so that third-party
        /// factories and IBaseAdaptedOutput classes can be introduced.
        ///</summary>
        List<IAdaptedOutputFactory> AdaptedOutputFactories { get; }
        
        /// <summary>
        /// <para>Initializes the LinkableComponent.</para>
        /// 
        /// <para>The <see cref="Initialize()"/> method will and must be invoked before any other 
        /// method or property in the ILinkableComponent interface is invoked or accessed, except 
        /// for the <see cref="Arguments"/> property.</para>
        /// 
        /// <para>Immediatly after the method is invoked, it changes the
        /// linkable component's Status to <see cref="LinkableComponentStatus.Initializing"/>.</para> 
        ///
        /// <para>When the method is executed and an error occurs, the Status of the component 
        /// will change to <see cref="LinkableComponentStatus.Failed"/>, and an exception will 
        /// be thrown. If the component initializes succesfully, the  status is changed to 
        /// <see cref="LinkableComponentStatus.Initialized"/>.</para> 
        ///
        /// <para>When the <see cref="Initialize()"/> method has been finished and the Status 
        /// is <see cref="LinkableComponentStatus.Initialized"/>, the properties Id, 
        /// Caption, Description, <see cref="Inputs"/>, <see cref="Outputs"/>, have been set,
        /// and the method <see cref="Validate"/> can be called.</para>
        /// 
        /// <para>It is only required that the method <see cref="Initialize()"/> can be invoked once. 
        /// If the <see cref="Initialize()"/> method is invoked more than once and the LinkableComponent 
        /// cannot handle this; an exception must be thrown.</para>
        /// 
        /// <para>REMARKS:</para>
        /// <para>The method will typically populate the component based on the
        /// values specified in its arguments, which can be retrieved with
        /// getArguments. Settings can be used to read input files, allocate memory,
        /// and organize input and output exchange items.</para>
        /// </summary>
        void Initialize();

        /// <summary>
        /// <para>Validates the populated instance of the LinkableComponent</para>
        /// 
        /// <para>This method must be accessible after the <see cref="Initialize()"/> method has been
        /// invoked and until the <see cref="Finish"/> method has been invoked. If this property
        /// is accessed before the <see cref="Initialize()"/> method has been invoked or after the
        /// <see cref="Finish"/> method has been invoked and the LinkableComponent cannot handle
        /// this an exception must be thrown.</para> 
        ///
        /// <para>The method will and must be invoked after the various provider/consumer relations between
        /// this component's exchange items and the exchange items of other components present
        /// in the composition.</para> 
        ///
        /// <para>Immediatly after the method is invoked, it changes the
        /// linkable component's Status to <see cref="LinkableComponentStatus.Validating"/>.</para> 
        ///
        /// <para>When the Validate method has finished, the Status of the component has changed
        /// to either <see cref="LinkableComponentStatus.Valid"/> or 
        /// <see cref="LinkableComponentStatus.Invalid"/>.</para> 
        ///
        /// </summary>
        /// 
        /// <returns>
        /// Returns null or an array of strings of length null if there are no messages
        /// at all. If there are messages while the components Status is 
        /// <see cref="LinkableComponentStatus.Valid"/>, the messages are purely informative.
        /// If there are messages while the components Status is 
        /// <see cref="LinkableComponentStatus.Invalid"/>, at least one of the messages 
        /// indicates a fatal error.
        /// </returns>
        string[] Validate();

        /// <summary>
        /// <para>Prepares the IBaseLinkableComponent for calls to the <see cref="Update"/> method</para>
        /// 
        /// <para>Before Prepare is called, the component are not required to honor
        /// any type of action that retrieves values from the component. After Prepare
        /// is called, the component must be ready for providing values.
        /// </para>
        /// 
        /// <para>This method must be accessible after the <see cref="Initialize()"/> method has been
        /// invoked and until the <see cref="Finish"/> method has been invoked. If this property
        /// is accessed before the <see cref="Initialize()"/> method has been invoked or after the
        /// <see cref="Finish"/> method has been invoked and the LinkableComponent cannot handle
        /// this an exception must be thrown.</para>
        /// 
        /// <para>Immediatly after the method is invoked, it changes the
        /// linkable component's Status to <see cref="LinkableComponentStatus.Preparing"/>.</para> 
        ///
        /// <para>When the method has finished, the Status of the component has changed
        /// to either <see cref="LinkableComponentStatus.Updated"/> or 
        /// <see cref="LinkableComponentStatus.Failed"/>.</para> 
        /// 
        /// <para>It is only required that the Prepare( ) method can be invoked once. If the 
        /// Prepare method is invoked more that once and the LinkableComponent cannot handle 
        /// this an exception must be thrown.</para>
        /// </summary>
        void Prepare();

        ///<summary>
        /// This method is called to let the component update itself, thus reaching its next state.
        /// 
        /// <para>Immediately after the method is invoked, it changes the
        /// linkable component's Status to <see cref="LinkableComponentStatus.Updating"/>.</para> 
        ///
        /// <para>The type of actions a component takes during the <see cref="Update"/> method depends
        /// on the type of component. A numerical model that progresses in time will typically
        /// compute a time step. A database whould typically look at the consumers of its output items,
        /// and perform one or more queries to be able to provide the values that the consumers require.
        /// A GIS system would typically re-evaluate the values in a grid coverage, so that its output
        /// output items can provide up-to-date values.</para>
        /// 
        /// <para>If the Update method is performed succesfully, the component sets its state to
        /// <see cref="LinkableComponentStatus.Updated"/>, unless after this Update action the 
        /// component is at the end of its computation, in which case it will be set its State 
        /// to <see cref="LinkableComponentStatus.Done"/>.</para>
        /// 
        /// <para>If during the Update method a problem arises, the component sets its state to
        /// <see cref="LinkableComponentStatus.Failed"/>, and throws an exception.</para>
        /// 
        ///</summary>
        ///<param name="requiredOutput">This optional parameter lets the caller specify the specific
        /// output items that should be updated. If it is omitted or if the length is 0, the component 
        /// will at least update its output items that have consumers, or all its output items, 
        /// depending on the component's implementation.</param>
        void Update(params IBaseOutput[] requiredOutput);

        /// <summary>
        /// <para>This method is and must be invoked as the last of any methods in the
        /// ILinkableComponent interface.</para>
        /// 
        /// <para>This method must be accessible after the <see cref="Prepare"/> method has been invoked.
        /// If this method is invoked before the <see cref="Prepare"/> method has been invoked 
        /// and the LinkableComponent cannot handled this, an exception must be thrown.</para>
        /// 
        /// <para>Immediatly after the method is invoked, it changes the
        /// linkable component's Status to <see cref="LinkableComponentStatus.Finishing"/>. 
        /// Once the finishing is completed, the component changes its status to 
        /// <see cref="LinkableComponentStatus.Finished"/> if it can not be restarted,
        /// or <see cref="LinkableComponentStatus.Created"/> if it can.</para> 
        ///
        /// </summary>
        void Finish();

    }
}
