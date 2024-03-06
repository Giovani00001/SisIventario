using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public static class Ds
    {
        public const string Exitosa = "Exitosa";
        public const string Error = "Error";
        public const string ImagenRuta = @"\Imagenes\Productos\";
        public const string Role_Admin = "Admin";
        public const string Role_Cliente = "Cliente";
        public const string Role_Inventario = "Inventario";
        public const string ssCarroCompras = "sesion carro compras";/// @Context.Session.GetInt32(Ds.ssCarroCompras);


        //estados orden
        public const string EstadoPendiente = "Pendiente";
        public const string EstadoAprobado = "Aprobado";
        public const string EstadoProceso = "Proceso";
        public const string EstadoEnviado = "Enviado";
        public const string EstadoCancelado = "Cancelado";
        public const string EstadoDevuelto = "Devuelto";

        //pagos

        public const string PagoEstadoPendiente = "Pendiente";
        public const string PagoEstadoAprobado = "Aprobado";
        public const string PagoEstadoRetrasado = "Retrasado";
        public const string PagoEstadoRechazado = "Rechazado";



    }
}
