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
using System.Timers;
using System.Media;

namespace VMS.ViewModels
{
    public partial class VmsViewModel:ObservableObject
    {
        public ObservableCollection<VmsDTO> _vmsLista { get; set; } = new ObservableCollection<VmsDTO>();
        VmsService _server;
        System.Timers.Timer _timer;
        Random _random;
        SoundPlayer _player;
        SoundPlayer _player2;
        SoundPlayer _player3;
        int _numCarteles;
        private readonly Dictionary<string, string[]> coloresReales = new()
{
    { "Rojo", new string[] { "#FF2E2E", "#FFE2E2" }},
    { "Morado", new string[] { "#932EFF", "#F2E2FF" }},
    { "Azul", new string[] { "#0000FF", "#E2E2FF" }},
    { "Naranja", new string[] { "#FFA500", "#FFE2C6" }},
    { "Cian", new string[] { "#00FFFF", "#E2FFFF" }},
    { "Magenta", new string[] { "#FF2EFF", "#FFE2FF" }}
         ,{"Apagado", new string [] {"Black","Transparent"}}
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
            _timer = new(5000);
            _player = new SoundPlayer("Assets/soundLight.wav");
            _player2 = new SoundPlayer("Assets/soundLight2.wav");
            _player3 = new SoundPlayer("Assets/back.wav");

           
            _random = new Random();
            _timer.Elapsed += _timer_ElapsedAsync;
            _timer.Start();
           
        }

        private async void _timer_ElapsedAsync(object? sender, ElapsedEventArgs e)
        {
            _player3.Play();
            int cartel = _random.Next(0, 16);
            var cart = _vmsLista[cartel];
            string[] s = new string[] { cart.Color ,cart.ColorClaro };

            int ii = _random.Next(1, 50);
            int delay = _random.Next(50, 400);
            if (ii % 2 == 0)
            {
                _player.Play();
            }
            else
            {
                _player2.Play();
            }
            for (int i = 0; i < ii; i++)
            {
                _vmsLista[cartel].ColorClaro = coloresReales["Apagado"][1];
                _vmsLista[cartel].Color = coloresReales["Apagado"][0];
                
                OnPropertyChanged(nameof(_vmsLista));
                
                
                await Task.Delay(delay);

                _vmsLista[cartel].ColorClaro = s[1];
                _vmsLista[cartel].Color = s[0];
                OnPropertyChanged(nameof(_vmsLista));
                await Task.Delay(delay);

            }
           //_player.Stop();
            //_player2.Stop();
            
        }

        

        private void _server_VmsRecibido(object? sender, VmsDTO e)
        {

            //_vmsLista[e.CartelId] = e;
            //_vmsLista[e.CartelId].ColorClaro = coloresReales[e.Color][1];
            //_vmsLista[e.CartelId].Color = coloresReales[e.Color][0];
            e.ColorClaro = coloresReales[e.Color][1];
            e.Color = coloresReales[e.Color][0];
            _vmsLista[e.CartelId] = e;
            
        }
        void LlenarCarteles()
        {
            for (int i = 0; i < 16; i++)
            {
              
                VmsDTO x = new()
                {
                    CartelId = i,
                    Color = coloresReales["Morado"][0],
                    Contenido = $"Disponible num. {i}",
                    Remitente = "",
                    ColorClaro = coloresReales["Morado"][1]
                    
                };
                _vmsLista.Add(x);
            }
        }
        
    }
}
