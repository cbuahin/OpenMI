using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Oatc.OpenMI.Sdk.Backbone;
using Oatc.OpenMI.Sdk.Spatial;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;

namespace Oatc.OpenMI.Sdk.UnitTests.Spatial
{
    [TestFixture]
    public class XYElementSearchTreeTest
    {
        [Test]
        public void PointSearchTest()
        {
            ElementSet elmtSet = ReadMesh(@"..\..\Spatial\Data\oresund.mesh");
            XYElementSearchTree<int> tree = XYElementSearchTree<int>.BuildSearchTree(elmtSet);

            // Point in element 746 (has index 745)
            XYPoint point = new XYPoint(355273.227764, 6201136.0892);
            XYExtent pointExtent = XYExtentUtil.GetExtent(point);
            ICollection<int> pointCandidates = tree.FindElements(pointExtent);
            Assert.Contains(745, (ICollection)pointCandidates);
            XYPolygon elmt745 = ElementMapper.CreateFromXYPolygon(elmtSet, 745);
            Assert.IsTrue(XYGeometryTools.IsPointInPolygon(point.X, point.Y, elmt745));

            // Point in element 3583 (has index 3582)
            XYPoint point2 = new XYPoint(354683.522377, 6167696.720773);
            XYExtent point2Extent = XYExtentUtil.GetExtent(point2);
            ICollection<int> point2Candidates = tree.FindElements(point2Extent);
            Assert.Contains(3582, (ICollection)point2Candidates);
            XYPolygon elmt3582 = ElementMapper.CreateFromXYPolygon(elmtSet, 3582);
            Assert.IsTrue(XYGeometryTools.IsPointInPolygon(point2.X, point2.Y, elmt3582));

            // Node 2001 is part of 7 elements
            XYPoint pointNode2001 = new XYPoint(355451.29058143805, 6167084.759883712);
            XYExtent pointNode2001Extent = XYExtentUtil.GetExtent(pointNode2001);
            ICollection<int> pointNode2001Candidates = tree.FindElements(pointNode2001Extent);
            Assert.AreEqual(7, pointNode2001Candidates.Count);

        }

        // TODO: Needs line and polygon search test

        [Test]
        public void CheckElementMapper()
        {
            ElementSet regularGrid = CreateRegularGridElmtSet(320000.0, 380000.0, 10000, 6120000.0, 6225000.0, 10000);
            //ElementSet regularGrid = CreateRegularGridElmtSet(320000.0, 380000.0, 5000, 6120000.0, 6225000.0, 5000);
            //ElementSet regularGrid = CreateRegularGridElmtSet(320000.0, 380000.0, 2500, 6120000.0, 6225000.0, 2500);
            //ElementSet regularGrid = CreateRegularGridElmtSet(320000.0, 380000.0, 1250, 6120000.0, 6225000.0, 1250);
            ElementSet mesh = ReadMesh(@"..\..\Spatial\Data\oresund.mesh");

            double y0 = 6120000.0;
            double x0 = 320000.0;

            // If this is not done, 
            //MoveSystemOrigo(mesh, x0, y0);
            //MoveSystemOrigo(regularGrid, x0, y0);

            ElementMapper mapper1 = new ElementMapper();
            mapper1.UseSearchTree = false;
            Stopwatch timer1 = new Stopwatch();
            timer1.Start();
            // Weighted sum
            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(ElementType.Polygon, ElementType.Polygon);
            mapper1.Initialise(methods[1], regularGrid, mesh);
            timer1.Stop();


            Console.Out.WriteLine("time 1 : " + timer1.Elapsed.TotalSeconds);

            DoubleSparseMatrix matrix1 = (DoubleSparseMatrix)mapper1.MappingMatrix;

            Assert.AreEqual(66, matrix1.ColumnCount);
            Assert.AreEqual(3636, matrix1.RowCount);
            Assert.AreEqual(4430, matrix1.Values.Count);

            // TODO: Pick out some test values
            foreach (KeyValuePair<DoubleSparseMatrix.Index, double> pair in matrix1.Values)
            {
                int row = pair.Key.Row;
                int col = pair.Key.Column;
//                Assert.AreEqual(pair.Value, string.Format("({0},{1})", row, col));
            }
        }


        /// <summary>
        /// ElementAreaOperationTest that makes two regular element sets, increasing both in size by a factor of 4 each time
        /// and computation time should also increase app. with a factor of 4.
        /// </summary>
        [Test]
        [Ignore("Run on demand")]
        public void PerformanceTest()
        {
            double[] dxs = new double[] { 80, 80, 40, 20, 10, 5, 2.5 };
            IIdentifiable[] methods = SpatialAdaptedOutputFactory.GetAvailableMethods(ElementType.Polygon, ElementType.Polygon);

            foreach (int dx in dxs)
            {
                ElementSet regularGrid1 = CreateRegularGridElmtSet(-0.3 * dx, 9 * 80, dx, -0.2 * dx, 9 * 80, dx);
                ElementSet regularGrid2 = CreateRegularGridElmtSet(0, 8 * 80, dx, 0, 8 * 80, dx);

                ElementMapper mapper = new ElementMapper();
                mapper.UseSearchTree = true;
                Stopwatch timer = new Stopwatch();
                timer.Start();
                // weighted sum
                mapper.Initialise(methods[1], regularGrid1, regularGrid2);
                timer.Stop();

                Console.Out.WriteLine("time for dx {0} = {1}", dx, timer.Elapsed.TotalSeconds);
            }
        }


        public static void MoveSystemOrigo(ElementSet mesh, double x0, double y0)
        {
            foreach (Element elmt in mesh.Elements)
            {
                foreach (Coordinate vertex in elmt.Vertices)
                {
                    vertex.X -= x0;
                    vertex.Y -= y0;
                }
            }
        }


        public static ElementSet CreateRegularGridElmtSet(double xmin, double xmax, double dx, double ymin, double ymax, double dy)
        {
            ElementSet elmtSet = new ElementSet("dummy");
            elmtSet.ElementType = ElementType.Polygon;

            double x = xmin;
            while (x < xmax)
            {
                double y = ymin;
                while (y < ymax)
                {
                    Element elmt = new Element();
                    elmt.AddVertex(new Coordinate(x, y, 0));
                    elmt.AddVertex(new Coordinate(x + dx, y, 0));
                    elmt.AddVertex(new Coordinate(x + dx, y + dy, 0));
                    elmt.AddVertex(new Coordinate(x, y + dy, 0));
                    elmtSet.AddElement(elmt);
                    y += dy;
                }
                x += dx;
            }
            return (elmtSet);
        }

        /// <summary>
        /// Read a DHI .mesh file and load all data into an ElementSet
        /// </summary>
        public static ElementSet ReadMesh(string filename)
        {
            TextReader tr = new StreamReader(filename, System.Text.Encoding.Default);
            string line;
            try
            {

                char[] separator = new char[] { ' ', '\t' };

                // Read header line
                line = tr.ReadLine();
                if (line == null)
                    throw new Exception("Can not load mesh file. File is empty");
                // Remove any leading spaces if present
                line = line.Trim();
                // split on space character
                string[] strings = line.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                // Read number of nodes
                int noNodes;
                if (!int.TryParse(strings[0], out noNodes))
                    throw new Exception(string.Format("Can not load mesh file (failed reading node header line): {0}", filename));
                // Read projection string
                string proj = strings[1];
                string wktString = proj.Trim();

                // Allocate memory for nodes
                int[] nodeIds = new int[noNodes];
                double[] x = new double[noNodes];
                double[] y = new double[noNodes];
                double[] z = new double[noNodes];
                int[] code = new int[noNodes];

                // Read nodes
                try
                {
                    for (int i = 0; i < noNodes; i++)
                    {
                        line = tr.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected end of file"); // used as inner exception

                        line = line.Trim();
                        strings = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        nodeIds[i] = int.Parse(strings[0]);
                        x[i] = double.Parse(strings[1], NumberFormatInfo.InvariantInfo);
                        y[i] = double.Parse(strings[2], NumberFormatInfo.InvariantInfo);
                        z[i] = double.Parse(strings[3], NumberFormatInfo.InvariantInfo);
                        code[i] = int.Parse(strings[4]);
                    }

                }
                catch (Exception inner)
                {
                    throw new Exception(string.Format("Can not load mesh file (failed reading nodes): {0}", filename), inner);
                }

                // Reading element header line
                int noElements;
                int maxNoNodesPerElement;
                int elmtCode;
                line = tr.ReadLine();
                if (line == null)
                    throw new Exception(string.Format("Can not load mesh file (unexpected end of file): {0}", filename));
                line = line.Trim();
                strings = line.Split(separator);
                if (strings.Length != 3)
                    throw new Exception(string.Format("Can not load mesh file (failed reading element header line): {0}", filename));
                try
                {
                    noElements = int.Parse(strings[0]);
                    maxNoNodesPerElement = int.Parse(strings[1]);
                    elmtCode = int.Parse(strings[2]);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Can not load mesh file (failed reading element header line): {0}", filename), ex);
                }

                // Element code must be 21 or 25 (21 for triangular meshes, 25 for mixed meshes)
                if (elmtCode != 21 || elmtCode != 25)
                {
                    // TODO: Do we care?
                }

                // Create elementset
                ElementSet elmtSet = new ElementSet("Dummy");
                elmtSet.ElementType = ElementType.Polygon;
                elmtSet.SpatialReferenceSystemWkt = wktString;

                // Read all elements
                try
                {
                    for (int i = 0; i < noElements; i++)
                    {
                        line = tr.ReadLine();
                        if (line == null)
                            throw new Exception("Unexpected end of file"); // used as inner exception

                        line = line.Trim();
                        strings = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        // Read element id
                        int elmtId = int.Parse(strings[0]);
                        // Create element
                        Element elmt = new Element(elmtId.ToString());

                        // figure out number of nodes
                        int noNodesInElmt = strings.Length - 1;
                        for (int j = 0; j < noNodesInElmt; j++)
                        {
                            int nodeNumber = int.Parse(strings[j + 1]);
                            // Check that the node number exists
                            if (nodeNumber < 0 || nodeNumber > noNodes) // used as inner exception:
                                throw new Exception("Node number in element table is negative or larger than number of nodes");
                            // It is only a node in the element if the node number is positive
                            if (nodeNumber > 0)
                            {
                                // Convert from number to index (subtract 1)
                                int nodeIndex = nodeNumber - 1;
                                // Add vertex to element
                                elmt.AddVertex(new Coordinate(x[nodeIndex], y[nodeIndex], 0));
                            }
                        }
                        if (elmt.VertexCount > 0)
                            elmtSet.AddElement(elmt);
                    }
                }
                catch (Exception inner)
                {
                    throw new Exception(string.Format("Can not load mesh file (failed reading elements): {0}", filename), inner);
                }
                return elmtSet;
            }
            finally
            {
                try
                {
                    tr.Close();
                }
                catch { }
            }

        }


    }
}