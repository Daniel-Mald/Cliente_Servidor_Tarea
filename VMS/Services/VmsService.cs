using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using VMS.Models.DTOs;

namespace VMS.Services
{
    public class VmsService
    {
        HttpListener _listener;
        public event EventHandler<VmsDTO>? VmsRecibido;
        public VmsService()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:9898/vms/");

        }
        public void IniciarServer()
        {
            if(!_listener.IsListening)
            {
                _listener.Start();
                

                new Thread(Escuchar) { IsBackground = true}.Start();
            }
        }
        public void DetenerServer()
        {
            _listener.Stop();
        }
        public void Escuchar()
        {
            while (true)
            {
                var _context = _listener.GetContext();
                var _pagina = File.ReadAllText("assets/index.html"); //falta poner la pagina html
                var _bufferPagina = Encoding.UTF8.GetBytes(_pagina);

                if(_context.Request.Url != null)
                {
                    if (_context.Request.Url.LocalPath == "/vms/")
                    {
                        _context.Response.ContentLength64 = _bufferPagina.Length;
                        _context.Response.OutputStream.Write(_bufferPagina,0, _bufferPagina.Length);
                        _context.Response.StatusCode = 200;
                        _context.Response.Close();
                    }
                    else if(_context.Request.HttpMethod == "POST" && _context.Request.Url.LocalPath == "/vms/cambiar")
                    {
                        
                        byte[] _bufferData =  new byte[_context.Request.ContentLength64];
                        _context.Request.InputStream.Read(_bufferData,0, _bufferData.Length);
                        string _data = Encoding.UTF8.GetString(_bufferData);
                        var _diccionario = HttpUtility.ParseQueryString(_data);

                        VmsDTO _vms = new()
                        {
                            Contenido = _diccionario["Contenido"] ?? "",
                            Color = _diccionario["Color"] ?? "#000",
                            CartelId = int.Parse(_diccionario["CartelId"] ?? "0"),
                            Remitente = _context.Request.RemoteEndPoint.Address.ToString()
                        };
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            VmsRecibido?.Invoke(this, _vms);
                        });

                        _context.Response.StatusCode = 302; 
                        _context.Response.RedirectLocation = "/vms/"; 
                        _context.Response.Close();
                    }
                }
                else
                {
                    _context.Response.StatusCode = 404;
                    _context.Response.Close();
                }

            }
        }
    }
}
