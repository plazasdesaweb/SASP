﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Modules
{
    public static class Globals
    {
        //Constantes de Afiliación
        public const int ID_TYPE_SUMA = 1;
        public const int ID_TYPE_PREPAGO = 2;
        public const string WEB_TYPE = "1";
        public const int ID_ESTATUS_AFILIACION_INICIAL = 0;
        public const int ID_ESTATUS_AFILIACION_ACTIVA = 2;
        public const int ID_ESTATUS_AFILIACION_ELIMINADA = 3;
        public const int ID_REASONS_INICIAL = 1;        

        //Constantes de Ordenes de Recarga
        public const int ID_ESTATUS_ORDEN_NUEVA = 0;
        public const int ID_ESTATUS_ORDEN_APROBADA = 1;
        public const int ID_ESTATUS_ORDEN_RECHAZADA = 2;
        public const int ID_ESTATUS_ORDEN_PROCESADA = 3;
        
        //Constantes de Detalle de Ordenes de Recarga
        public const int ID_ESTATUS_DETALLEORDEN_INCLUIDO = 0;
        public const int ID_ESTATUS_DETALLEORDEN_APROBADO = 1;
        public const int ID_ESTATUS_DETALLEORDEN_EXCLUIDO = 2;
        public const int ID_ESTATUS_DETALLEORDEN_PROCESADO = 3;

        //Constantes de Cards
        public const string TIPO_CUENTA_SUMA = "7";
        public const string TIPO_CUENTA_PREPAGO = "5";
        public const string ID_ESTATUS_TARJETA_SUSPENDIDA = "6";
        public const string TRANSCODE_CONSULTA_DE_SALDO = "121";
        public const string TRANSCODE_ANULACION = "161";
        //Constantes de Cards - Prepago
        public const string TRANSCODE_COMPRA_PREPAGO = "145";
        public const string TRANSCODE_RECARGA_PREPAGO = "200";
        public const string TRANSCODE_ANULACION_RECARGA_PREPAGO = "161";        
        public const string TRANSCODE_TRANSFERENCIA_CREDITO_PREPAGO = "201";
        public const string TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_PREPAGO = "661";
        public const string TRANSCODE_TRANSFERENCIA_DEBITO_PREPAGO = "202";
        public const string TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_PREPAGO = "761";
        //Constantes de Cards - Suma
        public const string TRANSCODE_TRANSFERENCIA_CREDITO_SUMA = "203";
        public const string TRANSCODE_ANULACION_TRANSFERENCIA_CREDITO_SUMA = "861";
        public const string TRANSCODE_TRANSFERENCIA_DEBITO_SUMA = "204";
        public const string TRANSCODE_ANULACION_TRANSFERENCIA_DEBITO_SUMA = "961";
        public const string TRANSCODE_ACREDITACION_SUMA = "318";
        public const string TRANSCODE_CANJE_SUMA = "319";
        public const string FACTOR_ACREDITACION = "1";
        public const string FACTOR_CANJE = "100";

        
    }
}