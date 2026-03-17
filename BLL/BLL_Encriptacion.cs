using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.IO;
using DAL;
using BE;

namespace BLL
{
    public class BLL_Encriptacion
    {
        // Con esto aplico la configuracion de enviar un correo cada vez que registramos un Usuario
        public static string GenerarContraseña() 
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0, 8);
            return clave;
        }

        // Lo uso para encriptar el tetxto a SHA256
        public static string ConvertirClave(string texto) 
        {
            StringBuilder br = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create()) 
            {
                Encoding  encoding = Encoding.UTF8;
                byte[] result = hash.ComputeHash(encoding.GetBytes(texto));
                foreach (byte b in result)
                    br.Append(b.ToString("X2"));
            }
            return br.ToString();   
        }

        public static bool EnviarCorreo(string correo, string asunto, string mensaje)
        {
            bool resultado = false;

            try
            {
                MailMessage gmail = new MailMessage();
                gmail.To.Add(correo);
                gmail.From = new MailAddress("lautaronahuelponce23@gmail.com");
                gmail.Subject = asunto;
                gmail.Body = mensaje;
                gmail.IsBodyHtml = true;

                var smtp = new SmtpClient()
                {
                    Credentials = new NetworkCredential("lautaronahuelponce23@gmail.com", "qrgchdlilorafrer"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                };
                smtp.Send(gmail);
                resultado = true;
            }
            catch (Exception ex) 
            {
                // Registrar error de envío de correo en bitacora
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error al enviar correo",
                    Detalle = ex.Message
                });

                resultado = false;
            }
            return resultado;
        }

        public static string ConvertirBase64(string ruta, out bool conversion) 
        {
            string TextoBase64 = string.Empty;
            conversion = true;

            try
            {
                byte[] bytes = File.ReadAllBytes(ruta);
                TextoBase64 = Convert.ToBase64String(bytes);
            }
            catch
            {
                conversion = false; 
            }

            return TextoBase64; 
        }

        public static bool ValidarPoliticaSeguridad(string contraseña, out string mensaje)
        {
            mensaje = "";
            if (contraseña.Length < 8)
            {
                mensaje = "La contraseña debe tener al menos 8 caracteres.";
                return false;
            }

            if (!contraseña.Any(char.IsUpper))
            {
                mensaje = "La contraseña debe contener al menos una letra mayúscula.";
                return false;
            }

            if (!contraseña.Any(char.IsLower))
            {
                mensaje = "La contraseña debe contener al menos una letra minúscula.";
                return false;
            }

            return true;
        }


        // Cifrado y Descifrado usando el metodo AES (para datos sensibles)

        private static readonly string ClaveSecreta = "ClaveFuerteSneakRush2025"; // pueden cambiarla si quieren
        private static readonly string IV = "SneakRushVectorIV"; // Tiene que tener 16 caracteres si o si / El IV es el vector de inicializacion

        public static string EncriptarTexto(string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                return textoPlano;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(ClaveSecreta.PadRight(32).Substring(0, 32));
                    aes.IV = Encoding.UTF8.GetBytes(IV.PadRight(16).Substring(0, 16));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(textoPlano);
                        }
                        return Convert.ToBase64String(ms.ToArray()); // lo guardamos en Base64 para SQL
                    }
                }
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error al encriptar texto AES",
                    Detalle = ex.Message
                });

                return textoPlano; // devolvemos el texto original para no romper el flujo
            }
        }

        public static string DesencriptarTexto(string textoCifrado)
        {
            if (string.IsNullOrEmpty(textoCifrado))
                return textoCifrado;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(ClaveSecreta.PadRight(32).Substring(0, 32));
                    aes.IV = Encoding.UTF8.GetBytes(IV.PadRight(16).Substring(0, 16));

                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(textoCifrado)))
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error al desencriptar texto AES",
                    Detalle = ex.Message
                });

                return textoCifrado; // devolvemos el cifrado para no romper la ejecución
            }
        }

        public static bool EsBase64(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) 
            {
                return false;
            }
            texto = texto.Trim();

            // Verifica si cumple el patrón base64
            if (texto.Length % 4 != 0) 
            {
                return false;
            }

            return texto.All(c =>
                char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '='
            );
        }

    }
}
