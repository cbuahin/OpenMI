/*
 * Copyright (c) 2005-2010, OpenMI Association
 * <http://www.openmi.org/>
 *
 * This file is part of openmi-standard2.jar
 *
 * openmi-standard2.jar is free software; you can redistribute it and/or
 * modify it under the terms of the Lesser GNU General Public License as
 * published by the Free Software Foundation; either version 3 of the
 * License, or (at your option) any later version.
 *
 * openmi-standard2.jar is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the Lesser GNU
 * General Public License for more details.
 *
 * You should have received a copy of the Lesser GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
package org.openmi.standard2;

import java.util.List;
import java.util.Observable;

/**
 * The IBaseLinkableComponent is the key interface in the OpenMI standard.
 * <p/>
 * OpenMI-compliance definition (The compliance refers to a set of basic interfaces as well as to
 * optional extension interfaces, e.g. for time and space dependent components):
 * <p/>
 * 1) An OpenMI-compliant component must implement the IBaseLinkableComponent
 * interface according to specifications provided as comments in the
 * OpenMI Standard version 2 interface source code.
 * <p/>
 * 2) An OpenMI compliant component can also comply to one ore more extensions,
 * by implementing the IBaseLinkableComponent interface and the extension interfaces
 * which it wishes to comply to, according to the specifications provided as comments
 * in the OpenMI Standard version 2 source code.</para>
 * <p/>
 * 3) An OpenMI-compliant component including its extensions must, when compiled, reference the
 * openmi-standard2*.jar, which are compiled and released by the OpenMI
 * Association.
 * <p/>
 * 4) An OpenMI-compliant component must be associated with a XML file, the so called
 * OMI file, which complies to (can be validated with) the LinkableComponent.xsd schema.
 * <p/>
 * 5) An OpenMI-compliant component must be associated with a XML file, the so called
 * compliancy info file, which complies to (can be validated with) the OpenMICompliancyInfo.xsd schema. This
 * file must be submitted to the OpenMI Association.
 * <p/>
 * 6) The OpenMI Association provides two additional interfaces that
 * OpenMI-compliant components may or may not implement: the IManageState
 * interface and the IByteStateConvertor interface. However, if these interfaces
 * are implemented, each method and property must be implemented according to
 * the comments given in the OpenMI Standard version 2 interface source code.
 * <p/>
 * 7) The OpenMI Association's downloadable OpenMI Standard version 2 zip file
 * provides the only recognized version of source code files, XML schemas and
 * library files.
 */
public interface IBaseLinkableComponent extends IIdentifiable {

    /**
     * Arguments needed to let the component do its work. An unmodifiable list
     * of (modifiable) arguments must be returned that is to be used to get
     * information about the arguments and to set argument values. Validation of
     * changes can be done either when they occur (e.g. using notifications) or
     * when the initialize method is called. Initialize will always be called
     * before any call to the update method of the linkable component.
     * <p/>
     * This property must be available as soon as the linkable component
     * instance is created.
     *
     * @return Unmodifiable list of IArgument for the component
     */
    public List<IArgument> getArguments();


    /**
     * Defines current status of the linkable component. See
     * {@link LinkableComponentStatus} for the possible values.
     * <p/>
     * The first status that a component sets is
     * LinkableComponentStatus.CREATED, as soon as it has been created. In
     * this status the only method that may be called validly is getArguments.
     * It should be used to modify settings (available as IArgument) of the
     * IBaseLinkableComponent before it will be initialized.
     *
     * @return current status of the component
     */
    public LinkableComponentStatus getStatus();


    /**
     * Returns an Observable to be used to receive notifications from the
     * component about status changes. See {@link LinkableComponentStatus} for
     * the possible states, and the OpenMI Standard version 2 documentation
     * for more information about the possible state changes.
     * <p/>
     * The argument passed in the Observer update call must be an instance of
     * the LinkableComponentStatusChangeEventArgs class.
     *
     * @return Observable for receiving status change notifications
     */
    public Observable getStatusChangedObservable();


    /**
     * The list of input items for which the component can receive results.
     * <p/>
     * Returns a list of the IBaseInput of this linkable component, in no
     * specific order. The list is only valid after the initialize method
     * has been called and until the validate method has been called. When it is
     * called before initialize or after validate and the component cannot
     * handle this, an IllegalStateException must be thrown.
     * <p/>
     * This method returns references to IBaseInput instances. Removing or adding
     * items through the list is not supported. Changes can be made to the items
     * (beware that this can also be done by others) and it is the responsibility
     * of the linkable component to make sure that such possible alterations do not
     * subsequently corrupt the component.
     *
     * @return List of the IBaseInputs for the linkable component
     * @throws IllegalStateException when inputs are not available yet
     */
    public List<IBaseInput> getInputs();


    /**
     * The list of output items for which the component can produce results.
     * <p/>
     * Returns a list of the IBaseOutput of this linkable component, in no
     * specific order. The list is only valid after the initialize method
     * has been called and until the validate method has been called. When it is
     * called before initialize or after validate and the component cannot
     * handle this, an IllegalStateException must be thrown.
     * <p/>
     * Note: The list only contains the core IBaseOutput of the component, not
     * the IBaseAdaptedOutput derived from each IBaseOutput (etc.). To get a complete
     * list of outputs traverse the chain of IBaseAdaptedOutput that start with the
     * IBaseOutputs returned in the list.
     * <p/>
     * This method returns references to IBaseOutput instances. Removing or adding
     * items through the list is not supported. Changes can be made to the items
     * (beware that this can also be done by others) and it is the responsibility
     * of the linkable component to make sure that such possible alterations do not
     * subsequently corrupt the component.
     *
     * @return List of the IBaseOutputs for the linkable component
     * @throws IllegalStateException when outputs are not available yet
     */
    public List<IBaseOutput> getOutputs();


    /**
     * Gets a list of {@link IAdaptedOutputFactory}, each allowing to create
     * {@link IBaseAdaptedOutput} items for making outputs fit to inputs in
     * case they do not already do so.
     * <p/>
     * Factories can be added to and removed from the list so that third-party
     * factories and IBaseAdaptedOutput classes can be introduced.
     *
     * @return List of IAdaptedOutputFactory known to the component
     */
    public List<IAdaptedOutputFactory> getAdaptedOutputFactories();


    /**
     * Initializes the linkable component. To be called before any other method
     * in the linkable component interface is invoked, except for getArguments.
     * Immediately after initialize is called it changes the linkable component
     * status to LinkableComponentStatus.INITIALIZING.
     * <p/>
     * When the method is executing and an error occurs, the status of the
     * component will change to LinkableComponentStatus.FAILED, and an exception
     * will be thrown. If the component initializes successfully, the status is
     * changed to LinkableComponentStatus.INITIALIZED.
     * <p/>
     * At the INITIALIZED state the id, caption, description, inputs and outputs
     * have been set and the validate method can be called.
     * <p/>
     * It is only required to call initialize once. If it is invoked more than
     * one time and the linkable component can not handle this an
     * IllegalStateException must be thrown.
     * <p/>
     * Note: The method will typically populate the component based on the
     * values specified in its arguments, which can be retrieved with
     * getArguments. Settings can be used to read input files, allocate memory,
     * and organize input and output exchange items.
     *
     * @throws IllegalStateException when the method is called at an inappropriate time
     */
    public void initialize();


    /**
     * Validates the populated instance of the linkable component. To be called
     * after the initialize method and before the finish method. If it is called
     * either before initialize or after finish and the component can not handle
     * this an IllegalStateException must be thrown.
     * <p/>
     * The method will and must be invoked after the various provider and
     * consumer relations between this component's exchange items and the
     * exchange items of other components present in the composition have been
     * established. After the method has been called it changes the status of
     * the component to LinkableComponentStatus.VALIDATING.
     * <p/>
     * When validate has finished the status of the component will have been
     * changed to either VALID or INVALID.
     * <p/>
     * The array of String returned can be null or contain zero or more messages
     * about the validation process. When the status is VALID the messages are
     * purely informative. When the status is INVALID at least one of the
     * messages indicates a fatal error.
     *
     * @return array of validation result messages, or null
     * @throws IllegalStateException when the method is called at an inappropriate time
     */
    public String[] validate();


    /**
     * Prepares the linkable component for calls to the update method.
     * <p/>
     * Before prepare is called the linkable component is not required to honor any
     * type of action that attempts to retrieve values from it. After prepare is
     * called it must be ready to provide the requested output.
     * <p/>
     * This method must be accessible after the initialize method has been
     * called and until the finish method has been invoked. If it is called
     * before initialize or after finish and the linkable component cannot
     * handle this an IllegalStateException must be thrown.
     * <p/>
     * Immediately after the method is invoked it changes the component's state
     * to LinkableComponentStatus.PREPARING. When the method has finished the
     * status of the component has changed to either UPDATED or FAILED.
     * <p/>
     * It is only required that the prepare method can be invoked once. If the
     * prepare method is invoked more than one time and the component can not
     * handle this an IllegalStateException must be thrown.
     *
     * @throws IllegalStateException when the method is called at an inappropriate time
     */
    public void prepare();


    /**
     * Requests the linkable component to update itself, thus reaching its next
     * state.
     * <p/>
     * Immediately after the method is invoked it changes the component status
     * to LinkableComponentStatus.UPDATING. During the update method a model
     * that progresses in time will typically compute a time step. A database
     * would typically look at the consumers of its outputs and perform one or
     * more queries to be able to provide the values requested. A GIS model
     * would e.g. re-evaluate the values in a grid coverage so that its outputs
     * can provide updated values.
     * <p/>
     * If the update is performed successfully the component sets its state to
     * LinkableComponentStatus.UPDATED. Unless after this update the component
     * is at the end of its computation, in which case it will set its state to
     * LinkableComponentStatus.DONE.
     * <p/>
     * When during the update a problem arises the component sets its state to
     * LinkableComponentStatus.FAILED and throws an exception.
     * <p/>
     * An array of required outputs can be provided as parameter, which
     * specifies the outputs that should be updated. If null is passed or a zero
     * length array the component will at least update its outputs that have
     * consumers, or all its outputs, depending on the component's
     * implementation. Note that adapted outputs based on core outputs of the
     * IBaseLinkableComponent can be included.
     *
     * @param requiredOutputs Array of IBaseOutput that need to be updated
     */
    public void update(IBaseOutput[] requiredOutputs);


    /**
     * To be invoked as the last of any of the methods in the interface. Allows
     * the linkable component to release and clean up any resources it has used
     * before the instance is destroyed.
     * <p/>
     * This method must be accessible after the prepare method has been called.
     * If finish is called before the prepare method and the linkable component
     * can not handle this an IllegalStateException must be thrown.
     * <p/>
     * Immediately after the method is called the linkable component changes its
     * status LinkableComponentStatus.FINISHING. Once the finishing is completed
     * the component state changes to LinkableComponentStatus.CREATED if it can
     * be initialized again for new usage. The state can also change to
     * LinkableComponentStatus.FINISHED when initialize can no longer be called
     * and the object instance is expected to be removed by the system.
     *
     * @throws IllegalStateException when the method is called at an inappropriate time
     */
    public void finish();
}
