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
                    Color = "red",
                    Contenido = "jamaica",
                    Remitente = ""
                };
                _vmsLista.Add(x);
            }
        }
    }
}
