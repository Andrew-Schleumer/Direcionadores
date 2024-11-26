using SharpKml.Dom;

namespace Placemarks.Repository
{
    public interface IPlacemarksRepository
    {
        Kml ReadKmlFile();
    }
}
