using SharpKml.Dom;

namespace Placemarks.Models
{
    public class PlacemarkView
    {
        public string? Name { get; set; } = string.Empty;
        public string? RuaCruzamento { get; set; } = string.Empty;
        public string? Referencia { get; set; } = string.Empty;
        public string? Bairro { get; set; } = string.Empty;
        public string? Situacao { get; set; } = string.Empty;
        public string? Cliente { get; set; } = string.Empty;
        public string? Data { get; set; } = string.Empty;
        public string? Coordenadas { get; set; } = string.Empty;

        public PlacemarkView() { }

        public PlacemarkView(string? name, string? ruaCruzamento, string? referencia, string? bairro, string? situacao, string? cliente, string? data, string? coordenadas)
        {
            Name = name;
            RuaCruzamento = ruaCruzamento;
            Referencia = referencia;
            Bairro = bairro;
            Situacao = situacao;
            Cliente = cliente;
            Data = data;
            Coordenadas = coordenadas;
        }

        public PlacemarkView(Placemark placemark)
        {
            Name = placemark.Name;
            foreach (var item in placemark.ExtendedData.Data)
            {
                if (item.Name == "CLIENTE")
                {
                    Cliente = item.Value;
                }
                else if (item.Name == "SITUAÇÃO")
                {
                    Situacao = item.Value;
                }
                else if (item.Name == "BAIRRO")
                {
                    Bairro = item.Value;
                }
                else if (item.Name == "REFERENCIA")
                {
                    Referencia = item.Value;
                }
                else if (item.Name == "RUA/CRUZAMENTO")
                {
                    RuaCruzamento = item.Value;
                }
                else if (item.Name == "DATA")
                {
                    Data = item.Value;
                }
                else if (item.Name == "COORDENADAS")
                {
                    Coordenadas = item.Value;
                }
            }
        }
    }
}
