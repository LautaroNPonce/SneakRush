using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using BE;
using BLL;
using ClosedXML.Excel;
using Presentacion_Administrador.Controllers;


namespace SneakRush.Controllers
{
    public class TiendaOnlineController : BaseController, BLL_ObserverIdioma
    {
        public TiendaOnlineController()
        {
            //  Lo utilizo para suscribir el controlador al sistema de idiomas
            BLL_IdiomaSubject.AgregarObservador(this);
        }

        // Este método lo uso para que se ejecutará automáticamente
        // cuando cambio el idioma desde el ComboBox (JS → BLL_IdiomaSubject)
        public void ActualizarIdioma(string codigoIdioma)
        {
            // Verifico que haya contexto HTTP antes de acceder a Session
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session["IdiomaActual"] = codigoIdioma;
            }

            // Con esto manejo la depuracion
            System.Diagnostics.Debug.WriteLine($"Idioma actualizado en TiendaOnlineController → {codigoIdioma}");
        }

        private bool EsCliente()
        {
            return Session["Cliente"] != null;
        }

        public ActionResult Index()
        {
            // Esto lo uso para mostrar el idioma actual en la vista
            ViewBag.Idioma = Session["IdiomaActual"] ?? "es";

            return View();
        }

        public ActionResult DetalleProducto(int idproducto = 0)
        {
            Producto oProducto = new Producto();
            bool conversion;

            oProducto = new BLL_Producto().Listar().Where(p => p.IdProducto == idproducto).FirstOrDefault();
            if(oProducto != null) 
            {
                oProducto.Base64 = BLL_Encriptacion.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);
                oProducto.Extension = Path.GetExtension(oProducto.NombreImagen);
            }
            return View(oProducto);
        }

        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> lista = new List<Categoria>();
            lista = new BLL_Categoria().Listar();

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarMarcaPorCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();
            lista = new BLL_Marca().ListarMarcaPorCategoria(idcategoria);
            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarProductos(int idcategoria, int idmarca)
        {
            List<Producto> lista = new List<Producto>();
            bool conversion;

            lista = new BLL_Producto().Listar().Select(p => new Producto()
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen,
                Base64 = BLL_Encriptacion.ConvertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out conversion),
                Extension = Path.GetExtension(p.NombreImagen),
                Activo = p.Activo

            }).Where(p => p.oCategoria.IdCategoria == (idcategoria == 0 ? p.oCategoria.IdCategoria : idcategoria) && p.oMarca.IdMarca == (idmarca == 0 ? p.oMarca.IdMarca : idmarca) &&
                     p.Stock > 0 && p.Activo == true
            ).ToList();

            var jsonresuelt = Json(new { data = lista }, JsonRequestBehavior.AllowGet);
            jsonresuelt.MaxJsonLength = int.MaxValue;

            return jsonresuelt;
        }

        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto) 
        {
            if (!EsCliente())
            {
                return Json(new { respuesta = false, mensaje = "Debés iniciar sesión para agregar productos al carrito." }, JsonRequestBehavior.AllowGet);
            }

            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            bool Existe = new BLL_Carrito().ExisteCarrito(idcliente, idproducto);
            bool respuesta = false;
            string mensaje = string.Empty;

            if (Existe)
            {
                mensaje = "El prouducto ya esta agregado al carrito";
            }
            else 
            {
                respuesta = new BLL_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            }
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CantidadCarrito() 
        {
            if (!EsCliente())
            {
                return Json(new { respuesta = false, mensaje = "Debés iniciar sesión para continuar." }, JsonRequestBehavior.AllowGet);
            }

            if (Session["Cliente"] == null)
            {
                return Json(new { cantidad = 0 }, JsonRequestBehavior.AllowGet);
            }

            int idCliente = ((Cliente)Session["Cliente"]).IdCliente;
            int cantidad = new BLL_Carrito().CantidadCarrito(idCliente);
            return Json(new { cantidad = cantidad }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ListarProductoCarrito() 
        {
            if (!EsCliente())
            {
                return Json(new { data = new List<Carrito>(), mensaje = "Debés iniciar sesión para ver tu carrito." }, JsonRequestBehavior.AllowGet);
            }

            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            List<Carrito> oLista = new List<Carrito>();
            bool conversion;
            oLista = new BLL_Carrito().ListarProducto(idcliente).Select(pc => new Carrito() {
                oProducto = new Producto()
                {
                    IdProducto = pc.oProducto.IdProducto,
                    Nombre = pc.oProducto.Nombre,
                    oMarca = pc.oProducto.oMarca,
                    Precio = pc.oProducto.Precio,
                    RutaImagen = pc.oProducto.RutaImagen,
                    Base64 = BLL_Encriptacion.ConvertirBase64(Path.Combine(pc.oProducto.RutaImagen,pc.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(pc.oProducto.NombreImagen)

                },
                Cantidad = pc.Cantidad
            }).ToList();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OperacionCarrito(int idproducto, bool sumar)
        {
            if (!EsCliente())
            {
                return Json(new { respuesta = false, mensaje = "Debés iniciar sesión para continuar." }, JsonRequestBehavior.AllowGet);
            }

            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new BLL_Carrito().OperacionCarrito(idcliente, idproducto, sumar, out mensaje);
            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCarrito(int idproducto) 
        {
            if (!EsCliente())
            {
                return Json(new { respuesta = false, mensaje = "Debés iniciar sesión para continuar." }, JsonRequestBehavior.AllowGet);
            }

            int idcliente = ((Cliente)Session["Cliente"]).IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new BLL_Carrito().EliminarCarrito(idcliente, idproducto);

            if (!respuesta)
            {
                mensaje = "No se pudo eliminar el producto del carrito.";
            }

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ObtenerProvincia() 
        {
            List<Provincia> oLista = new List<Provincia>();
            oLista = new BLL_Ubicacion().ObtenerProvincia();
            return Json(new { lista = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObtenerPartido(string IdProvincia)
        {
            List<Partido> oLista = new List<Partido>();
            oLista = new BLL_Ubicacion().ObtenerPartido(IdProvincia);
            return Json(new { lista = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ObtenerLocalidad(string IdProvincia, string IdPartido)
        {
            List<Localidad> oLista = new List<Localidad>();
            oLista = new BLL_Ubicacion().ObtenerLocalidad(IdProvincia,IdPartido);
            return Json(new { lista = oLista }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Carrito() 
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)
        {
            if (!EsCliente())
            {
                return Json(new { Status = false, mensaje = "Debés iniciar sesión para procesar el pago." }, JsonRequestBehavior.AllowGet);
            }

            decimal total = 0;

            DataTable detalle_venta = new DataTable();
            detalle_venta.Locale = new CultureInfo("en-US");
            detalle_venta.Columns.Add("IdProducto", typeof(string));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("Total", typeof(decimal));

            foreach (Carrito oCarrito in oListaCarrito)
            {
                decimal subtotal = Convert.ToDecimal(oCarrito.Cantidad.ToString()) * oCarrito.oProducto.Precio;

                total += subtotal;

                detalle_venta.Rows.Add(new object[] {
                    oCarrito.oProducto.IdProducto,
                    oCarrito.Cantidad,
                    subtotal
                });
            }

            oVenta.MontoTotal = total;
            oVenta.IdCliente = ((Cliente)Session["Cliente"]).IdCliente;

            // Esto generar ID dinámico
            string nuevoIdTransferencia = GenerarIdTransaccion();
            oVenta.IdTrasferencia = nuevoIdTransferencia;

            TempData["Venta"] = oVenta;
            TempData["DetalleVenta"] = detalle_venta;

            return Json(new
            {
                Status = true,
                Link = "/TiendaOnline/PagosEfectuados?idTrasferencia=" + nuevoIdTransferencia + "&status=true"
            }, JsonRequestBehavior.AllowGet);
        }

        // Esto es para incrementar el ID de Transaccion: code0001 
        private string GenerarIdTransaccion()
        {
            BLL_Venta logicaVenta = new BLL_Venta();
            int ultimoId = logicaVenta.ObtenerUltimoIdVenta();
            int nuevoNumero = ultimoId + 1;
            string idGenerado = "code" + nuevoNumero.ToString("D4");
            return idGenerado;
        }


        public async Task<ActionResult> PagosEfectuados() 
        {
            string idtrasferencia = Request.QueryString["idTrasferencia"];
            bool status = Convert.ToBoolean(Request.QueryString["status"]);

            ViewData["Status"] = status;

            if (status)
            {
                Venta oVenta = (Venta)TempData["Venta"];
                DataTable detalle_venta = (DataTable)TempData["DetalleVenta"];

                oVenta.IdTrasferencia = idtrasferencia;

                string mensaje = string.Empty;

                bool respuesta = new BLL_Venta().Registrar(oVenta, detalle_venta, out mensaje);

                ViewData["IdTrasferencia"] = oVenta.IdTrasferencia;

                if (respuesta)
                {
                    // Esto es el registro de la bitácora en el proceso del pago
                    Cliente cliente = (Cliente)Session["Cliente"];

                    new BLL_Bitacora().Registrar(new Bitacora
                    {
                        Fecha = DateTime.Now,
                        TipoUsuario = "Usuario",
                        Accion = "Compra realizada",
                        Detalle = $"El cliente {cliente.Nombres} {cliente.Apellidos} (ID: {cliente.IdCliente}) realizó una compra por ${oVenta.MontoTotal} con ID de transacción {oVenta.IdTrasferencia}.",
                        Usuario = cliente.Correo
                    });
                }
            }
            return View();
        }

        public ActionResult MisCompras()
        {
            if (!EsCliente())
            {
                ViewBag.Mensaje = "Debés iniciar sesión para ver tus compras.";
                return View("NoAutorizado");
            }

            if (Session["Cliente"] == null)
            {
                return RedirectToAction("Index", "AccesoCliente");
            }

            int idCliente = ((Cliente)Session["Cliente"]).IdCliente;

            List<Venta> listaCompras = new BLL_Venta().ListarPorCliente(idCliente);

            return View(listaCompras);
        }

        [HttpGet]
        public ActionResult CambiarIdioma(string codigo)
        {
            try
            {
                // Se notifican a todos los observadores
                BLL_IdiomaSubject.CambiarIdioma(codigo);

                // Devuelvo un OK vacío (no hay vista asociada)
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                // Si algo sale mal, registramos el error
                System.Diagnostics.Debug.WriteLine($"Error al cambiar idioma: {ex.Message}");
                return new HttpStatusCodeResult(500, "Error al cambiar idioma");
            }
        }
    }
}