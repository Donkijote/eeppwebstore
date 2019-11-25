using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Checkout
    {
        public BindSelect BindSelect { get; set; }
        public List<CartProductList> ProductList { get; set; }
        public tblUsers User { get; set; }
        public tblAddresses Address { get; set; }
        public CheckoutForm Form { get; set; }
    }

    public class CheckoutForm
    {
        public CheckoutInfo CheckoutInfo { get; set; }
        public CheckoutShipping CheckoutShipping { get; set; }
        public CheckoutPayMethod CheckoutPayMethod { get; set; }
        public CheckoutBill CheckoutBill { get; set; }

    }

    public class CheckoutInfo
    {
        public int CheckoutDocument { get; set; }
        public string CheckoutId { get; set; }
        public string CheckoutName { get; set; }
        public string CheckoutLastName { get; set; }
        public string CheckoutEmail { get; set; }
        public int CheckoutPhone { get; set; }
        public int CheckoutBill { get; set; }
        public MainAddress MainAddress { get; set; }
        public NewAddress NewAddress { get; set; }

    }

    public class CheckoutShipping
    {
        public int strShippingType { get; set; }
        public int CheckoutSameAddress { get; set; }
        public int CheckoutSamePerson { get; set; }
        public string CheckoutAnotherPersonName { get; set; }
        public string CheckoutAnotherPersonLastName { get; set; }
        public string CheckoutAnotherPersonId { get; set; }
        public int CheckoutAnotherPersonPhone { get; set; }
        public int CheckoutAnotherState { get; set; }
        public int CheckoutAnotherProvince { get; set; }
        public int CheckoutAnotherComune { get; set; }
        public string CheckoutAnotherCity { get; set; }
        public string CheckoutAnotherAddressOne { get; set; }
        public string CheckoutAnotherAddressTwo { get; set; }
        public string CheckoutAnotherType{ get; set; }
        public int? CheckoutAnotherZip { get; set; }
    }

    public class CheckoutPayMethod
    {
        public string CheckoutPay { get; set; }
    }

    public class CheckoutBill
    {
        public int SubTotal { get; set; }
        public int Discount { get; set; }
        public int TotalN { get; set; }
        public int Taxes { get; set; }
        public int Shipping { get; set; }
    }

    public class MainAddress
    {
        public int CheckoutState { get; set; }
        public int CheckoutProvince { get; set; }
        public int CheckoutComune { get; set; }
        public string CheckoutCity { get; set; }
        public string CheckoutAddressOne { get; set; }
        public string CheckoutAddressTwo { get; set; }
        public string CheckoutType { get; set; }
        public int? CheckoutZip { get; set; }
    }

    public class NewAddress
    {
        public int CheckoutStateNewAddress { get; set; }
        public int CheckoutProvinceNewAddress { get; set; }
        public int CheckoutComuneNewAddress { get; set; }
        public string CheckoutCityNewAddress { get; set; }
        public string CheckoutAddressOneNewAddress { get; set; }
        public string CheckoutAddressTwoNewAddress { get; set; }
        public string CheckoutTypeNewAddress { get; set; }
        public int? CheckoutZipNewAddress { get; set; }
    }

    public class Responses
    {
        public string title { get; set; }
        public string responseText { get; set; }
    }
}