using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Visual;

namespace Module_02_Challange
{
    [Transaction(TransactionMode.Manual)]
    public class Module02Challenge : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // pick elements and filter them into list
            UIDocument uidoc = uiapp.ActiveUIDocument;
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            TaskDialog.Show("Selected Elements", "I selected " + pickList.Count.ToString() + " elements!");

            // filter selected elements for model curves
            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    CurveElement curveElem = elem as CurveElement;
                    
                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        modelCurves.Add(curveElem);
                    }
                }
            }

            // collecting curve data
            foreach (CurveElement currentCurve in modelCurves)
            {
                Curve curve = currentCurve.GeometryCurve;
                //XYZ startPoint = curve.GetEndPoint(0);
                //XYZ endPoint = curve.GetEndPoint(1);

                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;
            }

            //create transaction with using statement
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Create Revit elements");

                Level newLevel = Level.Create(doc, 20);
                newLevel.Name = "Test";

                WallType wallType1 = GetWallTypeByName(doc, "Storefront");
                WallType wallType2 = GetWallTypeByName(doc, "Generic - 8\"");
                DuctType ductType1 = GetDuctTypeByName(doc, "Default");
                PipeType pipeType1 = GetPipeTypeByName(doc, "Default");
                MEPSystemType ductSystemType1 = GetDuctSystemType(doc, "Supply Air");
                MEPSystemType pipeSystemType1 = GetDuctSystemType(doc, "Hydronic Supply");

                foreach (CurveElement currentCurve in modelCurves)
                {
                   
                    Curve curve = currentCurve.GeometryCurve;
                    GraphicsStyle curveGS = currentCurve.LineStyle as GraphicsStyle;

                    //if (curveGS.Name == "A-GLAZ")
                    //{
                    //    Wall.Create(doc, curve, wallType1.Id, newLevel.Id, 20, 0, false, false);
                    //}

                    //if (curveGS.Name == "A-WALL")
                    //{
                    //    Wall.Create(doc, curve, wallType2.Id, newLevel.Id, 20, 0, false, false);
                    //}

                    //if(curveGS.Name == "M-DUCT")
                    //{
                    //    Duct newDuct = Duct.Create(doc, ductSystemType1.Id, ductType1.Id, newLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                    //}

                    //if(curveGS.Name == "P-PIPE")
                    //{
                    //    Pipe.Create(doc, pipeSystemType1.Id, pipeType1.Id, newLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                    //}


                    switch (curveGS.Name)
                    {
                        case "A-GLAZ":
                            Wall.Create(doc, curve, wallType1.Id, newLevel.Id, 20, 0, false, false);
                            break;

                        case "A-WALL":
                            Wall.Create(doc, curve, wallType2.Id, newLevel.Id, 20, 0, false, false);
                            break;

                        case "M-DUCT":
                            Duct newDuct = Duct.Create(doc, ductSystemType1.Id, ductType1.Id, newLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                            break;

                        case "P-PIPE":
                            Pipe.Create(doc, pipeSystemType1.Id, pipeType1.Id, newLevel.Id, curve.GetEndPoint(0), curve.GetEndPoint(1));
                            break;

                            //default:
                            //Wall.Create(doc, curve, newLevel.Id, false);
                            //break;
                    }

                }



                t.Commit();
            }
                 

            return Result.Succeeded;
        }

        internal WallType GetWallTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collectorW = new FilteredElementCollector(doc);
            collectorW.OfClass(typeof(WallType));

            foreach (WallType curWType in collectorW)
            {
                if (curWType.Name == typeName)
                {
                    return curWType;
                }
            }

            return null;
        }

        internal PipeType GetPipeTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collectorP = new FilteredElementCollector(doc);
            collectorP.OfClass(typeof(PipeType));

            foreach (PipeType curPType in collectorP)
            {
                if (curPType.Name == typeName)
                {
                    return curPType;
                }
            }

            return null;
        }

        internal MEPSystemType GetPipeSystemType(Document doc, string typeName)
        {
            FilteredElementCollector systemCollectorP = new FilteredElementCollector(doc);
            systemCollectorP.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curPMEPType in systemCollectorP)
            {
                if (curPMEPType.Name == typeName)
                {
                    return curPMEPType;
                }
            }

            return null;
        }

        internal DuctType GetDuctTypeByName(Document doc, string typeName)
        {
            FilteredElementCollector collectorD = new FilteredElementCollector(doc);
            collectorD.OfClass(typeof(DuctType));

            foreach (DuctType curDType in collectorD)
            {
                if (curDType.Name == typeName)
                {
                    return curDType;
                }
            }

            return null;
        }

        internal MEPSystemType GetDuctSystemType (Document doc, string typeName)
        {
            FilteredElementCollector systemCollectorD = new FilteredElementCollector(doc);
            systemCollectorD.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType curDMEPType in systemCollectorD)
            {
                if (curDMEPType.Name == typeName)
                {
                    return curDMEPType;
                }
            }

            return null;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnModule02Challenge";
            string buttonTitle = "Button 1";

            Utils.ButtonDataClass myButtonData1 = new Utils.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }

}
