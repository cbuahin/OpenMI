using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Backbone.Generic;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.Spatial
{
  public class LinearOperationAdaptedOutput : AbstractAdaptedOutput
  {
    double _a = 1.0;
    double _b;

    public LinearOperationAdaptedOutput(ITimeSpaceOutput adaptee)
      : base(adaptee.Id + " => LinearOperation", adaptee)
    {
        Caption = adaptee.Id + " => y = A*x + B";
      Description = "Performs a linear operation on the form: y = Ax + B";
      Arguments.Add(new ArgumentDouble("A", 1.0) { Description = "A in y = A*x + B" });
      Arguments.Add(new ArgumentDouble("B", 0.0) { Description = "B in y = A*x + B" });
    }

    public override void Initialize()
    {
      _a = (double)Arguments[0].Value;
      _b = (double)Arguments[1].Value;
    }

    public override ITimeSpaceValueSet GetValues(IBaseExchangeItem querySpecifier)
    {
      return Convert((ITimeSpaceValueSet)Adaptee.GetValues(querySpecifier));
    }

    public override void Refresh()
    {
      foreach (IBaseAdaptedOutput adaptedOutput in AdaptedOutputs)
      {
        adaptedOutput.Refresh();
      }
    }

    public override ITimeSpaceValueSet Values
    {
      get
      {
        return Convert((ITimeSpaceValueSet)Adaptee.Values);
      }
    }

    private ValueSetArray<double> Convert(ITimeSpaceValueSet values)
    {
      ValueSetArray<double> res = new ValueSetArray<double>();

      for (int n = 0; n < values.Values2D.Count; ++n)
      {
        double[] elmtValues = new double[values.Values2D[n].Count];
        res.Values2DArray.Add(elmtValues);

        for (int m = 0; m < values.Values2D[n].Count; m++)
        {
          elmtValues[m] = _a * (double)values.Values2D[n][m] + _b;
        }
      }

      return res;

    }

    public override ISpatialDefinition SpatialDefinition
    {
      get
      {
        if (_adaptee != null)
          return _adaptee.SpatialDefinition;
        return (base.SpatialDefinition);
      }
    }

    public override ITimeSet TimeSet
    {
      get { return _adaptee.TimeSet; }
    }

    public override IValueDefinition ValueDefinition
    {
      get { return Adaptee.ValueDefinition; }
    }

  }
}
