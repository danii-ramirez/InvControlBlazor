using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace InvControl.Shared.Helpers
{
    public enum TipoEntidad
    {
        [Display(Description = "Login")]
        Login = 1,
        [Display(Description = "Usuario")]
        Usuario = 2,
        [Display(Description = "Rol")]
        Rol = 3,
        [Display(Description = "SKU")]
        SKU = 4,
        [Display(Description = "Canal de venta")]
        CanalVenta = 5,
        [Display(Description = "Chofer")]
        Chofer = 6,
        [Display(Description = "Transporte")]
        Transporte = 7,
        [Display(Description = "Remito")]
        Remito = 8,
        [Display(Description = "Marca")]
        Marca = 9,
        [Display(Description = "Tipo de contenedor")]
        TipoContenedor = 10

    }

    public enum RemitoEstado
    {
        Pendiente = 1,
        Aprobado = 2,
        Procesado = 3,
        Rechazado = 4
    }

    public enum StockMovimientoEstado
    {
        Ingreso = 1,
        Salida = 2
    }

    public enum Tipo
    {
        Entrada,
        Salida
    }

    public enum BimboConcepto
    {
        NombreColumna = 1,
        MotivoAjuste = 2,
        TipoEstoque = 3,
    }

    public enum BimboNombreColumna
    {
        CanalVenta = 1,
        NroRemito = 2,
        CodigoSku = 3,
        NombreSku = 4,
        Cantidad = 5,
        TipoEstoque = 6,
        MotivoAjuste = 7
    }

    public enum BimboOperacion
    {
        [Display(Description = "Todos")]
        Todos,
        [Display(Description = "Disponible para procesar")]
        Disponible,
        [Display(Description = "SKU no encontrado")]
        SkuNoEncontrado,
        [Display(Description = "Canal de venta no encontrado")]
        CanalVentaNoEncontrado
    }
}
