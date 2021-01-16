using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Nuka.Sample.API.Certificates
{
    public class Certificate
    {
        public static X509Certificate2 Get()
        {
            var password = "P@ssw0rd";
            var assembly = typeof(Certificate).GetTypeInfo().Assembly;

            using var stream = assembly.GetManifestResourceStream("Nuka.Sample.API.Certificates.sample_api.pfx");
            return new X509Certificate2(ReadStream(stream), password);
        }

        private static byte[] ReadStream(Stream input)
        {
            int read;
            var buffer = new byte[16 * 1024];

            using var ms = new MemoryStream();
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }
    }
}