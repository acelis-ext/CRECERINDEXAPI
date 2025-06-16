using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrecerIndex.Repository.Extensions
{
    public static class Functions
    {
        public static IEnumerable<TSource> Pagination<TSource>(this IEnumerable<TSource> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static String GetDocumentTypeCrecer(int idTypedDocument, string snroDocumento)
        {
            string _strDocument = string.Empty;
            switch (idTypedDocument)
            {
                case 1:
                    _strDocument = "DocumentoIdentidad";//DNI
                    break;
                case 4:
                    _strDocument = "CarnetExtranjeria";//PASAPORTE
                    break;
                case 6:
                    _strDocument = snroDocumento.Trim().Substring(1, 2) == "10" ? "RUCNatural" : "RUC";
                    break;
                case 7:
                    _strDocument = "Pasaporte";//CE
                    break;
                default:
                    _strDocument = null;
                    break;
            }
            return _strDocument;
        }
    }
}
