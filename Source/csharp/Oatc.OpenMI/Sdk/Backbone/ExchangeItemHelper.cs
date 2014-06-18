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
using System.Collections.Generic;
﻿using OpenMI.Standard2;
﻿using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Backbone
{
  /// <summary>
  /// This class offers support in check whether exchange items are connectable, whether an
  /// additional consumer can be added to an (adapted)output etc.
  /// </summary>
  public class ExchangeItemHelper
  {
    /// <summary>
    /// Check if a consumer can be added to an output or adapted output. Throw an exception if not.
    /// </summary>
    /// <param name="provider">The output item</param>
    /// <param name="consumer">The new consumer</param>
    public static void CheckProviderConsumerConnectable(IBaseOutput provider, IBaseInput consumer)
    {
      if (!ProviderConsumerConnectable(provider, consumer))
      {
        throw new Exception("consumer \"" + consumer.Caption +
          "\" and provider \"" + provider.Caption +
          "\" are not connectable, put an adapted output in between");
      }
    }

    /// <summary>
    /// Check if a consumer can be added to an output or adapted output. If both the output and the
    /// new consumer are time space items, the element sets and the time set types are checked.
    /// </summary>
    /// <param name="provider">The output item</param>
    /// <param name="consumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ProviderConsumerConnectable(IBaseOutput provider, IBaseInput consumer)
    {
      if (provider is ITimeSpaceOutput && consumer is ITimeSpaceInput)
      {
        return ProviderConsumerConnectableForTimeAndElementSet((ITimeSpaceOutput)provider, (ITimeSpaceInput)consumer);
      }
      return true;
    }

    /// <summary>
    /// Check if a provider and a consumer can be connected, as far as the
    /// time set types are concerned (has durations or not).
    /// </summary>
    /// <param name="provider">The output item</param>
    /// <param name="consumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ProviderConsumerConnectableForTimeSet(ITimeSpaceOutput provider, ITimeSpaceInput consumer)
    {
      return ProviderConsumerConnectableForTimeAndOrElementSet(provider, consumer, true, false);
    }

    /// <summary>
    /// Check if a provider and a consumer can be connected, as far as the
    /// element set is concerned (type and sizes).
    /// </summary>
    /// <param name="provider">The output item</param>
    /// <param name="consumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ProviderConsumerConnectableForElementSet(ITimeSpaceOutput provider, ITimeSpaceInput consumer)
    {
      return ProviderConsumerConnectableForTimeAndOrElementSet(provider, consumer, false, true);
    }

    /// <summary>
    /// Check if a provider and a consumer can be connected, for both the
    /// time set types (has durations or not) and the element set (type and sizes).
    /// </summary>
    /// <param name="provider">The output item</param>
    /// <param name="consumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ProviderConsumerConnectableForTimeAndElementSet(ITimeSpaceOutput provider, ITimeSpaceInput consumer)
    {
      return ProviderConsumerConnectableForTimeAndOrElementSet(provider, consumer, true, true);
    }

    /// <summary>
    /// Check if a new consumer is compatible with existing consumers. Throw an exception if not.
    /// </summary>
    /// <param name="outputItem">The output item</param>
    /// <param name="newConsumer">The new consumer</param>
    public static void CheckConsumersCompatible(IBaseOutput outputItem, IBaseInput newConsumer)
    {
      if (!ConsumersCompatible(outputItem, newConsumer))
      {
        throw new Exception("consumer \"" + newConsumer.Caption +
          "\" can not be added to \"" + outputItem.Caption +
          "\", because it is incompatible with existing consumers");
      }
    }

    /// <summary>
    /// Check if a consumer can be added to an output or adapted output. If both the output and the
    /// new consumer are time space items, the element sets and the time set types are checked.
    /// </summary>
    /// <param name="outputItem">The output item</param>
    /// <param name="newConsumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ConsumersCompatible(IBaseOutput outputItem, IBaseInput newConsumer)
    {
      if (outputItem is ITimeSpaceOutput && newConsumer is ITimeSpaceInput)
      {
        return ConsumersCompatibleForTimeAndElementSet((ITimeSpaceOutput)outputItem, (ITimeSpaceInput)newConsumer);
      }
      return true;
    }

    /// <summary>
    /// Check if a time/space consumer can be added to an output or adapted output, as far as the
    /// time set types are concerned (has durations or not).
    /// </summary>
    /// <param name="outputItem">The output item</param>
    /// <param name="newConsumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ConsumersCompatibleForTimeSet(ITimeSpaceOutput outputItem, ITimeSpaceInput newConsumer)
    {
      return ConsumersCompatibleForTimeAndOrElementSet(outputItem, newConsumer, true, false);
    }

    /// <summary>
    /// Check if a time/space consumer can be added to an output or adapted output, as far as the
    /// element set is concerned (type and sizes).
    /// </summary>
    /// <param name="outputItem">The output item</param>
    /// <param name="newConsumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ConsumersCompatibleForElementSet(ITimeSpaceOutput outputItem, ITimeSpaceInput newConsumer)
    {
      return ConsumersCompatibleForTimeAndOrElementSet(outputItem, newConsumer, false, true);
    }

    /// <summary>
    /// Check if a time/space consumer can be added to an output or adapted output, for both the
    /// time set types (has durations or not) and the element set (type and sizes).
    /// </summary>
    /// <param name="outputItem">The output item</param>
    /// <param name="newConsumer">The new consumer</param>
    /// <returns>True if the consumer can be added indeed</returns>
    public static bool ConsumersCompatibleForTimeAndElementSet(ITimeSpaceOutput outputItem, ITimeSpaceInput newConsumer)
    {
      return ConsumersCompatibleForTimeAndOrElementSet(outputItem, newConsumer, true, true);
    }

    /// <summary>
    /// Check if the element set and the time set of a provider fit accoring to what the consumer
    /// requires. This method can be used to check of the providing component yet needs to do another Update() step.
    /// </summary>
    /// <param name="provider">The provider</param>
    /// <param name="consumer">The consumer</param>
    /// <returns>True if both element set and the time set fit</returns>
    public static bool OutputAndInputFit(ITimeSpaceExchangeItem provider, ITimeSpaceExchangeItem consumer)
    {
      bool timeFits = OutputAndInputTimeSetsFit(provider, consumer);
      bool elementSetFits = true;
      if (timeFits)
      {
        elementSetFits = OutputAndInputElementSetsFit(provider, consumer);
      }
      return timeFits && elementSetFits;
    }

    /// <summary>
    /// Check if the time set of a provider fits accoring to what the consumer
    /// requires. This method can be used to check of the providing component yet needs to do another Update() step.
    /// </summary>
    /// <param name="provider">The provider</param>
    /// <param name="consumer">The consumer</param>
    /// <returns>True if the time set fits</returns>
    public static bool OutputAndInputTimeSetsFit(ITimeSpaceExchangeItem provider, ITimeSpaceExchangeItem consumer)
    {
      if (provider == null)
      {
        throw new ArgumentNullException("provider");
      }

      if (consumer == null)
      {
        throw new ArgumentNullException("consumer");
      }

      bool timeFits = true;
      ITimeSet sourceTimeSet = provider.TimeSet;
      ITimeSet targetTimeSet = consumer.TimeSet;
      if (sourceTimeSet == null)
      {
        if (targetTimeSet != null)
        {
          // NOTE: Source has no timeset specification, source has.
          // Source fits target if target requires only one time step.
          timeFits = targetTimeSet.Times.Count == 1;
        }
      }
      else
      {
        if (targetTimeSet == null)
        {
          // NOTE: Target has no timeset specification, source has.
          // Source fits target if source has values for only one time step available.
          timeFits = sourceTimeSet.Times.Count == 1;
        }
        else
        {
          /*
           * SH/AM: TODO I Think this code is wrong, IOutput and IAdaptedOutput should be treated the same
           * (SH: reactivated (if (provider is IAdaptedOutput) code
           * to make things work for time extrapolators again.
           */
          // Both source and target have time set specification
          if (provider is ITimeSpaceAdaptedOutput)
          {
            // NOTE: Source is an adaptedOutput that has a time set.
            // Most probably a timeinterpolator, but:
            // TODO: Check how we can find out that it is a time interpolator.
            // For now: check if the target's last required time is included in the source's time horizon
            if (sourceTimeSet.TimeHorizon == null)
            {
              throw new Exception("Error when checking if the times of AdaptedOutput \"" + provider.Id +
                        " fits the times required by inputItem \"" + consumer.Id +
                        "\": no time horizon available in the adaptedOutput");
            }
            ITime lastRequiredTime = targetTimeSet.Times[targetTimeSet.Times.Count - 1];
            double lastRequiredTimeAsMJD = lastRequiredTime.End().StampAsModifiedJulianDay;
            double endOfSourceTimeHorizon = sourceTimeSet.TimeHorizon.End().StampAsModifiedJulianDay;
            timeFits = lastRequiredTimeAsMJD <= (endOfSourceTimeHorizon + Time.EpsilonForTimeCompare);
          }
          else
          {
            timeFits = false;
            // regular (output) exchange item, check if all times fit
            IList<ITime> sourceTimes = sourceTimeSet.Times;
            IList<ITime> requiredTimes = targetTimeSet.Times;
            if (sourceTimes.Count == requiredTimes.Count)
            {
              timeFits = true;
              for (int timeIndex = 0; timeIndex < requiredTimes.Count; timeIndex++)
              {
                if ((requiredTimes[timeIndex].DurationInDays > 0 && !(sourceTimes[timeIndex].DurationInDays > 0)) ||
                   (sourceTimes[timeIndex].DurationInDays > 0 && !(requiredTimes[timeIndex].DurationInDays > 0)))
                {
                  throw new Exception("Incompatible times (stamp versus span) between outputItem \"" + provider.Id +
                            " and inputItem \"" + consumer.Id + "\"");
                }
                if (requiredTimes[timeIndex].Equals(sourceTimes[timeIndex])) continue;
                timeFits = false;
                break;
              }
            }
          }
        }
      }
      return timeFits;
    }

    /// <summary>
    /// Check if the element set of a provider fits accoring to what the consumer
    /// requires. This method can be used to check of the providing component yet needs to do another Update() step.
    /// </summary>
    /// <param name="provider">The provider</param>
    /// <param name="consumer">The consumer</param>
    /// <returns>True if the element set fits</returns>
    public static bool OutputAndInputElementSetsFit(ITimeSpaceExchangeItem provider, ITimeSpaceExchangeItem consumer)
    {
      if (provider == null)
      {
        throw new ArgumentNullException("provider");
      }

      if (consumer == null)
      {
        throw new ArgumentNullException("consumer");
      }

      bool elementSetFits = true;
      IElementSet sourceElementSet = provider.ElementSet();
      IElementSet targetElementSet = consumer.ElementSet();
      if (sourceElementSet == null)
      {
        if (targetElementSet != null)
        {
          // NOTE: Source has no elementset specification, source has.
          // Source fits target if target requires only one element.
          elementSetFits = targetElementSet.ElementCount == 1;
        }
      }
      else
      {
        if (targetElementSet == null)
        {
          // NOTE: Target has no elementset specification, source has.
          // Source fits target if source has values for only one element available.
          elementSetFits = sourceElementSet.ElementCount == 1;
        }
        else
        {
          // Both source and target have an element set specification
          // If the source is a regular exchange item, the #elements will fit
          // (has been checked configuration time)

          // If it is a spatial extension, we need to check if valeus on the newly required
          // element set can be delivered
          if (provider is ITimeSpaceAdaptedOutput)
          {
            // TODO: Check how we can find out that it is a spatial adaptedOutput.
            // TODO: If it is, how do we check whether the values on the target element set can be delivered
          }
        }
      }
      return elementSetFits;
    }

    /// <summary>
    /// Check if the content of a value set is consistent with the number of time steps and the
    /// number of elements defined by the exchange item.
    /// </summary>
    /// <param name="exchangeItem">The exchange item specifying time and space</param>
    /// <param name="valueSet">The value set to be checked</param>
    public static void CheckValueSizes(ITimeSpaceExchangeItem exchangeItem, ITimeSpaceValueSet valueSet)
    {
      int timesCount = 1;
      if (exchangeItem.TimeSet != null)
      {
        if (exchangeItem.TimeSet.Times != null)
        {
          timesCount = exchangeItem.TimeSet.Times.Count;
        }
        else
        {
          timesCount = 0;
        }
      }

      if (ValueSet.GetTimesCount(valueSet) != timesCount)
      {
        throw new Exception("ExchangeItem \"" + exchangeItem.Caption +
          "\": Wrong #times in valueSet (" + ValueSet.GetTimesCount(valueSet) + "), expected #times (" + timesCount + ")");
      }

      int elementCount = 1;
      if (exchangeItem.ElementSet() != null)
      {
        elementCount = exchangeItem.ElementSet().ElementCount;
      }
      if (ValueSet.GetElementCount(valueSet) != elementCount)
      {
        throw new Exception("ExchangeItem \"" + exchangeItem.Caption +
          "\": Wrong #times in valueSet (" + ValueSet.GetElementCount(valueSet) + "), expected #times (" + elementCount + ")");
      }
    }

    private static bool ConsumersCompatibleForTimeAndOrElementSet(IBaseOutput outputItem, IBaseInput consumer, bool doCheckTime, bool doCheckSpace)
    {
      // Check which time/space consumers are already there
      bool canBeAdded = true;
      if (outputItem.Consumers.Count > 0)
      {
        if (doCheckSpace)
        {
          // TODO (JG): make instead check on exchangeItem.ElementSet instead of an arbitray elementSet
          // check if #elements are consistent
          if (consumer.ElementSet() != null &&
              outputItem.Consumers[0].ElementSet() != null &&
              consumer.ElementSet().ElementCount != outputItem.Consumers[0].ElementSet().ElementCount)
          {
            canBeAdded = false;
          }
        }
        if (doCheckTime)
        {
          ITimeSpaceInput consumerAsTimeSpace = consumer as ITimeSpaceInput;
          if (consumerAsTimeSpace != null)
          {
            List<ITimeSpaceInput> existingConsumers = new List<ITimeSpaceInput>();
            foreach (IBaseInput existingConsumer in outputItem.Consumers)
            {
              if (existingConsumer is ITimeSpaceOutput)
              {
                existingConsumers.Add((ITimeSpaceInput)existingConsumer);
              }
            }
            if (existingConsumers.Count > 0)
            {
              // TODO (JG): Is this a requirement?
              if (consumerAsTimeSpace.TimeSet != null &&
                  existingConsumers[0].TimeSet != null &&
                  consumerAsTimeSpace.TimeSet.HasDurations != existingConsumers[0].TimeSet.HasDurations
                )
              {
                canBeAdded = false;
              }
            }
          }
        }
      }
      return canBeAdded;
    }

    private static bool ProviderConsumerConnectableForTimeAndOrElementSet(IBaseOutput provider, IBaseInput consumer, bool doCheckTime, bool doCheckSpace)
    {
      // Check which time/space consumers are already there
      if (doCheckSpace)
      {
        // check if #elements and elementType are consistent
        if (consumer.ElementSet() != null &&
          provider.ElementSet() != null
          )
        {
          if (consumer.ElementSet().ElementCount != provider.ElementSet().ElementCount)
          {
            return false;
          }
          if (consumer.ElementSet().ElementType != provider.ElementSet().ElementType &&
            consumer.ElementSet().ElementCount != 1)
          {
            return false;
          }
        }
      }
      if (doCheckTime)
      {
        ITimeSpaceInput consumerAsTimeSpace = consumer as ITimeSpaceInput;
        ITimeSpaceInput providerAsTimeSpace = provider as ITimeSpaceInput;
        if (consumerAsTimeSpace != null && providerAsTimeSpace != null)
        {
          if (consumerAsTimeSpace.TimeSet != null &&
              providerAsTimeSpace.TimeSet != null &&
              consumerAsTimeSpace.TimeSet.HasDurations != providerAsTimeSpace.TimeSet.HasDurations)
          {
            return false;
          }
        }
      }

      return true;
    }

    public static ITime GetEarliestConsumerTime(IBaseOutput output)
    {
      ITime earliestRequiredTime = null;
      foreach (IBaseAdaptedOutput adaptedOutput in output.AdaptedOutputs)
      {
        earliestRequiredTime = CheckTimeHorizonMinimum(earliestRequiredTime, GetEarliestConsumerTime(adaptedOutput));
      }
      foreach (IBaseInput input in output.Consumers)
      {
        ITimeSpaceInput timeSpaceInput = input as ITimeSpaceInput;
        if (timeSpaceInput != null && timeSpaceInput.TimeSet != null)
        {
          earliestRequiredTime = CheckTimeHorizonMinimum(earliestRequiredTime, timeSpaceInput.TimeSet.TimeHorizon);
        }
      }
      return earliestRequiredTime;
    }

    private static ITime CheckTimeHorizonMinimum(ITime currentEarliestRequiredTime, ITime consumerEarliestTime)
    {
      if (currentEarliestRequiredTime == null)
        return (consumerEarliestTime);

      if (consumerEarliestTime != null)
      {
        if (consumerEarliestTime.StampAsModifiedJulianDay < currentEarliestRequiredTime.StampAsModifiedJulianDay)
        {
          return consumerEarliestTime;
        }
      }
      return currentEarliestRequiredTime;
    }
  }
}
