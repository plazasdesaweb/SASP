using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers.Prepago
{
    [AuditingFilter]
    [HandleError]
    public class ClientePrepagoController : Controller
    {
        private ClientePrepagoRepository repCliente = new ClientePrepagoRepository();
        private BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();
        private OrdenRecargaRepository repOrden = new OrdenRecargaRepository();

        public ActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Filter(string rif)
        {
            ClientePrepago cliente = repCliente.Find(rif);
            if (cliente != null)
            {
                return View("Edit", cliente);
            }
            else
            {
                cliente = new ClientePrepago()
                {
                    rifCliente = rif
                };
                return View("Create", cliente);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientePrepago cliente)
        {
            ViewModel viewmodel = new ViewModel();
            if (repCliente.Save(cliente))
            {
                viewmodel.Title = "Prepago / Cliente / Crear Cliente";
                viewmodel.Message = "Cliente creado exitosamente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = cliente.rifCliente;
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Crear Cliente";
                viewmodel.Message = "Error de aplicacion: No se pudo crear el Cliente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "IndexAll";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterReview()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FilterReview(string name, string rif)
        {
            List<ClientePrepago> clientes = repCliente.Find(name, rif);
            return View("Index", clientes);
        }

        public ActionResult Index(string rif)
        {
            List<ClientePrepago> clientes = repCliente.Find("", rif);
            return View(clientes);
        }

        public ActionResult IndexAll()
        {
            List<ClientePrepago> clientes = repCliente.Find("", "");
            return View("Index", clientes);
        }

        public ActionResult Edit(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientePrepago cliente)
        {
            ViewModel viewmodel = new ViewModel();
            if (repCliente.SaveChanges(cliente))
            {
                viewmodel.Title = "Prepago / Cliente / Editar Cliente";
                viewmodel.Message = "La información del Cliente ha sido actualizada satisfactoriamente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "Index";
                viewmodel.RouteValues = cliente.rifCliente;
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Editar Cliente";
                viewmodel.Message = "Error de aplicacion: No se pudo crear el Cliente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "IndexAll";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult Delete(int id)
        {
            ViewModel viewmodel = new ViewModel();
            ClientePrepago cliente = repCliente.Find(id);
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").ToList();
            if (beneficiarios.Count != 0)
            {
                viewmodel.Title = "Prepapago / Cliente / Eliminar";
                viewmodel.Message = "No se puede eliminar este Cliente, tiene Beneficiarios asociados.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "IndexAll";
                return RedirectToAction("GenericView", viewmodel);
            }
            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ClientePrepago cliente)
        {
            ViewModel viewmodel = new ViewModel();
            if (repCliente.BorrarCliente(cliente.idCliente))
            {
                viewmodel.Title = "Prepago / Cliente / Borrar Cliente";
                viewmodel.Message = "Cliente borrado exitosamente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "IndexAll";
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Borrar Cliente";
                viewmodel.Message = "Error de aplicacion: No se pudo borrar el Cliente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "IndexAll";
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterBeneficiario(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            return View(cliente);
        }

        [HttpPost]
        public ActionResult FilterBeneficiario(int id, string numdoc)
        {
            BeneficiarioPrepago beneficiario;
            BeneficiarioPrepagoIndex beneficiarioIndex = repBeneficiario.Find(numdoc, "", "", "", "").FirstOrDefault();
            //NO ES Beneficiario PrepagoPlazas
            if (beneficiarioIndex == null)
            {
                beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = repAfiliado.Find(numdoc),
                    Cliente = repCliente.Find(id)
                };
                //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
                beneficiario.Afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
                //SI ES SUMA. VOY A EDIT, SI NO A CREATE
                if (beneficiario.Afiliado.type == "Suma")
                {
                    //MUESTRO MENSAJE DE CAMBIO SUMA A PREPAGO
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Prepago / Cliente / Crear Beneficiario / Filtro de Búsqueda";
                    viewmodel.Message = "El número de documento ya posee afiliación a Suma Plaza's. Complete la afiliación a Prepago Plaza's.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "CreateBeneficiarioSuma";
                    viewmodel.RouteValues = id.ToString() + "?idBeneficiario=" + beneficiario.Afiliado.id.ToString();
                    return RedirectToAction("GenericView", viewmodel);                                         
                }
                else
                {
                    beneficiario.Afiliado.typeid = Globals.ID_TYPE_PREPAGO;
                    beneficiario.Afiliado.type = "Prepago";
                    beneficiario.Afiliado.idClientePrepago = beneficiario.Cliente.idCliente;
                    beneficiario.Afiliado.NombreClientePrepago = beneficiario.Cliente.nameCliente;
                    //verifico si tiene tarjeta en Cards
                    if (beneficiario.Afiliado.pan != "0" && beneficiario.Afiliado.estatustarjeta == "Activa")
                    {
                        ViewModel viewmodel = new ViewModel();
                        viewmodel.Title = "Prepago / Cliente / Crear Beneficiario / Filtro de Búsqueda";
                        viewmodel.Message = "El número de documento indicado ya posee una Tarjea Activa con el número " + beneficiario.Afiliado.pan;
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "CreateBeneficiarioConTarjeta";
                        viewmodel.RouteValues = id.ToString() + "?numdoc=" + numdoc;
                        return RedirectToAction("GenericView", viewmodel);
                    }
                    else
                    {
                        return View("CreateBeneficiario", beneficiario.Afiliado);
                    }
                }
            }
            //ES Beneficiario PrepagoPlazas
            else
            {
                //ES Beneficiario PrepagoPlazas de el cliente
                beneficiario = repBeneficiario.Find(beneficiarioIndex.Afiliado.id);
                if (beneficiario.Cliente.idCliente == id)
                {
                    //beneficiario.Afiliado = repAfiliado.Find(beneficiario.Afiliado.id);
                    beneficiario = repBeneficiario.Find(beneficiario.Afiliado.id);
                    return View("EditBeneficiario", beneficiario.Afiliado);
                }
                //ES Beneficiario Prepago Plazas de otro cliente
                else
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Prepago / Cliente / Crear Beneficiario / Filtro de Búsqueda";
                    viewmodel.Message = "El Beneficiario está asociado con otro Cliente. No se puede asociar con el Cliente actual.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterBeneficiario";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
        }

        [HttpPost]
        public ActionResult CreateBeneficiario(AfiliadoSuma Afiliado, ClientePrepago Cliente, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CreateBeneficiarioSuma(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = repAfiliado.Find(idBeneficiario),
                    Cliente = repCliente.Find(id)
                };
            beneficiario.Afiliado = repAfiliado.ReiniciarAfiliacionSumaAPrepago(beneficiario.Afiliado);
            if (repAfiliado.SaveChanges(beneficiario.Afiliado))
            {
                beneficiario.Afiliado.idClientePrepago = beneficiario.Cliente.idCliente;
                beneficiario.Afiliado.NombreClientePrepago = beneficiario.Cliente.nameCliente;
                return View("EditBeneficiario", beneficiario.Afiliado);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Cliente / Crear Beneficiario / Filtro de Búsqueda";
                viewmodel.Message = "Error de aplicación: No se pudo guardar afiliacion Prepago";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterBeneficiario";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult CreateBeneficiarioConTarjeta(int id, string numdoc)
        {
            BeneficiarioPrepago beneficiario;
            beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(numdoc),
                Cliente = repCliente.Find(id)
            };
            //CARGO VALOR POR DEFECTO EN LISTA DE ESTADOS
            beneficiario.Afiliado.ListaEstados.Insert(0, new ESTADO { COD_ESTADO = " ", DESCRIPC_ESTADO = "Seleccione un Estado" });
            beneficiario.Afiliado.typeid = Globals.ID_TYPE_PREPAGO;
            beneficiario.Afiliado.type = "Prepago";
            beneficiario.Afiliado.idClientePrepago = beneficiario.Cliente.idCliente;
            beneficiario.Afiliado.NombreClientePrepago = beneficiario.Cliente.nameCliente;
            return View("CreateBeneficiario", beneficiario.Afiliado);
        }

        [HttpPost]
        public ActionResult CreateBeneficiarioConTarjeta(AfiliadoSuma Afiliado, ClientePrepago Cliente, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.Save(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    int idafiliado = repAfiliado.Find(Afiliado.docnumber, "", "", "", "").First().id;
                    beneficiario.Afiliado = repAfiliado.Find(idafiliado);
                    if (repAfiliado.Aprobar(beneficiario.Afiliado))
                    {
                        viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                        viewmodel.Message = "Solicitud de afiliación creada y aprobada exitosamente.";
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "FilterReviewBeneficiarios";
                        viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
                    }
                    else
                    {
                        viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                        viewmodel.Message = "Solicitud de afiliación creada, pero no se pudo aprobar automáticamente.";
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "FilterReviewBeneficiarios";
                        viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
                    }
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        [HttpPost]
        public ActionResult EditBeneficiarioSuma(AfiliadoSuma Afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (repAfiliado.SaveChanges(Afiliado, fileNoValidado))
            {
                BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
                {
                    Afiliado = Afiliado,
                    Cliente = repCliente.Find(Afiliado.idClientePrepago)
                };
                if (repBeneficiario.Save(beneficiario))
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                    viewmodel.Message = "Solicitud de afiliación creada exitosamente.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Crear Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo crear solicitud de afiliación.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult FilterReviewBeneficiarios(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            return View(cliente);
        }

        [HttpPost]
        public ActionResult FilterReviewBeneficiarios(int id, string numdoc, string name, string email, string estadoAfiliacion, string estadoTarjeta)
        {
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, numdoc, name, email, estadoAfiliacion, estadoTarjeta).ToList();
            if (beneficiarios.Count > 0)
            {
                return View("IndexBeneficiarios", beneficiarios);
            }
            else
            {
                BeneficiarioPrepagoIndex beneficiario = new BeneficiarioPrepagoIndex()
                {
                    Cliente = repCliente.Find(id)
                };
                beneficiarios.Add(beneficiario);
                return View("IndexBeneficiarios", beneficiarios);
            }
        }

        public ActionResult EditBeneficiario(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = repBeneficiario.Find(idBeneficiario);
            return View(beneficiario.Afiliado);
        }

        [HttpPost]
        public ActionResult EditBeneficiario(AfiliadoSuma Afiliado, HttpPostedFileBase fileNoValidado)
        {
            ViewModel viewmodel = new ViewModel();
            if (!repAfiliado.SaveChanges(Afiliado, fileNoValidado))
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Revisar Afiliación";
                viewmodel.Message = "Error de aplicacion: No se pudo actualizar afiliación.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Revisar Afiliación";
                viewmodel.Message = "La información del beneficiario ha sido actualizada satisfactoriamente.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = Afiliado.idClientePrepago.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult AprobarBeneficiario(int id, int idBeneficiario)
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            //SI ES AFILIADO SUMA, SE CAMBIA EL TIPO Y YA NO SE BLOQUEA LA TARJETA ACTUAL
            if (beneficiario.Afiliado.type == "Suma")
            {
                if (repAfiliado.Aprobar(beneficiario.Afiliado))
                {
                    beneficiario.Afiliado = repAfiliado.CambiarAPrepago(beneficiario.Afiliado);
                    //if (repAfiliado.BloquearTarjeta(beneficiario.Afiliado) == true)
                    //{
                    //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    //    viewmodel.Title = "Prepago / Cliente / Beneficiario / Aprobar Afiliación:";
                    //    viewmodel.Message = "Afiliación Aprobada. Se creó una nueva tarjeta Prepago Plaza's";
                    //    viewmodel.ControllerName = "ClientePrepago";
                    //    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    //    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                    //}
                    //else
                    //{
                    //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    //    viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                    //    viewmodel.Message = "Afiliación Aprobada. No se pudo crear una nueva tarjeta Prepago Plaza's";
                    //    viewmodel.ControllerName = "ClientePrepago";
                    //    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    //    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                    //}
                    repAfiliado.SaveChanges(beneficiario.Afiliado);
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Afiliación Prepago Aprobada.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                }
                else
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Error de aplicacion: No se pudo aprobar afiliación.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                }
            }
            else
            {
                if (repAfiliado.Aprobar(beneficiario.Afiliado))
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Afiliación Prepago Aprobada.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                }
                else
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Aprobar Afiliación:";
                    viewmodel.Message = "Error de aplicacion: No se pudo aprobar afiliación.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                }
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult DeleteBeneficiario(int id, int idBeneficiario)
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            if (repBeneficiario.Delete(beneficiario))
            {
                beneficiario.Afiliado = repAfiliado.CambiarASuma(beneficiario.Afiliado);
                //YA NO SE CAMBIA LA TARJETA AL DAR DE BAJA
                //if (repAfiliado.BloquearTarjeta(beneficiario.Afiliado))
                //{
                //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                //    viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                //    viewmodel.Message = "Beneficiario eliminado con éxito. Se creó una nueva tarjeta Suma Plaza's";
                //    viewmodel.ControllerName = "ClientePrepago";
                //    viewmodel.ActionName = "FilterReviewBeneficiarios";
                //    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                //}
                //else
                //{
                //    repAfiliado.SaveChanges(beneficiario.Afiliado);
                //    viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                //    viewmodel.Message = "Beneficiario eliminado con éxito. No se pudo crear una nueva tarjeta Suma Plaza's";
                //    viewmodel.ControllerName = "ClientePrepago";
                //    viewmodel.ActionName = "FilterReviewBeneficiarios";
                //    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                //}
                if (repAfiliado.SaveChanges(beneficiario.Afiliado))
                {
                    //SE APRUEBA AFILIACION SUMA DE FORMA AUTOMATICA
                    if (repAfiliado.Aprobar(beneficiario.Afiliado))
                    {
                        viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                        viewmodel.Message = "Beneficiario eliminado con éxito.";
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "FilterReviewBeneficiarios";
                        viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                    }
                    else
                    {
                        viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                        viewmodel.Message = "Afiliación Prepago eliminada, pero no se aprobó de forma automática la afiliación a Suma";
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "FilterReviewBeneficiarios";
                        viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                    }
                }
                else
                {
                    viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                    viewmodel.Message = "Beneficiario eliminado con éxito, pero no se pudo actualizar afiliación a Suma";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReviewBeneficiarios";
                    viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
                }
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Eliminar Beneficiario";
                viewmodel.Message = "Error de aplicacion: No se pudo eliminar beneficiario.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = beneficiario.Cliente.idCliente.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ImprimirTarjeta(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View("ImpresoraImprimirTarjeta", beneficiario);
        }

        public ActionResult ReImprimirTarjeta(int id, int idBeneficiario)
        {
            ViewModel viewmodel = new ViewModel();
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            if (repAfiliado.BloquearTarjeta(beneficiario.Afiliado))
            {
                return View("ImpresoraImprimirTarjeta", beneficiario);
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / ReImprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de reimpresión de la Tarjeta";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }


        [HttpPost]
        public ActionResult ImprimirTarjeta(int id, int idBeneficiario, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(idBeneficiario);
            if (repAfiliado.ImprimirTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Tarjeta impresa y activada correctamente";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Operaciones con la Impresora / Imprimir Tarjeta";
                viewmodel.Message = "Falló el proceso de impresión y activación de la Tarjeta.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CrearPin(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View("PinPadCrearPin", beneficiario);
        }

        public ActionResult CambiarPin(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View("PinPadCambiarPin", beneficiario);
        }

        public ActionResult ReiniciarPin(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View("PinPadReiniciarPin", beneficiario);
        }

        public ActionResult BloquearTarjeta(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult BloquearTarjeta(int id, int idBeneficiario, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(idBeneficiario);
            if (repAfiliado.BloquearTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Bloquear Tarjeta";
                viewmodel.Message = "Tarjeta bloqueada correctamente";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Bloquear Tarjeta";
                viewmodel.Message = "Falló el proceso de bloqueo de la Tarjeta";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SuspenderTarjeta(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult SuspenderTarjeta(int id, int idBeneficiario, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(idBeneficiario);
            if (repAfiliado.SuspenderTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Suspender Tarjeta";
                viewmodel.Message = "Tarjeta suspendida correctamente";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Suspender Tarjeta";
                viewmodel.Message = "Falló el proceso de suspensión de la Tarjeta";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ReactivarTarjeta(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult ReactivarTarjeta(int id, int idBeneficiario, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(idBeneficiario);
            if (repAfiliado.ReactivarTarjeta(afiliado))
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Reactivar Tarjeta";
                viewmodel.Message = "Tarjeta reactivada correctamente";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Reactivar Tarjeta";
                viewmodel.Message = "Falló el proceso de reactivación de la Tarjeta";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SuspenderCliente(int id)
        {
            ViewModel viewmodel = new ViewModel();
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").Where(b => b.Afiliado.estatustarjeta == "Activa").ToList();
            if (beneficiarios.Count == 0)
            {
                viewmodel.Title = "Prepago / Cliente / Suspender Cliente";
                viewmodel.Message = "El cliente no posee beneficiarios con tarjeta Activa";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReview";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View(beneficiarios);
            }
        }

        [HttpPost]
        public ActionResult SuspenderCliente(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Boolean result = true;
            AfiliadoSuma afiliado;
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").Where(b => b.Afiliado.estatustarjeta == "Activa").ToList();
            foreach (BeneficiarioPrepagoIndex b in beneficiarios)
            {
                afiliado = repAfiliado.Find(b.Afiliado.id);
                if (repAfiliado.SuspenderTarjeta(afiliado) == false)
                {
                    result = false;
                }
            }
            if (result)
            {
                viewmodel.Title = "Prepago / Cliente / Suspender Cliente";
                viewmodel.Message = "Se han suspendido las tarjetas de los beneficiarios con exito.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Suspender Cliente";
                viewmodel.Message = "Ocurrieron fallas en el proceso de suspensión de las tarjetas. Verifique el estado de las tarjetas de los beneficiarios.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ReactivarCliente(int id)
        {
            ViewModel viewmodel = new ViewModel();
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").Where(b => b.Afiliado.estatustarjeta == "Suspendida").ToList();
            if (beneficiarios.Count == 0)
            {
                viewmodel.Title = "Prepago / Cliente / Suspender Cliente";
                viewmodel.Message = "El cliente no posee beneficiarios con tarjeta Suspendida";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReview";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View(beneficiarios);
            }
        }

        [HttpPost]
        public ActionResult ReactivarCliente(int id, string mode = "post")
        {
            ViewModel viewmodel = new ViewModel();
            Boolean result = true;
            AfiliadoSuma afiliado;
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").Where(b => b.Afiliado.estatustarjeta == "Suspendida").ToList();
            foreach (BeneficiarioPrepagoIndex b in beneficiarios)
            {
                afiliado = repAfiliado.Find(b.Afiliado.id);
                if (repAfiliado.ReactivarTarjeta(afiliado) == false)
                {
                    result = false;
                }
            }
            if (result)
            {
                viewmodel.Title = "Prepago / Cliente / Reactivar Cliente";
                viewmodel.Message = "Se han reactivado las tarjetas de los beneficiarios con exito.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Suspender Cliente";
                viewmodel.Message = "Ocurrieron fallas en el proceso de reactivación de la tarjetas. Verifique el estado de las tarjetas de los beneficiarios.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult SaldosMovimientos(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(beneficiario.Afiliado);
            if (SaldosMovimientos != null)
            {
                BeneficiarioPrepagoSaldosMovimientos beneficiarioSaldosMovimientos = new BeneficiarioPrepagoSaldosMovimientos()
                {
                    denominacionPrepago = "Bs.",
                    denominacionSuma = "Más",
                    Beneficiario = beneficiario,
                    SaldosMovimientos = SaldosMovimientos,
                };
                return View(beneficiarioSaldosMovimientos);
            }
            else
            {
                //ERROR EN METODO FIND
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Saldos y Movimientos";
                viewmodel.Message = "Ha ocurrido un error de aplicación (FindSaldosMovimientos). Notifique al Administrador.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult Acreditar(int id, int idBeneficiario)
        {
            BeneficiarioPrepago beneficiario = new BeneficiarioPrepago()
            {
                Afiliado = repAfiliado.Find(idBeneficiario),
                Cliente = repCliente.Find(id)
            };
            return View(beneficiario);
        }

        [HttpPost]
        public ActionResult Acreditar(int id, int idBeneficiario, string monto, string observaciones)
        {
            ViewModel viewmodel = new ViewModel();
            AfiliadoSuma afiliado = repAfiliado.Find(idBeneficiario);
            string resultado = repAfiliado.Acreditar(afiliado, monto);
            if (resultado != null)
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Acreditar";
                viewmodel.Message = "Acreditación exitosa. Clave de aprobación: " + resultado;
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Beneficiario / Acreditar ";
                viewmodel.Message = "Falló el proceso de acreditación.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReviewBeneficiarios";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult CreateOrdenRecarga(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").ToList();
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrden(cliente, beneficiarios.FindAll(b => b.Afiliado.estatus == "Activa" && b.Afiliado.estatustarjeta == "Activa"));
            if (detalleOrden.Count == 0)
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden";
                viewmodel.Message = "No se puede crear Orden. El cliente no tiene beneficiarios activos con tarjeta activa.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterReview";
                return RedirectToAction("GenericView", viewmodel);
            }
            else
            {
                return View(detalleOrden);
            }
        }

        [HttpPost]
        public ActionResult CreateOrdenRecarga(int id, IList<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas)
        {
            int idOrden = repOrden.CrearOrden(id, detalleOrden.ToList(), MontoTotalRecargas);
            if (idOrden != 0)
            {
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                //viewmodel.Message = "Orden Aprobada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden";
                viewmodel.Message = "Falló el proceso de creación de la Orden.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterOrdenes";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult CargarArchivoRecarga(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            List<DetalleOrdenRecargaPrepago> detalleOrden = new List<DetalleOrdenRecargaPrepago>();
            DetalleOrdenRecargaPrepago detalle = new DetalleOrdenRecargaPrepago()
            {
                idCliente = cliente.idCliente,
                nameCliente = cliente.nameCliente,
                rifCliente = cliente.rifCliente,
                phoneCliente = cliente.phoneCliente
            };
            detalleOrden.Add(detalle);
            return View(detalleOrden);
        }

        public FileResult DescargarArchivoFilePath()
        {
            string file = Server.MapPath("~/App_Data/Plantilla.xls");
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(file, contentType, Path.GetFileName(file));
        }

        [HttpPost]
        public ActionResult CreateOrdenRecargaArchivo(int id, HttpPostedFileBase file)
        {
            ViewModel viewmodel = new ViewModel();
            //subir y guardar archivo
            string path = "";
            string idtemp = System.Web.HttpContext.Current.Session.SessionID + ".xls";
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Server.MapPath("~/App_Data/" + idtemp);
                    file.SaveAs(path);
                }
                catch (Exception ex)
                {
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden desde Archivo";
                    viewmodel.Message = "Error de aplicacion: No se pudo cargar archivo (" + ex.Message + ")";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterOrdenes";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                }
                ClientePrepago cliente = repCliente.Find(id);
                List<BeneficiarioPrepagoIndex> beneficiarios = repCliente.FindBeneficiarios(id, "", "", "", "", "").ToList();
                if (beneficiarios.Count == 0)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden desde Archivo";
                    viewmodel.Message = "No se puede crear Orden. El cliente no tiene beneficiarios activos con tarjeta activa.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterReview";
                    return RedirectToAction("GenericView", viewmodel);
                }
                else
                {
                    //leer el archivo
                    List<DetalleOrdenRecargaPrepago> detalleOrdenArchivo = repOrden.GetBeneficiariosArchivo(path);
                    //borrar el archivo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    if (detalleOrdenArchivo == null)
                    {
                        viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden desde Archivo";
                        viewmodel.Message = "Error de aplicacion: No se pudo leer el archivo.";
                        viewmodel.ControllerName = "ClientePrepago";
                        viewmodel.ActionName = "FilterOrdenes";
                        viewmodel.RouteValues = id.ToString();
                        return RedirectToAction("GenericView", viewmodel);
                    }
                    else
                    {
                        List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.DetalleParaOrdenArchivo(cliente, beneficiarios.FindAll(b => b.Afiliado.estatus == "Activa" && b.Afiliado.estatustarjeta == "Activa"), detalleOrdenArchivo);
                        return View("CreateOrdenRecarga", detalleOrden);
                    }
                }
            }
            else
            {
                //borrar el archivo
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Crear Orden desde Archivo";
                viewmodel.Message = "Error de aplicacion: El archivo está vacío";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterOrdenes";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult FilterOrdenes(int id)
        {
            ClientePrepago cliente = repCliente.Find(id);
            return View(cliente);
        }

        [HttpPost]
        public ActionResult FilterOrdenes(int id, string numero, string fecha, string estadoOrden, string Referencia, string claseOrden, string Observaciones)
        {
            List<OrdenRecargaPrepago> ordenes = new List<OrdenRecargaPrepago>();
            OrdenRecargaPrepago orden;
            if (numero != "")
            {
                orden = repOrden.Find(Convert.ToInt32(numero));
                if (orden != null)
                {
                    ordenes.Add(orden);
                }
            }
            else
            {
                ordenes = repOrden.Find(fecha, estadoOrden, Referencia, claseOrden, Observaciones).Where(o => o.Cliente.idCliente == id).ToList();
            }
            if (ordenes.Count > 0)
            {
                return View("IndexOrdenes", ordenes);
            }
            else
            {
                orden = new OrdenRecargaPrepago()
                {
                    Cliente = repCliente.Find(id)
                };
                ordenes.Add(orden);
                return View("IndexOrdenes", ordenes);
            }
        }

        public ActionResult DetalleOrden(int id, int idOrden)
        {
            List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(idOrden);
            return View(detalleOrden);
        }

        [HttpPost]
        public ActionResult AprobarOrden(int id, int idOrden, IList<DetalleOrdenRecargaPrepago> detalleOrden, decimal MontoTotalRecargas, string indicadorGuardar, string DocumentoReferencia, string Observaciones)
        {
            ViewModel viewmodel = new ViewModel();
            detalleOrden.First().documentoOrden = DocumentoReferencia;
            detalleOrden.First().observacionesOrden = Observaciones;
            if (indicadorGuardar == "Aprobar")
            {
                if (repOrden.AprobarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Orden Aprobada.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterOrdenes";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                    //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                }
                else
                {
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de aprobación de la Orden.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterOrdenes";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
            else
            {
                if (repOrden.GuardarOrden(detalleOrden.ToList(), MontoTotalRecargas))
                {
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Datos de la Orden actualizados.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterOrdenes";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                    //return RedirectToAction("DetalleOrden", new { id = id, idOrden = idOrden });
                }
                else
                {
                    viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                    viewmodel.Message = "Falló el proceso de guardado de la Orden.";
                    viewmodel.ControllerName = "ClientePrepago";
                    viewmodel.ActionName = "FilterOrdenes";
                    viewmodel.RouteValues = id.ToString();
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
        }

        public ActionResult RechazarOrden(int id, int idOrden)
        {
            ViewModel viewmodel = new ViewModel();
            if (repOrden.RechazarOrden(idOrden))
            {
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Orden Rechazada.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterOrdenes";
                viewmodel.RouteValues = id.ToString();
            }
            else
            {
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden";
                viewmodel.Message = "Falló el proceso de rechazo de la Orden.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterOrdenes";
                viewmodel.RouteValues = id.ToString();
            }
            return RedirectToAction("GenericView", viewmodel);
        }

        public ActionResult ProcesarOrden(int id, int idOrden)
        {
            if (repOrden.ProcesarOrden(idOrden))
            {
                //ViewModel viewmodel = new ViewModel();
                //viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden / Procesar Orden";
                //viewmodel.Message = "Orden Procesada.";
                //viewmodel.ControllerName = "ClientePrepago";
                //viewmodel.ActionName = "FilterOrdenes";
                //viewmodel.RouteValues = id.ToString();
                //return RedirectToAction("GenericView", viewmodel);
                //List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(idOrden);
                //return View("ResultadoOrden", detalleOrden);
                List<DetalleOrdenRecargaPrepago> detalleOrden = repOrden.FindDetalleOrden(idOrden);
                return View("DetalleOrden", detalleOrden);
            }
            else
            {
                ViewModel viewmodel = new ViewModel();
                viewmodel.Title = "Prepago / Cliente / Ordenes de Recarga / Detalle de la Orden / Procesar Orden";
                viewmodel.Message = "Falló el procesamiento de la Orden.";
                viewmodel.ControllerName = "ClientePrepago";
                viewmodel.ActionName = "FilterOrdenes";
                viewmodel.RouteValues = id.ToString();
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

        //public FileContentResult GetImageBeneficiario(int id)
        //{
        //    Photos_Affiliate Photo = repAfiliado.Find(id).picture;
        //    if (Photo.photo != null)
        //    {
        //        return File(Photo.photo, Photo.photo_type);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public JsonResult CiudadList(string id)
        {
            List<CIUDAD> ciudades = repAfiliado.GetCiudades(id);

            return Json(new SelectList(ciudades, "COD_CIUDAD", "DESCRIPC_CIUDAD"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MunicipioList(string id)
        {
            List<MUNICIPIO> municipios = repAfiliado.GetMunicipios(id);

            return Json(new SelectList(municipios, "COD_MUNICIPIO", "DESCRIPC_MUNICIPIO"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ParroquiaList(string id)
        {
            List<PARROQUIA> parroquias = repAfiliado.GetParroquias(id);

            return Json(new SelectList(parroquias, "COD_PARROQUIA", "DESCRIPC_PARROQUIA"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UrbanizacionList(string id)
        {
            List<URBANIZACION> urb = repAfiliado.GetUrbanizaciones(id);

            return Json(new SelectList(urb, "COD_URBANIZACION", "DESCRIPC_URBANIZACION"), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
