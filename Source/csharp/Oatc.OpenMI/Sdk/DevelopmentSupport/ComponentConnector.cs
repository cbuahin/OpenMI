using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.DevelopmentSupport
{
  /// <summary>
  /// Abstract utility class for coupling components in
  /// user code (not using GUI). 
  /// </summary>
  public abstract class ComponentConnector
  {
    
    /// <summary>
    /// Connect the output defined by the <paramref name="outputId"/> with the input
    /// defined by the <paramref name="inputId"/>, and put the adaptors 
    /// listed by the id's <paramref name="adaptorIds"/> in between.
    /// <para>
    /// If any of the adaptors needs custom parameters, this method can not be used.
    /// </para>
    /// </summary>
    /// <param name="outputComponent">Component that has the output</param>
    /// <param name="outputId">Id of output</param>
    /// <param name="inputComponent">Component that has the input</param>
    /// <param name="inputId">Id of input</param>
    /// <param name="adaptorIds">Id of adaptors. They must be available from the output component</param>
    /// <param name="scale">If scale is different from 0, a linear operation adaptor is also added.</param>
    public static void Connect(ITimeSpaceComponent outputComponent, string outputId, ITimeSpaceComponent inputComponent, string inputId, IList<string> adaptorIds, double scale)
    {
      ITimeSpaceInput input = inputComponent.FindInputItem(inputId);
      ITimeSpaceOutput output = outputComponent.FindOutputItem(outputId);

      // End point of connection
      ITimeSpaceOutput leaf = output;

      // Add all adaptors
      foreach (string adaptorID in adaptorIds)
      {
        ITimeSpaceAdaptedOutput adaptor = outputComponent.FindAdaptor(adaptorID, leaf, input);
        if (adaptor == null)
          throw new Exception("Coult not find output adaptor with id: " + adaptorID);
        adaptor.Initialize();
        leaf = adaptor;
      }

      // Add a linear conversion adaptor
      if (scale != 0)
      {
        ITimeSpaceAdaptedOutput adaptor = outputComponent.FindAdaptor("LinearOperation", leaf, input);
        if (adaptor == null)
          throw new Exception("Coult not find output linear conversion adaptor");
        adaptor.Arguments[0].Value = scale;
        adaptor.Initialize();
        leaf = adaptor;
      }

      // Connect leaf adaptor/output to input
      leaf.AddConsumer(input);
    }

    /// <summary>
    /// Utility method that checks if the dimensions of the two units match.
    /// If the dimensions do not match, an error string that can be put into
    /// an exception is produced. If they match, null is returned.
    /// </summary>
    /// <param name="q1">First quantity to check</param>
    /// <param name="q2">Second quantity to check</param>
    /// <returns>Null if matching, otherwise an error string.</returns>
    public static string CheckIfDimensionsMatch(IQuantity q1, IQuantity q2)
    {
      IDimension dim1 = q1.Unit.Dimension;
      IDimension dim2 = q2.Unit.Dimension;

      int count = 0;
      string error = "Dimensions does not match:";

      foreach (DimensionBase dimBase in Enum.GetValues(typeof(DimensionBase)))
      {
        double dimbaseval1 = dim1.GetPower(dimBase);
        double dimbaseval2 = dim2.GetPower(dimBase);
        if (dimbaseval1 != dimbaseval2)
        {
          count++;
          error += string.Format(" ({0},{1},{2})", dimBase,
                                 dimbaseval1.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                 dimbaseval2.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
      if (count == 0)
        return null;
      return error;
    }

  }
}
