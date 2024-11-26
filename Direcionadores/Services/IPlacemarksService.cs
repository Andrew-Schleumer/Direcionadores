using Newtonsoft.Json.Linq;
using Placemarks.Models;
using SharpKml.Dom;

namespace Placemarks.Services
{
    public interface IPlacemarksService
    {
        Kml ReadKmlFile();

        JObject GetFilters();

        List<PlacemarkView> GetJson(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento);

        byte[] GetFilteredKml(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento);
    }
}
