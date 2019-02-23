using System;
using System.Collections.Generic;

namespace Scalider.Security
{

    public partial class Crc
    {

        private static readonly IDictionary<string, CrcParameters> CrcAlgorithmParameters =
            new Dictionary<string, CrcParameters>(StringComparer.OrdinalIgnoreCase)
            {
                // CRC8
                {CrcAlgorithmNames.CRC8, new CrcParameters(8, 0x07, 0x00, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_CDMA2000, new CrcParameters(8, 0x9B, 0xFF, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_DARC, new CrcParameters(8, 0x39, 0x00, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_DVB_S2, new CrcParameters(8, 0xD5, 0x00, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_EBU, new CrcParameters(8, 0x1D, 0xFF, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_I_CODE, new CrcParameters(8, 0x1D, 0xFD, 0x00, false, false)},
                {CrcAlgorithmNames.CRC8_ITU, new CrcParameters(8, 0x07, 0x00, 0x55, false, false)},
                {CrcAlgorithmNames.CRC8_MAXIM, new CrcParameters(8, 0x31, 0x00, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_ROHC, new CrcParameters(8, 0x07, 0xFF, 0x00, true, true)},
                {CrcAlgorithmNames.CRC8_WCDMA, new CrcParameters(8, 0x9B, 0x00, 0x00, true, true)},

                // CRC16
                {CrcAlgorithmNames.CRC_A, new CrcParameters(16, 0x1021, 0xC6C6, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_ARC, new CrcParameters(16, 0x8005, 0x0000, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_AUG_CCITT, new CrcParameters(16, 0x1021, 0x1D0F, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_BUYPASS, new CrcParameters(16, 0x8005, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_CDMA2000, new CrcParameters(16, 0xC867, 0xFFFF, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_CCITT_FALSE, new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DDS_110, new CrcParameters(16, 0x8005, 0x800D, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DECT_R, new CrcParameters(16, 0x0589, 0x0000, 0x0001, false, false)},
                {CrcAlgorithmNames.CRC16_DECT_X, new CrcParameters(16, 0x0589, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_DNP, new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_EN_13757, new CrcParameters(16, 0x3D65, 0x0000, 0xFFFF, false, false)},
                {CrcAlgorithmNames.CRC16_GENIBUS, new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, false, false)},
                {CrcAlgorithmNames.CRC16_KERMIT, new CrcParameters(16, 0x1021, 0x0000, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_MAXIM, new CrcParameters(16, 0x8005, 0x0000, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_MCRF4XX, new CrcParameters(16, 0x1021, 0xFFFF, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_MODBUS, new CrcParameters(16, 0x8005, 0xFFFF, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_RIELLO, new CrcParameters(16, 0x1021, 0xB2AA, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_T10_DIF, new CrcParameters(16, 0x8BB7, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_TELEDISK, new CrcParameters(16, 0xA097, 0x0000, 0x0000, false, false)},
                {CrcAlgorithmNames.CRC16_TMS37157, new CrcParameters(16, 0x1021, 0x89EC, 0x0000, true, true)},
                {CrcAlgorithmNames.CRC16_USB, new CrcParameters(16, 0x8005, 0xFFFF, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_X_25, new CrcParameters(16, 0x1021, 0xFFFF, 0xFFFF, true, true)},
                {CrcAlgorithmNames.CRC16_XMODEM, new CrcParameters(16, 0x1021, 0x0000, 0x0000, false, false)},

                // CRC32
                {CrcAlgorithmNames.CRC32, new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {
                    CrcAlgorithmNames.CRC32_BZIP2,
                    new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, false, false)
                },
                {CrcAlgorithmNames.CRC32_C, new CrcParameters(32, 0x1EDC6F41, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {CrcAlgorithmNames.CRC32_D, new CrcParameters(32, 0xA833982B, 0xFFFFFFFF, 0xFFFFFFFF, true, true)},
                {CrcAlgorithmNames.CRC32_JAMCRC, new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, true, true)},
                {
                    CrcAlgorithmNames.CRC32_MPEG2,
                    new CrcParameters(32, 0x04C11DB7, 0xFFFFFFFF, 0x00000000, false, false)
                },
                {
                    CrcAlgorithmNames.CRC32_POXIS,
                    new CrcParameters(32, 0x04C11DB7, 0x00000000, 0xFFFFFFFF, false, false)
                },
                {CrcAlgorithmNames.CRC32_Q, new CrcParameters(32, 0x814141AB, 0x00000000, 0x00000000, false, false)},
                {CrcAlgorithmNames.CRC32_XFER, new CrcParameters(32, 0x000000AF, 0x00000000, 0x00000000, false, false)}
            };
        
    }

}