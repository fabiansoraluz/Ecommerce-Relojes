﻿using BlazorEcommerce.Server.Repositorios;
using BlazorEcommerce.Shared;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace BlazorEcommerce.Server.Servicios.PersonaSV
{
    public class PersonaServicio : IPersonaServicio
    {
        private readonly IGenericoRepositorio<Persona> _personaRepositorio;
        private readonly IMapper _mapper;
        public readonly IConfiguration? _configuration;
        public PersonaServicio(IGenericoRepositorio<Persona> personaRepositorio, IMapper mapper, IConfiguration configuration)
        {
            _personaRepositorio = personaRepositorio;
            _mapper = mapper;
            _configuration = configuration;

        }
        public async Task<ResponseDTO<PersonaDTO>> Obtener(int id)
        {

            ResponseDTO<PersonaDTO> response = new ResponseDTO<PersonaDTO>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var consulta = _personaRepositorio.Consultar(p => p.IdPersona == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                    response.Resultado = _mapper.Map<PersonaDTO>(fromDbModelo);
                else
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se encontraron coincidencias";
                }

            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = null;
            }
            return response;
        }
        public async Task<ResponseDTO<List<PersonaDTO>>> Lista(string Rol, string Valor)
        {
            ResponseDTO<List<PersonaDTO>> response = new ResponseDTO<List<PersonaDTO>>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var consulta = _personaRepositorio.Consultar(p =>
                    p.Rol == Rol &&
                    string.Concat(p.NombreCompleto.ToLower(), p.Correo.ToLower()).Contains(Valor.ToLower())
                );

                List<PersonaDTO> lista = _mapper.Map<List<PersonaDTO>>(await consulta.ToListAsync());
                response.Resultado = lista;
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = null;
            }
            return response;
        }


        public async Task<ResponseDTO<PersonaDTO>> Crear(PersonaDTO modelo)
        {
            ResponseDTO<PersonaDTO> response = new ResponseDTO<PersonaDTO>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var dbModelo = _mapper.Map<Persona>(modelo);

                // Encriptar la clave
                dbModelo.Clave = HashClave(modelo.Clave);

                var rspModelo = await _personaRepositorio.Crear(dbModelo);

                if (rspModelo.IdPersona != 0)
                {
                    response.Resultado = _mapper.Map<PersonaDTO>(rspModelo);
                }
                else
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se pudo crear";
                }
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = null;
            }

            return response;
        }


        private string HashClave(string clave)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(clave));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }



        public async Task<ResponseDTO<bool>> Editar(PersonaDTO modelo)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var consulta = _personaRepositorio.Consultar(p => p.IdPersona == modelo.IdPersona);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    fromDbModelo.NombreCompleto = modelo.NombreCompleto;
                    fromDbModelo.Correo = modelo.Correo;
                    fromDbModelo.Clave = modelo.Clave;

                    var respuesta = await _personaRepositorio.Editar(fromDbModelo);

                    if (!respuesta)
                    {
                        response.EsCorrecto = false;
                        response.Mensaje = "No se pudo editar";
                    }
                }
                else
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se encontraron resultados";
                }

            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = false;
            }

            return response;
        }

        public async Task<ResponseDTO<bool>> Eliminar(int id)
        {
            ResponseDTO<bool> response = new ResponseDTO<bool>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var consulta = _personaRepositorio.Consultar(p => p.IdPersona == id);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    var respuesta = await _personaRepositorio.Eliminar(fromDbModelo);

                    if (!respuesta)
                    {
                        response.EsCorrecto = false;
                        response.Mensaje = "No se pudo eliminar";
                    }
                }
                else
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se encontraron resultados";
                }

            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = false;
            }

            return response;
        }

        public async Task<ResponseDTO<SesionDTO>> Autorizacion(LoginDTO modelo)
        {
            ResponseDTO<SesionDTO> response = new ResponseDTO<SesionDTO>()
            {
                Mensaje = "Ok",
                EsCorrecto = true
            };

            try
            {
                var consulta = _personaRepositorio.Consultar(p => p.Correo == modelo.Correo);
                var fromDbModelo = await consulta.FirstOrDefaultAsync();

                if (fromDbModelo != null)
                {
                    // Encriptar la clave ingresada y compararla con la clave encriptada almacenada
                    string claveEncriptada = HashClave(modelo.Clave);
                    if (claveEncriptada == fromDbModelo.Clave)
                    {
                        response.Resultado = _mapper.Map<SesionDTO>(fromDbModelo);
                    }
                    else
                    {
                        response.EsCorrecto = false;
                        response.Mensaje = "Credenciales inválidas";
                    }
                }
                else
                {
                    response.EsCorrecto = false;
                    response.Mensaje = "No se encontraron coincidencias";
                }
            }
            catch (Exception ex)
            {
                response.EsCorrecto = false;
                response.Mensaje = ex.Message;
                response.Resultado = null;
            }
            return response;
        }
    }
}
