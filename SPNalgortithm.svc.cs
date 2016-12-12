using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace SPNService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceContract(Namespace = "SPNService")]
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SPNService
    {
        [OperationContract]
        public string Encryption(string key, string text)
        {
            string deneme = text;
            /* burada girilen textin uzunluğunun 2'şerli olarak şifrelenmesi için eğer uzunluk tek hane ise textin 
            sonunda 0 eklenerek çift haneye tamamlanıyor. Encrypt fonksiyonu eklendiğinde eklenen 0 değeri çıkarılacak!!*/
            if (text.Length % 2 == 1)
            {
                text += "0";
            }
            /* Kullanıcının girdiği türkçe karakterlerin dönüşümleri yapılıyor. */
            string[] a = { "ş", "ç", "ö", "ğ", "ü", "ı", "Ş", "Ö", "Ç", "Ü", "İ", "Ğ" };
            string[] b = { "s", "c", "o", "g", "u", "i", "S", "O", "C", "U", "I", "G" };
            for (int i = 0; i < a.Length; i++)
            {
                text = text.Replace(a[i], b[i]);
            }
            for (int i = 0; i < 8; i++)
            {
                key = key.Replace(a[i], b[i]);
            }
            /* Girilen texti ikili olarak ayırıp string tipinde bir diziye atıyoruz. */
            string[] ikiliText = new string[text.Length / 2];
            for (int i = 0; i < text.Length / 2; i++)
            {
                ikiliText[i] = text.Substring(i * 2, 2);
            }
            string[] encrypted = new string[text.Length / 2];
            for (int i = 0; i < ikiliText.Length; i++)
            {
                encrypted[i] = encryption(key, ikiliText[i]);
            }
            string encryptedText = "";
            foreach (var item in encrypted)
            {
                encryptedText += item;
            }
            if (deneme.Length % 2 != 0)
            {
                encryptedText = encryptedText.Substring(0, encryptedText.Length - 1);
            }

            return encryptedText;
        }

        private string encryption(string key, string plain)
        {
            string[] ikiliTextler = new string[4];
            int bitsOfText1 = ToBits(Convert.ToChar(plain.Substring(0, 1)));
            int bitsOfText2 = ToBits(Convert.ToChar(plain.Substring(1, 1)));
            int[] bitsOfPass = new int[8];
            for (int i = 0; i < 8; i++)
            {
                bitsOfPass[i] = ToBits(Convert.ToChar(key.Substring(i, 1)));
            }
            for (int i = 0; i < 4; i++)
            {
                ikiliTextler[i] = GetIntBinaryString(bitsOfPass[i * 2] ^ bitsOfText1) + GetIntBinaryString(bitsOfPass[i * 2 + 1] ^ bitsOfText2);
                bitsOfText1 = bitsToInt(randomer(ikiliTextler[i]).Substring(0, 8).ToCharArray());
                bitsOfText2 = bitsToInt(randomer(ikiliTextler[i]).Substring(8, 8).ToCharArray());
            }
            Char ikiliTextlerRandom1 = Convert.ToChar(bitsOfText1);
            Char ikiliTextlerRandom2 = Convert.ToChar(bitsOfText2);
            string ikiliTextlerRandom = ikiliTextlerRandom1.ToString() + ikiliTextlerRandom2.ToString();
            return ikiliTextlerRandom;
        }
        private string randomer(string ikiliText)
        {
            char[] arrays = ikiliText.ToCharArray();
            char[] arrayp = new char[16];
            for (int i = 0; i < 16; i += 1)
            {
                arrayp[((i % 4) * 4) + Convert.ToInt16(i / 4)] = arrays[i];
            }
            string Randomikili = new string(arrayp);
            return Randomikili;
        }
        static int ToBits(char text)
        {
            byte a = Convert.ToByte(text);
            return Convert.ToInt32(a);
        }
        private string GetIntBinaryString(int n)
        {
            char[] b = new char[8];
            int pos = 7;
            int i = 0;
            while (i < 8)
            {
                if ((n & (1 << i)) != 0)
                    b[pos] = '1';
                else
                    b[pos] = '0';
                pos--;
                i++;
            }
            return new string(b);
        }
        private int bitsToInt(char[] bits)
        {
            int t = 0;
            for (int i = 7; i >= 0; i--)
            {
                t += Convert.ToInt32(Math.Pow(2, i)) * Convert.ToInt32(bits[i].ToString());
            }
            return t;
        }
    }
}

