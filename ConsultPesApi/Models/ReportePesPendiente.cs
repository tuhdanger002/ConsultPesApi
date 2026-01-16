namespace ConsultPesApi.Models
{
    public class ReportePesPendiente
    {
        public string NUM_PES { get; set; }
        public DateTime FECHA_SOL { get; set; }
        public string RESPONSABLE { get; set; }
        public string USUARIO { get; set; }
        public string NOMBREUSER { get; set; }
        public DateTime FECHA_INI { get; set; }
        public DateTime FECHA_FIN { get; set; }
        public string ALCANCE { get; set; }
        public string ALIMENTADOR { get; set; }
        public string CAUSA { get; set; }
        public string DIRECCION { get; set; }

        public string OBSERVACIÓN { get; set; }
        public int CANTIDAD_CLIENTES { get; set; }
        public decimal Potencia_KVA { get; set; }
        public int Id { get; set; }
        public string Nuevo { get; set; }
        public decimal Clients { get; set; }
        public string Grupo { get; set; }
        public string Municipio { get; set; }
        public decimal Potencia { get; set; }
        public string Provincia { get; set; }
        public string SE { get; set; }
        public string Sector { get; set; }
        public string ZDI { get; set; }
        public string Apoyo_Adjunto { get; set; }
        public string Apoyo_Completo { get; set; }
        public string LINEA_VIVA { get; set; }

    }
}
