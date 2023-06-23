using BlazorEcommerce.Server.Servicios.PersonaSV;
using BlazorEcommerce.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaServicio _personaServicio;
        public readonly IConfiguration? _configuration;
        public PersonaController(IPersonaServicio personaServicio, IConfiguration? configuration)
        {
            _personaServicio = personaServicio;
            _configuration = configuration;
        }

        [HttpGet("Lista/{Rol:alpha}/{Valor:alpha?}")]
        public async Task<IActionResult> Lista(string Rol, string Valor = "NA")
        {
            if (Valor == "NA") Valor = "";
            return Ok(await _personaServicio.Lista(Rol, Valor));
        }

        [HttpGet("Obtener/{Id:int}")]
        public async Task<IActionResult> Obtener(int Id)
        {
            return Ok(await _personaServicio.Obtener(Id));
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] PersonaDTO modelo)
        {
            var Email = modelo.Correo;
            var Nombre = modelo.NombreCompleto;

            var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

            var message = new MailMessage();
            message.From = new MailAddress(emailSettings.Username);
            message.To.Add(new MailAddress(Email));
            message.Subject = "¡Bienvenido a TuZapatillaOnline!";
            message.Body = $"Estimado/a {Nombre} ,\r\n\r\nQueremos darte la más cordial bienvenida a TuZapatillaOnline, la tienda en línea donde podrás encontrar una gran variedad de zapatillas para todas las ocasiones.\r\n\r\nNos complace que hayas decidido registrarte en nuestro sitio y confiar en nosotros para adquirir tus zapatillas. Estamos seguros de que encontrarás el modelo perfecto para ti entre nuestra amplia selección de marcas y estilos.\r\n\r\nAdemás, queremos informarte que con tu cuenta en TuZapatillaOnline podrás disfrutar de ventajas exclusivas como acceso a ofertas especiales, seguimiento de tus pedidos en línea y más.\r\n\r\nSi tienes alguna pregunta o necesitas ayuda en cualquier momento, no dudes en ponerte en contacto con nosotros a través de nuestro sitio web o correo electrónico.\r\n\r\n¡Gracias por formar parte de la comunidad de TuZapatillaOnline! Esperamos que tengas una excelente experiencia de compra con nosotros.\r\n\r\nSaludos cordiales,\r\n TuZapatillaOnline";

            using (var client = new SmtpClient(emailSettings.SmtpServer, emailSettings.SmtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                client.EnableSsl = true;

                client.Send(message);
            }

            return Ok(await _personaServicio.Crear(modelo));
        }

        [HttpPost("Autorizacion")]
        public async Task<IActionResult> Autorizacion([FromBody]LoginDTO modelo)
        {
            return Ok(await _personaServicio.Autorizacion(modelo));
        }

        [HttpPut("Editar")]
        public async Task<IActionResult> Editar([FromBody] PersonaDTO modelo)
        {
            return Ok(await _personaServicio.Editar(modelo));
        }

        [HttpDelete("Eliminar/{Id:int}")]
        public async Task<IActionResult> Eliminar(int Id)
        {
            return Ok(await _personaServicio.Eliminar(Id));
        }



    }
}
