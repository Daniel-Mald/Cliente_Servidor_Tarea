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
using System.Text.Json;
using System.IO;
using VMS.Models;

namespace VMS.ViewModels
{
    public partial class VmsViewModel:ObservableObject
    {
        public ObservableCollection<VmsDTO> _vmsLista { get; set; } = new ObservableCollection<VmsDTO>();
        CartelParpadeante[] _listaParpadeo;
        VmsService _server;
        System.Timers.Timer _timer;
        Random _random;
        SoundPlayer _player;
        SoundPlayer _player2;
        SoundPlayer _player3;
        public string IP
        {
            get
            {
                return string.Join(",", Dns.GetHostAddresses(Dns.GetHostName()).
                    Where(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).
                    Select(x => x.ToString()));
            }
        }
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



        private readonly Dictionary<string, string> _pictogramas = new()
        {
            {"Peligro","Solid_SkullCrossbones" },
            {"No fumar","Solid_SmokingBan" },
            {"Gato","Solid_Cat" },
            {"Stop","Regular_StopCircle" },
            {"Steam","Brands_Steam" },
            {"Fantasma","Solid_Ghost" },
            {"Oculto","Solid_EyeSlash" },
            {"Wifi","Solid_Wifi" },
            {"Incognito","Solid_UserSecret" },
            {"Atencion","Solid_ExclamationCircle" },
            {"Comida","Solid_Utensils" },
            {"Microfono","Solid_Microphone" },
            {"Gasolina","Solid_GasPump" },
            {"Cuervo","Solid_Crow" },
            {"None","None" }

        };
        public VmsViewModel()
        {
            
            _server = new VmsService();
            _server.VmsRecibido += _server_VmsRecibido;
            _server.IniciarServer();
            
            CargarDatos();
            _listaParpadeo = _vmsLista.Select(x => new CartelParpadeante
            {
                Color = x.Color,
                ColorClaro = x.ColorClaro,
                Id = x.CartelId,
                Parpadeando = false
            }).ToArray();

            _timer = new(5000);
            _player = new SoundPlayer("Assets/soundLight.wav");
            _player2 = new SoundPlayer("Assets/soundLight2.wav");
            _player3 = new SoundPlayer("Assets/back.wav");

           
            _random = new Random();
            _timer.Elapsed += _timer_ElapsedAsync;
            _timer.Start();
            _player3.Play();

        }

        private async void _timer_ElapsedAsync(object? sender, ElapsedEventArgs e)
        {
            
            int cartel = _random.Next(1, 15);
            //VmsDTO? cartelQueParpadea = _listaParpadeo.Where(x => x.CartelId == cartel).FirstOrDefault();
            
            if (_listaParpadeo[cartel].Parpadeando == true)
            {
                return;
            }
            else
            {
                
                    var cart = _vmsLista[cartel];
                _listaParpadeo[cartel].Parpadeando = true;

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
                   
                    
                    _vmsLista[cartel].ColorClaro = _listaParpadeo[cartel].ColorClaro;
                    _vmsLista[cartel].Color = _listaParpadeo[cartel].Color;



                    OnPropertyChanged(nameof(_vmsLista));
                    await Task.Delay(delay);

                }

                _listaParpadeo[cartel].Parpadeando = false;
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

            _listaParpadeo[e.CartelId].ColorClaro = e.ColorClaro;
            _listaParpadeo[e.CartelId].Color = e.Color;

            GuardadMensajes();

            
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
                    ColorClaro = coloresReales["Morado"][1],
                    Pictograma = "None"
                    
                };
                _vmsLista.Add(x);
            }
        }
        private string _archivo = "../data.json";
        public void GuardadMensajes()
        {
            var json = JsonSerializer.Serialize(_vmsLista);
            File.WriteAllText(_archivo, json);
        }

        public void CargarDatos()
        {
            if (File.Exists(_archivo))
            {
                var json = File.ReadAllText(_archivo);
                var datos = JsonSerializer.Deserialize<ObservableCollection<VmsDTO>?>(json);
                if (datos != null)
                {
                    foreach (var item in datos)
                    {
                        if(item.Color == "Black")
                        {
                            item.Color = coloresReales["Morado"][0];
                            item.ColorClaro = coloresReales["Morado"][1];
                        }
                    }
                    _vmsLista = datos;
                }
                else
                {
                    LlenarCarteles();

                }
            }
            else
            {
                LlenarCarteles();
                GuardadMensajes();
            }
        }

    }
}
