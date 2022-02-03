using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIPipesWallsDoors
{
    class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public DelegateCommand PipeCommand { get; }
        public DelegateCommand WallCommand { get; }
        public DelegateCommand DoorCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            PipeCommand = new DelegateCommand(OnPipeCommand);
            WallCommand = new DelegateCommand(OnWallCommand);
            DoorCommand = new DelegateCommand(OnDoorCommand);
        }

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }


        private void OnDoorCommand()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Doors info", $"Количество дверей в проекте: {doors.Count}");

            RaiseShowRequest();
        }

        private void OnWallCommand()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var walls = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            double wallsVolume = 0;
            foreach (Wall wall in walls)
            {
                Parameter volumeParameter = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                double volumeValue = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                wallsVolume += volumeValue;
            }

            TaskDialog.Show("Wall info", $"Объем стен в проекте: {wallsVolume:F2} м³");

            RaiseShowRequest();
        }

        private void OnPipeCommand()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                          .OfClass(typeof(Pipe))
                          .Cast<Pipe>()
                          .ToList();

            TaskDialog.Show("Pipe info", $"Количество труб в проекте: {pipes.Count.ToString()}");
            
            RaiseShowRequest();
        }
    }
}
