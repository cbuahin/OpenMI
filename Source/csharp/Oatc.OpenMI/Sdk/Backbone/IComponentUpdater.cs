using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// Generic interface for utility classes that can update
  /// a component.
  /// </summary>
  public interface IComponentUpdater
  {
    /// <summary>
    /// Update component for the given specification
    /// </summary>
    /// <param name="querySpecifier">Specification that defines the update</param>
    /// <returns>True if the update was successfull.</returns>
    bool Update(IBaseExchangeItem querySpecifier);
  }

  /// <summary>
  /// An <see cref="IComponentUpdater"/> that will try update a component 
  /// until a specific <see cref="ITimeSpaceOutput"/> item has reached
  /// the time of a query item.
  /// <para>
  /// If the component is "busy" (<see cref="LinkableComponentStatus.Updating"/>)
  /// then the component will not be updated.
  /// </para>
  /// </summary>
  public class TimeComponentUpdater : IComponentUpdater
  {
    private readonly ITimeSpaceOutput _output;

    public TimeComponentUpdater(ITimeSpaceOutput output)
    {
      _output = output;
    }

    public bool Update(IBaseExchangeItem basequerySpecifier)
    {
      return Update(_output, basequerySpecifier);
    }

    public static bool Update(ITimeSpaceOutput output, IBaseExchangeItem basequerySpecifier)
    {
      ITimeSpaceExchangeItem querySpecifier = basequerySpecifier as ITimeSpaceExchangeItem;
      if (querySpecifier == null)
        throw new ArgumentException("querySpecifier must be an ITimeSpaceExchangeItem - add an adaptor");

      // Time set of query must be defined and have at least 1 time
      if (querySpecifier.TimeSet == null ||
          querySpecifier.TimeSet.Times == null ||
          querySpecifier.TimeSet.Times.Count == 0)
      {
        throw new Exception("Given the TimeSet of output item \"" + output.Id +
                            "\", it can not produce one set of values for \"" + querySpecifier.Id + "\"");
      }

      // Output time set must be defined
      if (output.TimeSet == null || output.TimeSet.Times == null)
      {
        throw new Exception("Invalid time specifier in output item \"" + output.Id +
                            "\" for in updating according to a time specification" + querySpecifier.Id);
      }

      // Compute until this time is available
      double queryTimeMjd = querySpecifier.TimeSet.Times[0].End().StampAsModifiedJulianDay;

      // The current available time from the output item
      double availableTimeMjd = Double.NegativeInfinity;
      if (output.TimeSet.Times.Count > 0)
        availableTimeMjd = output.TimeSet.Times[output.TimeSet.Times.Count - 1].End().StampAsModifiedJulianDay;

      // Update component until querytime is available
      // If component is "busy" (LinkableComponentStatus.Updating), the
      // component will not be updated.
      IBaseLinkableComponent component = output.Component;
      while ((component.Status == LinkableComponentStatus.Valid ||
              component.Status == LinkableComponentStatus.Updated) &&
             availableTimeMjd + Time.EpsilonForTimeCompare < queryTimeMjd)
      {
        component.Update();
        availableTimeMjd = output.TimeSet.Times[output.TimeSet.Times.Count - 1].End().StampAsModifiedJulianDay;
      }

      // Return true if component was updated up until queryTimeMjd
      return (availableTimeMjd + Time.EpsilonForTimeCompare >= queryTimeMjd);
    }
  }
}
