using System.Collections.Generic;

namespace Anyline_Windows_UWP_Examples.Model
{
    public static class ExampleList
    {
        private static readonly string AnalogDigital = "Analog/Digital Meter";
        private static readonly string DialMeter = "Dial Meter";
        private static readonly string PhotoCapture = "Photo Capture";
        private static readonly string MRZ = "Passport/MRZ";
        private static readonly string DrivingLicense = "Driving License";
        private static readonly string GermanIDFront = "German ID Front";
        private static readonly string LicensePlate = "EU License Plate";
        private static readonly string UniversalSerialNumber = "Universal Serial Number";
        private static readonly string VIN = "Vehicle Identification Number";
        private static readonly string ShippingContainer = "Shipping Container";
        private static readonly string Barcode = "Barcode";
        private static readonly string Bottlecap = "Bottlecap";
        private static readonly string VoucherCode = "Voucher Code";
        private static readonly string CowTag = "Cattle Tag";

        public static List<ExampleCategory> Items { get; } = new List<ExampleCategory>
        {
            new ExampleCategory("Meter", new List<ExamplePlugin>{
                new ExamplePlugin(AnalogDigital, "energy_config_analog_digital"),
                new ExamplePlugin(DialMeter, "energy_config_dial"),
                new ExamplePlugin(PhotoCapture, "energy_config_photo"),
            }),
            new ExampleCategory("ID", new List<ExamplePlugin>{
                new ExamplePlugin(MRZ, "id_config_mrz"),
                new ExamplePlugin(DrivingLicense, "id_config_driving_license"),
                new ExamplePlugin(GermanIDFront, "id_config_german_id_front"),
            }),
            new ExampleCategory("Vehicle", new List<ExamplePlugin>{
                new ExamplePlugin(LicensePlate, "vehicle_config_license_plate"),
            }),
            new ExampleCategory("MRO", new List<ExamplePlugin>{
                new ExamplePlugin(UniversalSerialNumber, "mro_config_usnr"),
                new ExamplePlugin(VIN, "mro_config_vin"),
                new ExamplePlugin(ShippingContainer, "mro_config_shipping_container"),
            }),
            new ExampleCategory("Others", new List<ExamplePlugin>{
                new ExamplePlugin(Barcode, "others_config_barcode"),
                new ExamplePlugin(Bottlecap, "others_config_bottlecap"),
                new ExamplePlugin(VoucherCode, "others_config_voucher_code"),
                new ExamplePlugin(CowTag, "others_config_cow_tag"),
            })
        };
    }
}
