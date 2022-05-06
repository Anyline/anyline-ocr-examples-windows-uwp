using System.Collections.Generic;

namespace Anyline_Windows_UWP_Examples.Model
{
    public static class ExampleList
    {
        private static readonly string AnalogDigital = "Analog/Digital Meter";
        private static readonly string DialMeter = "Dial Meter";
        private static readonly string PhotoCapture = "Photo Capture";
        private static readonly string MRZ = "Passport/MRZ";
        private static readonly string UniversalID = "Universal ID";
        private static readonly string ArabicID = "Arabic IDs";
        private static readonly string CyrillicID = "Cyrillic IDs";
        private static readonly string LicensePlate = "License Plates";
        private static readonly string LicensePlateUS = "License Plates - US";
        private static readonly string LicensePlateAfrica = "License Plates - Africa";
        private static readonly string UniversalSerialNumber = "Universal Serial Number";
        private static readonly string VIN = "Vehicle Identification Number";
        private static readonly string TIN_UNIVERSAL = "Tire Identification Number - Universal";
        private static readonly string TIN_DOT = "Tire Identification Number - DOT (North America only)";
        private static readonly string TIRE_SIZE = "Tire Size";
        private static readonly string COMMERCIAL_TIRE_ID = "Commercial Tire ID";
        private static readonly string ShippingContainerHorizontal = "Shipping Container - Horizontal";
        private static readonly string ShippingContainerVertical = "Shipping Container - Vertical";
        private static readonly string Barcode = "Barcode";

        public static List<ExampleCategory> Items { get; } = new List<ExampleCategory>
        {
            new ExampleCategory("Meter", new List<ExamplePlugin>{
                new ExamplePlugin(AnalogDigital, "energy_config_analog_digital"),
                new ExamplePlugin(DialMeter, "energy_config_dial"),
                new ExamplePlugin(PhotoCapture, "energy_config_photo"),
            }),
            new ExampleCategory("ID", new List<ExamplePlugin>{
                new ExamplePlugin(UniversalID, "id_config_universal_id"),
                new ExamplePlugin(ArabicID, "id_config_arabic_id"),
                new ExamplePlugin(CyrillicID, "id_config_cyrillic_id"),
                new ExamplePlugin(MRZ, "id_config_mrz")
            }),
            new ExampleCategory("Vehicle", new List<ExamplePlugin>{
                new ExamplePlugin(LicensePlate, "vehicle_config_license_plate"),
                new ExamplePlugin(LicensePlateUS, "vehicle_config_license_plate_us"),
                new ExamplePlugin(LicensePlateAfrica, "vehicle_config_license_plate_africa"),
                new ExamplePlugin(VIN, "mro_config_vin"),
            }),
            new ExampleCategory("Tire", new List<ExamplePlugin>{
                new ExamplePlugin(TIN_UNIVERSAL, "tin_universal"),
                new ExamplePlugin(TIN_DOT, "tin_dot"),
                new ExamplePlugin(TIRE_SIZE, "tire_size"),
                new ExamplePlugin(COMMERCIAL_TIRE_ID, "commercial_tire_id"),
            }),
            new ExampleCategory("MRO", new List<ExamplePlugin>{
                new ExamplePlugin(UniversalSerialNumber, "mro_config_usnr"),
                new ExamplePlugin(ShippingContainerHorizontal, "mro_config_shipping_container_horizontal"),
                new ExamplePlugin(ShippingContainerVertical, "mro_config_shipping_container_vertical"),
            }),
            new ExampleCategory("Others", new List<ExamplePlugin>{
                new ExamplePlugin(Barcode, "others_config_barcode")
            })
        };
    }
}
