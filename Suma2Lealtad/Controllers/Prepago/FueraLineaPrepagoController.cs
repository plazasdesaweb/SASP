using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Suma2Lealtad.Controllers.Prepago
{
    [AuditingFilter]
    [HandleError]
    public class FueraLineaPrepagoController : Controller
    {
        private const string TIPO_CUENTA_PREPAGO = "5";
        BeneficiarioPrepagoRepository repBeneficiario = new BeneficiarioPrepagoRepository();
        private AfiliadoSumaRepository repAfiliado = new AfiliadoSumaRepository();

        //
        // GET: /FueraLineaPrepago/Filter/

        public ActionResult Filter()
        {
            return View();
        }

        //
        // POST: /FueraLineaPrepago/Filter/

        [HttpPost]
        public ActionResult Filter(string numdoc, string name)
        {
            List<BeneficiarioPrepagoIndex> beneficiarios = new List<BeneficiarioPrepagoIndex>();
            if (numdoc != "")
            {
                beneficiarios = repBeneficiario.Find(numdoc, "", "", "", "").OrderBy(x => x.Cliente.nameCliente).ThenBy(y => y.Afiliado.docnumber).ToList();
            }
            else
            {
                beneficiarios = repBeneficiario.Find("", name, "", "", "").OrderBy(x => x.Cliente.nameCliente).ThenBy(y => y.Afiliado.docnumber).ToList();
            }
            return View("Table", beneficiarios);
        }

        //
        // GET: /FueraLineaPrepago/Table/

        public ActionResult Table()
        {
            return View();
        }

        //
        // GET: /FueraLineaPrepago/AddNew/1

        public ActionResult AddNew(int id)
        {

            BeneficiarioPrepago record = repBeneficiario.Find(id);

            //BeneficiarioPrepago beneficiario = repBeneficiario.Find(id);
            SaldosMovimientos SaldosMovimientos = repAfiliado.FindSaldosMovimientos(record.Afiliado);

            BeneficiarioPrepagoViewModel model = new BeneficiarioPrepagoViewModel()
            {
                numdoc = record.Afiliado.docnumber,
                beneficiario = record.Afiliado.name + " " + record.Afiliado.lastname1,
                monto = "0,00",
                saldo = SaldosMovimientos.Saldos.First(x => x.accounttype.Equals(TIPO_CUENTA_PREPAGO)).saldo
            };

            return View(model);

        }

        //
        // POST: /FueraLineaPrepago/AddNew

        [HttpPost]
        public ActionResult AddNew(BeneficiarioPrepagoViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (decimal.Parse(model.monto) <= 0)
                {
                    ModelState.AddModelError("Monto", "El Monto de Transacción debe ser superior a cero.");
                }
                else if (model.monto.IndexOf(",") == -1)
                {
                    ModelState.AddModelError("Monto", "El Monto de Transacción debe contener coma (,) como símbolo separador decimal.");
                }
                else if (decimal.Parse(model.saldoactual) <= 0 || decimal.Parse(model.monto) > decimal.Parse(model.saldoactual))
                {
                    ModelState.AddModelError("Monto", "El Monto de Transacción supera el Saldo Disponible.");
                }
                else
                {
                    ViewModel viewmodel = new ViewModel();
                    string respuesta = repBeneficiario.CompraFueraLinea(model.documento, model.montotrx);
                    if (respuesta == null)
                    {
                        viewmodel.Title = "Prepago / Fuera de Línea / Crear Transacción de Compra";
                        viewmodel.Message = "La Transacción no pudo ser efectuada. Revise los estatus de la Tarjeta o Cuenta e intente de nuevo.";
                        viewmodel.ControllerName = "FueraLineaPrepago";
                        viewmodel.ActionName = "Filter";
                    }
                    else
                    {
                        viewmodel.Title = "Prepago / Fuera de Línea / Crear Transacción de Compra";
                        viewmodel.Message = "La Transacción ha sido efectuada satisfactoriamente. Clave de aprobación: " + respuesta;
                        viewmodel.ControllerName = "FueraLineaPrepago";
                        viewmodel.ActionName = "Filter";
                    }
                    return RedirectToAction("GenericView", viewmodel);
                }
            }

            return View();

        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

    }
}
