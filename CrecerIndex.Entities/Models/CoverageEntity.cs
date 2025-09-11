using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Entities.Models
{
    public class CoverageEntity
    {
        public string codigo_de_producto { get; set; }
        public string nombre_de_producto { get; set; }//falta en crecer
        public string numero_de_poliza { get; set; }
        public string Numero_Credito { get; set; }
        public string tipo_documento { get; set; }
        public string numero_documento { get; set; }
        public string razon_social { get; set; }
        public string nombre { get; set; }
        public string apellido_pat { get; set; }
        public string apellido_mat { get; set; }
        public string nombreLargo { get; set; }
        public string Fecha_Inicio_Openitem { get; set; }
        public string Fecha_Fin_Openitem { get; set; }
        public decimal suma_Asegurada { get; set; }
        public decimal Prima_total_OpenItem { get; set; }
        public string Moneda_OptenItem { get; set; }
        public string Fecha_Desembolso_del_Credito { get; set; }
        public string Fecha_Vcto_del_Credito { get; set; }
        public string Fecha_del_OpenItem { get; set; }
        public string APO_AGREGATEDPOLICYID { get; set; }
        public string TP_TPT_ID { get; set; }
    }

    public class CoverageEntitySigma
    {
        public int NID { get; set; }
        public string NID_PRODUCTO { get; set; }
        public string SDESCRIPCION_PRODUCTO { get; set; }
        public string SNUMERO_POLIZA { get; set; }
        public string SNUMERO_CREDITO { get; set; }
        public string STIPO_DOCUMENTO_ASEGURADO { get; set; }
        public string SNRO_DOCUMENTO_ASEGURADO { get; set; }
        public string SNOMBRE_COMPLETO { get; set; }
        public string SNOMBRES_RAZONSOCIAL_ASEGURADO { get; set; }
        public string SAPELLIDO_PATERNO_ASEGURADO { get; set; }
        public string SAPELLIDO_MATERNO_ASEGURADO { get; set; }
        public string SINICIO_CIBERTURA { get; set; }
        public string SFIN_COBERTURA { get; set; }
        public decimal NMONTO_ASEGURADO { get; set; }
        public decimal NPRIMA { get; set; }
        public string SMONEDA { get; set; }
        public string SFECHA_DESEMBOLSO_CREDITO { get; set; }
        public string SFECHA_VENCIMIENTO_CREDITO { get; set; }
        public string SFECHA_PROCESO { get; set; }
        public decimal OPERATIONPK { get; set; }


        public string CONTRATANTE { get; set; }
        public string CANAL { get; set; }
        public string ES_CANAL_VINCULADO { get; set; }
        public string ESTADO_POLIZA { get; set; }
        public string EVENTO_POLIZA { get; set; }
        public string ESTADO_UR { get; set; }


        public string INI_POLIZA { get; set; }
        public string FIN_POLIZA { get; set; }
    }

    public class FilterCoverageEntity
    {
        public string? sdocumentnumber { get; set; }
        public string? sdocumenttype { get; set; }
        public string? sname { get; set; }
        public PaginationEntity pagination { get; set; }

        public FilterCoverageEntity()
        {
            pagination = new PaginationEntity();
        }
    }
}
