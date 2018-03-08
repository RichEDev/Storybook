using System;

/// <summary>
/// Summary description for cSecureData
/// </summary>
public class SecureData
{
    public SecureData()
    {
        //			tdes = new System.Security.Cryptography.TripleDESCryptoServiceProvider();
        //			tdes.Key = key;
        //			tdes.IV = iv;
    }

    public string Encrypt(string data)
    {
        System.Security.Cryptography.RijndaelManaged encryptor = new System.Security.Cryptography.RijndaelManaged();

        byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(data);

        System.IO.MemoryStream stream = new System.IO.MemoryStream();
        System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(stream, encryptor.CreateEncryptor(key, iv), System.Security.Cryptography.CryptoStreamMode.Write);
        
        System.IO.StreamWriter writer = new System.IO.StreamWriter(cs);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        
        return Convert.ToBase64String(stream.ToArray());
    }

    public string Decrypt(string data)
    {
        System.Security.Cryptography.RijndaelManaged decryptor = new System.Security.Cryptography.RijndaelManaged();

        System.IO.MemoryStream stream = new System.IO.MemoryStream();

        byte[] inputByteArray = Convert.FromBase64String(data);
        //stream.WriteTo(stream1);

        System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(stream, decryptor.CreateDecryptor(key, iv), System.Security.Cryptography.CryptoStreamMode.Write);

        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    private byte[] key
    {
        get
        {
            byte[] thekey = new byte[32];

            thekey[0] = 201;
            thekey[1] = 34;
            thekey[2] = 61;
            thekey[3] = 177;
            thekey[4] = 73;
            thekey[5] = 61;
            thekey[6] = 42;
            thekey[7] = 198;
            thekey[8] = 179;
            thekey[9] = 115;
            thekey[10] = 39;
            thekey[11] = 113;
            thekey[12] = 42;
            thekey[13] = 80;
            thekey[14] = 255;
            thekey[15] = 104;
            thekey[16] = 185;
            thekey[17] = 137;
            thekey[18] = 89;
            thekey[19] = 174;
            thekey[20] = 45;
            thekey[21] = 65;
            thekey[22] = 172;
            thekey[23] = 144;
            thekey[24] = 206;
            thekey[25] = 102;
            thekey[26] = 201;
            thekey[27] = 71;
            thekey[28] = 178;
            thekey[29] = 0;
            thekey[30] = 11;
            thekey[31] = 4;

            return thekey;
        }
    }

    private byte[] iv
    {
        get
        {
            byte[] theiv = new byte[16];
            theiv[0] = 148;
            theiv[1] = 93;
            theiv[2] = 123;
            theiv[3] = 24;
            theiv[4] = 109;
            theiv[5] = 9;
            theiv[6] = 122;
            theiv[7] = 147;
            theiv[8] = 64;
            theiv[9] = 112;
            theiv[10] = 218;
            theiv[11] = 217;
            theiv[12] = 11;
            theiv[13] = 116;
            theiv[14] = 235;
            theiv[15] = 55;

            return theiv;
        }
    }
}

