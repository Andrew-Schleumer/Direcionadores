using Placemarks.Repository;
using Newtonsoft.Json.Linq;
using SharpKml.Dom;
using SharpKml.Engine;
using Placemarks.Exceptions;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Placemarks.Models;
using System.Security.Cryptography.Xml;
using SharpKml.Base;

namespace Placemarks.Services
{
    public class PlacemarksService : IPlacemarksService
    {
        private readonly IPlacemarksRepository _placemarkRepository;

        public PlacemarksService(IPlacemarksRepository placemarkRepository)
        {
            _placemarkRepository = placemarkRepository;
        }

        public Kml ReadKmlFile()
        {
            return _placemarkRepository.ReadKmlFile();
        }

        public void CheckFilters(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento)
        {
            JObject filters = GetFilters();

            if (cliente != null && cliente.Trim() != "" && !filters.GetValue("Clientes").Values().Contains(cliente))
            {
                throw new FilteringException("Cliente: " + cliente + " invalido");
            }
            else if (situacao != null && situacao.Trim() != "" && !filters.GetValue("Situações").Values().Contains(situacao))
            {
                throw new FilteringException("Situação: " + situacao + " invalido");
            }
            else if (bairro != null && bairro.Trim() != "" && !filters.GetValue("Bairros").Values().Contains(bairro))
            {
                throw new FilteringException("Bairro: " + bairro + " invalido");
            }
            else if (referencia != null && referencia.Trim() != "" && referencia.Length < 3)
            {
                throw new FilteringException("Referencia: " + referencia + " invalido");
            }
            else if (ruaCruzamento != null && ruaCruzamento.Trim() != "" && ruaCruzamento.Length < 3)
            {
                throw new FilteringException("Rua/Cruzamento: " + ruaCruzamento + " invalido");
            } else if((cliente == null || cliente.Trim() == "") && (situacao == null || situacao.Trim() == "") && (bairro == null || bairro.Trim() == "") && (referencia == null || referencia.Trim() == "") && (ruaCruzamento == null || ruaCruzamento.Trim() == ""))
            {
                throw new FilteringException("Nenhum filtro fornecido.");
            }
        }

        public byte[] GetFilteredKml(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento)
        {
            CheckFilters(cliente, situacao, bairro, referencia, ruaCruzamento);

            List<Placemark> placemarks = new List<Placemark>();

            Kml filteredKml = new Kml();

            Kml kml = ReadKmlFile();

            Document document = new Document();
            document.Name = "";
            filteredKml.Feature = document;

            Folder folder = new Folder();
            folder.Name = "Placemarks Filtrados";
            document.AddFeature(folder);

            foreach (var placemark in kml.Flatten().OfType<Placemark>().ToArray())
            {
                foreach (var item in placemark.ExtendedData.Data)
                {
                    if (cliente != null && cliente != "" && item.Name == "CLIENTE" && item.Value == cliente)
                    {
                        folder.AddFeature(placemark.Clone());
                        break;
                    }
                    else if (situacao != null && situacao != "" && item.Name == "SITUAÇÃO" && item.Value == situacao)
                    {
                        folder.AddFeature(placemark.Clone());
                        break;
                    }
                    else if (bairro != null && bairro != "" && item.Name == "BAIRRO" && item.Value == bairro)
                    {
                        folder.AddFeature(placemark.Clone());
                        break;
                    }
                    else if (referencia != null && referencia != "" && item.Name == "REFERENCIA" && item.Value.Contains(referencia))
                    {
                        folder.AddFeature(placemark.Clone());
                        break;
                    }
                    else if (ruaCruzamento != null && ruaCruzamento != "" && item.Name == "RUA/CRUZAMENTO" && item.Value.Contains(ruaCruzamento))
                    {
                        folder.AddFeature(placemark.Clone());
                        break;
                    }
                }
            }

            KmlFile kmlFile = KmlFile.Create(filteredKml, false);

            using MemoryStream memStream = new();
            kmlFile.Save(memStream);

            var data = memStream.ToArray();
            return data;

        }

        public List<PlacemarkView> GetJson(string? cliente, string? situacao, string? bairro, string? referencia, string? ruaCruzamento)
        {
            List<PlacemarkView> result = new List<PlacemarkView>();

            CheckFilters(cliente, situacao, bairro, referencia, ruaCruzamento);

            Kml kml = ReadKmlFile();
            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                foreach (var item in placemark.ExtendedData.Data)
                {
                    if (cliente != null && cliente != "" && item.Name == "CLIENTE" && item.Value == cliente)
                    {
                        result.Add(new PlacemarkView(placemark));
                        break;
                    }
                    else if (situacao != null && situacao != "" && item.Name == "SITUAÇÃO" && item.Value == situacao)
                    {
                        result.Add(new PlacemarkView(placemark));
                        break;
                    }
                    else if (bairro != null && bairro != "" && item.Name == "BAIRRO" && item.Value == bairro)
                    {
                        result.Add(new PlacemarkView(placemark));
                        break;
                    }
                    else if (referencia != null && referencia != "" && item.Name == "REFERENCIA" && item.Value.Contains(referencia))
                    {
                        result.Add(new PlacemarkView(placemark));
                        break;
                    }
                    else if (ruaCruzamento != null && ruaCruzamento != "" && item.Name == "RUA/CRUZAMENTO" && item.Value.Contains(ruaCruzamento))
                    {
                        result.Add(new PlacemarkView(placemark));
                        break;
                    }
                }
            }
            return result;
        }

        public JObject GetFilters()
        {
            Kml kml = ReadKmlFile();

            HashSet<string> cliente = new HashSet<string>();
            HashSet<string> situacao = new HashSet<string>();
            HashSet<string> bairro = new HashSet<string>();

            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                foreach (var item in placemark.ExtendedData.Data)
                {
                    if (item.Name == "CLIENTE")
                    {
                        cliente.Add(item.Value);
                    }
                    else if (item.Name == "SITUAÇÃO")
                    {
                        situacao.Add(item.Value);
                    }
                    else if (item.Name == "BAIRRO")
                    {
                        bairro.Add(item.Value);
                    }
                }
            }

            HashSet<string> clienteFiltrado = cliente.Distinct().Where(item => item != "").ToHashSet();
            HashSet<string> situacaoFiltrado = situacao.Distinct().Where(item => item != "").ToHashSet();
            HashSet<string> bairroFiltrado = bairro.Distinct().Where(item => item != "").ToHashSet();

            JObject filters = new JObject
            {
                ["Clientes"] = new JArray(clienteFiltrado),
                ["Situações"] = new JArray(situacaoFiltrado),
                ["Bairros"] = new JArray(bairroFiltrado)
            };

            return filters;
        }
    }
}
