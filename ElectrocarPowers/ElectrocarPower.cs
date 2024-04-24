using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectrocarPowers {
    [Serializable]
    public class ElectrocarPower {
        [JsonPropertyName("object_category_Id")] // Задаем имена для сериализации
        public string ObjectCategoryId { get; set; } = ""; // csv файл кривой, в нем этот столбец пустой
        [JsonPropertyName("ID")]
        public int Id { get; set; }
        [JsonPropertyName("Name")]
        public string Name { get; set; }
        [JsonPropertyName("AdmArea")]
        public string AdmArea { get; set; }
        [JsonPropertyName("District")]
        public string District { get; set; }
        [JsonPropertyName("Address")]
        public string Address { get; set; }
        [JsonPropertyName("Longitude_WGS84")]
        public string Longitude_WGS84 { get; set; }
        [JsonPropertyName("Latitude_WGS84")]
        public string Latitude_WGS84 { get; set; }
        [JsonPropertyName("global_id")]
        public string GlobalId { get; set; }
        [JsonPropertyName("geodata_center")]
        public string GeodataCenter { get; set; } = ""; // csv файл кривой, в нем этот столбец пустой
        [JsonPropertyName("geoarea")]
        public string GeoArea { get; set; } = ""; // csv файл кривой, в нем этот столбец пустой
        /// <summary>
        /// Конструктор без параметров.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public ElectrocarPower() {
            throw new ArgumentNullException();
        }
        /// <summary>
        /// Конструктор с параметрами.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="admArea"></param>
        /// <param name="district"></param>
        /// <param name="address"></param>
        /// <param name="longitude_WGS84"></param>
        /// <param name="latitude_WGS84"></param>
        /// <param name="globalId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        [JsonConstructor]
        public ElectrocarPower(int id, string name, string admArea, string district, string address, string longitude_WGS84, string latitude_WGS84, string globalId) {
            if (name == null || admArea == null || district == null || address == null || longitude_WGS84 == null || latitude_WGS84 == null || globalId == null) {
                throw new ArgumentNullException();
            }
            Id = id;
            Name = name;
            AdmArea = admArea;
            District = district;
            Address = address;
            Longitude_WGS84 = longitude_WGS84;
            Latitude_WGS84 = latitude_WGS84;
            GlobalId = globalId;
        }
        /// <summary>
        /// Строковое представление объекта в csv формате.
        /// </summary>
        /// <returns></returns>
        public string ToCSV() {
            return $"{ObjectCategoryId};{Id};{Name};{AdmArea};{District};{Address};{Longitude_WGS84};{Latitude_WGS84};{GlobalId};{GeodataCenter};{GeoArea};";
        }
        /// <summary>
        /// Строковое представление объекта в json формате.
        /// </summary>
        /// <returns></returns>
        public string ToJSON() {
            return JsonSerializer.Serialize(this);
        }

    }
}
