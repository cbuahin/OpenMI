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

package org.openmi.standard2.timespace;

import org.openmi.standard2.IBaseAdaptedOutput;

/**
 * An {@link ITimeSpaceAdaptedOutput} adds one or more data operations on top of an output item.
 * It is in itself an {@link IBaseAdaptedOutput}. The adaptedOutput extends an output
 * item with functionality as spatial interpolation, temporal interpolation, unit conversion
 * etc.
 * <p/>ITimeSpaceAdaptedOutput instances are created by means of an {@link org.openmi.standard2.IAdaptedOutputFactory}.
 * <p/>
 * <p/>The IAdaptedOutput is based on the adaptor design pattern. It adapts
 * an IOutput or another IAdaptedOutput to make it suitable for new use or
 * purpose. The object being adapted is typically called the "adaptee". The
 * IAdaptedOutput replaces the DataOperation that was used in OpenMI 1.x.
 */
public interface ITimeSpaceAdaptedOutput extends IBaseAdaptedOutput, ITimeSpaceOutput {

}