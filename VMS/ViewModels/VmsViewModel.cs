using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VMS.Models.DTOs;
using VMS.Services;

namespace VMS.ViewModels
{
    public partial class VmsViewModel:ObservableObject
    {
        public ObservableCollection<VmsDTO> _vmsLista { get; set; } = new ObservableCollection<VmsDTO>();
        VmsService _server;
        int _numCarteles;
        private readonly Dictionary<string, string[]> coloresReales = new()
{
    { "Rojo", new string[] { "#FF2E2E", "#FFE2E2" }},
    { "Morado", new string[] { "#932EFF", "#F2E2FF" }},
    { "Azul", new string[] { "#0000FF", "#E2E2FF" }},
    { "Naranja", new string[] { "#FFA500", "#FFE2C6" }},
    { "Cian", new string[] { "#00FFFF", "#E2FFFF" }}
};

        public string IP 
        {
            get
            {
                return string.Join(",", Dns.GetHostAddresses(Dns.GetHostName()).
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).
                    Select(x => x.ToString()));
            }
        }
        public VmsViewModel()
        {
            _server = new VmsService();
            _server.VmsRecibido += _server_VmsRecibido;
            _server.IniciarServer();
            LlenarCarteles();
        }

        private void _server_VmsRecibido(object? sender, VmsDTO e)
        {
            _vmsLista[e.CartelId] = e;
        }
        void LlenarCarteles()
        {
            for (int i = 0; i < 5; i++)
            {
              
                VmsDTO x = new()
                {
                    CartelId = i,
                    Color = coloresReales["Rojo"][0],
                    Contenido = "jamaica",
                    Remitente = "",
                    ColorClaro = coloresReales["Rojo"][1]
                    
                };
                _vmsLista.Add(x);
            }
        }
        
    }
}
