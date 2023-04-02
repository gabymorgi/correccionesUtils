using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;

namespace checkCopied
{
    internal class MailSender
    {
        /**
         * 0:      Alumnos que hicieron contenido opcional
         * 1: 10 - Alumnos con una nota perfecta
         * 2: 09 - Alumnos con errores menores
         * 3: 08 - Alumnos con errores, pero que aún así lograron aprobar la materia
         * 4: 06 - Alumnos que llegaron con lo justo a la nota
         * 5: 04 - Alumnos que no llegaron a la nota, y tendrán que presentarse al recuperatorio
         * 6: 00 - Alumnos que no entregaron el examen
         */
        static string[] messages = new string[]
        {
            @"¡Felicidades por tu arduo trabajo y dedicación!. 🥳
Aprecio mucho el hecho de que hayas decidido ir más allá de lo requerido y hacer contenido adicional.
Tu esfuerzo no pasó desapercibido",
            @"¡Increíble trabajo! Tu dedicación y esfuerzo han dado sus frutos. 🎉
Tu nota refleja tu gran comprensión de los conceptos y tu habilidad para aplicarlos.
Espero que sigas así y estoy seguro de que tendrás mucho éxito en el futuro.",
            @"Buen trabajo en general. 👏
Aunque tu nota refleja algunos errores menores, espero que hayas aprendido de ellos y que puedas aplicar lo que has aprendido en futuros proyectos.
¡Sigue adelante!",
            @"Felicitaciones por haber aprobado el examen. 🙌
Aunque tu nota refleja algunos errores, estoy seguro de que has aprendido mucho en este curso y estoy emocionado por ver cómo aplicarás tus habilidades en el futuro.
¡Sigue adelante y nunca dejes de aprender!",
            @"¡Felicidades por aprobar el examen! 😊
Espero que hayas aprendido mucho en este curso y que puedas aplicar tus habilidades en futuros proyectos.
Siéntete orgulloso de tu trabajo y sigue adelante.",
            @"Lamento que no hayas logrado aprobar el examen. 😕
Sé que tienes el potencial para hacerlo y espero que puedas sacar lo mejor de ti mismo en el recuperatorio.
Si necesitas ayuda adicional, no dudes en contactarme.
¡Te deseo todo lo mejor en tu preparación para el recuperatorio!",
            @"Lamento que no hayas entregado el examen. 🥺
Si hubo algún problema que te impidió entregarlo, por favor házmelo saber.",
        };

        static string headerTemplate = @"Estimado/a [NOMBRE_ALUMNO],

Espero que estés teniendo un excelente día. Te escribo para informarte sobre las notas del examen integrador de FrontEnd III.

Tu nota fue [NOTA] y tu calificación final es [CALIFICACION_FINAL].

[MESSAGE]

Te comparto las observaciones realizadas que complementan tu nota:

[ANOTACIONES]

Recuerden que pueden solicitar una revisión respondiento a este email si consideran que hubo algún error en la calificación.

En cualquier caso, les agradezco por haber tomado este curso conmigo y espero haber cumplido con sus expectativas.
¡Felicidades a todos por el trabajo realizado!

Atentamente,
ChatGPT y Gabriel Morgillo";

        static string asunto = "FrontEnd III Correccion Integrador";

        /**
         * csv must have this format:
         * senderEmail; senderPassword
         * nombre, email, notaIntegrador, notaFinal, anotaciones
         */
        static void SendEmailsFromCSV(string csvPath)
        {
            var lineas = File.ReadAllLines(csvPath);

            //get the first element of the list
            var firstLine = lineas[0];
            var senderEmail = firstLine.Split(',')[0];
            var senderPassword = firstLine.Split(',')[1];
            //pop the first element of the list
            lineas = lineas.Skip(1).ToArray();
            foreach (var linea in lineas)
            {
                // Parsea la línea del CSV para obtener el nombre del integrante y la URL del repositorio
                var campos = linea.Split(',');
                var nombreIntegrante = campos[0].Trim();
                var email = campos[1].Trim();
                var nota = campos[2].Trim();
                var anotations = campos[3].Trim();
                var calificacionFinal = int.Parse(campos[4].Trim());

                var body = headerTemplate
                    .Replace("[NOMBRE_ALUMNO]", nombreIntegrante)
                    .Replace("[NOTA]", nota)
                    .Replace("[CALIFICACION_FINAL]", calificacionFinal.ToString())
                    .Replace("[MESSAGE]", messages[calificacionFinal])
                    .Replace("[ANOTACIONES]", anotations);

                SendEmail(senderEmail, senderPassword, email, asunto, body);
            }
        }

        /**
         * You must allow less secure apps in your gmail account to use this method
         * https://myaccount.google.com/security
         */
        public static void SendEmail(string from, string psw, string to, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            //smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from, psw);
            MailMessage message = new MailMessage(from, to, subject, body);
            smtpClient.Send(message);
        }
    }
}
