using SharpKml.Dom;
using SharpKml.Engine;
using SharpKml.Dom.GX;

namespace Placemarks.Repository
{
    public class PlacemarksRepository : IPlacemarksRepository
    {
        public Kml? ReadKmlFile()
        {
            TextReader reader = File.OpenText(".\\Resources\\DIRECIONADORES1.kml");
            KmlFile file = KmlFile.Load(reader);
            Kml? kml = file.Root as Kml;
            
            if (kml == null)
            {
                return null;
            }
            return kml;
        }
    }
}
