using System.ComponentModel;

namespace UGRS.Core.Auctions.Enums.System
{
    public enum ConfigurationKeyEnum : int
    {
        [DescriptionAttribute("Precio del alimento")]
        FOOD_ITEM_PRICE = 1,
        [DescriptionAttribute("Código del alimento")]
        FOOD_ITEM_CODE = 2,
        [DescriptionAttribute("Código de la comisión")]
        COMISSION_ITEM_CODE = 3,
        [DescriptionAttribute("IVA del alimento")]
        FOOD_TAX_CODE = 4,
        [DescriptionAttribute("IVA de la comisión")]
        COMISSION_TAX_CODE = 5,
        [DescriptionAttribute("Almacén de subastas")]
        AUCTIONS_WAREHOUSE = 6,
        [DescriptionAttribute("Almacén de alimento")]
        FOOD_WAREHOUSE = 7,
        [DescriptionAttribute("Almacén de corrales")]
        CORRALS_WAREHOUSE = 8,
        [DescriptionAttribute("Almacén de rechazo")]
        REJECTION_WAREHOUSE = 9,
        [DescriptionAttribute("Series de los documentos de subastas")]
        DOCUMENTS_SERIES = 10,
        [DescriptionAttribute("Series de los socios de negocios")]
        BUSINESS_PARTNER_SERIES = 11,
        [DescriptionAttribute("Centro de costos de subasta")]
        AUCTION_COSTING_CODE = 12,
        [DescriptionAttribute("Cuenta deudores")]
        DEBTORS_ACCOUNT = 13,
        [DescriptionAttribute("Cuenta acreedores")]
        CREDITORS_ACCOUNT = 14,
        [DescriptionAttribute("Cuenta guias")]
        GUIDES_ACCOUNT = 15,
        [DescriptionAttribute("Método de pago de contado")]
        CASH_PAYMENT_METHOD = 16,
        [DescriptionAttribute("Método de pago de crédito")]
        CREDIT_PAYMENT_METHOD = 17,
        [DescriptionAttribute("Termino de pago de contado")]
        CASH_PAYMENT_TERMS = 18,
        [DescriptionAttribute("Termino de pago de crédito")]
        CREDIT_PAYMENT_TERMS = 19,
        [DescriptionAttribute("Cobro del 3 porciento")]
        THREE_PERCENT_PAYMENT = 20,
        [DescriptionAttribute("Cuenta de no cobro para guias")]
        NO_PAYMENT_GUIDES = 21,
        [DescriptionAttribute("Version de la aplicacion")]
        APP_VERSION = 22
    }
}
